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
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<TapEvent>(TapEventFired);
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
