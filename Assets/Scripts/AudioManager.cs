using Injection;
using UnityEngine;
using Zenject;

public class AudioManager : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private AudioSource perfectPlacementAudioSource;
    [SerializeField] private AudioSource normalPlacementAudioSource;

    [SerializeField] private float pitchIncrement = 0.2f; // Amount to increase pitch on each perfect placement
    [SerializeField] private float maxPitch = 3f; // Maximum pitch value
    [SerializeField] private float defaultPitch = 1f; // Default pitch value

    private void OnEnable()
    {
        signalBus.Subscribe<PerfectPlacementEvent>(PlayPerfectPlacementSound);
        signalBus.Subscribe<NormalPlacementEvent>(PlayNormalPlacementSound);
        signalBus.Subscribe<LevelInitializedEvent>(()=> perfectPlacementAudioSource.pitch = defaultPitch);
    }

    private void OnDisable()
    {
        signalBus.TryUnsubscribe<PerfectPlacementEvent>(PlayPerfectPlacementSound);
        signalBus.TryUnsubscribe<NormalPlacementEvent>(PlayNormalPlacementSound);
        signalBus.TryUnsubscribe<LevelInitializedEvent>(() => perfectPlacementAudioSource.pitch = defaultPitch);
    }

    /// <summary>
    /// Plays the perfect placement sound and increases the pitch.
    /// </summary>
    private void PlayPerfectPlacementSound()
    {
        if (perfectPlacementAudioSource != null)
        {
            // Increase the pitch, but clamp it to the maximum value
            perfectPlacementAudioSource.pitch = Mathf.Clamp(perfectPlacementAudioSource.pitch + pitchIncrement, defaultPitch, maxPitch);

            // Play the sound
            perfectPlacementAudioSource.Play();
        }
    }

    /// <summary>
    /// Plays the normal placement sound and resets the pitch.
    /// </summary>
    private void PlayNormalPlacementSound()
    {
        if (perfectPlacementAudioSource != null)
        {
            // Reset the pitch to the default value
            perfectPlacementAudioSource.pitch = defaultPitch;
        }

        if (normalPlacementAudioSource != null)
        {
            // Play the normal placement sound
            normalPlacementAudioSource.Play();
        }
    }
}
