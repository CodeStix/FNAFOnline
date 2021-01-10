using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundEffects : MonoBehaviour
{
    public string[] startSounds;
    public Sound[] soundEffects;

    public static float volumeMultiplier = 1f;
    public static float masterVolume = 1f;

    private Dictionary<Sound, AudioSource> audioSources = new Dictionary<Sound, AudioSource>();
    private Dictionary<Sound, AudioLowPassFilter> audioLPFs = new Dictionary<Sound, AudioLowPassFilter>();
    private Dictionary<Sound, AudioReverbFilter> audioReverbs = new Dictionary<Sound, AudioReverbFilter>();

    private static SoundEffects soundEffectsClass = null;

    public const string NO_SOUND_NAME = "NONE";
    public const string AUDIO_SOURCE_NAME = "Audio Source: ";
    public const float ALLTIME_VOLUME_MULTIPLIER = 0.75f;

    private void Awake()
    {
        if (soundEffectsClass != null)
        {
            Debug.LogWarning("Multiple SoundEffect instances found. Destroying other one.");
            Destroy(soundEffectsClass.gameObject);
        }

        soundEffectsClass = this;
    }

    void Start()
    {
        audioLPFs.Clear();
        audioSources.Clear();
        audioReverbs.Clear();

        foreach (Sound soundEff in soundEffects)
        {
            GameObject objToSpawn = new GameObject(AUDIO_SOURCE_NAME + soundEff.name);
            GameObject spawnedObj = Instantiate(objToSpawn, transform);
            Destroy(objToSpawn);

            AudioSource audioSrc = spawnedObj.AddComponent<AudioSource>();
            audioSrc.playOnAwake = false;
            audioSrc.loop = soundEff.loop;
            audioSources.Add(soundEff, audioSrc);

            AudioLowPassFilter audioLPF = spawnedObj.AddComponent<AudioLowPassFilter>();
            audioLPF.cutoffFrequency = soundEff.LPF;
            audioLPFs.Add(soundEff, audioLPF);

            AudioReverbFilter audioReverb = spawnedObj.AddComponent<AudioReverbFilter>();
            audioReverb.reverbPreset = soundEff.reverb;
            audioReverbs.Add(soundEff, audioReverb);
        }

        foreach (string s in startSounds)
            Play(s);
    }

    private void OnDestroy()
    {
        if (soundEffectsClass == this)
            soundEffectsClass = null;
    }

    public void PlayGlobal(string name)
    {
        Play(name);
    }

    public void StopGlobal(string name)
    {
        Stop(name);
    }

    public static void Stop(string name)
    {
        soundEffectsClass.StopSound(name);
    }

    public void StopSound(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        foreach (Sound s in audioSources.Keys)
            if (s.name.Trim().ToUpper() == name.Trim().ToUpper())
                audioSources[s].Stop();
    }

    public static void Play(string name)
    {
        soundEffectsClass.PlaySound(name);
    }

    public void PlaySound(string name)
    {
        bool playedSomething = false;

        if (string.IsNullOrEmpty(name) || name.ToLower() == NO_SOUND_NAME.ToLower())
            return;

        List<Sound> soundEffects = new List<Sound>(audioSources.Keys);
        for (int s = 0; s < audioSources.Keys.Count; s++)
        {
            Sound soundEff = soundEffects[s];
            if (soundEff.name.Trim().ToUpper() != name.Trim().ToUpper())
                continue;
            AudioSource audioSrc = audioSources[soundEff];
            //audioSrc.spatialBlend = NO_3D_VALUE;
            soundEff.PlayFor(audioSrc);
            playedSomething = true;
        }

        if (!playedSomething)
            Debug.LogWarning("Sound effect with name '" + name + "' was not found.");
    }

    public void AllLPFs(float value)
    {
        foreach(AudioLowPassFilter s in audioLPFs.Values)
            s.cutoffFrequency = value;
    }

}



[System.Serializable]
public class Sound
{
    public string name = DEFAULT_NAME;
    public float volume = 1.0f;
    public float pitch = 1.0f;
    [Range(-1f,1f)]
    public float panStereo = 0f;
    [Space]
    public bool randomizePitch = false;
    public float minPitch = 0.85f;
    public float maxPitch = 1.15f;
    [Space]
    public AudioClip[] sounds = new AudioClip[1];
    public bool randomizeSounds = true;
    [Space]
    public bool loop = false;
    public float LPF = 22000.0f;
    public AudioReverbPreset reverb = AudioReverbPreset.Off;

    private int currentSound = 0;

    public const string DEFAULT_NAME = "A Sound Effect";

    public AudioClip GetSound()
    {
        if (randomizeSounds)
            return sounds[Random.Range(0, sounds.Length)];
        else
            if (++currentSound >= sounds.Length)
            currentSound = 0;
        return sounds[currentSound];

    }

    public void PlayFor(AudioSource audioSource)
    {
        audioSource.volume = SoundEffects.masterVolume * volume * SoundEffects.volumeMultiplier * SoundEffects.ALLTIME_VOLUME_MULTIPLIER;
        audioSource.pitch = randomizePitch ? Random.Range(minPitch, maxPitch) : pitch;
        audioSource.clip = GetSound();
        audioSource.panStereo = panStereo;
        audioSource.Play();
    }
}