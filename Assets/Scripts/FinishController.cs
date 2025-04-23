using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

/// <summary>
/// Handles the finish line logic in the game.
/// </summary>
/// <remarks>
/// This class is responsible for detecting when the player reaches the finish line and triggering the success event.
/// </remarks>
public class FinishController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private PlatformSpawner platformSpawner;

    [SerializeField] private int platformDistanceToFinish = 5; // Distance to the finish line
    public int PlatformDistanceToFinish => platformDistanceToFinish; // Public getter for the distance to the finish line

    private Tween moveTweenX;

    private void Start()
    {
        SetFinishPosition();
    }

    public void SetFinishPosition()
    {
        float zSize = platformSpawner.PlatformSize.z;

        transform.position = new Vector3(0, platformSpawner.NextPlatformPosition.y + 0.34f,
            platformSpawner.NextPlatformPosition.z + (zSize * platformDistanceToFinish - 0.5f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            signalBus.Fire(new GameSuccessEvent());
        }
    }

    public void MoveToXPos(float xPos)
    {
        if (moveTweenX != null)
        {
            moveTweenX.Kill(); // Kill the previous tween if it exists
        }

        moveTweenX = transform.DOMoveX(xPos, 1f);
    }
}
