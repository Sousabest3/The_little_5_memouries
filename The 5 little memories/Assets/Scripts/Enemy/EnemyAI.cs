using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    public string enemyID;
    public BattleEncounterManager encounterManager;
    public string battleSceneName = "Battle";

    [Header("Movimento")]
    public float moveSpeed = 2f;
    public float chaseRange = 5f;

    [Header("Patrulha")]
    public Transform[] patrolPoints;
    private int currentPatrolIndex = 0;

    private Transform player;
    private bool isChasing = false;
    private Rigidbody2D rb;

    private void Start()
    {
        if (EnemyStateManager.Instance != null && EnemyStateManager.Instance.IsDefeated(enemyID))
        {
            Destroy(gameObject);
            return;
        }

        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPatrolIndex];
        Vector2 direction = (target.position - transform.position).normalized;

        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        // Se está muito próximo do ponto atual, vai para o próximo
        if (Vector2.Distance(transform.position, target.position) < 0.1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (encounterManager != null)
        {
            encounterManager.ChooseRandomEnemy();
        }

        if (SceneMemory.Instance != null)
        {
            SceneMemory.Instance.lastSceneName = SceneManager.GetActiveScene().name;
            SceneMemory.Instance.lastEnemyID = enemyID;
        }

        SceneManager.LoadScene(battleSceneName);
    }
}
