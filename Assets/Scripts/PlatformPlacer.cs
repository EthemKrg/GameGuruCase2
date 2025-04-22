using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

/// <summary>
/// Handles the placement of platforms and manages their interactions.
/// </summary>
public class PlatformPlacer : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private PlatformSpawner platformSpawner;
    [Inject] private ObjectPool objectPool;
    [Inject] private CharacterMovementController characterMovementController;

    [SerializeField] private float perfectPlacementTolerance = 0.1f; // Tolerance for determining perfect placement

    /// <summary>
    /// Subscribes to necessary signals when the object is enabled.
    /// </summary>
    private void OnEnable()
    {
        signalBus.Subscribe<StopMovingPlatformEvent>(StopMovingPlatform);
    }

    /// <summary>
    /// Unsubscribes from signals when the object is disabled.
    /// </summary>
    private void OnDisable()
    {
        signalBus.Unsubscribe<StopMovingPlatformEvent>(StopMovingPlatform);
    }

    /// <summary>
    /// Stops the movement of the current platform and handles placement logic.
    /// </summary>
    private void StopMovingPlatform()
    {
        GameObject previousPlatform = platformSpawner.PreviousPlatform;
        GameObject currentPlatform = platformSpawner.CurrentPlatform;

        if (currentPlatform != null)
            currentPlatform.transform.DOKill(); // Stop any ongoing animation on the current platform

        if (previousPlatform != null && currentPlatform != null)
        {
            bool isPerfectPlacement = IsPerfectPlacement(previousPlatform, currentPlatform);

            if (isPerfectPlacement)
            {
                // Handle perfect placement logic
                Debug.Log("Perfect placement!");
                Vector3 newPosition = currentPlatform.transform.localPosition;
                newPosition.x = previousPlatform.transform.localPosition.x;
                currentPlatform.transform.localPosition = newPosition;

                signalBus.Fire(new PerfectPlacementEvent());
            }
            else
            {
                // Handle imperfect placement logic
                Debug.Log("Imperfect placement. Cutting...");
                CutExcessPart(previousPlatform, currentPlatform);

                signalBus.Fire(new NormalPlacementEvent());
            }
        }
    }

    /// <summary>
    /// Checks if the current platform is perfectly aligned with the previous platform.
    /// </summary>
    /// <param name="previousPlatform">The previous platform.</param>
    /// <param name="currentPlatform">The current platform.</param>
    /// <returns>True if the placement is perfect, otherwise false.</returns>
    private bool IsPerfectPlacement(GameObject previousPlatform, GameObject currentPlatform)
    {
        float differenceX = Mathf.Abs(previousPlatform.transform.localPosition.x - currentPlatform.transform.localPosition.x);

        return Mathf.Abs(differenceX) <= perfectPlacementTolerance;
    }

    /// <summary>
    /// Cuts the excess part of the current platform that does not overlap with the previous platform.
    /// </summary>
    /// <param name="previousPlatform">The previous platform.</param>
    /// <param name="currentPlatform">The current platform.</param>
    private void CutExcessPart(GameObject previousPlatform, GameObject currentPlatform)
    {
        // Calculate the X-axis difference between the two platforms
        float differenceX = currentPlatform.transform.localPosition.x - previousPlatform.transform.localPosition.x;

        // Get the width of the current platform
        float platformSizeX = currentPlatform.transform.localScale.x;

        // Calculate the width of the overlapping part
        float overlapWidth = platformSizeX - Mathf.Abs(differenceX);
        if (overlapWidth <= 0)
        {
            // If there is no overlap, there is no part to cut
            return;
        }

        float excessWidth = platformSizeX - overlapWidth;

        // Calculate the position of the excess part
        float excessPartXPos = currentPlatform.transform.localPosition.x + Mathf.Sign(differenceX) * (overlapWidth * 0.5f + excessWidth * 0.5f);
        Vector3 excessPartPosition = new Vector3(excessPartXPos, currentPlatform.transform.localPosition.y, currentPlatform.transform.localPosition.z);

        // Set the size of the excess part
        Vector3 excessPartScale = new Vector3(excessWidth, currentPlatform.transform.localScale.y, currentPlatform.transform.localScale.z);

        // Create the excess part
        GameObject excessPart = objectPool.GetObject(ObjectPool.ObjectType.Platform);
        excessPart.transform.localPosition = excessPartPosition;
        excessPart.transform.localScale = excessPartScale;

        // Drop the excess part downwards with more realistic effects
        excessPart.transform.DOMoveY(-5f, 1f)
            .SetEase(Ease.InQuad) // Add acceleration for a more natural fall
            .OnComplete(() =>
            {
                objectPool.ReturnObject(excessPart); // Return the excess part to the object pool after the animation
            });

        // Add rotation to the falling part
        float randomRotationSpeed = Random.Range(100f, 300f); // Randomize rotation speed
        excessPart.transform.DORotate(new Vector3(0, 0, 360f), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear) // Keep rotation linear for a consistent spin
            .SetLoops(-1, LoopType.Incremental); // Ensure continuous rotation during the fall


        // Update the size of the current platform
        float newWidth = overlapWidth;
        currentPlatform.transform.localScale = new Vector3(newWidth, currentPlatform.transform.localScale.y, currentPlatform.transform.localScale.z);

        // Update the position of the current platform
        float newXPos = currentPlatform.transform.localPosition.x - Mathf.Sign(differenceX) * (excessWidth * 0.5f);
        currentPlatform.transform.localPosition = new Vector3(newXPos, currentPlatform.transform.localPosition.y, currentPlatform.transform.localPosition.z);


        // Update the player's x position to match the new platform position
        characterMovementController.MoveToXPos(newXPos);
    }
}
