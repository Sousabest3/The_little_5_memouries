using System.Collections;
// AdvancedNPCFollower.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AdvancedNPCFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float followSpeed = 2.5f;
    public float minFollowDistance = 1.2f;
    public float maxFollowDistance = 3f;
    public float smoothTime = 0.2f;

    [Header("Visual Feedback")]
    public Animator animator;
    public bool flipSprite = true;

    private Rigidbody2D rb;
    private Vector2 currentVelocity;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (target == null) return;

        Vector2 direction = target.position - transform.position;
        float distance = direction.magnitude;

        // Atualiza animação
        if (animator != null)
        {
            animator.SetBool("IsMoving", distance > minFollowDistance);
        }

        // Flip sprite baseado na direção
        if (flipSprite && spriteRenderer != null)
        {
            spriteRenderer.flipX = direction.x < 0;
        }

        // Movimento físico suave
        if (distance > maxFollowDistance)
        {
            Vector2 targetPosition = (Vector2)target.position - direction.normalized * minFollowDistance;
            rb.velocity = Vector2.SmoothDamp(rb.velocity, 
                (targetPosition - (Vector2)transform.position) * followSpeed, 
                ref currentVelocity, 
                smoothTime);
        }
        else if (distance < minFollowDistance)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
