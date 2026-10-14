using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace FlashlightSystem
{
    public class FLAudioManager : MonoBehaviour
    {
        [Header("List of Sound Effect SO's")]
        [SerializeField] private Sound[] sounds = null; // Array of Sound ScriptableObjects to manage

        [Header("Sound Mixer Group")]
        [SerializeField] private AudioMixerGroup mixerGroup = null; // Audio mixer group to route all sounds through

        [Header("Should persist?")]
        [SerializeField] private bool persistAcrossScenes = true; // Keeps audio manager across scene loads

        public static FLAudioManager instance;

        void Awake()
        {
            // Singleton pattern to ensure only one instance exists
            if (instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                if (persistAcrossScenes)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }

            // Set up audio sources for each sound object
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.loop = s.loop;
                s.source.outputAudioMixerGroup = mixerGroup;
            }
        }

        // Plays a given sound with optional pitch/volume variance
        public void Play(Sound sound)
        {
            Sound s = sounds.FirstOrDefault(item => item == sound);

            if (s == null)
            {
                Debug.LogWarning("Sound: " + sound + " not found!");
                return;
            }

            // Apply random volume and pitch variation
            s.source.volume = s.volume * (1f + Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

            s.source.Play();
        }

        // Stops playback of a sound if it's currently playing
        public void StopPlaying(Sound sound)
        {
            Sound s = sounds.FirstOrDefault(item => item == sound);
            if (s == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }

            // Still applies variance even when stopping (optional, though might be unnecessary here)
            s.source.volume = s.volume * (1f + Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
            s.source.pitch = s.pitch * (1f + Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
            s.source.Stop();
        }
    }
}
