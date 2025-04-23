using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

/// <summary>
/// Handles the spawning and movement of platforms in the game.
/// </summary>
public class PlatformSpawner : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private ObjectPool objectPool;
    [Inject] private FinishController finishController;
    [Inject] private PlatformColor platformColor;
    [Inject] private CharacterMovementController characterMovementController;

    [SerializeField] private float basePlatformSpeed; // The initial speed of the platform movement
    private float platformSpeed = 3f; // Current speed of the platform movement
    [SerializeField] private float minPlatformSpeed = 0.4f; // Minimum speed of the platform movement
    [SerializeField] private float platformSpeedModifier = 0.1f; // Speed reduction modifier for perfect placements

    [SerializeField] private int spawnXpos = 5; // X position for spawning platforms

    [SerializeField] private Vector3 platformSize = new Vector3(3f, 0.6f, 3f); // Size of the platform
    public Vector3 PlatformSize => platformSize; // Public getter for the platform size

    [SerializeField] private Vector3 nextPlatformPosition = Vector3.zero; // Position for the next platform to spawn
    public Vector3 NextPlatformPosition => nextPlatformPosition; // Public getter for the next platform position

    private GameObject previousPlatform; // Reference to the last spawned platform
    public GameObject PreviousPlatform => previousPlatform; // Public getter for the previous platform

    private GameObject currentPlatform; // Reference to the currently moving platform
    public GameObject CurrentPlatform => currentPlatform; // Public getter for the current platform

    private int spawnCount = 0; // Counter for the number of platforms will be spawned

    private void OnEnable()
    {
        signalBus.Subscribe<LevelInitializedEvent>(StartLevel);
        signalBus.Subscribe<PerfectPlacementEvent>(IncreasePlatformSpeedAndSpawnNew);
        signalBus.Subscribe<NormalPlacementEvent>(ResetPlatformSpeedAndSpawnNew);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<LevelInitializedEvent>(StartLevel);
        signalBus.Unsubscribe<PerfectPlacementEvent>(IncreasePlatformSpeedAndSpawnNew);
        signalBus.Unsubscribe<NormalPlacementEvent>(ResetPlatformSpeedAndSpawnNew);
    }

    /// <summary>
    /// Initializes the level by spawning the first platform and starting the platform movement.
    /// </summary>
    private void StartLevel()
    {
        platformSpeed = basePlatformSpeed;
        spawnCount = finishController.PlatformDistanceToFinish; // Set the spawn count based on the distance to the finish line

        nextPlatformPosition = characterMovementController.transform.position; // Set the next platform position to the player's position
        nextPlatformPosition.y = 0;

        finishController.SetFinishPosition(); // Set the finish line position

        previousPlatform = SpawnPlatformAtPoint(nextPlatformPosition);
        SpawnAndMovePlatform();
    }

    /// <summary>
    /// Spawns a new platform and starts its movement.
    /// </summary>
    private void SpawnAndMovePlatform()
    {
        if (spawnCount <= 0)
        {
            return;
        }
        spawnCount--;

        previousPlatform = currentPlatform;

        currentPlatform = objectPool.GetObject(ObjectPool.ObjectType.Platform);
        currentPlatform.transform.DOKill(); // Stop any ongoing animations on the platform
        currentPlatform.transform.localEulerAngles = Vector3.zero; // Reset rotation
        currentPlatform.transform.localScale = previousPlatform.transform.localScale; // Match the scale of the previous platform
        currentPlatform.transform.localPosition = nextPlatformPosition;

        currentPlatform.GetComponent<MeshRenderer>().material = platformColor.GetNextMaterial();

        float targetXpos = nextPlatformPosition.x * -1f;
        currentPlatform.transform.DOLocalMoveX(targetXpos, platformSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.Linear);

        IncrementNextPosition();
    }

    /// <summary>
    /// Spawns a platform at a specific position.
    /// </summary>
    /// <param name="point">The position to spawn the platform at.</param>
    /// <returns>The spawned platform.</returns>
    private GameObject SpawnPlatformAtPoint(Vector3 point)
    {
        if (spawnCount <= 0)
        {
            return null;
        }
        spawnCount--;

        currentPlatform = objectPool.GetObject(ObjectPool.ObjectType.Platform);
        currentPlatform.transform.DOKill(); // Stop any ongoing animations on the platform
        currentPlatform.transform.localEulerAngles = Vector3.zero; // Reset rotation
        currentPlatform.transform.localScale = platformSize; // Match the scale of the previous platform
        currentPlatform.transform.localPosition = point;

        currentPlatform.GetComponent<MeshRenderer>().material = platformColor.GetNextMaterial();

        IncrementNextPosition();

        return currentPlatform;
    }

    /// <summary>
    /// Updates the position for the next platform to spawn.
    /// </summary>
    private void IncrementNextPosition()
    {
        spawnXpos = spawnXpos * -1; // Alternate the X position for the next platform
        nextPlatformPosition = new Vector3(spawnXpos, 0, nextPlatformPosition.z + platformSize.z);
    }

    /// <summary>
    /// Resets the platform speed to its base value and spawns a new platform.
    /// </summary>
    public void ResetPlatformSpeedAndSpawnNew()
    {
        platformSpeed = basePlatformSpeed;
        SpawnAndMovePlatform();
    }

    /// <summary>
    /// Increases the platform speed and spawns a new platform.
    /// </summary>
    public void IncreasePlatformSpeedAndSpawnNew()
    {
        platformSpeed -= platformSpeedModifier;
        platformSpeed = Mathf.Clamp(platformSpeed, minPlatformSpeed, basePlatformSpeed);
        SpawnAndMovePlatform();
    }
}
