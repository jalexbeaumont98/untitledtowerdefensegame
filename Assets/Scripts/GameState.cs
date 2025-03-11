using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameState : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private List<EnemyBase> enemies;
    [SerializeField] private List<TowerPlaceData> towers;
    [SerializeField] public GameObject towerHighlightPrefab, rangeCirclePrefab;

    [SerializeField] public GameObject MainMenuPrefab;
    [SerializeField] public GameObject LoadingBarPrefab;


    public IReadOnlyList<TowerPlaceData> Towers => towers;
    public IReadOnlyList<EnemyBase> Enemies => enemies;




    [Header("Attribute")]
    [SerializeField] private string levelName;
    [SerializeField] private int money = 1000;
    [SerializeField] private int baseHealth;
    [SerializeField] private float savedGameTime = 1;
    [SerializeField] private float fastForwardTime;

    public static int MaxUpgrade = 2;

    public delegate void GameStateEvent();

    public event GameStateEvent NoEnemiesRemain;
    public event GameStateEvent OnTimeChanged;
    public event GameStateEvent OnTowerListUpdated;

    public static GameState Instance;

    public bool loadedTowers = false;
    public bool loadedEnemies = false;
    public static int maxUpgrade = 3;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {

        LoadingBarPrefab.SetActive(true);

        savedGameTime = 1;


    }

    public void StartGame()
    {
        EventHandler.Instance.InvokeStartGameEvent();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadFinish(int type)
    {
        if (type == 0)
        {
            loadedTowers = true;
            EventHandler.Instance.InvokeTowersLoadedEvent();
        }
         
        if (type == 1)
        {
            loadedEnemies = true;
            EventHandler.Instance.InvokeEnemiesLoadedEvent();
        }
         

        if (loadedEnemies && loadedTowers)
        {
            print("game is ready to start");

            StartCoroutine(SetObjectEnabledDisabledOnDelay(LoadingBarPrefab, 0.1f, false));
            StartCoroutine(SetObjectEnabledDisabledOnDelay(MainMenuPrefab, 0.2f, true));
            //MainMenuPrefab.SetActive(true);
        }


    }

    IEnumerator SetObjectEnabledDisabledOnDelay(GameObject go, float delay, bool isActive)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        go.SetActive(isActive);

    }

    public string GetLevelName()
    {
        return levelName;
    }

    public void AddEnemyToList(EnemyBase enemy)
    {
        enemies.Add(enemy);
    }


    public void RemoveEnemyFromList(EnemyBase enemy)
    {
        enemies.Remove(enemy);

        if (enemies.Count <= 0) NoEnemiesRemain.Invoke();
    }


    public int GetMoney()
    {
        return money;
    }

    public bool AttemptPurchase(int cost)
    {
        if (money - cost >= 0) return true;

        return false;
    }

    public void SetMoney(int amount)
    {
        money += amount;

    }

    public int GetHealth()
    {
        return baseHealth;
    }

    public void SetHealth(int input)
    {
        baseHealth -= input;
    }


    public bool CheckEnemyPaths() //for checking if a path exists
    {
        foreach (EnemyBase enemy in enemies)
        {
            if (!enemy.CheckForObstructions()) return false; //return if a single enemy can't reach the goal
        }

        return true; //otherwise return true
    }

    public void ToggleTime()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            EventHandler.Instance.InvokeTimeToggledEvent(false);
        }

        else
        {
            Time.timeScale = savedGameTime;
            EventHandler.Instance.InvokeTimeToggledEvent(true);
        }


    }

    public void PauseTime()
    {
        if (Time.timeScale != 0) ToggleTime();
    }

    public void ResumeTime()
    {
        if (Time.timeScale == 0) ToggleTime();
    }

    public void SetTime()
    {

        //todo this will need to change when you add pause impact effects

        if (Time.timeScale != 0)
        {
            if (Time.timeScale > 1) Time.timeScale = 1;

            else Time.timeScale = fastForwardTime;

            savedGameTime = Time.timeScale;
        }

        else
        {
            if (savedGameTime != fastForwardTime) savedGameTime = fastForwardTime;

            else savedGameTime = 1;
        }

        OnTimeChanged?.Invoke();


    }

    public bool IsTimePaused()
    {
        if (Time.timeScale > 0) return false;
        return true;
    }

    public float GetSavedGameTime()
    {
        return savedGameTime;
    }

    public void AddTowerToList(TowerPlaceData tower)
    {
        towers.Add(tower);
    }

    /*
    public void SetTowerList(List<TowerPlaceData> towerPlaceDatas)
    {
        towers = towerPlaceDatas;
        OnTowerListUpdated?.Invoke();
    }
    */

    public void SetTowerList(List<TowerPlaceData> towerPlaceDatas)
    {
        towers = towerPlaceDatas; // You can still set the private field internally.

        LoadFinish(0);

        OnTowerListUpdated?.Invoke();
    }

    public void FreeTowerListMemory()
    {
        towers.Clear();
    }



}
