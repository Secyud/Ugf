#region

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

#endregion

namespace Secyud.Ugf.Animation
{
	public class AnimDataSequence
	{
		public readonly List<AnimPathSequence> AnimPathSequences = new();
		private float _delta;
		private float _frameRate = 30;
		private readonly int _frames;

		public int Id;
		public float Length;

		public string Name;


		public AnimDataSequence(AnimationClip clip, Animator animator)
		{
			var playableGraph = PlayableGraph.Create("ConvertHumanoidAnimation");
			playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);
			var animPlayableOutput =
				AnimationPlayableOutput.Create(playableGraph, "AnimationOutput", animator);
			var animClipPlayable = AnimationClipPlayable.Create(playableGraph, clip);
			animPlayableOutput.SetSourcePlayable(animClipPlayable);

			Length = clip.length;
			Name = clip.name;
			FrameRate = clip.frameRate;

			var root = animator.transform;
			List<Transform> bones = new();

			for (var i = 0; i < (int)HumanBodyBones.LastBone; i++)
			{
				var boneTransform = animator.GetBoneTransform((HumanBodyBones)i);

				if (boneTransform == null) continue;

				bones.Add(boneTransform);

				var pathSequence = new AnimPathSequence
				{
					Path = boneTransform.RelativePathTo(root)
				};
				AnimPathSequences.Add(pathSequence);
			}

			var time = 0f;
			var frameDuration = 1f / FrameRate;

			_frames = 0;

			while (time <= clip.length)
			{
				animClipPlayable.SetTime(time);
				playableGraph.Evaluate();

				for (var i = 0; i < bones.Count; i++)
				{
					var bone = bones[i];
					var pathSequence = AnimPathSequences[i];

					var localPosition = bone.localPosition;
					pathSequence.LocalPosition.X.AddKey(time, localPosition.x);
					pathSequence.LocalPosition.Y.AddKey(time, localPosition.y);
					pathSequence.LocalPosition.Z.AddKey(time, localPosition.z);

					var localRotation = bone.localRotation;
					pathSequence.LocalRotation.X.AddKey(time, localRotation.x);
					pathSequence.LocalRotation.Y.AddKey(time, localRotation.y);
					pathSequence.LocalRotation.Z.AddKey(time, localRotation.z);
					pathSequence.LocalRotation.W.AddKey(time, localRotation.w);
				}

				// 下一帧的时间
				time += frameDuration;
				_frames++;
			}

			playableGraph.Destroy();
		}


		public AnimDataSequence(BinaryReader reader)
		{
			Load(reader);
		}

		public float FrameRate
		{
			get => _frameRate;
			set
			{
				_frameRate = value;
				_delta = 1f / FrameRate;
			}
		}

		public List<AnimSequenceFrame> GetFrame(float time)
		{
			List<AnimSequenceFrame> sequenceFrames = new(AnimPathSequences.Count);
			foreach (var sequence in AnimPathSequences)
			{
				var frame = new AnimSequenceFrame
				{
					Path = sequence.Path,
					LocalPosition = new Vector3(
						sequence.LocalPosition.X.Evaluate(time),
						sequence.LocalPosition.Y.Evaluate(time),
						sequence.LocalPosition.Z.Evaluate(time)
					),
					LocalRotation = new Quaternion(
						sequence.LocalRotation.X.Evaluate(time),
						sequence.LocalRotation.Y.Evaluate(time),
						sequence.LocalRotation.Z.Evaluate(time),
						sequence.LocalRotation.W.Evaluate(time)
					)
				};

				sequenceFrames.Add(frame);
			}

			return sequenceFrames;
		}

		public void Save(BinaryWriter writer)
		{
			writer.Write(Id);
			writer.Write(Length);
			writer.Write(FrameRate);
			writer.Write(Name);
			writer.Write(AnimPathSequences.Count);
			writer.Write(_frames);
			foreach (var animPathSequence in AnimPathSequences) animPathSequence.Save(writer, _frames);
		}

		public void Load(BinaryReader reader)
		{
			Id = reader.ReadInt32();
			Length = reader.ReadSingle();
			FrameRate = reader.ReadSingle();
			Name = reader.ReadString();
			var count = reader.ReadInt32();
			var frames = reader.ReadInt32();
			for (var i = 0; i < count; i++)
			{
				AnimPathSequence sequence = new();
				sequence.Load(reader, _frames, _delta);
				AnimPathSequences.Add(sequence);
			}
		}
	}
}