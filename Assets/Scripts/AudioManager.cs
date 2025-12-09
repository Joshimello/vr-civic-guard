using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioMapEntry
{
    public string key;
    public AudioClip audioClip;
}

public class AudioManager : MonoBehaviour
{
    [Header("Audio Configuration")]
    [SerializeField] private List<AudioMapEntry> audioMap = new List<AudioMapEntry>();

    [Header("Audio Source Settings")]
    [SerializeField] private int maxOneTimeSources = 10;

    private Dictionary<string, AudioClip> audioDict;
    private Dictionary<string, AudioSource> persistentSources;
    private Queue<AudioSource> oneTimeSourcePool;
    private List<AudioSource> activeOneTimeSources;

    private void Awake()
    {
        // Initialize dictionaries and lists
        audioDict = new Dictionary<string, AudioClip>();
        persistentSources = new Dictionary<string, AudioSource>();
        oneTimeSourcePool = new Queue<AudioSource>();
        activeOneTimeSources = new List<AudioSource>();

        // Build the audio dictionary from the map
        BuildAudioDictionary();

        // Create pool of audio sources for one-time sounds
        CreateOneTimeSourcePool();
    }

    private void BuildAudioDictionary()
    {
        audioDict.Clear();

        foreach (AudioMapEntry entry in audioMap)
        {
            if (!string.IsNullOrEmpty(entry.key) && entry.audioClip != null)
            {
                if (audioDict.ContainsKey(entry.key))
                {
                    Debug.LogWarning($"AudioManager: Duplicate key '{entry.key}' found in audio map. Using first occurrence.");
                }
                else
                {
                    audioDict.Add(entry.key, entry.audioClip);
                }
            }
        }
    }

    private void CreateOneTimeSourcePool()
    {
        for (int i = 0; i < maxOneTimeSources; i++)
        {
            GameObject sourceObj = new GameObject($"OneTimeAudioSource_{i}");
            sourceObj.transform.SetParent(transform);
            AudioSource source = sourceObj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            oneTimeSourcePool.Enqueue(source);
        }
    }

    private void Update()
    {
        // Clean up finished one-time audio sources
        for (int i = activeOneTimeSources.Count - 1; i >= 0; i--)
        {
            if (!activeOneTimeSources[i].isPlaying)
            {
                AudioSource finishedSource = activeOneTimeSources[i];
                activeOneTimeSources.RemoveAt(i);
                oneTimeSourcePool.Enqueue(finishedSource);
            }
        }
    }

    /// <summary>
    /// Plays audio continuously. Only one instance per key can play at a time.
    /// Ideal for background music or ambient sounds. Loops by default with volume 1.0.
    /// </summary>
    /// <param name="key">The key of the audio clip to play</param>
    public void PlayAudio(string key)
    {
        if (!audioDict.ContainsKey(key))
        {
            Debug.LogWarning($"AudioManager: Audio key '{key}' not found in audio map.");
            return;
        }

        // Stop existing audio with the same key if playing
        if (persistentSources.ContainsKey(key))
        {
            persistentSources[key].Stop();
        }
        else
        {
            // Create new persistent audio source
            GameObject sourceObj = new GameObject($"PersistentAudioSource_{key}");
            sourceObj.transform.SetParent(transform);
            AudioSource newSource = sourceObj.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            persistentSources.Add(key, newSource);
        }

        AudioSource source = persistentSources[key];
        source.clip = audioDict[key];
        source.loop = true;
        source.volume = 1f;
        source.Play();
    }

    /// <summary>
    /// Stops audio that was started with PlayAudio.
    /// </summary>
    /// <param name="key">The key of the audio clip to stop</param>
    public void StopAudio(string key)
    {
        if (persistentSources.ContainsKey(key))
        {
            persistentSources[key].Stop();
        }
        else
        {
            Debug.LogWarning($"AudioManager: No persistent audio with key '{key}' is currently playing.");
        }
    }

    /// <summary>
    /// Plays audio once. Multiple instances can play simultaneously.
    /// Ideal for sound effects. Uses volume 1.0.
    /// </summary>
    /// <param name="key">The key of the audio clip to play</param>
    public void PlayAudioOnce(string key)
    {
        if (!audioDict.ContainsKey(key))
        {
            Debug.LogWarning($"AudioManager: Audio key '{key}' not found in audio map.");
            return;
        }

        if (oneTimeSourcePool.Count == 0)
        {
            Debug.LogWarning("AudioManager: No available audio sources for one-time playback. Consider increasing maxOneTimeSources.");
            return;
        }

        AudioSource source = oneTimeSourcePool.Dequeue();
        source.clip = audioDict[key];
        source.loop = false;
        source.volume = 1f;
        source.Play();

        activeOneTimeSources.Add(source);
    }

    /// <summary>
    /// Checks if a persistent audio is currently playing.
    /// </summary>
    /// <param name="key">The key of the audio clip to check</param>
    /// <returns>True if the audio is playing, false otherwise</returns>
    public bool IsAudioPlaying(string key)
    {
        if (persistentSources.ContainsKey(key))
        {
            return persistentSources[key].isPlaying;
        }
        return false;
    }

    /// <summary>
    /// Sets the volume of a persistent audio source.
    /// </summary>
    /// <param name="key">The key of the audio clip</param>
    /// <param name="volume">Volume level (0-1)</param>
    public void SetAudioVolume(string key, float volume)
    {
        if (persistentSources.ContainsKey(key))
        {
            persistentSources[key].volume = Mathf.Clamp01(volume);
        }
        else
        {
            Debug.LogWarning($"AudioManager: No persistent audio with key '{key}' found.");
        }
    }

    /// <summary>
    /// Stops all persistent audio sources.
    /// </summary>
    public void StopAllAudio()
    {
        foreach (var source in persistentSources.Values)
        {
            source.Stop();
        }
    }

    /// <summary>
    /// Pauses a persistent audio source.
    /// </summary>
    /// <param name="key">The key of the audio clip to pause</param>
    public void PauseAudio(string key)
    {
        if (persistentSources.ContainsKey(key))
        {
            persistentSources[key].Pause();
        }
        else
        {
            Debug.LogWarning($"AudioManager: No persistent audio with key '{key}' found.");
        }
    }

    /// <summary>
    /// Unpauses a persistent audio source.
    /// </summary>
    /// <param name="key">The key of the audio clip to unpause</param>
    public void UnPauseAudio(string key)
    {
        if (persistentSources.ContainsKey(key))
        {
            persistentSources[key].UnPause();
        }
        else
        {
            Debug.LogWarning($"AudioManager: No persistent audio with key '{key}' found.");
        }
    }

    private void OnValidate()
    {
        // Rebuild dictionary when values change in inspector
        if (Application.isPlaying)
        {
            BuildAudioDictionary();
        }
    }
}
