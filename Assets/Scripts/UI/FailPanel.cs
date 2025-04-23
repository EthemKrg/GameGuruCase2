using DG.Tweening;
using Injection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Handles the fail panel logic in the game.
/// </summary>
/// <remarks>
/// This class is responsible for displaying the fail panel when the player fails and providing a restart button.
/// </remarks>
public class FailPanel : Panel
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private Button restartButton;

    private void OnEnable()
    {
        signalBus.Subscribe<GameOverEvent>(FailTriggered);

        // Use the inherited SetDisabled method to initialize the panel
        SetDisabled();

        restartButton.onClick.AddListener(OnRestartButtonClicked);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<GameOverEvent>(FailTriggered);

        // Remove the listener to prevent duplicate calls
        restartButton.onClick.RemoveListener(OnRestartButtonClicked);
    }

    /// <summary>
    /// Handles the restart button click event by reloading the current scene.
    /// </summary>
    private void OnRestartButtonClicked()
    {
        // Kill all active DOTween animations
        DOTween.KillAll();

        Debug.Log("Restarting the game...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Displays the fail panel when the player fails.
    /// </summary>
    private void FailTriggered()
    {
        base.SetEnabled(); // Call the inherited method to enable the panel
    }
}
