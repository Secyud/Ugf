using System;
using UnityEngine;

namespace Secyud.Ugf.FunctionalComponents
{
	public class TransChange:MonoBehaviour
	{
		[SerializeField] private Vector3 MoveSpeed;
		[SerializeField] private Vector3 ScaleSpeed;
		[SerializeField] private Vector3 RotateSpeed;

		private Transform _trans;
		private Vector3 _rotate;

		private void Awake()
		{
			_trans = transform;
		}

		private void Update()
		{
			_trans.position += MoveSpeed * Time.deltaTime;
			_trans.localScale += ScaleSpeed * Time.deltaTime;
			_rotate += RotateSpeed;
			_trans.rotation = Quaternion.Euler(_rotate);
		}
	}
}