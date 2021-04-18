using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    public float volumeChangeSpeed = 5f;
    public AudioMixer mixer;
    public Sound[] sounds;

    void Awake()
    {
        // convierte al objeto en un singleton
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.outputAudioMixerGroup = s.volumeMixer;
        }
    }

    public void Play(string soundName)
    {
        try
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            s.source.Play();
        }
        catch (Exception)
        {
            Debug.LogWarning("'" + soundName + "' clip not found!");
        }
    }

    public void Stop(string soundName)
    {
        try
        {
            Sound s = Array.Find(sounds, sound => sound.name == soundName);
            s.source.Stop();
        }
        catch (Exception)
        {
            Debug.LogWarning("'" + soundName + "' clip not found!");
        }
    }

    public void TransitionMusic(string oldMusic, string newMusic)
    {
        try
        {
            Sound oldS = Array.Find(sounds, sound => sound.name == oldMusic);
            Sound newS = Array.Find(sounds, sound => sound.name == newMusic);
            StartCoroutine(SlowlyDecreaseVolume(oldS, newS));
        }
        catch (Exception)
        {
            Debug.LogWarning("'" + oldMusic + "' or '" + newMusic + "' clip not found!");
        }
    }

    public IEnumerator SlowlyDecreaseVolume(Sound oldSound, Sound newSound)
    {
        float oldValue = oldSound.volume;
        float elapsed = 0f;

        while (elapsed < volumeChangeSpeed)
        {
            elapsed += Time.deltaTime;

            oldSound.volume = Mathf.Lerp(oldValue, 0f, elapsed / volumeChangeSpeed);
            SetVolumeLevel("MusicVol", oldSound.volume);

            yield return null;
        }

        oldSound.source.Stop();
        newSound.source.Play();
    }

    public void ChangeMasterVol(float sliderValue) => SetVolumeLevel("MasterVol", sliderValue);

    public void ChangeMusicVol(float sliderValue) => SetVolumeLevel("MusicVol", sliderValue);

    public void ChangeSoundVol(float sliderValue) => SetVolumeLevel("SoundVol", sliderValue);

    private void SetVolumeLevel(string exposedValueName, float value)
    {
        // convierte los valores lineales a dB para el mixer
        mixer.SetFloat(exposedValueName, Mathf.Log10(value) * 20);
    }

    public float GetVolumeLevel(string exposedValueName)
    {
        mixer.GetFloat(exposedValueName, out float value);
        // retorna el valor linear del mixer
        return Mathf.Pow(10, value / 20); ;
    }
}
