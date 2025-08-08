using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    Vector2 movement;
    Rigidbody2D rb;
    

    public float moveSpeed = 1f;
    public ContactFilter2D movementFilter;
    public float collisionOffset = 0.05f;
    List<RaycastHit2D> castCollisions= new List<RaycastHit2D>();

    Animator animator;
    SpriteRenderer spriteRenderer;

    public bool canMove = true;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    

    // Update is called once per frame
    void Update()
    {

        
    }
    // FixedUpdate handles all event involves physics
    void FixedUpdate()
    {
        if(!canMove) return;
        // If there is no movement, play player_idle
        if (movement == Vector2.zero)
        {
            animator.SetBool("IsMoving",false);
        }
        else
        {
            //flip the x when x<0 so player sprite will face the other direction
            spriteRenderer.flipX = movement.x<0;
        }
        int count = checkCollisions(movement);
        
        //if no collision move player
        if (count == 0)
        {
            transform.Translate(movement * Time.deltaTime);
            return;
        }
        else
        {

        }

        //if there is collison, move player in the direction of the first collision
        count = checkCollisions(new Vector2(movement.x,0));
        if(count == 0)
        {
            transform.Translate(new Vector2(movement.x,0)*Time.deltaTime);
            return;
        }
        count = checkCollisions(new Vector2(0,movement.y));
        if(count == 0)
        {
            transform.Translate(new Vector2(0,movement.y)*Time.deltaTime);
            return;
        }
    }

    int checkCollisions(Vector2 direction)
    {
        int count = rb.Cast(direction,movementFilter,castCollisions,moveSpeed * Time.deltaTime + collisionOffset);
        return count;
    }
    void OnMove(InputValue movementValue)
    {
        animator.SetBool("IsMoving",true);
        movement = movementValue.Get<Vector2>();
        
    }
    void OnFire()
    {
        animator.SetTrigger("Attack");
    }

    public void LockMovement()
    {
        canMove = false;
    }
    public void UnlockMovement()
    {
        canMove = true;
    }
    public SwordAttack swordAttack;

    public void SwordAttack()
    {
        LockMovement();
        if (spriteRenderer.flipX)
        {
            swordAttack.AttackLeft();
        }
        else
        {
            swordAttack.AttackRight();
        }
    }

    public void StopAttack()
    {
        UnlockMovement();
        swordAttack.StopAttack();
    }
}
