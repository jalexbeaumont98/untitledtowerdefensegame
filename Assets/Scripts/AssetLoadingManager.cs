using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;
using Unity.VisualScripting;


public class AssetLoadingManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameState gameState;
    [SerializeField] private WaveManager waveManager;
    [SerializeField] private static string turretsJSONName = "turrets";
    [SerializeField] Sprite[] sprites;


    [Header("Attribute")]
    [SerializeField] private string levelName;
    [SerializeField] private string turretJSON = "TurretJSON/turrets";
    [SerializeField] WaveData waveData;
    [SerializeField] private List<EnemyData> enemyDataList;

    private TurretData turretData;


    private void Start()
    {
        waveManager = FindObjectOfType<WaveManager>();
        gameState = GameState.Instance;

        levelName = gameState.GetLevelName();

        Addressables.LoadAssetAsync<TextAsset>(levelName).Completed += LoadWaveData;

        //LoadWaveDate();

        //LoadTurretData();

        Addressables.LoadAssetAsync<TextAsset>(turretJSON).Completed += LoadTurretData;


    }



    private void LoadWaveData(AsyncOperationHandle<TextAsset> handle)
    {


        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset jsonLevelFile = handle.Result;
            Debug.Log($"JSON loaded successfully: {jsonLevelFile.text}");


            // Parse JSON
            waveData = JsonConvert.DeserializeObject<WaveData>(jsonLevelFile.text);

            PrintWaveData();

            CreateEnemyList();

            waveManager.SetWaveData(waveData, enemyDataList);


        }
        else
        {
            Debug.LogError($"Failed to load JSON at address '{levelName}'.");
        }


    }




    private async void CreateEnemyList()
    {

        foreach (var wave in waveData.waves)
        {
            print(wave.description);
            
            foreach (var action in wave.actions)
            {
                if (action.type == "spawn")
                {
                    for (int i = 0; i < action.amount; i++)
                    {

                        EnemyData enemy = new EnemyData(action.enemyType, null);


                        if (action.UsesPrototype())
                        {
                            //print("enemy uses prototype");

                            if (CheckEnemyNotDuplicate(action.enemyType))
                            {
                                GameObject enemyPrefab;

                                if (CheckEnemyNotDuplicate(action.prototype)) enemyPrefab = await LoadEnemyAsync(action.prototype);

                                else enemyPrefab = GetEnemyDuplicate(action.prototype).prefab;

                                //set variations
                                enemy.prefab = CreateModifiedClone(enemyPrefab, action.enemyType, action.variations);
                                
                                enemyDataList.Add(enemy);

                            }


                        }

                        else
                        {

                            if (CheckEnemyNotDuplicate(action.enemyType))
                            {
                                enemy.prefab = await LoadEnemyAsync(action.enemyType);
                                enemyDataList.Add(enemy);

                            }

                        }



                    }
                }

            }
        }

        GameState.Instance.LoadFinish(1);
    }

    private bool CheckEnemyNotDuplicate(string enemyType)
    {
        if (enemyDataList.Count <= 0) return true;

        foreach (EnemyData enemyData in enemyDataList)
        {
            if (enemyData.name == enemyType) return false;
        }

        return true;
    }

    private EnemyData GetEnemyDuplicate(string enemyType)
    {

        if (enemyDataList.Count <= 0) return null;

        foreach (EnemyData enemyData in enemyDataList)
        {
            if (enemyData.name == enemyType) return enemyData;
        }

        return null;
    }

    public async Task<GameObject> LoadEnemyAsync(string enemyType)
    {
        // Start the asynchronous loading operation
        AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(enemyType);

        // Await the task to complete
        await handle.Task;

        // Check the status of the operation
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result; // Return the loaded enemy
        }
        else
        {
            Debug.LogError("Failed to load enemy: " + enemyType);
            return null; // Return null in case of failure
        }
    }

    public GameObject CreateModifiedClone(GameObject originalPrefab, string newName, Dictionary<string, object> variations)
    {
        if (originalPrefab == null)
        {
            Debug.LogError("Original prefab is not assigned.");
            return null;
        }

        // Clone the prefab into memory
        GameObject clonedObject = Instantiate(originalPrefab);

        // Mark it inactive to prevent it from appearing in the scene
        clonedObject.SetActive(false);

        // Variations
        clonedObject.name = newName;

        PrintDictionaryContents(variations);

        clonedObject.GetComponent<EnemyBase>().SetVariations(variations);

        // Return the modified object
        return clonedObject;
    }

    public void PrintDictionaryContents(Dictionary<string, object> dictionary)
    {
        foreach (var pair in dictionary)
        {
            string key = pair.Key;
            object value = pair.Value;

            // Print the key, value, and its type
            Debug.Log($"Key: {key}, Value: {value}, Type: {value?.GetType().Name ?? "null"}");
        }
    }


    void PrintWaveData()
    {
        foreach (Wave wave in waveData.waves)
        {
            foreach (WaveAction waveAction in wave.actions)
            {
                //Debug.Log(waveAction.type);
            }
        }
    }


    void LoadTurretData(AsyncOperationHandle<TextAsset> handle)
    {

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            TextAsset jsonTurretFile = handle.Result;
            Debug.Log($"JSON loaded successfully: {jsonTurretFile.text}");
            // Parse JSON
            turretData = JsonConvert.DeserializeObject<TurretData>(jsonTurretFile.text);

            StartCoroutine(RunLoadTurrets());

        }
        else
        {
            Debug.LogError($"Failed to load JSON at address '{turretJSON}'.");
        }

    }


    private IEnumerator RunLoadTurrets()
    {
        print("running RunLoadTurrets");
        yield return LoadTurretsIntoDataList();
    }


    public async Task LoadTurretsIntoDataList()
    {

        print("now into LoadTurretsIntoDataList");

        List<TowerPlaceData> towers = new List<TowerPlaceData>();

        foreach (Turret turret in turretData.turrets)
        {
            print("in the foreach block");

            if (turret != null)
            {
                print("turret is not null!");

                
                if (turret == null || string.IsNullOrEmpty(turret.name) || string.IsNullOrEmpty(turret.tile)) continue;

                if (!turret.unlocked) continue;
                

                Tile tile = await LoadAddressableAssetAsync<Tile>("TileBases/" + turret.tile);

                GameObject prefab = null;

                if (!string.IsNullOrEmpty(turret.prefab))
                {
                    prefab = await LoadAddressableAssetAsync<GameObject>("TurretBases/" + turret.prefab);
                }

                List<TurretUpgradeData> upgradesA = await ProcessUpgradePathAsync(turret.upgradePath1);
                List<TurretUpgradeData> upgradesB = await ProcessUpgradePathAsync(turret.upgradePath2);


                towers.Add(new TowerPlaceData(turret.name, turret.price, prefab, tile, turret.sellPrice, turret.description, upgradesA, upgradesB));


            }
        }

        print("TowerList should be loaded");
        gameState.SetTowerList(towers);

    }



    private async Task<List<TurretUpgradeData>> ProcessUpgradePathAsync(List<TurretUpgrade> upgradePath)
    {
        List<TurretUpgradeData> upgrades = new List<TurretUpgradeData>();

        foreach (TurretUpgrade turretUpgrade in upgradePath)
        {
            if (turretUpgrade == null) continue;

            GameObject newSprite = null;
            Tile newTile = null;
            GameObject newBullet = null;

            // Load new sprite (if applicable)
            if (turretUpgrade.isNewSprite)
            {
                newSprite = await LoadAddressableAssetAsync<GameObject>(turretUpgrade.newSprite);
            }

            // Load new tile (if applicable)
            if (turretUpgrade.isNewTile)
            {
                newTile = await LoadAddressableAssetAsync<Tile>("TileBases/" + turretUpgrade.newTile);
            }

            // Load new bullet (if applicable)
            if (turretUpgrade.isNewBullet)
            {
                newBullet = await LoadAddressableAssetAsync<GameObject>("BulletBases/" + turretUpgrade.newBullet);
            }

            // Create TurretUpgradeData
            upgrades.Add(new TurretUpgradeData(
                turretUpgrade.upgradePath,
                turretUpgrade.price,
                turretUpgrade.fireRate,
                turretUpgrade.rotationSpeed,
                turretUpgrade.targetingRange,
                turretUpgrade.isNewSprite,
                turretUpgrade.isNewTile,
                turretUpgrade.isNewBullet,
                turretUpgrade.canTargetStealth,
                turretUpgrade.canTargetFlying,
                newSprite,
                newTile,
                newBullet,
                turretUpgrade.description
            ));
        }

        return upgrades;
    }


    private async Task<T> LoadAddressableAssetAsync<T>(string address) where T : Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(address);
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            return handle.Result;
        }

        Debug.LogError($"Failed to load Addressable asset at address: {address}");
        return null;
    }


    public void SaveTurrets()
    {
        string json = JsonUtility.ToJson(turretData, true);
        string filePath = Path.Combine(Application.dataPath, "Resources/TurretJSON/", $"{turretsJSONName}.json");
        File.WriteAllText(filePath, json);
    }


    public void UnlockTurret(string turretName)
    {
        Turret turret = turretData.turrets.Find(t => t.name == turretName);
        if (turret != null && !turret.unlocked)
        {
            turret.unlocked = true;
            SaveTurrets();
            Debug.Log($"{turretName} has been unlocked.");
        }
        else
        {
            Debug.LogWarning($"Turret {turretName} not found or already unlocked.");
        }
    }


}
