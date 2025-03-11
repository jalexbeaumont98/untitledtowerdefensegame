using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public void ResumeGame()
    {
        GameState.Instance.ResumeTime();
        gameObject.SetActive(false);
    }

    public void OpenSettingsMenu()
    {
        print("This will eventually open the settings menu");
    }

    public void QuitGame() 
    {
        GameState.Instance.QuitGame();
    }
}
