using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Vector2 movement;
    Rigidbody2D rb;
    public float Health
    {
        set
        {
            health = value;
            if (health <= 0)
            {
                Defeated();
            }
        }
        get { return health; }
    }

    public float health = 1;


    Animator animator;
    public float moveSpeed = 1f;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void Defeated()
    {
        animator.SetTrigger("defeated");
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        // Move the enemy in a random direction
        Vector2 movement = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        rb.AddForce(movement.normalized * moveSpeed * Time.deltaTime);
    }
    
}

