using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    private bool isMoving;

    private Vector2 input;
    
private Animator animator;

public LayerMask solidObjectsLayer;

private void Awake()
{
    animator = GetComponent<Animator>();
}

    private void Update()
    {
        if (!isMoving)
        {
            // Reset input
            input = Vector2.zero;

            // Check for specific key inputs
            if (Input.GetKey(KeyCode.W)) input.y = 1;  // Move up
            if (Input.GetKey(KeyCode.S)) input.y = -1; // Move down
            if (Input.GetKey(KeyCode.A)) input.x = -1; // Move left
            if (Input.GetKey(KeyCode.D)) input.x = 1;  // Move right
            Debug.Log("This is input.x" + input.x);
            Debug.Log("This is input.y" + input.y);

            

            if (input != Vector2.zero)
            {
               animator.SetFloat("MoveX", input.x);
           animator.SetFloat("MoveY", input.y);
                
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if(IsWalkable(targetPos))
                StartCoroutine(Move(targetPos));
            }
        }
        animator.SetBool("IsMoving", isMoving);

    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null)
        {
            return false;

        }
        return true;

    }
}
