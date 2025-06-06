using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    public bool canMove = true;
    private Vector2 lastNonZeroInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (PauseController.IsGamePaused)
        {
            StopMovement();
            return;
        }

        if (moveInput != Vector2.zero)
        {
            lastNonZeroInput = moveInput;
        }

        animator.SetBool("IsWalking", canMove && moveInput != Vector2.zero);

        if (canMove)
        {
            rb.velocity = moveInput * moveSpeed;
        }
        else
        {
            StopMovement();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (!canMove || PauseController.IsGamePaused) return;

        Vector2 rawInput = context.ReadValue<Vector2>();

        // Limita a 4 direções (horizontal OU vertical)
        if (Mathf.Abs(rawInput.x) > Mathf.Abs(rawInput.y))
        {
            moveInput = new Vector2(Mathf.Sign(rawInput.x), 0f);
        }
        else if (Mathf.Abs(rawInput.y) > 0)
        {
            moveInput = new Vector2(0f, Mathf.Sign(rawInput.y));
        }
        else
        {
            moveInput = Vector2.zero;
        }

        if (context.canceled)
        {
            animator.SetBool("IsWalking", false);
            animator.SetFloat("LastInputX", lastNonZeroInput.x);
            animator.SetFloat("LastInputY", lastNonZeroInput.y);
        }
        else
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
        }

        if (moveInput != Vector2.zero)
        {
            lastNonZeroInput = moveInput;
        }
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
        if (!canMove)
        {
            StopMovement();
        }
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("IsWalking", false);
    }

    public Vector3 GetCurrentPosition()
    {
        return transform.position;
    }
}
