namespace Game.Shared.Services.Audio
{
    public interface IAudioService
    {
        void PlaySfx(string sfxName);
        void PlayMusic(string musicName);
        void SetMasterVolume(float volume);
    }
}
