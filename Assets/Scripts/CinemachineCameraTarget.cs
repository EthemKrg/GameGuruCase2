using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

public class CinemachineCameraTarget : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private float rotationSpeed = 10f; // Speed of rotation around the target
    [SerializeField] private float resetDuration = 1f; // Duration for resetting the rotation

    private Tween tween; // Tween instance for managing animations
    private Quaternion initialRotation; // Stores the initial rotation of the camera target

    private void OnEnable()
    {
        signalBus.Subscribe<GameSuccessEvent>(RotateAround);
        signalBus.Subscribe<LevelInitializedEvent>(ResetRotation);
    }

    private void OnDisable()
    {
        signalBus.Unsubscribe<GameSuccessEvent>(RotateAround);
        signalBus.Unsubscribe<LevelInitializedEvent>(ResetRotation);
    }

    private void Start()
    {
        // Save the initial rotation of the camera target
        initialRotation = transform.rotation;
    }

    /// <summary>
    /// Rotates the camera target around the follow target.
    /// </summary>
    private void RotateAround()
    {
        // Kill any existing tween to avoid conflicts
        tween?.Kill();

        // Rotate the camera target around the follow target
        tween = DOTween.To(
            () => transform.eulerAngles,
            value => transform.eulerAngles = value,
            new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 360f, transform.eulerAngles.z),
            5f / rotationSpeed // Duration depends on rotation speed
        ).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }

    /// <summary>
    /// Resets the camera target's rotation to its initial state using a tween.
    /// </summary>
    private void ResetRotation()
    {
        // Kill any existing tween to avoid conflicts
        tween?.Kill();

        // Tween the rotation back to the initial rotation
        tween = transform.DORotateQuaternion(initialRotation, resetDuration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                Debug.Log("Rotation reset complete.");
            });
    }
}
