using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Animator animator;
    public bool canMove = true;
    private Vector2 lastNonZeroInput; // Armazena a última direção válida

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(PauseController.IsGamePaused)
        {
            StopMovement();
            return;
        }

        // Atualiza a última direção válida quando há movimento
        if(moveInput != Vector2.zero)
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

        moveInput = context.ReadValue<Vector2>();
        
        if (context.canceled)
        {
            animator.SetBool("IsWalking", false);
            // Usa a última direção válida ao invés da direção atual (que é zero)
            animator.SetFloat("LastInputX", lastNonZeroInput.x);
            animator.SetFloat("LastInputY", lastNonZeroInput.y);
        }
        else
        {
            animator.SetFloat("InputX", moveInput.x);
            animator.SetFloat("InputY", moveInput.y);
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
}