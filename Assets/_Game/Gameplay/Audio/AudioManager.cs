using UnityEngine;

namespace ConquerChronicles.Gameplay.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private SFXLibrary _sfxLibrary;

        private AudioSource _musicSource;
        private AudioSource[] _sfxSources;
        private int _nextSfxIndex;

        private const int SFX_POOL_SIZE = 8;
        private const string PREF_SFX_VOL = "SFXVolume";
        private const string PREF_MUSIC_VOL = "MusicVolume";

        private float _sfxVolume = 1f;
        private float _musicVolume = 0.5f;

        public SFXLibrary Library => _sfxLibrary;

        private void Awake()
        {
            _sfxVolume = PlayerPrefs.GetFloat(PREF_SFX_VOL, 1f);
            _musicVolume = PlayerPrefs.GetFloat(PREF_MUSIC_VOL, 0.5f);

            // Music source
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.playOnAwake = false;
            _musicSource.volume = _musicVolume;

            // SFX pool
            _sfxSources = new AudioSource[SFX_POOL_SIZE];
            for (int i = 0; i < SFX_POOL_SIZE; i++)
            {
                _sfxSources[i] = gameObject.AddComponent<AudioSource>();
                _sfxSources[i].loop = false;
                _sfxSources[i].playOnAwake = false;
                _sfxSources[i].volume = _sfxVolume;
            }
        }

        public void PlaySFX(AudioClip clip)
        {
            if (clip == null) return;
            var source = _sfxSources[_nextSfxIndex];
            source.clip = clip;
            source.volume = _sfxVolume;
            source.Play();
            _nextSfxIndex = (_nextSfxIndex + 1) % SFX_POOL_SIZE;
        }

        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;
            if (_musicSource.clip == clip && _musicSource.isPlaying) return;
            _musicSource.clip = clip;
            _musicSource.volume = _musicVolume;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(PREF_SFX_VOL, _sfxVolume);
                for (int i = 0; i < _sfxSources.Length; i++)
                    _sfxSources[i].volume = _sfxVolume;
            }
        }

        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = Mathf.Clamp01(value);
                PlayerPrefs.SetFloat(PREF_MUSIC_VOL, _musicVolume);
                _musicSource.volume = _musicVolume;
            }
        }
    }
}
