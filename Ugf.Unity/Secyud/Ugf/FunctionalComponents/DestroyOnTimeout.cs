#region

using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.FunctionalComponents
{
	public class DestroyOnTimeout : MonoBehaviour
	{
		[SerializeField] private float OutTime;
		private float _timeRecord;

		private void Awake()
		{
			_timeRecord = 0;
		}

		private void Update()
		{
			_timeRecord += Time.deltaTime;
			if (OutTime < _timeRecord)
				Destroy(gameObject);
		}
	}
}