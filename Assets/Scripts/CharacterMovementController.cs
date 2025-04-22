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

    public float speed = 5f;

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
            }
        }
    }

    private void OnEnable()
    {
        signalBus.Subscribe<GameStartedEvent>(() => CanMove = true); // Start movement when the game starts
        signalBus.Subscribe<LevelInitializedEvent>(StartLevel); // Initialize the level
        signalBus.Subscribe<GameOverEvent>(GameOver); // Stop movement when game over
    }

    private void OnDisable()
    {
        signalBus.TryUnsubscribe<GameStartedEvent>(() => CanMove = false); // Stop movement when the game ends
        signalBus.TryUnsubscribe<LevelInitializedEvent>(StartLevel); // Unsubscribe from level initialization
        signalBus.TryUnsubscribe<GameOverEvent>(GameOver); // Unsubscribe from game over event
    }

    private void StartLevel()
    {
        CanMove = false; // Disable movement at the start of the level

        // Check fail condition every second
        InvokeRepeating(nameof(CheckFailCondition), 1f, 0.5f);
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
}
