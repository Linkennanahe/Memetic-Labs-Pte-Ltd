using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordAttack : MonoBehaviour
{
    public Collider2D swordCollider;
    Vector2 rightAttackOffset;
    void Start()
    {
        swordCollider = GetComponent<Collider2D>();
        rightAttackOffset = transform.localPosition;

    }
    public void AttackLeft()
    {
        print("Attack Left!");
        swordCollider.enabled = true;
        transform.localPosition = new Vector3(rightAttackOffset.x * -1, rightAttackOffset.y);
        
    }
    public void AttackRight()
    {
        print("Attack Right!");
        swordCollider.enabled = true;
        transform.localPosition = rightAttackOffset;
    }
		public void StopAttack()
    {
        swordCollider.enabled = false;
    }
    public float damage = 3;

    // ...

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.Health -= damage;
            }
        }
    }
}
