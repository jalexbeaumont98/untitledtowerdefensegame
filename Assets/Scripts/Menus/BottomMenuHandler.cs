using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BottomMenuHandler : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Button waveButton;
    [SerializeField] private TextMeshProUGUI waveButtonText;
    [SerializeField] private Button pauseTimeButton;
    [SerializeField] private TextMeshProUGUI pauseButtonText;
    [SerializeField] private GameState gameState;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private TextMeshProUGUI baseHealthText;
    [SerializeField] private BaseHandler baseHandler;
    [SerializeField] private GameObject RoundDescriptionPopup;

    [SerializeField] private bool waveComplete = false;
    [SerializeField] private bool levelComplete = false;


    [SerializeField] private GameObject RoundPopupInstance;

    // Method to handle the event
    private void HandleBoolEvent(bool value)
    {
        Debug.Log($"Event triggered with value: {value}");
    }

    private void Start()
    {


        gameState = GameState.Instance;

        waveManager = FindObjectOfType<WaveManager>();

        baseHandler = FindObjectOfType<BaseHandler>();

        waveButtonText = waveButton.GetComponentInChildren<TextMeshProUGUI>();

        pauseButtonText = pauseTimeButton.GetComponentInChildren<TextMeshProUGUI>();


        //Event Code

        waveManager.OnSpawnWave += () => SetWaveButtonFastForward();

        waveManager.OnFinishedLevel += () => SetLevelComplete(true);

        waveManager.OnFinishedWave += () => SetWaveComplete(true);

        gameState.NoEnemiesRemain += () => SetWaveButtonFinishedWave();

        baseHandler.OnBaseHit += () => SetBaseHealthText();

        gameState.OnTimeChanged += () => SetWaveButtonFastForward();

        EventHandler.Instance.OnTimeToggledEvent += SetPauseTimeButton;

        //End Event Code

        SetBaseHealthText();
    }




    public void WaveButtonPress()
    {

        pauseTimeButton.interactable = true;

        switch (waveManager.GetMode())
        {
            case -1: //Not Ready
                return;

            case 0: //Ready
                waveManager.SpawnWave();
                break;

            case 1: //Spawning
                gameState.SetTime();
                break;

            case 2: //Finished
                pauseTimeButton.interactable = false;
                return;
        }




    }

    private void SetBaseHealthText()
    {
        baseHealthText.text = "BASE HP: " + gameState.GetHealth().ToString();
    }


    private void SetWaveComplete(bool _waveComplete)
    {
        waveComplete = _waveComplete;

        if (WaveManager.Instance.GetWaveIndex() == 0) SpawnRoundPopup(WaveManager.Instance.GetWaveDescription(), WaveManager.Instance.GetWaveIndex());
    }
    private void SetLevelComplete(bool _levelComplete)
    {
        levelComplete = _levelComplete;
    }


    public void SetWaveButtonFastForward()
    {

        waveComplete = false;
        if (gameState.GetSavedGameTime() > 1)
        {
            waveButtonText.text = "Stop FF";
        }
        else
        {
            waveButtonText.text = "Fast Forward";
        }

    }

    public void TogglePause()
    {

        gameState.ToggleTime();
    }

    private void SetPauseTimeButton(bool value)
    {
        if (value) pauseButtonText.text = "Pause";

        else pauseButtonText.text = "Unpause";
    }

    public void SetWaveButtonFinishedWave()
    {
        if (levelComplete)
        {
            SetWaveButtonFinishedLevel();
        }

        else if (waveComplete)
        {
            if (WaveManager.Instance.GetWaveDescription() != "")
            {
                SpawnRoundPopup(WaveManager.Instance.GetWaveDescription(), WaveManager.Instance.GetWaveIndex());
            }

            else waveButtonText.text = "Start Wave " + (waveManager.GetWaveIndex() + 1);
        }

    }

    public void SetWaveButtonExternal()
    {
        if (levelComplete)
        {
            SetWaveButtonFinishedLevel();
        }

        else if (waveComplete) waveButtonText.text = "Start Wave " + (waveManager.GetWaveIndex() + 1);
    }

    public void SetWaveButtonFinishedLevel()
    {
        waveButtonText.text = "You win!";
    }

    private void SpawnRoundPopup(string description, int waveIndex)
    {
        waveIndex++;

        if (description != "")
        {
            Canvas canvas = FindObjectOfType<Canvas>();

            if (!RoundPopupInstance) Instantiate(RoundDescriptionPopup, canvas.transform).GetComponent<RoundPopupMenu>().SetPopup(description, waveIndex);

            else
            {
                RoundPopupInstance.SetActive(true);
                RoundPopupInstance.GetComponent<RoundPopupMenu>().SetPopup(description, waveIndex);
            }

        }
    }


}
