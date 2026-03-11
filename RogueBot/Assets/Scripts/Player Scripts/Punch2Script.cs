using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch2Script : MonoBehaviour
{
    string ENEMY_TAG = "Enemy";
    float attackDamage;
    Transform player;
    [SerializeField] PlayerAttack playerAttack;


    private void Awake()
    {
        // Gets the transform of the Player GameObject to find the direction the knockback
        // should be
        //
        player = transform.parent.parent;

        playerAttack = player.GetComponent<PlayerAttack>();
    }
    private void Start()
    {
        attackDamage = playerAttack.AttackDamage2;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(ENEMY_TAG))
        {
            EnemyKnockback enemyKnockback = collision.gameObject.GetComponent<EnemyKnockback>();
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            
            // Only knocks back enemy if the second punch is the
            // finishing blow
            // .
            if (attackDamage >= enemy.enemyCurrentHealth)
            {
                // Sets the knockback boolean to true in enemyKnockback Script, this will trigger
                // the knockback function for the enemy
                // .
                enemyKnockback.enemyIsKnockback = true;
            }
            playerAttack.hitSound2.Play();
            enemyKnockback.directionPunch = (collision.gameObject.transform.position - player.position).normalized;
            enemy.StartCoroutine("EnemyTakeDamage", attackDamage);
        }
    }
}
