using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    PlayerAttack playerAttack;

    // Hard coded the amount of hearts player will have
    // .
    public GameObject[] hearts;
    int life = 3;
    bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();

        life = hearts.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            playerAttack.playerDie();
            this.enabled = false;
        }
    }

    // Run in PlayerAttack script once he gets damaged
    //
    public void takeDamage(int damage)
    {
        if (!dead)
        {
            life -= damage;
            Destroy(hearts[life].gameObject); // destroys the right most heart
            if(life < 1)
            {
                dead = true;
            }

        }
    }
}
