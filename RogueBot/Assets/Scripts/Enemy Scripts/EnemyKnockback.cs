using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using d = UnityEngine.Debug;

public class EnemyKnockback : MonoBehaviour
{
    public Animator animator;
    Rigidbody2D rb;
    CapsuleCollider2D capsule2d;
    Enemy enemy;
    public LayerMask playerLayer;

    [Header("Enemy Knockback")]
    [SerializeField] float knockbacKDuration = 0.3f;
    public float knockbackForce = 200f;
    public Vector2 directionPunch;
    public float directionPunchYAxis = 0.8f;
    public bool enemyIsKnockback = false; // Trigger a Knockback after getting hit
    public bool inKnockback = false; // Enemy is in the midst of a knockback
    private bool immobileCoroutineRunning = false;
    //private bool knockbackCoroutineRunning = false;

    /*[Header("Knockback Collision")]
    public Transform knockbackColliderPoint;
    public Vector2 knockbackColliderSize;
    public Transform knockbackColliderPointLegs;
    public Vector2 knockbackColliderSizeLegs;
    public bool knockbackCollision_ = false;
    public float knockbackCollisionForce;
    */
    

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        capsule2d = GetComponent<CapsuleCollider2D>();
        enemy = GetComponent<Enemy>();
    }


    private void FixedUpdate()
    {
        // The boolean is set to true under the Punch2Script to trigger the knockback
        // function
        // .
        if (enemyIsKnockback)
        {
            // This will be true until the immobile coroutine (After enemy starts moving again)
            //
            inKnockback = true; 
            //knockbackCollision_ = true;
            Knockback();
        }

        if (inKnockback && !immobileCoroutineRunning && enemy.enemyGroundCheck())
        {
            immobileCoroutineRunning = true;
            //knockbackCollision_ = false;
            StartCoroutine(Immobile());
        }

        //enemyKnockbackCollision();
    }
    /*
    void enemyKnockbackCollision()
    {
        if (false) // need to debug
        {
            Collider2D[] enemies = Physics2D.OverlapBoxAll(knockbackColliderPoint.position, knockbackColliderSize, 90f);
            if (enemies != null)
            {
                foreach (Collider2D enemy in enemies)
                {
                    if (enemy.CompareTag(ENEMY_TAG))
                    {
                        //inKnockback = true; // Enable this to prevent enemy from moving
                        //enemy.GetComponent<Rigidbody2D>().AddForce(directionPunch * (knockbackCollisionForce) * Time.deltaTime, ForceMode2D.Impulse);
                        //enemy.GetComponent<Animator>().SetBool("EnemyIsKnockedBack", true);
                        enemy.GetComponent<EnemyKnockback>().enemyIsKnockback= true;
                        d.Log(enemies);
                        d.Log("HEAD");

                        knockbackCollision_ = false;
                    }
                }
            }

            enemies = Physics2D.OverlapBoxAll(knockbackColliderPointLegs.position, knockbackColliderSizeLegs, 0f);
            if (enemies != null)
            {
                foreach (Collider2D enemy in enemies)
                {
                    if (enemy.CompareTag(ENEMY_TAG))
                    {
                        // inKnockback = true; // Enable this to prevent enemy from moving
                        //enemy.GetComponent<Rigidbody2D>().AddForce(directionPunch * (knockbackCollisionForce) * Time.deltaTime, ForceMode2D.Impulse);
                        //enemy.GetComponent<Animator>().SetBool("EnemyIsKnockedBack", true);
                        enemy.GetComponent<EnemyKnockback>().enemyIsKnockback= true;
                        d.Log(directionPunch);
                        d.Log("LEG");
                        knockbackCollision_ = false;
                    }
                }
            }
            
        }
    }
    */

    // Knockback() is triggered in the fixedUpdate()
    //
    bool knockbackCoroutine = false;
    // Runs if enemyIsKnockback is true under fixedUpdate
    // .
    void Knockback()
    {
        if (!knockbackCoroutine)
        {
            knockbackCoroutine = true;
            //knockbackCollision_ = true; // Sets the domino effect knockback to other enemies
            directionPunch.y = directionPunchYAxis;
            rb.AddForce(directionPunch * knockbackForce * Time.deltaTime, ForceMode2D.Impulse);
            animator.SetBool("EnemyIsKnockedBack", true);


            enemyIsKnockback = false; // Disable this to let enemy move again
            knockbackCoroutine = false;
        }
    }

    // Runs when inKnockback is true and if it's on the ground
    // .
    IEnumerator Immobile()
    {
        capsule2d.enabled= false;
        yield return new WaitForSeconds(1.5f);
        enemy.enemyMove(0f);

        // if the enemy got knocked back and is dead, enemy will stay down and
        // be stuck in the animation pose until Game Object is destroyed. These
        // are set in the Enemy script.
        //
        // if enemy isn't dead, enemy will get back up
        //
        if (enemy.enemyCurrentHealth > 0)
        {
            yield return new WaitForSeconds(0.8f);
            capsule2d.enabled = true;

            // Enemy gets up and isn't in a knockback anymore
            // .
            animator.SetBool("EnemyIsKnockedBack", false);
            inKnockback = false;

            immobileCoroutineRunning = false;
        }
        // if enemy is dead, the currenthealth would be 0, which triggers the
        // enemydie() in the enemy script.
        //
        else
            enemy.enemyDie();

    }


    /*private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(knockbackColliderPoint.position, knockbackColliderSize);
        Gizmos.DrawWireCube(knockbackColliderPointLegs.position, knockbackColliderSizeLegs);
    }*/
}
