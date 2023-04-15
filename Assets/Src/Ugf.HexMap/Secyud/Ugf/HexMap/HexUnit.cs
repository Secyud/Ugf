﻿#region

using System.Collections;
using System.Collections.Generic;
using Secyud.Ugf.Unity;
using UnityEngine;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    /// Component representing a unit that occupies a cell of the hex map.
    /// </summary>
    public class HexUnit : MonoBehaviour, IHasId
    {
        private const float RotationSpeed = 180f;
        private const float TravelSpeed = 4f;

        public HexGrid Grid { get; set; }
        public int Id { get; set; }

        /// <summary>
        /// Cell that the unit occupies.
        /// </summary>
        public HexCell Location
        {
            get => _location;
            set
            {
                if (_location)
                {
                    _location.Unit = null;
                }

                _location = value;
                value.Unit = this;
                var trans = transform;
                trans.localPosition = value.Position;
                Grid.MakeChildOfColumn(trans, value.ColumnIndex);
            }
        }

        private HexCell _location, _currentTravelLocation;

        /// <summary>
        /// Orientation that the unit is facing.
        /// </summary>
        public float Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                transform.localRotation = Quaternion.Euler(0f, value, 0f);
            }
        }

        /// <summary>
        /// Speed of the unit, in cells per turn.
        /// </summary>
        public int Speed => 24;

        /// <summary>
        /// Vision range of the unit, in cells.
        /// </summary>
        public int VisionRange => 3;

        private float _orientation;

        private List<HexCell> _pathToTravel;

        /// <summary>
        /// Validate the position of the unit.
        /// </summary>
        public void ValidateLocation() => transform.localPosition = _location.Position;

        /// <summary>
        /// Travel along a path.
        /// </summary>
        /// <param name="path">List of cells that describe a valid path.</param>
        public void Travel(List<HexCell> path)
        {
            _location.Unit = null;
            _location = path[^1];
            _location.Unit = this;
            _pathToTravel = path;
            StopAllCoroutines();
            StartCoroutine(TravelPath());
        }

        private IEnumerator TravelPath()
        {
            Vector3 a, b, c = _pathToTravel[0].Position;
            yield return LookAt(_pathToTravel[1].Position);

            if (!_currentTravelLocation)
            {
                _currentTravelLocation = _pathToTravel[0];
            }

            int currentColumn = _currentTravelLocation.ColumnIndex;

            float t = Time.deltaTime * TravelSpeed;
            for (int i = 1; i < _pathToTravel.Count; i++)
            {
                _currentTravelLocation = _pathToTravel[i];
                a = c;
                b = _pathToTravel[i - 1].Position;

                int nextColumn = _currentTravelLocation.ColumnIndex;
                if (currentColumn != nextColumn)
                {
                    if (nextColumn < currentColumn - 1)
                    {
                        a.x -= HexMetrics.InnerDiameter * HexMetrics.WrapSize;
                        b.x -= HexMetrics.InnerDiameter * HexMetrics.WrapSize;
                    }
                    else if (nextColumn > currentColumn + 1)
                    {
                        a.x += HexMetrics.InnerDiameter * HexMetrics.WrapSize;
                        b.x += HexMetrics.InnerDiameter * HexMetrics.WrapSize;
                    }

                    Grid.MakeChildOfColumn(transform, nextColumn);
                    currentColumn = nextColumn;
                }

                c = (b + _currentTravelLocation.Position) * 0.5f;

                for (; t < 1f; t += Time.deltaTime * TravelSpeed)
                {
                    transform.localPosition = Bezier.GetPoint(a, b, c, t);
                    Vector3 d = Bezier.GetDerivative(a, b, c, t);
                    d.y = 0f;
                    transform.localRotation = Quaternion.LookRotation(d);
                    yield return null;
                }

                t -= 1f;
            }

            _currentTravelLocation = null;

            a = c;
            b = _location.Position;
            c = b;
            for (; t < 1f; t += Time.deltaTime * TravelSpeed)
            {
                transform.localPosition = Bezier.GetPoint(a, b, c, t);
                Vector3 d = Bezier.GetDerivative(a, b, c, t);
                d.y = 0f;
                transform.localRotation = Quaternion.LookRotation(d);
                yield return null;
            }

            var trans = transform;
            trans.localPosition = _location.Position;
            _orientation = trans.localRotation.eulerAngles.y;
            ListPool<HexCell>.Add(_pathToTravel);
            _pathToTravel = null;
        }

        private IEnumerator LookAt(Vector3 point)
        {
            if (HexMetrics.Wrapping)
            {
                float xDistance = point.x - transform.localPosition.x;
                if (xDistance < -HexMetrics.InnerRadius * HexMetrics.WrapSize)
                {
                    point.x += HexMetrics.InnerDiameter * HexMetrics.WrapSize;
                }
                else if (xDistance > HexMetrics.InnerRadius * HexMetrics.WrapSize)
                {
                    point.x -= HexMetrics.InnerDiameter * HexMetrics.WrapSize;
                }
            }

            var trans = transform;
            var localPosition = trans.localPosition;
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


        /// <summary>
        /// Terminate the unit.
        /// </summary>
        public void Die()
        {
            _location.Unit = null;
            Destroy(this);
        }

        private void OnEnable()
        {
            if (_location)
            {
                transform.localPosition = _location.Position;
                if (_currentTravelLocation)
                {
                    _currentTravelLocation = null;
                }
            }
        }
    }
}