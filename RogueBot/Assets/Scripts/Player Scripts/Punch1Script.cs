using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using d = UnityEngine.Debug;

// For RightArmSolver_Target
public class Punch1Script : MonoBehaviour
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

        // Get the instance of the attack script so that it is easier to
        // edit the attack damage without going through the child objects.
        // So it can be edit on the Player object instead.
        //
        playerAttack = player.GetComponent<PlayerAttack>();
    }
    private void Start()
    {
        attackDamage = playerAttack.AttackDamage1;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(ENEMY_TAG))
        {
            collision.gameObject.GetComponent<Enemy>().StartCoroutine("EnemyTakeDamage", attackDamage);
            playerAttack.hitSound1.Play();
        }
    }

}
