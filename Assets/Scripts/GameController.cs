using Injection;
using UnityEngine;
using Zenject;

public class GameController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

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
        Debug.Log("Tap Event Fired");
        if (!isGameStarted)
        {
            Debug.Log("Game Started");
            isGameStarted = true;
            signalBus.Fire(new GameStartedEvent());

            return;
        }
    }
}
