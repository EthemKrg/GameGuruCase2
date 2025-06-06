using DG.Tweening;
using Injection;
using UnityEngine;
using Zenject;

/// <summary>
/// Controls the movement of the character, including forward movement and animations.
/// </summary>
public class CharacterMovementController : MonoBehaviour
{
    [Inject] private SignalBus signalBus;

    [SerializeField] private float baseSpeed; // The initial speed of the character
    private float speed = 5f;
    [SerializeField] private float maxSpeed = 4f; // Minimum speed of the character
    [SerializeField] private float speedModifier = 0.1f; // Speed reduction modifier for perfect placements

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    [SerializeField] private bool canMove = false;

    private Tween moveTweenX; // Tween for moving the character

    public bool CanMove
    {
        get { return canMove; }
        set
        {
            canMove = value;
            if (!canMove)
            {
                animator.SetTrigger("dance");
                rb.velocity = Vector3.zero;
            }
            else
            {
                animator.SetTrigger("run");
                animator.transform.localRotation = Quaternion.Euler(0, 0, 0); // Reset rotation when starting to run
                animator.transform.localPosition = new Vector3(0, -1, 0); // Reset position when starting to run
            }
        }
    }

    private void OnEnable()
    {
        signalBus.Subscribe<GameStartedEvent>(() => CanMove = true); // Start movement when the game starts
        signalBus.Subscribe<LevelInitializedEvent>(StartLevel); // Initialize the level
        signalBus.Subscribe<GameOverEvent>(GameOver); // Stop movement when game over
        signalBus.Subscribe<GameSuccessEvent>(GameSuccess); // Trigger success animation
        signalBus.Subscribe<PerfectPlacementEvent>(IncreaseSpeed);
        signalBus.Subscribe<NormalPlacementEvent>(ResetSpeed);
    }

    private void OnDisable()
    {
        signalBus.TryUnsubscribe<GameStartedEvent>(() => CanMove = false); // Stop movement when the game ends
        signalBus.TryUnsubscribe<LevelInitializedEvent>(StartLevel); // Unsubscribe from level initialization
        signalBus.TryUnsubscribe<GameOverEvent>(GameOver); // Unsubscribe from game over event
        signalBus.TryUnsubscribe<GameSuccessEvent>(GameSuccess); // Unsubscribe from success event
        signalBus.TryUnsubscribe<PerfectPlacementEvent>(IncreaseSpeed); // Unsubscribe from perfect placement event
        signalBus.TryUnsubscribe<NormalPlacementEvent>(ResetSpeed); // Unsubscribe from normal placement event
    }

    private void StartLevel()
    {
        CanMove = false; // Disable movement at the start of the level

        speed = baseSpeed; // Store the initial speed

        // Check fail condition every second
        InvokeRepeating(nameof(CheckFailCondition), 1f, 0.5f);
    }

    private void GameSuccess()
    {
        canMove = false; // Stop movement when the game is successful
        animator.SetTrigger("dance");
    }

    private void GameOver()
    {
        rb.isKinematic = true; // Stop the character's movement
        animator.SetTrigger("default");
    }

    /// <summary>
    /// Called once per frame. Updates the player's movement if canMove is true.
    /// </summary>
    void Update()
    {
        if (canMove)
        {
            // Maintain the current Y-axis velocity while moving forward
            rb.velocity = new Vector3(transform.forward.x * speed, rb.velocity.y, transform.forward.z * speed);
        }
        else
        {
            // Stop horizontal movement but maintain the current Y-axis velocity
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    public void MoveToXPos(float xPos)
    {
        if(moveTweenX != null)
        {
            moveTweenX.Kill(); // Kill the previous tween if it exists
        }

        moveTweenX = transform.DOMoveX(xPos, 1f);
    }

    private void CheckFailCondition()
    {
        if (transform.position.y <= -7f)
        {
            Debug.Log("Game Over!"); // Log game over message
            CancelInvoke(nameof(CheckFailCondition));
            signalBus.Fire(new GameOverEvent()); // Fire game over event if the character falls
        }
    }

    private void IncreaseSpeed()
    {
        speed += speedModifier; // Decrease speed for perfect placement

        speed = Mathf.Clamp(speed, baseSpeed, maxSpeed); // Ensure speed does not go below minimum
    }

    private void ResetSpeed()
    {
        speed = baseSpeed; // Reset speed to the initial value
    }
}
