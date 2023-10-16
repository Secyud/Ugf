using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Secyud.Ugf.HexMap
{
    public class HexUnit:MonoBehaviour
    {
        private const float RotationSpeed = 360f;

        [SerializeField] private UnityEvent<Color> HighlightEvent;
        
        private readonly Dictionary<Type, UnitProperty> _properties = new();
        private HexCell _location;
        private float _orientation;
        public int Id { get; set; }

        public HexCell Location
        {
            get => _location;
            set
            {
                if (_location) _location.Unit = null;

                _location = value;
                _location.Unit = this;
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
            if (_location)
            {
                transform.localPosition = _location.Position;
            }
        }

        public void SetProperty([NotNull]UnitProperty property)
        {
            _properties[property.GetType()] = property;
            property.Initialize(this);
        }
        
        public TProperty Get<TProperty>()
            where TProperty : UnitProperty
        {
            if (!_properties.TryGetValue(typeof(TProperty), out UnitProperty property))
            {
                property = U.Get<TProperty>();
                _properties[typeof(TProperty)] = property;
                property.Initialize(this);
            }

            return property as TProperty;
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
        
        public void Die()
        {
            Destroy(gameObject);
        }
    }
}