using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyClickHandler : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Camera cameraMain;
    [SerializeField] GameObject highlightPrefab;
    [SerializeField] EnemyBase enemy;

    public static EnemyClickHandler Instance;
    public LayerMask enemyLayer;

    public delegate void OnEnemySelected(EnemyBase enemy);

    public event OnEnemySelected onEnemySelected;


    private GameObject highlightPrefabInstance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }


    private void Start()
    {
        cameraMain = Camera.main;

        EventHandler.Instance.onDeselectEnemyEvent += EnemyDeselect;
    }


    public void CheckForEnemyClick()
    {

        print("checking for baddies!");

        Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        if (hit.collider != null && hit.collider.CompareTag("Enemy")) // Ensure the object has the "Enemy" tag
        {
            HandleEnemySelection(hit.collider.gameObject);
            // Handle enemy click logic here
        }

    }

    private void HandleEnemySelection(GameObject enemyHit)
    {
        // Your logic for selecting the enemy
        Debug.Log($"Selected enemy: {enemyHit.name}");
        // Example: highlight the enemy, show health, etc.
        enemy = enemyHit.GetComponent<EnemyBase>();

        if (enemy)
        {   
            HighlightEnemy();
            onEnemySelected?.Invoke(enemy);
        }
    }

    private void HighlightEnemy() {
        EnemyDeselect();
        highlightPrefabInstance = enemy.SelectEnemy(highlightPrefab);
    }

    private void EnemyDeselect()
    {
        DestroyHighlightPrefab();
    }

    private void DestroyHighlightPrefab()
    {
        if (highlightPrefabInstance) Destroy(highlightPrefabInstance);
    }

}
