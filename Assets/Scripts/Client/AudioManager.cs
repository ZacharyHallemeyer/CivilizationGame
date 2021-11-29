using System;
using UnityEngine;

/// <summary>
/// Handles game audio
/// </summary>
public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume;
        [Range(.1f, 3f)]
        public float pitch;

        public bool looping;
        public bool isSoundEffect;

        [HideInInspector]
        public AudioSource source;
    }

    public Sound[] sounds;
    public static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        // Set volumes pref if there is none
        if (PlayerPrefs.GetFloat("SoundEffectsVolume", 100) == 100)
            PlayerPrefs.SetFloat("SoundEffectsVolume", .75f);
        if (PlayerPrefs.GetFloat("MusicVolume", 100) == 100)
            PlayerPrefs.SetFloat("MusicVolume", .75f);

        // Set each source
        foreach (Sound _sound in sounds)
        {
            _sound.source = gameObject.AddComponent<AudioSource>();
            _sound.source.clip = _sound.clip;

            if (_sound.isSoundEffect)
                _sound.source.volume = PlayerPrefs.GetFloat("SoundEffectsVolume");
            else
                _sound.source.volume = PlayerPrefs.GetFloat("MusicVolume");


            _sound.source.pitch = _sound.pitch;
            _sound.source.loop = _sound.looping;
        }
        SetMusicVolume();
        SetSoundEffectVolume();
    }

    private void Start()
    {
        Play("BackgroundMusic");
    }

    /// <summary>
    /// Play audio clip with cooresponding name
    /// </summary>
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null) return;
        s.source.Play();
    }

    /// <summary>
    /// Stop audio clip with cooresponding name
    /// Note: Override
    /// </summary>
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
            return;
        s.source.Stop();
    }    
    
    /// <summary>
    /// Stop audio clip with cooresponding name
    /// Note: Override
    /// </summary>
    public void Stop(Sound _sound)
    {
        _sound.source.Stop();
    }

    /// <summary>
    /// Decreases volume of audio clip with cooresponding name. Returns true if volume is less than .01f     
    /// </summary>
    public void FadeOut(string _name)
    {
        Sound _sound = Array.Find(sounds, sound => sound.name == name);
        FadeOutHelper(_sound, _name);
    }

    public void FadeOutHelper(Sound _sound, string _soundName)
    {
        _sound.source.volume -= .1f;
        if (_sound.source.volume <= .01f)
        {
            ResetSound(_sound);
        }
        else
        {
            FadeOutHelper(_sound, _soundName);
        }
    }

    /// <summary>
    /// Resets volume of audio clip with cooresponding name to player prefered volume
    /// Note: Override
    /// </summary>
    /// <param name="name"></param>
    public void ResetSound(string name)
    {
        Sound _sound = Array.Find(sounds, sound => sound.name == name);
        _sound.source.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
    }

    /// <summary>
    /// Resets volume of audio clip with cooresponding name to player prefered volume
    /// Note: Override
    /// </summary>
    /// <param name="name"></param>
    public void ResetSound(Sound _sound)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
    }

    /// <summary>
    /// Set music audio clip's volume to player preference
    /// </summary>
    public void SetMusicVolume()
    {
        foreach (Sound _sound in sounds)
        {
            if (_sound.source == null) return;
            if (!_sound.isSoundEffect)
                _sound.source.volume = PlayerPrefs.GetFloat("MusicVolume", .75f);
        }
    }

    /// <summary>
    /// Set sounds (other than music) audio clip's volume to player preference
    /// </summary>
    public void SetSoundEffectVolume()
    {
        foreach (Sound _sound in sounds)
        {
            if (_sound.source == null) return;
            if (_sound.isSoundEffect)
                _sound.source.volume = PlayerPrefs.GetFloat("SoundEffectsVolume", .75f);
        }
    }
}
