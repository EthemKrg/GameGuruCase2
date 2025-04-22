using DG.Tweening;
using Injection;
using System.Collections;
using System.Collections.Generic;
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
        GameObject previousPlatform = platformSpawner.CurrentPlatform;
        DOTween.Kill(platformSpawner.CurrentPlatform);

        bool isPerfectPlacement = false;


    }

}
