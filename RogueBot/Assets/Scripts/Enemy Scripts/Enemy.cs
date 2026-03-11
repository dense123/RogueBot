using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using d = UnityEngine.Debug;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    string PLAYER_TAG = "Player";
    string ENEMY_TAG = "Enemy";
    string GROUND_TAG = "Ground";
    string SPEED_ENEMY_ANIM = "Speed";

    public Animator animator;
    Rigidbody2D rb;
    CapsuleCollider2D capsule2d;
    EnemyKnockback enemyKnockback;
    GameManagerScript gameManager;
    public LayerMask playerLayer;
    public LayerMask GroundLayer; // To detect ground
    public GameObject DamageTextPrefab; // For when enemy gets hit, the damage taken is shown
    //public Transform player;

    [Header("Enemy Health")]
    public float enemyMaxHealth = 100f;
    public float enemyCurrentHealth;
    private int enemyDamage = 1;            // Sends 1 attack damage to player (Hearts)

    [Header("Movement")]
    Vector2 moveDirection = Vector2.right;  // Spawned facing right
    Vector2 localScale;                     // For flipping the enemy sprite

    [Header("Movement towards player")]
    public float sightDistance = 6f;        // Raycast distance
    public Transform castPoint;             // For raycast
    public float moveSpeed = 100f;
    public float attackSpeed = 200f;
    public float castRadius = 3.2f;
    public Vector2 boxCastRange;            // For checking platforms in front of enemy, not below
    bool groundCheck_ = false;              // Check ground below enemy

    [Header("Check if falling/jumping")] 
    [SerializeField] Transform GroundCheckBox;
    [SerializeField] Vector2 GroundCheckBoxSize;

    public bool endCutscene = false;

    public bool isHurt;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsule2d = GetComponent<CapsuleCollider2D>();
        enemyKnockback= GetComponent<EnemyKnockback>();
        gameManager = FindObjectOfType<GameManagerScript>();
        localScale = transform.localScale; // for flipping of sprite

    }
    void Start()
    {
        enemyCurrentHealth = enemyMaxHealth;
    }
    private void FixedUpdate()
    {
        // Enemy usually are not colliding with each other
        //
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(ENEMY_TAG), LayerMask.NameToLayer(ENEMY_TAG), true);

        if (!endCutscene)
        {
            movement();
            enemyAttack();

        }
    }

    IEnumerator EnemyTakeDamage(float damageByPlayer)
    {
        enemyCurrentHealth -= damageByPlayer;
        animator.SetTrigger("EnemyHurt");
        
        // Shows the damage taken above the enemy. This is to display something for the
        // player to realise that enemy did get hit, and by what amount. The text will
        // be a child game object to the enemy and have an offset in the Damage Text script
        // to display above the enemy's head.
        // . 
        GameObject DamageText;
        DamageText = Instantiate(DamageTextPrefab, transform); 
        DamageText.transform.GetChild(0).GetComponent<TextMesh>().text = damageByPlayer.ToString();
           

        // Set "isHurt" to true so that the enemy will stop moving towards 
        // player once it gets hit. It will start moving after the set amount of time.
        //
        isHurt = true;

        if (enemyCurrentHealth <= 0 && !PlayerAttack.instance.isAttacking)
        {
            enemyDie();
        }

        // Enemy will be immobile for 0.6 seconds to stop them from moving, give player
        // a chance to hit a 2nd attack and make it easy for them
        // .
        yield return new WaitForSeconds(0.6f);
        isHurt = false;
    }
    
    // Checks if the enemy is grounded, as when they get knocked back, they will
    // be in the air for a few milli secconds. This ground check will help run the
    // immobile function to allow them to get up after being grounded, or to check
    // whether they are dead
    // .
    public bool enemyGroundCheck()
    {
        Collider2D ground = Physics2D.OverlapBox(GroundCheckBox.position, GroundCheckBoxSize, 0f, GroundLayer);
        if (ground != null)
        {
            return true;   
        }
        return false;
    }

    // Boolean is to fix the bug where this will run multiple times
    bool runEnemyDie = false;
    public void enemyDie()
    {
        if (!runEnemyDie)
        {
            // disables the capsule collider so that player will not be able
            // to hit the enemy and run into it. 
            // Only the edge collider will hold the enemy on the platform to prevent
            // enemy from falling through
            //
            gameManager.TotalEnemies--;
            animator.SetBool("EnemyIsDead", true);
            capsule2d.enabled= false;
        
            // Use a time delay before running the function in game Manager
            //
            Invoke("enemyDieTimeDelay", 2f); 
            runEnemyDie= true;
            this.enabled = false; // disables this script
        }
    }

    void enemyDieTimeDelay()
    {
        enemyDestroy();

        // Calls the function in game manager script which checks if
        // total enemies are 0
        // .
        gameManager.CompleteLevel();
    }

    void enemyDestroy()
    {
        Destroy(gameObject,3f);
    }

    void movement()
    {
        // If enemy isn't hurt or being knocked back and is grounded, he will move.
        // Enemy won't move if hurt because enemy will continue moving and colliding
        // towards player which will be annoying.
        //
        // Enemy won't move when being knocked back, once he lands, enemy is still in
        // a knockback process for a set amount of time, this is so the player can
        // visualise what had happened compared to if the enemy gets back up instantly
        // after landing from a knockback
        // .
        if (!isHurt && !enemyKnockback.enemyIsKnockback && enemyGroundCheck() 
            && !enemyKnockback.inKnockback)
        {
            enemyMove(moveSpeed);

            // If there is no ground in front, the enemy will turn around
            //
            if (!groundCheck_)
            {
                flipSprite();
                moveDirection = -moveDirection;
            }
        }
    }

    // Enemy raycasting, this will allow enemy to detect player and charge
    // towards player, or detect wall and turn around
    // .
    public void enemyAttack()
    {

        // This is set so that if the enemy is hurt, he would be immobile and stop moving,
        // similar to the if statement under movement() function
        // .
        if (!isHurt && !enemyKnockback.enemyIsKnockback && enemyGroundCheck() && !enemyKnockback.inKnockback)
        {
        // This raycast allows enemy to detect whether the player is in front
        // It is set to both in front and behind the enemy based on the castPoint position
        // However, it is set so that the enemy would only detect the player in front
        // and not behind. This is so the player can sneak up on the enemy
        //
        RaycastHit2D raycast = Physics2D.Raycast(castPoint.position, moveDirection, sightDistance);

            if (raycast)
            {
                // If the enemy sees player, he will charge towards him
                //
                if (raycast.collider.CompareTag(PLAYER_TAG))
                {
                    enemyMove(attackSpeed);
                }
                // If the enemy sees a wall, he will not turn around until
                // he is close enough
                // .
                if (raycast.collider.CompareTag(GROUND_TAG))
                {
                    if (raycast.distance < 0.5f || !groundCheck_)
                    {
                        flipSprite();

                        // switches the movement and the raycast direction
                        //
                        moveDirection = -moveDirection;
                    }
                    else if(groundCheck_)
                    {
                        enemyMove(moveSpeed);
                    }
                }
            }
        }
        // For visualisation of raycasting
        //
        Debug.DrawRay(castPoint.position, moveDirection * sightDistance, Color.green);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (this.enabled)
        {
            // Damages the player function
            //
            if (collision.gameObject.CompareTag(PLAYER_TAG))
            {
                // if the player isn't blocking and if the enemy isnt hurt,
                // then the damage will register. The "ishurt" boolean is so that
                // when player is attacking while enemy is charging towards player,
                // the enemy charge attack wouldn't affect the player's combo, making it
                // less annoying to pull off.
                // 
                if (!isHurt && !enemyKnockback.inKnockback)
                {
                    // Get the direction of the attack and normalise the long value
                    //
                    Vector2 direction = (collision.gameObject.transform.position - transform.position).normalized;

                    // Store the array to send into the coroutine, this will damage the player
                    // I used an array to send to the Player Script as I am only able to send in
                    // one parameter
                    // .
                    object[] object_ = new object[2] { enemyDamage, direction };

                    // Hurt player on collision using the method in the Player Script
                    // Get the player component that collided with the enemy
                    //
                    collision.gameObject.GetComponent<PlayerAttack>().StartCoroutine("PlayerHurt", object_);
                }
                else Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
            }
        }
    }

    // Used to set the movement speed and stop movement of enemy
    //
    public void enemyMove(float speed)
    {
        rb.velocity = moveDirection * speed * Time.deltaTime;
        animator.SetFloat(SPEED_ENEMY_ANIM, speed);
    }
    void flipSprite()
    {
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    // Triggers check for the ground in front of enemy for footing
    // .
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            groundCheck_ = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            groundCheck_ = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(GROUND_TAG))
        {
            groundCheck_ = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(GroundCheckBox.position, GroundCheckBoxSize);
    }
}
