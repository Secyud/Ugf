#region

using System;
using System.IO;

#endregion

namespace Secyud.Ugf.Animation
{
	[Serializable]
	public class AnimPathSequence
	{
		public string Path;
		public CurveVector3 LocalPosition = new();
		public CurveQuaternion LocalRotation = new();

		public void Save(BinaryWriter writer, int frames)
		{
			writer.Write(Path);

			for (int i = 0; i < frames; i++)
			{
				writer.Write(LocalPosition.X.keys[i].value);
				writer.Write(LocalPosition.Y.keys[i].value);
				writer.Write(LocalPosition.Z.keys[i].value);
				writer.Write(LocalRotation.X.keys[i].value);
				writer.Write(LocalRotation.Y.keys[i].value);
				writer.Write(LocalRotation.Z.keys[i].value);
				writer.Write(LocalRotation.W.keys[i].value);
			}
		}

		public void Load(BinaryReader reader, int frames, float delta)
		{
			reader.ReadString();
			float time = 0f;
			for (int i = 0; i < frames; i++)
			{
				LocalPosition.X.AddKey(time, reader.ReadSingle());
				LocalPosition.Y.AddKey(time, reader.ReadSingle());
				LocalPosition.Z.AddKey(time, reader.ReadSingle());
				LocalRotation.X.AddKey(time, reader.ReadSingle());
				LocalRotation.Y.AddKey(time, reader.ReadSingle());
				LocalRotation.Z.AddKey(time, reader.ReadSingle());
				LocalRotation.W.AddKey(time, reader.ReadSingle());
				time += delta;
			}
		}
	}
}