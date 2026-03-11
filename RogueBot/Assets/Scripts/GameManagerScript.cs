using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using d = UnityEngine.Debug;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class GameManagerScript : MonoBehaviour
{
    public bool gameEnded;

    public bool gameCompleted = false;

    Enemy enemy;
    GameObject[] totalEnemiesGO;
    int totalEnemies;

    public int TotalEnemies
    {
        get{ return totalEnemies; }
        set{ totalEnemies = value; }
    }

    [Header("Door")]
    public GameObject door;
    Animator doorAnimator;
    string DOOROPEN_ANIM = "isDoorOpen";

    public GameObject levelCompleteUI;
    public TimerScript timer;

    string PLAYER_TAG = "Player";
    string ENEMY_TAG = "Enemy";
    
    private void Awake()
    {
        if(door != null)
        {
            doorAnimator = door.GetComponent<Animator>();

        }
        gameEnded = false;
        enemy = GetComponent<Enemy>();
        totalEnemiesGO = GameObject.FindGameObjectsWithTag(ENEMY_TAG);
        totalEnemies = totalEnemiesGO.Length;
   }

    // This runs when in the enemy script, everytime they are destroyed,
    // it goes through this if statement to check if the door should be unlocked
    // or if the player should complete the game
    // .
    public void CompleteLevel()
    {
        // Runs if all the enemies are destroyed. This would usually open a door.
        // .
        if (!gameCompleted && totalEnemies < 1)
        {
            d.Log("All Enemies Gone");
            doorAnimator.SetBool(DOOROPEN_ANIM, true);

        }
    }

    // This runs when player's health reaches 0
    public void EndGame()
    {
        if (!gameEnded)
        {
            gameEnded = true;
            d.Log("GAME OVER");
            Restart();
        }
    }

    // This usually runs when player goes through the winTrigger set at the
    // .
    public void Win()
    {
        if (!gameCompleted)
        {
            gameCompleted= true;
            GameObject.FindGameObjectWithTag(PLAYER_TAG).GetComponent<Player>().canMovement = false;
            levelCompleteUI.SetActive(true);
        }
    }

    // This runs when player dies, and can be run in the pause menu
    // .
    public void Restart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
