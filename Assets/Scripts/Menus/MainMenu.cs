using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour
{

    public void PlayButton()
    {
        GameState.Instance.StartGame();
        gameObject.SetActive(false);
    }


    public void QuitButton()
    {
        Application.Quit();
    }
}
