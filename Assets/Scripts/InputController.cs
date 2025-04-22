using Injection;
using UnityEngine;
using Zenject;

public class InputController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;   

    /// <summary>
    /// This method detects mouse clicks and touch inputs.
    /// </summary>
    void Update()
    {
        // Detect mouse clicks
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            HandleInput();
        }
        else if (Input.touchCount > 0) // Detect touch inputs
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began) // When the touch begins
                {
                    HandleInput();
                }
            }
        }
    }

    /// <summary>
    /// Additional actions based on the position can be implemented here.
    /// </summary>
    private void HandleInput()
    {
        signalBus.Fire(new TapEvent());
    }
}
