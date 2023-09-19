#region

using Secyud.Ugf.HexMap.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

#endregion

namespace Secyud.Ugf.HexMap
{
    /// <summary>
    ///     Component representing a unit that occupies a cell of the hex map.
    /// </summary>
    public class HexUnit : MonoBehaviour
    {
        private const float RotationSpeed = 360f;
        private const float TravelSpeed = 4f;
        [SerializeField] private SpriteRenderer Border;

        private PlayableDirector _playableDirector;
        private HexCell _location, _currentTravelLocation;
        private float _orientation;
        private List<HexCell> _pathToTravel;

        private HexUnitPlay _loopPlay;
        private IUnitBase _unitBase;

        public IUnitBase UnitBase
        {
            get => _unitBase;
            internal set
            {
                _unitBase = value;
                if (_unitBase is not null)
                    _unitBase.Unit = this;
            }
        }

        public TUnit Get<TUnit>() where TUnit : class
        {
            return _unitBase as TUnit;
        }

        public HexGrid Grid { get; set; }

        private PlayableDirector PlayableDirector
        {
            get
            {
                if (!_playableDirector)
                    TryGetComponent(out _playableDirector);
                return _playableDirector;
            }
        }

        /// <summary>
        ///     Cell that the unit occupies.
        /// </summary>
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

        /// <summary>
        ///     Orientation that the unit is facing.
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

        private void OnEnable()
        {
            if (_location)
            {
                transform.localPosition = _location.Position;
                if (_currentTravelLocation) _currentTravelLocation = null;
            }
        }

        public int Id { get; set; }

        public void SetHighlight(Color? color)
        {
            if (color is null)
            {
                Border.gameObject.SetActive(false);
            }
            else
            {
                Border.gameObject.SetActive(true);
                Border.color = color.Value;
            }
        }

        /// <summary>
        ///     Validate the position of the unit.
        /// </summary>
        public void ValidateLocation()
        {
            transform.localPosition = _location.Position;
        }

        /// <summary>
        ///     Travel along a path.
        /// </summary>
        /// <param name="path">List of cells that describe a valid path.</param>
        /// <param name="travelEndAction"></param>
        public void Travel(List<HexCell> path)
        {
            _location.Unit = null;
            _location = path[^1];
            _location.Unit = this;
            _pathToTravel = path;
            StopAllCoroutines();
            StartCoroutine(TravelPath());
            Grid.ClearPath();
        }

        private IEnumerator TravelPath()
        {
            Vector3 a, b, c = _pathToTravel[0].Position;
            yield return LookAt(_pathToTravel[1].Position);

            if (!_currentTravelLocation) _currentTravelLocation = _pathToTravel[0];

            float t = Time.deltaTime * TravelSpeed;
            for (int i = 1; i < _pathToTravel.Count; i++)
            {
                _currentTravelLocation = _pathToTravel[i];
                a = c;
                b = _pathToTravel[i - 1].Position;

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

            Transform trans = transform;
            trans.localPosition = _location.Position;
            _orientation = trans.localRotation.eulerAngles.y;
            ListPool<HexCell>.Add(_pathToTravel);
            _pathToTravel = null;
        }

        private IEnumerator LookAt(Vector3 point)
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

        public void SetLoopPlay(HexUnitPlay play)
        {
            _loopPlay = play;
        }


        /// <summary>
        ///     Terminate the unit.
        /// </summary>
        public void Die()
        {
            _unitBase?.OnDying();
            _location.Unit = null;
            Destroy(this);
        }

        public void OnPlayFinished()
        {
            if (_loopPlay)
                _loopPlay.ContinuePlay(this);
            _unitBase?.OnEndPlay();
        }
    }
}