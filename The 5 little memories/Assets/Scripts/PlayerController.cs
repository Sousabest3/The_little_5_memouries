using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;

    private bool isMoving;

    private Vector2 input;
    
//private Animator animator;

//private void Awake()
//{
    //animator = GetComponent<Animator>();
//}

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
               // animator.SetFloat("moveX", input.x);
           // animator.SetFloat("moveY", input.y);
                
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                StartCoroutine(Move(targetPos));
            }
        }
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
}
