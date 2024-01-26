using UnityEngine;
using UnityEngine.UI;

namespace Secyud.Ugf.Unity.AudioManagement
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private Slider _masterSlider;
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _soundSlider;
        [SerializeField] private Toggle _masterToggle;
        [SerializeField] private Toggle _musicToggle;
        [SerializeField] private Toggle _soundToggle;

        private AudioManageService _service;

        private void Awake()
        {
            _service = U.Get<AudioManageService>();
            _masterSlider.interactable = !_service.MasterDisabled;
            _musicSlider.interactable = !_service.MusicDisabled;
            _soundSlider.interactable = !_service.SoundDisabled;
            _masterSlider.SetValueWithoutNotify(_service.MasterVolume);
            _musicSlider.SetValueWithoutNotify(_service.MusicVolume);
            _soundSlider.SetValueWithoutNotify(_service.SoundVolume);
            _masterToggle.SetIsOnWithoutNotify(_service.MasterDisabled);
            _musicToggle.SetIsOnWithoutNotify(_service.MusicDisabled);
            _soundToggle.SetIsOnWithoutNotify(_service.SoundDisabled);
        }

        public void ChangeMasterVolume(float volume)
        {
            _service.MasterVolume = volume;
            _masterToggle.SetIsOnWithoutNotify(_service.MasterDisabled);
        }

        public void ChangeMusicVolume(float volume)
        {
            _service.MusicVolume = volume;
            _musicToggle.SetIsOnWithoutNotify(_service.MusicDisabled);
        }

        public void ChangeSoundVolume(float volume)
        {
            _service.SoundVolume = volume;
            _soundToggle.SetIsOnWithoutNotify(_service.SoundDisabled);
        }

        public void ChangeMasterDisabled(bool disabled)
        {
            _service.MasterDisabled = disabled;
            _masterSlider.interactable = !_service.MasterDisabled;
        }

        public void ChangeMusicDisabled(bool disabled)
        {
            _service.MusicDisabled = disabled;
            _musicSlider.interactable = !_service.MusicDisabled;
        }

        public void ChangeSoundDisabled(bool disabled)
        {
            _service.SoundDisabled = disabled;
            _soundSlider.interactable = !_service.SoundDisabled;
        }
    }
}