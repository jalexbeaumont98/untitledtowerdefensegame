using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputHandler : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private CameraController cameraController;
    [SerializeField] private TileClickHandler tileClickHandler;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameState gameState;


    [Header("Attribute")]
    [SerializeField] private int inputType = 0;
    [SerializeField] private float dragSpeed = 2.0f;  // Speed at which the camera will move


    private Vector3 previousMousePosition;


    enum InputType
    {
        DragCamera = 0,
        TileSelect = 1
    }

    private int inputTypes = 0;




    private Vector3 dragOrigin;

    public static InputHandler Instance;


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        gameState = GameState.Instance;
        cameraController = GetComponent<CameraController>();
        tileClickHandler = TileClickHandler.Instance;
        mainCamera = Camera.main;
    }


    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            tileClickHandler.SetCurrentPlacement(0);
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            tileClickHandler.SetCurrentPlacement(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            tileClickHandler.SetCurrentPlacement(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            tileClickHandler.SetCurrentPlacement(3);
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (inputType != 1) EventHandler.Instance.InvokeRightClickEvent(10);
            ChangeInputType();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {

            gameState.ToggleTime();

        }



        if (Input.GetMouseButtonDown(0))
        {
            

            if (inputType == 1)
            {
                tileClickHandler.PlaceTile();
                return;
            }

            

            if (inputType == 0)
            {
                dragOrigin = Input.mousePosition;
                return;
            }

        }

        if (Input.GetMouseButton(0))
        {
            
            HandleDrag();

        }

        if (Input.GetMouseButtonUp(0))
        {
            if (inputType == 0)
            {
                tileClickHandler.SelectTile();
                EnemyClickHandler.Instance.CheckForEnemyClick();
            }

        }

        HandleScrollWheel();



        if (inputType == 1)
        {
            Vector3 currentMousePosition = Input.mousePosition;

            if (currentMousePosition != previousMousePosition)
            {
                //Debug.Log("Mouse has moved.");
                previousMousePosition = currentMousePosition;

                tileClickHandler.PreviewTile();
            }
        }



    }

    public void HandleDrag()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; // Ignore drag if interacting with UI
        }

        if (inputType == 0)
        {


            Vector3 difference = Input.mousePosition - dragOrigin;
            Vector3 newPos = mainCamera.transform.position - difference * dragSpeed * Time.unscaledDeltaTime; ;

            cameraController.PanCamera(newPos);
            dragOrigin = Input.mousePosition;


        }
    }

    public void HandleScrollWheel()
    {

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; // Ignore drag if interacting with UI
        }

        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");
        cameraController.HandleZooming(scrollData);
    }


    public void ChangeInputType(int input = -1)
    {
        print("inpout type has changed");

        if (input != -1)
        {
            inputType = input;
            EventHandler.Instance.InvokeInputToggledEvent(inputType);
            return;
        }

        inputType++;
        if (inputType >= inputTypes) inputType = 0;

        if (input != 1) tileClickHandler.DumpPreview();

        EventHandler.Instance.InvokeInputToggledEvent(inputType);
    }


}
