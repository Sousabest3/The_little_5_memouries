using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator))]
public class EnemyAI : MonoBehaviour
{
    public float patrolRadius = 3f;
    public float moveSpeed = 2f;
    public float chaseDistance = 5f;
    public Transform player;
    public string battleSceneName;

    private Vector2 patrolCenter;
    private Vector2 patrolTarget;
    private Rigidbody2D rb;
    private Animator animator;

    private bool isChasing = false;
    private bool isInBattle = false;

    private void Start()
    {
        patrolCenter = transform.position;
        patrolTarget = GetRandomPatrolPoint();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isInBattle || player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseDistance)
        {
            isChasing = true;
        }
        else if (distanceToPlayer > chaseDistance * 1.2f)
        {
            isChasing = false;
        }

        if (isChasing)
            ChasePlayer();
        else
            Patrol();
    }

    private void Patrol()
    {
        if (Vector2.Distance(transform.position, patrolTarget) < 0.2f)
        {
            patrolTarget = GetRandomPatrolPoint();
        }

        MoveTo(patrolTarget);
    }

    private void ChasePlayer()
    {
        MoveTo(player.position);
    }

    private void MoveTo(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);

        // Animations (opcional)
        animator.SetFloat("MoveX", direction.x);
        animator.SetFloat("MoveY", direction.y);
        animator.SetBool("IsMoving", true);
    }

    private Vector2 GetRandomPatrolPoint()
    {
        return patrolCenter + (Random.insideUnitCircle * patrolRadius);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isInBattle) return;

        if (other.CompareTag("Player"))
        {
            isInBattle = true;
            rb.velocity = Vector2.zero;
            animator.SetBool("IsMoving", false);
            SceneTransitionManager.Instance.ChangeScene(battleSceneName, Vector2.zero);
        }
    }
}
