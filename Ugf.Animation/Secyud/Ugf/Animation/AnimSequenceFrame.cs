#region

using System;
using UnityEngine;

#endregion

namespace Secyud.Ugf.Animation
{
	[Serializable]
	public class AnimSequenceFrame
	{
		public string Path;
		public Vector3 LocalPosition;
		public Quaternion LocalRotation;
	}
}