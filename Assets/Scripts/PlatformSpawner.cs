using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

public class PlatformSpawner : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private ObjectPool objectPool;

    private float basePlatformSpeed;
    [SerializeField] private float platformSpeed = 3f;
    [SerializeField] private float minPlatformSpeed = 0.4f;
    [SerializeField] private float platformSpeedModifier = 0.1f;

    [SerializeField] private int spawnXpos = 5;
    [SerializeField] private Vector3 platformSize = new Vector3(3f, 0.6f, 3f);

    [SerializeField] private Vector3 nextPlatformPosition = Vector3.zero;
    
    private GameObject previousPlatform;
    public GameObject PreviousPlatform => previousPlatform;

    private GameObject currentPlatform;
    public GameObject CurrentPlatform => currentPlatform;


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

    private void StartLevel()
    {
        basePlatformSpeed = platformSpeed;

        previousPlatform = SpawnPlatformAtPoint(nextPlatformPosition);

        SpawnAndMovePlatform();
    }

    private void SpawnAndMovePlatform()
    {
        previousPlatform = currentPlatform;

        currentPlatform = objectPool.GetObject(ObjectPool.ObjectType.Platform);

        currentPlatform.transform.localPosition = nextPlatformPosition;

        float targetXpos = nextPlatformPosition.x * -1f;

        Debug.Log($"Moving platform from {nextPlatformPosition.x} to {targetXpos}");
        currentPlatform.transform.DOLocalMoveX(targetXpos, platformSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

        IncrementNextPosition();
    }

    private GameObject SpawnPlatformAtPoint(Vector3 point)
    {
        currentPlatform = objectPool.GetObject(ObjectPool.ObjectType.Platform);
        currentPlatform.transform.position = point;
        currentPlatform.SetActive(true);

        IncrementNextPosition();

        return currentPlatform;
    }

    private void IncrementNextPosition()
    {
        spawnXpos = spawnXpos * -1;

        nextPlatformPosition = new Vector3(spawnXpos, 0, nextPlatformPosition.z + platformSize.z);
    }

    public void ResetPlatformSpeedAndSpawnNew()
    {
        platformSpeed = basePlatformSpeed;

        SpawnAndMovePlatform();
    }

    public void IncreasePlatformSpeedAndSpawnNew()
    {
        platformSpeed -= platformSpeedModifier;

        platformSpeed = Mathf.Clamp(platformSpeed, minPlatformSpeed, basePlatformSpeed);

        SpawnAndMovePlatform();
    }
}
