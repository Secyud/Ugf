﻿#region

using Secyud.Ugf.HexUtilities;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    ///     Component that controls the singleton camera that navigates the hex map.
    /// </summary>
    public class HexMapCamera : MonoBehaviour
    {
        [SerializeField] private float StickMinZoom;
        [SerializeField] private float StickMaxZoom;
        [SerializeField] private float SwivelMinZoom;
        [SerializeField] private float SwivelMaxZoom;
        [SerializeField] private float MoveSpeedMinZoom;
        [SerializeField] private float MoveSpeedMaxZoom;
        [SerializeField] private float RotationSpeed;
        [SerializeField] private float BorderWidthMax;
        [SerializeField] private float BorderWidthMin;
        [SerializeField] private HexGrid Grid;
        private Vector3 _targetPosition;
        private bool _moveToTarget;
        private float _rotationAngle;

        private Transform _swivel, _stick;

        private float _zoom = 1f;

        private void Awake()
        {
            _swivel = transform.GetChild(0);
            _stick = _swivel.GetChild(0);
        }

        private void Update()
        {
            float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
            if (zoomDelta != 0f)
            {
                AdjustZoom(zoomDelta);
            }

            float rotationDelta = Input.GetAxis("Rotation");
            if (rotationDelta != 0f)
            {
                AdjustRotation(rotationDelta);
            }

            float xDelta = Input.GetAxis("Horizontal");
            float zDelta = Input.GetAxis("Vertical");
            if (xDelta != 0f || zDelta != 0f)
            {
                AdjustPosition(xDelta, zDelta);
            }

            if (_moveToTarget)
            {
                float speed = MoveSpeedMinZoom / 128;
                Vector3 vector = _targetPosition - transform.localPosition;
                Vector3 tmp = vector.normalized * speed;
                if (vector.magnitude > tmp.magnitude)
                {
                    vector = tmp;
                }

                transform.localPosition += vector;
                if ((vector - transform.localPosition).magnitude < 1f)
                {
                    _moveToTarget = false;
                }
            }
        }

        private void OnEnable()
        {
            ValidatePosition();
        }


        /// <summary>
        ///     Validate the position of the singleton camera.
        /// </summary>
        public void ValidatePosition()
        {
            AdjustPosition(0.5f, 0.5f);
            AdjustZoom(-999);
        }

        private void AdjustZoom(float delta)
        {
            _zoom = Mathf.Clamp01(_zoom + delta);
            float w = Screen.currentResolution.width / 2f;
            float h = Screen.currentResolution.height / 2f;
            w = (Input.mousePosition.x - w) / h;
            h = (Input.mousePosition.y - h) / h;


            float distance = Mathf.Lerp(StickMinZoom, StickMaxZoom, _zoom);
            _stick.localPosition = new Vector3(0f, 0f, distance);

            float angle = Mathf.Lerp(SwivelMinZoom, SwivelMaxZoom, _zoom);
            _swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);

            AdjustPosition(w, h);
        }

        private void AdjustRotation(float delta)
        {
            _rotationAngle += delta * RotationSpeed * Time.deltaTime;
            if (_rotationAngle < 0f)
                _rotationAngle += 360f;
            else if (_rotationAngle >= 360f) _rotationAngle -= 360f;

            transform.localRotation = Quaternion.Euler(0f, _rotationAngle, 0f);
        }

        private void AdjustPosition(float xDelta, float zDelta)
        {
            Vector3 direction = transform.localRotation *
                                new Vector3(xDelta, 0f, zDelta).normalized;
            float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            float distance =
                Mathf.Lerp(MoveSpeedMinZoom, MoveSpeedMaxZoom, _zoom) *
                damping * Time.deltaTime;

            Vector3 position = transform.localPosition;
            position += direction * distance;
            transform.localPosition = ClampPosition(position);
            _moveToTarget = false;
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            float borderWidth = Mathf.Lerp(BorderWidthMin, BorderWidthMax, 1 - _zoom);

            float xMin = (borderWidth + 0.5f) * HexMetrics.InnerDiameter;
            float xMax = (Grid.CellCountX - borderWidth - 0.5f) * HexMetrics.InnerDiameter;
            position.x = Mathf.Clamp(position.x, xMin, xMax);

            float zMin = (borderWidth + 1) * (1.5f * HexMetrics.OuterRadius);
            float zMax = (Grid.CellCountZ - borderWidth - 1) * (1.5f * HexMetrics.OuterRadius);
            position.z = Mathf.Clamp(position.z, zMin, zMax);

            return position;
        }

        public void SetTargetPosition(Vector3 position)
        {
            _targetPosition = position;
            _targetPosition.y = 0;
            _moveToTarget = true;
        }
    }
}