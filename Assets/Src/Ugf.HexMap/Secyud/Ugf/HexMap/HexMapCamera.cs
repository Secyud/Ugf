﻿#region

using System;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    /// Component that controls the singleton camera that navigates the hex map.
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
        [SerializeField] private HexGrid Grid;
        [SerializeField] private int MaxDistance;
        [SerializeField] private int MinDistance;

        private Transform _swivel, _stick;

        private float _zoom = 1f;

        private float _rotationAngle;

        public void Adjust()
        {
            var gridSize = Math.Max(Grid.CellCountX, Grid.CellCountZ);
            if (MaxDistance > gridSize * 2)
                MaxDistance = gridSize * 2;
            if (MinDistance > MaxDistance / 2)
                MinDistance = MaxDistance / 2;

            if (StickMaxZoom > -80)
                StickMaxZoom = -80;

            if (StickMinZoom > StickMaxZoom * 3)
                StickMinZoom = StickMaxZoom * 3;
        }


        /// <summary>
        /// Validate the position of the singleton camera.
        /// </summary>
        public void ValidatePosition() => AdjustPosition(0f, 0f);

        private void Awake()
        {
            _swivel = transform.GetChild(0);
            _stick = _swivel.GetChild(0);
        }

        private void OnEnable()
        {
            ValidatePosition();
        }

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

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
        }

        private void AdjustZoom(float delta)
        {
            _zoom = Mathf.Clamp01(_zoom + delta);

            float distance = Mathf.Lerp(StickMinZoom, StickMaxZoom, _zoom);
            _stick.localPosition = new Vector3(0f, 0f, distance);

            float angle = Mathf.Lerp(SwivelMinZoom, SwivelMaxZoom, _zoom);
            _swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);

            AdjustPosition(0, 0);
        }

        private void AdjustRotation(float delta)
        {
            _rotationAngle += delta * RotationSpeed * Time.deltaTime;
            if (_rotationAngle < 0f)
            {
                _rotationAngle += 360f;
            }
            else if (_rotationAngle >= 360f)
            {
                _rotationAngle -= 360f;
            }

            transform.localRotation = Quaternion.Euler(0f, _rotationAngle, 0f);
        }

        private void AdjustPosition(float xDelta, float zDelta)
        {
            Vector3 direction =
                transform.localRotation *
                new Vector3(xDelta, 0f, zDelta).normalized;
            float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
            float distance =
                Mathf.Lerp(MoveSpeedMinZoom, MoveSpeedMaxZoom, _zoom) *
                damping * Time.deltaTime;

            Vector3 position = transform.localPosition;
            position += direction * distance;
            transform.localPosition =
                Grid.Wrapping ? WrapPosition(position) : ClampPosition(position);
        }

        private Vector3 ClampPosition(Vector3 position)
        {
            float distance = Mathf.Lerp(MinDistance, MaxDistance, 1 - _zoom);

            float xMax = (Grid.CellCountX - 0.5f) * HexMetrics.InnerDiameter;
            position.x = Mathf.Clamp(position.x, distance, xMax - distance);

            float zMax = (Grid.CellCountZ - 1) * (1.5f * HexMetrics.OuterRadius);
            position.z = Mathf.Clamp(position.z, distance, zMax - distance);

            return position;
        }

        private Vector3 WrapPosition(Vector3 position)
        {
            float width = Grid.CellCountX * HexMetrics.InnerDiameter;
            while (position.x < 0f)
            {
                position.x += width;
            }

            while (position.x > width)
            {
                position.x -= width;
            }

            float zMax = (Grid.CellCountZ - 1) * (1.5f * HexMetrics.OuterRadius);
            position.z = Mathf.Clamp(position.z, 0f, zMax);

            Grid.CenterMap(position.x);
            return position;
        }
    }
}