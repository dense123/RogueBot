using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenuScript : MonoBehaviour
{
    public GameObject TransitionPrefab;
    public void PlayGameOrNextLevel()
    {
       //Instantiate(TransitionPrefab, );
        // Loads next scene in build settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); 
    }

    // Can be run in the main menu or the pause menu
    // .
    public void ApplicationQuit()
    {
        Debug.Log("QUIT");
        Application.Quit();
    }

}
