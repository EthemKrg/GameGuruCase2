using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

public class PlatformController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private ObjectPool objectPool;

    private float basePlatformSpeed;
    [SerializeField] private float platformSpeed = 3f;
    [SerializeField] private float minPlatformSpeed = 0.4f;
    [SerializeField] private float platformSpeedModifier = 0.1f;

    [SerializeField] private int spawnXpos = 5;
    [SerializeField] private Vector3 platformSize = new Vector3(3f, 0.6f, 3f);

    private Vector3 nextPlatformPosition = Vector3.zero;

    private void OnEnable()
    {
        signalBus.Subscribe<PerfectPlacementEvent>(IncreasePlatformSpeed);
        signalBus.Subscribe<NormalPlacementEvent>(ResetPlatformSpeed);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<PerfectPlacementEvent>(IncreasePlatformSpeed);
        signalBus.Unsubscribe<NormalPlacementEvent>(ResetPlatformSpeed);
    }

    void Start()
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

        platform.transform.DOLocalMoveX(-spawnXpos, platformSpeed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

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

    private void ResetPlatformSpeed()
    {
        platformSpeed = basePlatformSpeed;
    }

    private void IncreasePlatformSpeed()
    {
        platformSpeed -= platformSpeedModifier;

        platformSpeed = Mathf.Clamp(platformSpeed, minPlatformSpeed, basePlatformSpeed);
    }
}
