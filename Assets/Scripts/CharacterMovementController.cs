using UnityEngine;

public class CharacterMovementController : MonoBehaviour
{
    // Speed of the player movement
    public float speed = 5f;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private Animator animator;

    [SerializeField] private bool canMove = false;
    public bool CanMove
    {
        get { return canMove; }
        set
        {
            canMove = value;
            if (!canMove)
            {
                animator.SetTrigger("dance");
                rb.velocity = Vector3.zero; // Stop movement when canMove is false
            }
            else
            {
                animator.SetTrigger("run");
            }
        }
    }

    private void Start()
    {
        CanMove = false; // Initialize canMove to false
    }

    /// <summary>
    /// Called once per frame. Updates the player's movement if canMove is true.
    /// </summary>
    void Update()
    {
        if (canMove)
        {
            // Set the velocity to move the player in a straight direction
            rb.velocity = transform.forward * speed;
        }
        else
        {
            // Stop the player's movement
            rb.velocity = Vector3.zero;
        }
    }
}
