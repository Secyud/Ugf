using System.Collections;
using JetBrains.Annotations;
using Secyud.Ugf.HexUtilities;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.HexMap
{
    public class HexUnit : MonoBehaviour
    {
        private const float RotationSpeed = 360f;

        [SerializeField] private UnityEvent<Color> HighlightEvent;

        private UnitProperty _property;
        private int _locationIndex = -1;
        private float _orientation;
        public int Id { get; set; }
        public HexGrid Grid { get; set; }

        public HexCell Location
        {
            get => Grid.GetCell(_locationIndex);
            set
            {
                if (_locationIndex >= 0)
                {
                    Grid.GetCell(_locationIndex).Unit = null;
                }

                _locationIndex = value.Index;
                Grid.GetCell(_locationIndex).Unit  = this;
                transform.localPosition = value.Position;
            }
        }

        public float Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                transform.localRotation = Quaternion.Euler(0f, value, 0f);
            }
        }

        private void OnEnable()
        {
            if (_locationIndex>=0)
            {
                transform.localPosition = Grid.GetCell(_locationIndex).Position;
            }
        }

        public void SetProperty([NotNull] UnitProperty property)
        {
            _property = property;
            property.Initialize(this);
        }

        public TProperty Get<TProperty>()
            where TProperty : UnitProperty
        {
            return _property as TProperty;
        }

        public void SetHighlight(Color color)
        {
            HighlightEvent.Invoke(color);
        }

        public IEnumerator LookAt(Vector3 point)
        {
            Transform trans = transform;
            Vector3 localPosition = trans.localPosition;
            point.y = localPosition.y;
            Quaternion fromRotation = trans.localRotation;
            Quaternion toRotation = Quaternion.LookRotation(point - localPosition);
            float angle = Quaternion.Angle(fromRotation, toRotation);

            if (angle > 0f)
            {
                float speed = RotationSpeed / angle;
                for (
                    float t = Time.deltaTime * speed;
                    t < 1f;
                    t += Time.deltaTime * speed
                )
                {
                    trans.localRotation =
                        Quaternion.Slerp(fromRotation, toRotation, t);
                    yield return null;
                }
            }

            trans.LookAt(point);
            _orientation = trans.localRotation.eulerAngles.y;
        }

        public HexDirection DirectionTo(HexCell cell)
        {
            return Location.DirectionTo(cell);
        }

        public HexDirection DirectionTo(HexUnit unit)
        {
            return DirectionTo(unit.Location);
        }

        public float DistanceTo(HexCell cell)
        {
            return Location.DistanceTo(cell);
        }

        public float DistanceTo(HexUnit unit)
        {
            return DistanceTo(unit.Location);
        }


        public virtual void Die()
        {
            Destroy(gameObject);
        }
    }
}