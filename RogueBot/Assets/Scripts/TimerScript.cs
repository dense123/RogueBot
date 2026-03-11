using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using d = UnityEngine.Debug;
public class TimerScript : MonoBehaviour
{
    // https://www.youtube.com/watch?v=o0j7PdU88a4
    // .
    TextMeshProUGUI timer;
    public GameManagerScript gameManager;
    public Level_1_Cutscene cutscene;
    bool cutscenePlaying = true;
    public float timeStart = 3f;
    float t = 0f;

    // Start is called before the first frame update
    void Start()
    {
        timer = GetComponent<TextMeshProUGUI>();
        t = timeStart;
    }

    // Update is called once per frame
    void Update()
    {
        // If theres is no cutscene, then allow for player movement
        // .
        if (cutscene == null)
        {
            cutscenePlaying = false;
        }

        // If the no cutscenes are playing, then continue the timer 
        // .
        if (!cutscenePlaying && !gameManager.gameEnded)
        {
            t -= 1f * Time.deltaTime; // Counts the timer down
            if (t <= 0f)
            {
                // Once timer reaches 0, the game will end and restart
                // .
                gameManager.EndGame();
                t = timeStart;
            }
            else if (t <= 60f)
            {
                // Shows the seconds and milli seconds instead of the minutes
                // since it is less than 1 minute
                // .
                timer.text = (t % 60).ToString("f2");
            }
            else
            {
                // Formats the time text for better visuals
                // .
                string minutes = ((int)t / 60).ToString();
                string seconds = (t % 60).ToString("00");

                timer.text = minutes + ":" + seconds;
            }
            
        }
    }
}
