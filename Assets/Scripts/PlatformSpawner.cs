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

    private GameObject currentPlatform;
    public GameObject CurrentPlatform => currentPlatform;

    private void OnEnable()
    {
        signalBus.Subscribe<PerfectPlacementEvent>(IncreasePlatformSpeedAndSpawnNew);
        signalBus.Subscribe<NormalPlacementEvent>(ResetPlatformSpeedAndSpawnNew);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<PerfectPlacementEvent>(IncreasePlatformSpeedAndSpawnNew);
        signalBus.Unsubscribe<NormalPlacementEvent>(ResetPlatformSpeedAndSpawnNew);
    }

    private void Start()
    {
        basePlatformSpeed = platformSpeed;

        SpawnPlatformAtPoint(nextPlatformPosition);
        IncrementNextPosition();

        SpawnAndMovePlatform();
    }

    private void SpawnAndMovePlatform()
    {
        GameObject platform = objectPool.GetObject(ObjectPool.ObjectType.Platform);

        platform.transform.localPosition = nextPlatformPosition;

        float targetXpos = nextPlatformPosition.x * -1f;
        platform.transform.DOLocalMoveX(targetXpos, platformSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

        IncrementNextPosition();
    }

    private void SpawnPlatformAtPoint(Vector3 point)
    {
        GameObject platform = objectPool.GetObject(ObjectPool.ObjectType.Platform);
        platform.transform.position = point;
        platform.SetActive(true);
    }

    private void IncrementNextPosition()
    {
        spawnXpos = spawnXpos * -1;

        nextPlatformPosition += new Vector3(spawnXpos, 0, platformSize.z);
    }

    private void ResetPlatformSpeedAndSpawnNew()
    {
        platformSpeed = basePlatformSpeed;

        SpawnAndMovePlatform();
    }

    private void IncreasePlatformSpeedAndSpawnNew()
    {
        platformSpeed -= platformSpeedModifier;

        platformSpeed = Mathf.Clamp(platformSpeed, minPlatformSpeed, basePlatformSpeed);

        SpawnAndMovePlatform();
    }
}
