using UnityEngine;

namespace Secyud.Ugf.Animation
{
    public sealed class AnimPlayer : MonoBehaviour
    {
        private Transform _character;
        private float _time;

        public float CurrentTime
        {
            get => _time;
            set
            {
                if (value > AnimDataSequence.Length) return;

                _time = value;
                SetPose();
            }
        }

        public bool Loop { get; set; } = false;
        public bool IsPlaying { get; private set; }
        public AnimDataSequence AnimDataSequence { get; private set; }

        private void Update()
        {
            if (!IsPlaying) return;

            if (_time > AnimDataSequence.Length)
            {
                if (Loop)
                {
                    Stop();
                    Play();
                }
                else
                {
                    IsPlaying = false;
                }

                return;
            }

            SetPose();

            // 增加时间
            _time += Time.deltaTime;
        }

        private void SetPose()
        {
            if (AnimDataSequence == null || _character == null) return;
            if (_time > AnimDataSequence.Length) return;

            // 获取指定时间的序列数据
            var frame = AnimDataSequence.GetFrame(_time);
            // 将获取到的数据赋值给对应的骨骼
            foreach (var frameOnePath in frame)
            {
                var boneTransform = _character.Find(frameOnePath.Path);
                if (boneTransform != null)
                {
                    boneTransform.localPosition = frameOnePath.LocalPosition;
                    boneTransform.localRotation = frameOnePath.LocalRotation;
                }
                else
                {
                    Debug.LogWarning($"[GestureGenerator] {frameOnePath.Path} is Null");
                }
            }
        }

        public void PrepareData(AnimDataSequence sequence, Transform character)
        {
            IsPlaying = false;
            _time = 0f;
            AnimDataSequence = sequence;
            _character = character;
        }

        public void Play()
        {
            IsPlaying = true;
        }

        public void Pause()
        {
            IsPlaying = false;
        }

        public void Stop()
        {
            IsPlaying = false;
            _time = 0f;
            SetPose();
        }

        public void Clear()
        {
            Stop();
            AnimDataSequence = null;
            _character = null;
        }
    }
}