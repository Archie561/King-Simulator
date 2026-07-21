using UnityEngine;

namespace Game.Shared.Services.Audio
{
    public class AudioService : IAudioService
    {
        public void PlaySfx(string sfxName)
        {
            // Placeholder: Use DOTween or AudioSource logic
            Debug.Log($"Playing SFX: {sfxName}");
        }

        public void PlayMusic(string musicName)
        {
            Debug.Log($"Playing Music: {musicName}");
        }

        public void SetMasterVolume(float volume)
        {
            // Set AudioListener volume or AudioMixer param
            AudioListener.volume = Mathf.Clamp01(volume);
        }
    }
}
