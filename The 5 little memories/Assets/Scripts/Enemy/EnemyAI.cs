using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class EnemyAI : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectionRadius = 3f;
    public Vector2 patrolAreaMin;
    public Vector2 patrolAreaMax;

    public string battleSceneName = "BattleScene";

    private Transform player;
    private Vector2 targetPosition;
    private bool isChasing = false;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ChooseNewPatrolTarget();
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectionRadius)
            isChasing = true;
        else if (distance > detectionRadius * 1.5f)
            isChasing = false;

        if (isChasing)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Movimento em 4 direções (sem diagonais)
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                direction.y = 0;
                direction.x = Mathf.Sign(direction.x);
            }
            else
            {
                direction.x = 0;
                direction.y = Mathf.Sign(direction.y);
            }

            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (Vector2.Distance(transform.position, targetPosition) < 0.2f)
        {
            ChooseNewPatrolTarget();
        }

        Vector2 dir = (targetPosition - (Vector2)transform.position).normalized;
        transform.position += (Vector3)dir * moveSpeed * Time.deltaTime;
    }

    private void ChooseNewPatrolTarget()
    {
        float x = Random.Range(patrolAreaMin.x, patrolAreaMax.x);
        float y = Random.Range(patrolAreaMin.y, patrolAreaMax.y);
        targetPosition = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Vector2 playerPos = other.transform.position;

            // Adiciona fade e salva a posição
            if (SceneTransitionManager.Instance != null)
            {
                SceneTransitionManager.Instance.ChangeScene(battleSceneName, playerPos);
            }
            else
            {
                // Backup se não houver transition manager
                UnityEngine.SceneManagement.SceneManager.LoadScene(battleSceneName);
            }
        }
    }
}
