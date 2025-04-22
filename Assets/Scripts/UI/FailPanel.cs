using DG.Tweening;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class FailPanel : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private Button restartButton;

    private void OnEnable()
    {
        signalBus.Subscribe<GameOverEvent>(FailTriggered);

        SetDisabled();
        restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<GameOverEvent>(FailTriggered);
    }

    private void OnRestartButtonClicked()
    {
        // Reload the current scene
        DOTween.KillAll();
        Debug.Log("Restarting the game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void FailTriggered()
    {
        canvasGroup.DOFade(1, fadeDuration)
        .OnComplete(() =>
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        });
    }

    private void SetDisabled()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }
}
