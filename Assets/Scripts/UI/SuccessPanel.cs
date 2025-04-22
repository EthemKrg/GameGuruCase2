using Injection;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// Handles the success panel logic in the game.
/// </summary>
/// <remarks>
/// This class is responsible for displaying the success panel when the player succeeds and providing a next level button.
/// </remarks>
public class SuccessPanel : Panel
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private Button nextLevelButton;

    private void OnEnable()
    {
        signalBus.Subscribe<GameSuccessEvent>(SuccessTriggered);

        // Use the inherited SetDisabled method to initialize the panel
        SetDisabled();
        nextLevelButton.onClick.AddListener(OnNextLevelButtonClicked);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<GameSuccessEvent>(SuccessTriggered);

        // Remove the listener to prevent duplicate calls
        nextLevelButton.onClick.RemoveListener(OnNextLevelButtonClicked);
    }

    /// <summary>
    /// Handles the next level button click event by loading the next scene.
    /// </summary>
    private void OnNextLevelButtonClicked()
    {

    }

    /// <summary>
    /// Displays the success panel when the player succeeds.
    /// </summary>
    private void SuccessTriggered()
    {
        base.SetEnabled(); // Call the inherited method to enable the panel
    }
}
