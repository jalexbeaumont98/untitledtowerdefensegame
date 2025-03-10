using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


public class TileClickHandler : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Tilemap tilemap_Grass;
    [SerializeField] private Tilemap tilemap_Walls;
    [SerializeField] private Tilemap tilemap_Previews;
    [SerializeField] private Tile invisTestPathTile;
    [SerializeField] private Tile grassTile;
    [SerializeField] private List<TurretBase> turrets;
    [SerializeField] private GameObject towerHighlightPrefab;
    [SerializeField] private GameObject unitCirclePrefab;




    [SerializeField] private NavMeshPlus.Components.NavMeshSurface Surface2D;

    [SerializeField] private GameState gameState;

    //[SerializeField] private List<TowerPlaceData> towers;

    [SerializeField] private Camera cameraMain;



    public static readonly Vector3Int InvalidPosition = new Vector3Int(-1, -1, -1);

    private int selectedTowerToPlace;
    private int selectedTowerToPlacePrice;

    private Vector3Int previousTilePreviewPosition;
    private GameObject previousTilePreviewPrefab;


    private Vector3Int nextPosition;

    public delegate void OnTileSelected(TurretBase turret);

    public event OnTileSelected onTileSelected;

    private TurretBase selectedTurret;

    private GameObject towerHighlight;

    public static TileClickHandler Instance;

    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        selectedTowerToPlace = 1;
        gameState = GameState.Instance;
        Surface2D.BuildNavMeshAsync();
        cameraMain = Camera.main;

        gameState.OnTowerListUpdated += () => UpdateTowerList();
        EventHandler.Instance.OnDeselectTurretEvent += TurretDeselect;

    }

    public void UpdateTowerList()
    {

        print("towers updated.");



    }

    public void SetCurrentPlacement(int input)
    {


        if (!gameState.Towers.Any()) return;

        selectedTowerToPlace = input;
        selectedTowerToPlacePrice = gameState.Towers[selectedTowerToPlace].price;

    }

    public void SelectTile()
    {
        Vector3Int cellPosition = GetCellPositionFromMouse();

        if (cellPosition == InvalidPosition)
        {
            //Debug.Log("No");
            return;
        }

        if (!IsOccupiedByTower(cellPosition)) return;

        TurretBase turret = GetTurretFromSelect(cellPosition);

        if (turret != null)
        {
            TurretDeselect();

            selectedTurret = turret;
            onTileSelected?.Invoke(turret);

            if (towerHighlight != null) Destroy(towerHighlight);

            Vector3 worldPosition = tilemap_Walls.CellToWorld(cellPosition);

            selectedTurret.SetSelected();
        }


    }



    public TurretBase GetTurretFromSelect(Vector3Int cellPosition)
    {

        foreach (TurretBase turret in turrets)
        {
            if (turret.gridPos == cellPosition)
            {
                return turret;
            }
        }

        return null;

    }

    public void ReplaceTile(Tile tile, Vector3Int cellPosition)
    {
        tilemap_Walls.SetTile(cellPosition, tile);

    }

    public void PlaceTile()
    {

        Vector3Int cellPosition = GetCellPositionFromMouse();

        print("X: " + cellPosition.x + " Y: " + cellPosition.y + " Z: " + cellPosition.z);

        if (cellPosition == InvalidPosition)
        {
            Debug.Log("No");
            return;
        }


        if (selectedTowerToPlace == 0) //check if were deleting
        {

            // todo: refund code here

            if (!IsOccupiedByTower(cellPosition)) return; //no tower here to sell

            SellTile(cellPosition);
            return;
        }


        if (!gameState.AttemptPurchase(selectedTowerToPlacePrice))
        {

            // todo: not enought money code here
            return;
        }



        if (!IsGrass(cellPosition)) return; //towers can only be built on grass tiles

        if (IsOccupiedByTower(cellPosition)) return; //can't build a tower on an occupied tile


        nextPosition = cellPosition;
        tilemap_Walls.SetTile(cellPosition, invisTestPathTile); //place an invisible tile to check if a path will still exist

        StartCoroutine(UpdateNavMeshCoroutine());  //update the nav mesh with a callback to validate the path

    }


    private void PlaceTileComplete()
    {

        gameState.SetMoney(-selectedTowerToPlacePrice); //now we take the money from the gamestate bank now that we know its a valid placement


        tilemap_Walls.SetTile(nextPosition, gameState.Towers[selectedTowerToPlace].tile); //set the new tile for realsies

        if (gameState.Towers[selectedTowerToPlace].prefab != null) //spawn a turret prefab if it has one
        {
            Vector3 worldPosition = tilemap_Walls.CellToWorld(nextPosition);
            TurretBase turret = Instantiate(gameState.Towers[selectedTowerToPlace].prefab, worldPosition, Quaternion.identity).GetComponent<TurretBase>();

            turret.SetTileSellPrice(gameState.Towers[selectedTowerToPlace].sellPrice);

            turret.gridPos = nextPosition;
            turrets.Add(turret);
            turret.towerData = gameState.Towers[selectedTowerToPlace].CreateImmutableCopy();
        }


    }

    public void OnNavMeshUpdated()
    {
        if (!gameState.CheckEnemyPaths()) //if a path doesn't exist
        {

            Debug.Log("path wasn't valid my guy");

            tilemap_Walls.SetTile(nextPosition, null); //delete the invisible tile
            Surface2D.UpdateNavMesh(Surface2D.navMeshData); //update the nav mesh with no callback
        }

        else
        {
            PlaceTileComplete(); //place the tile using the saved next tile info
            DumpPreview();
        }

    }


    private IEnumerator UpdateNavMeshCoroutine()
    {
        // Start the NavMesh update and get the AsyncOperation
        AsyncOperation asyncOperation = Surface2D.UpdateNavMesh(Surface2D.navMeshData);

        // Wait until the NavMesh update is complete
        yield return new WaitUntil(() => asyncOperation.isDone);

        // Call the function after NavMesh update
        OnNavMeshUpdated();
    }


    public void PreviewTile()
    {

        Vector3Int cellPosition = GetCellPositionFromMouse();

        if (cellPosition == InvalidPosition)
        {
            //Debug.Log("No");
            DumpPreview();
            return;
        }


        if (previousTilePreviewPosition == cellPosition) return; //make sure it isn't the same tile


        if (previousTilePreviewPosition != InvalidPosition) tilemap_Previews.SetTile(previousTilePreviewPosition, null); //set the previous preview tile to null if it exists


        if (previousTilePreviewPrefab != null) Destroy(previousTilePreviewPrefab); //destroy the previous preview prefab if it exists


        previousTilePreviewPosition = cellPosition; //set previous to current


        if (selectedTowerToPlace != 0) //if were not deleting we need to make sure there's no tower in the current spot
        {
            if (!IsGrass(cellPosition)) return;

            if (IsOccupiedByTower(cellPosition)) return; //check the wall tilemap to see if the tile is occupied

            tilemap_Previews.SetTile(cellPosition, gameState.Towers[selectedTowerToPlace].tile); //set the preview tile


            if (gameState.Towers[selectedTowerToPlace].prefab != null) //some towers like walls don't have an additional prefab to spawn
            {
                Vector3 worldPosition = tilemap_Previews.CellToWorld(cellPosition); //get the world position from the tile map

                GameObject disabledPrefab = Instantiate(gameState.Towers[selectedTowerToPlace].prefab, worldPosition, Quaternion.identity); //instantiate the prefab

                disabledPrefab.GetComponent<TurretBase>().SetPreviewMode(); //set the preview prefab to preview mode (lowers its opacity and disables itself)

                previousTilePreviewPrefab = disabledPrefab; //so we can delete it later 


            }

        }

        else tilemap_Previews.SetTile(cellPosition, gameState.Towers[selectedTowerToPlace].tile);




    }

    public void DumpPreview() //dumps the preview tile, its associated prefab and resets the previous preview tile position
    {
        if (previousTilePreviewPrefab != null) Destroy(previousTilePreviewPrefab);

        tilemap_Previews.ClearAllTiles();

        previousTilePreviewPosition = InvalidPosition;
    }

    private Vector3Int GetCellPositionFromMouse() //grabs the tile that the mouse is over
    {
        Ray ray = cameraMain.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);

        Vector3Int cellPosition = InvalidPosition;

        if (hit.collider != null)
        {

            Vector3 hitPoint = hit.point;
            cellPosition = tilemap_Grass.WorldToCell(hitPoint);
        }

        return cellPosition;
    }


    private int GetTileSellPrice(Vector3Int cellPosition)
    {
        int sellPrice = 0;

        TileBase tile = tilemap_Walls.GetTile(cellPosition);


        foreach (TowerPlaceData towerData in gameState.Towers)
        {
            if (tile == towerData.tile) sellPrice = towerData.sellPrice;

        }


        return sellPrice;
    }

    private int RemoveAndDestroyTurret(Vector3Int cellPosition)
    {
        TurretBase turretToDestroy = null;

        int soldPrice = 0;

        foreach (TurretBase turret in turrets)
        {
            if (turret.gridPos == cellPosition)
            {
                turretToDestroy = turret;
                break;
            }
        }

        if (turretToDestroy == null)
        {
            print("okay so the turret is null...");
            return soldPrice;
        }

        soldPrice = turretToDestroy.GetTotalSellPrice();

        turrets.Remove(turretToDestroy);
        turretToDestroy.DestroySelf();

        return soldPrice;
    }

    /// <summary>
    /// Method <c>IsGrass</c> Checks if a cell position in the grass tileset has a grass tile. Returns true if a grass tile is found at the cell position.
    /// </summary>
    private bool IsGrass(Vector3Int cellPosition)
    {
        if (tilemap_Grass.GetTile(cellPosition) != null) return true;

        return false;
    }

    /// <summary>
    /// Method <c>IsOccupiedByTower</c> Checks if a cell position in the tower tileset already has a tower tile. Returns true if a tower is found at the cell position.
    /// </summary>
    private bool IsOccupiedByTower(Vector3Int cellPosition)
    {
        if (tilemap_Walls.GetTile(cellPosition) != null) return true;

        return false;
    }

    public Tilemap GetTowerMap()
    {
        return tilemap_Walls;
    }

    public void SellTile(Vector3Int cellPosition)
    {
        int sellPrice = RemoveAndDestroyTurret(cellPosition); //remove and destroy the turret if it exists, if it doesn't it'll return 0

        if (sellPrice == 0) //no turret so get the sell price from the tower data
        {
            gameState.SetMoney(GetTileSellPrice(cellPosition));
        }

        else
        {
            gameState.SetMoney(sellPrice);
        }


        tilemap_Walls.SetTile(cellPosition, null); //set to null at cell position
        Surface2D.UpdateNavMesh(Surface2D.navMeshData); //update nav mesh
    }

    private void TurretDeselect()
    {
        if (!selectedTurret) return;

        selectedTurret.DeselectTurret();
        selectedTurret = null;
    }



}
