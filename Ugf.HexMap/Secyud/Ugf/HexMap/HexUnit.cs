#region

using Secyud.Ugf.HexMap.Utilities;
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
		private const float RotationSpeed = 180f;
		private const float TravelSpeed = 4f;
		public PlayableDirector PlayableDirector;

		[SerializeField] private SpriteRenderer SpriteRenderer;
		[SerializeField] private SpriteRenderer Border;

		private HexCell _location, _currentTravelLocation;

		private float _orientation;

		private List<HexCell> _pathToTravel;

		public HexGrid Grid { get; set; }

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
				value.Unit = this;
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

		/// <summary>
		///     Vision range of the unit, in cells.
		/// </summary>
		public int VisionRange => 3;

		private void OnEnable()
		{
			if (_location)
			{
				transform.localPosition = _location.Position;
				if (_currentTravelLocation) _currentTravelLocation = null;
			}
		}

		public int Id { get; set; }


		public void SetSprite(Sprite sprite)
		{
			SpriteRenderer.sprite = sprite;
		}

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

			if (!_currentTravelLocation) _currentTravelLocation = _pathToTravel[0];

			var t = Time.deltaTime * TravelSpeed;
			for (var i = 1; i < _pathToTravel.Count; i++)
			{
				_currentTravelLocation = _pathToTravel[i];
				a = c;
				b = _pathToTravel[i - 1].Position;

				c = (b + _currentTravelLocation.Position) * 0.5f;

				for (; t < 1f; t += Time.deltaTime * TravelSpeed)
				{
					transform.localPosition = Bezier.GetPoint(a, b, c, t);
					var d = Bezier.GetDerivative(a, b, c, t);
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
				var d = Bezier.GetDerivative(a, b, c, t);
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
			var trans = transform;
			var localPosition = trans.localPosition;
			point.y = localPosition.y;
			var fromRotation = trans.localRotation;
			var toRotation = Quaternion.LookRotation(point - localPosition);
			var angle = Quaternion.Angle(fromRotation, toRotation);

			if (angle > 0f)
			{
				var speed = RotationSpeed / angle;
				for (
					var t = Time.deltaTime * speed;
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
		///     Terminate the unit.
		/// </summary>
		public void Die()
		{
			_location.Unit = null;
			Destroy(this);
		}
	}
}