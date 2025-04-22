using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

public class TapToStart : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [SerializeField] private CanvasGroup canvasGroup;

    private void OnEnable()
    {
        signalBus.Subscribe<GameStartedEvent>(StartGame);
        signalBus.Subscribe<LevelInitializedEvent>(LevelInitialized);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<GameStartedEvent>(StartGame);
        signalBus.Unsubscribe<LevelInitializedEvent>(LevelInitialized);
    }

    private void LevelInitialized()
    {
        canvasGroup.DOFade(1, 1f);
    }

    private void StartGame()
    {
        canvasGroup.DOFade(0, 1f);
    }
}
