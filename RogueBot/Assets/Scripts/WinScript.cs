using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinScript : MonoBehaviour
{// win trigger
    
    public GameManagerScript gameManager;
    
    // Calls the win function in the game manager script after
    // going through the win trigger
    // .
    private void OnTriggerStay2D(Collider2D collision)
    {
        gameManager.Win();
    }
}
