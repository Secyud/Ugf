using Secyud.Ugf.DependencyInjection;
using Secyud.Ugf.Unity.AssetLoading;
using UnityEngine;
using UnityEngine.Audio;

namespace Secyud.Ugf.Unity.AudioManagement
{
    public class AudioManageService : IRegistry
    {
        private float _musicVolume = 1;
        private float _soundVolume = 1;
        private float _masterVolume = 1;
        private bool _musicDisabled;
        private bool _soundDisabled;
        private bool _masterDisabled;
        private bool _isInitialized;

        public IObjectContainer<AudioMixer> AudioMixer { get; set; }

        private void SetAudioVolume(string param, float value)
        {
            if (AudioMixer is null)
                throw new UgfNotRegisteredException(
                    nameof(AudioManageService),
                    nameof(AudioMixer));

            if (value <= 0.0f) value = 0.0001f;
            value = Mathf.Log10(value) * 20.0f;

            AudioMixer.GetValue().SetFloat(param, value);
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                if (_musicDisabled)
                    _musicDisabled = false;
                _musicVolume = value;
                SetAudioVolume("vMusic", value);
            }
        }

        public float SoundVolume
        {
            get => _soundVolume;
            set
            {
                if (_soundDisabled)
                    _soundDisabled = false;
                _soundVolume = value;
                SetAudioVolume("vSound", value);
            }
        }

        public float MasterVolume
        {
            get => _masterVolume;
            set
            {
                if (_masterDisabled)
                    _masterDisabled = false;
                _masterVolume = value;
                SetAudioVolume("vMaster", value);
            }
        }

        public bool MusicDisabled
        {
            get => _musicDisabled;
            set
            {
                _musicDisabled = value;
                SetAudioVolume("vMusic", value ? 0 : _musicVolume);
            }
        }

        public bool SoundDisabled
        {
            get => _soundDisabled;
            set
            {
                _soundDisabled = value;
                SetAudioVolume("vSound", value ? 0 : _soundVolume);
            }
        }

        public bool MasterDisabled
        {
            get => _masterDisabled;
            set
            {
                _masterDisabled = value;
                SetAudioVolume("vMaster", value ? 0 : _masterVolume);
            }
        }
    }
}