using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using System.Xml.Linq;


public class WaveManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameState gameState;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject enemyTrooper;

    


    [Header("Attribute")]
    [SerializeField] private string levelName;
    [SerializeField] WaveData waveData;
    [SerializeField] private List<EnemyData> enemyDataList;



    public delegate void WaveDataDelegate();

    public event WaveDataDelegate OnSpawnWave;
    public event WaveDataDelegate OnFinishedWave;
    public event WaveDataDelegate OnFinishedLevel;

    private int waveIndex = 0;

    private int mode = -1;

    

    private enum waveMode
    {
        NotReady = -1,
        Ready = 0,
        Spawning = 1,
        Finished = 2

    }

    public static WaveManager Instance;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    private void Start()
    {

        gameState = GetComponent<GameState>();

        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;

        EventHandler.Instance.StartGameEvent += FinishWave;
        
    }

    public void SetWaveData(WaveData waveDataIn, List<EnemyData> enemyDataIn)
    {
        waveData = waveDataIn;
        enemyDataList = enemyDataIn;

        
        
    }

    
    public void SpawnWave()
    {
        mode = (int)waveMode.Spawning;
        OnSpawnWave?.Invoke();
        StartCoroutine(SpawnWaveCo());
    }


    private IEnumerator SpawnWaveCo()
    {
        var wave = waveData.waves[waveIndex];

        foreach (var action in wave.actions)
        {
            if (action.type == "spawn")
            {
                for (int i = 0; i < action.amount; i++)
                {
                    SpawnEnemy(action.enemyType);
                    yield return new WaitForSeconds(action.duration); // Small delay between spawns
                }
            }
            else if (action.type == "wait")
            {
                Debug.Log("waiting for " + action.duration + " seconds");
                yield return new WaitForSeconds(action.duration);
            }
        }

        waveIndex++;

        if (waveIndex >= waveData.waves.Count) FinishLevel();

        else FinishWave();
        

    }

    private void FinishWave()
    {
        

        mode = (int)waveMode.Ready;
        OnFinishedWave?.Invoke();
    }


    private void FinishLevel()
    {
        mode = (int)waveMode.Finished;
        Debug.Log("Level Complete!");
        OnFinishedLevel?.Invoke();
    }


    void SpawnEnemy(string enemyType)
    {
        // Implement your enemy spawning logic here
        Debug.Log("Spawning enemy: " + enemyType);

        foreach (EnemyData enemyData in enemyDataList)
        {
            if (enemyData.name == enemyType)
            {
                GameObject newEnemy = Instantiate(enemyData.prefab, spawnPoint);
                newEnemy.SetActive(true);
                newEnemy.GetComponent<EnemyBase>().enemyName = enemyData.name;
                gameState.AddEnemyToList(newEnemy.GetComponent<EnemyBase>());

                return;
            }
            
        }

        Debug.Log("Enemy Type " + enemyType + " not found in resources folder");

    }

    public int GetMode() { return mode; }

    public int GetWaveIndex()
    {
        return waveIndex;
    }

    public string GetWaveDescription()
    {
        return waveData.waves[waveIndex].description;
    }

}
