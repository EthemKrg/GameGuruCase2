using Injection;
using UnityEngine;
using Zenject;

public class GameController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private PlatformSpawner platformSpawner;

    private bool isGameStarted = false;

    private void OnEnable()
    {
        signalBus.Subscribe<TapEvent>(TapEventFired);
        signalBus.Subscribe<LevelInitializedEvent>(()=> isGameStarted = false);
    }

    private void OnDisable()
    {
        signalBus.TryUnsubscribe<TapEvent>(TapEventFired);
        signalBus.TryUnsubscribe<LevelInitializedEvent>(() => isGameStarted = false);
    }

    private void Start()
    {
        signalBus.Fire(new LevelInitializedEvent());
    }

    private void TapEventFired()
    {
        if (!isGameStarted)
        {
            Debug.Log("Game Started");
            isGameStarted = true;
            signalBus.Fire(new GameStartedEvent());
        }

        signalBus.Fire(new StopMovingPlatformEvent());
    }
}
