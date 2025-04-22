using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

public class PlatformPlacer : MonoBehaviour
{
    [Inject] private SignalBus signalBus;
    [Inject] private PlatformSpawner platformSpawner;

    [SerializeField] private float perfectPlacementTolerance = 0.1f;

    private void OnEnable()
    {
        signalBus.Subscribe<StopMovingPlatformEvent>(StopMovingPlatform);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<StopMovingPlatformEvent>(StopMovingPlatform);
    }


    private void StopMovingPlatform()
    {
        GameObject previousPlatform = platformSpawner.PreviousPlatform;
        GameObject currentPlatform = platformSpawner.CurrentPlatform;

        if(currentPlatform != null)
            currentPlatform.transform.DOKill();

        if (previousPlatform != null && currentPlatform != null)
        {
            bool isPerfectPlacement = IsPerfectPlacement(previousPlatform, currentPlatform);

            if (isPerfectPlacement)
            {
                // Handle perfect placement logic
                Vector3 newPosition = currentPlatform.transform.position;
                newPosition.x = previousPlatform.transform.position.x;  
                currentPlatform.transform.position = newPosition;

                Debug.Log("Perfect placement. Stopping movement...");
                signalBus.Fire(new PerfectPlacementEvent());
            }
            else
            {
                // Handle imperfect placement logic
                signalBus.Fire(new NormalPlacementEvent());

                Debug.Log("Imperfect placement. Adjusting...");
            }
        }
    }


    private bool IsPerfectPlacement(GameObject previousPlatform, GameObject currentPlatform)
    {
        float differenceX = Mathf.Abs(previousPlatform.transform.position.x - currentPlatform.transform.position.x);

        return Mathf.Abs(differenceX) <= perfectPlacementTolerance;
    }


}
