using System;
using UnityEngine;

namespace Secyud.Ugf.FunctionalComponents
{
	public class TransChange:MonoBehaviour
	{
		[SerializeField] private Vector3 MoveSpeed;
		[SerializeField] private Vector3 ScaleSpeed;

		private Transform _trans;

		private void Awake()
		{
			_trans = transform;
		}

		private void Update()
		{
			_trans.position += MoveSpeed * Time.deltaTime;
			_trans.localScale += ScaleSpeed * Time.deltaTime;
		}
	}
}