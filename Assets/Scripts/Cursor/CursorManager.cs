using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using HFarm.Map;

public class CursorManager : MonoBehaviour
{
    public Sprite normal, tool, seed, item;

    private Sprite currentSprite;
    private Image cursorImage;
    private RectTransform cursorCanvas;

    private Camera mainCamera;
    private Grid currentGrid;

    private Vector3 mouseWorldPos;
    private Vector3Int mouseGridPos;

    private bool cursorEnable;
    private bool cursorPositionValid;

    private ItemDetails currentItem;

    private Transform PlayerTransform => FindObjectOfType<Player>().transform;

    private void OnEnable()
    {
        EventHandler.ItemSelectedEvent += OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent += OnAfterSceneUnloadEvent;
    }

    private void OnDisable()
    {
        EventHandler.ItemSelectedEvent -= OnItemSelectedEvent;
        EventHandler.BeforeSceneUnloadEvent += OnBeforeSceneUnloadEvent;
        EventHandler.AfterSceneLoadedEvent -= OnAfterSceneUnloadEvent;
    }

    private void Start()
    {
        cursorCanvas = GameObject.FindGameObjectWithTag("CursorCanvas").GetComponent<RectTransform>();
        cursorImage = cursorCanvas.GetChild(0).GetComponent<Image>();
        currentSprite = normal;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (cursorCanvas == null)
            return;
        cursorImage.transform.position = Input.mousePosition;
        if (!InteractWithUI() && cursorEnable)
        {
            SetCursorImage(currentSprite);
            CheckCursorValid();
            CheckPlayerInput();
        }
        else
            SetCursorImage(normal);
    }

    private void CheckPlayerInput()
    {
        if (Input.GetMouseButtonDown(0) && cursorPositionValid)
        {
            EventHandler.CallMouseClickedEvent(mouseWorldPos, currentItem);
        }
    }

    private void OnBeforeSceneUnloadEvent()
    {
        cursorEnable = false;
    }

    private void OnAfterSceneUnloadEvent()
    {
        currentGrid = FindObjectOfType<Grid>();
    }

    private void OnItemSelectedEvent(ItemDetails itemDetails, bool isSelected)
    {
        if (!isSelected)
        {
            currentItem = null;
            cursorEnable = false;
            currentSprite = normal;
        }
        else
        {
            currentItem = itemDetails;
            currentSprite = itemDetails.itemType switch
            {
                ItemType.Seed => seed,
                ItemType.Commodity => item,
                ItemType.ChopTool => tool,
                ItemType.HoeTool => tool,
                ItemType.WaterTool => tool,
                ItemType.BreakTool => tool,
                ItemType.ReapTool => tool,
                ItemType.Furniture => tool,
                _ => normal
            };
            cursorEnable = true;
        }
    }

    private void SetCursorImage(Sprite sprite)
    {
        cursorImage.sprite = sprite;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void SetCursorValid()
    {
        cursorPositionValid = true;
        cursorImage.color = new Color(1, 1, 1, 1);
    }

    private void SetCursorInValid()
    {
        cursorPositionValid = false;
        cursorImage.color = new Color(1, 0, 0, 0.4f);
    }

    private void CheckCursorValid()
    {
        mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCamera.transform.position.z));
        mouseGridPos = currentGrid.WorldToCell(mouseWorldPos);

        var playerGridPos = currentGrid.WorldToCell(PlayerTransform.position);
        if (Mathf.Abs(mouseGridPos.x - playerGridPos.x) > currentItem.itemUseRadius || Mathf.Abs(mouseGridPos.y - playerGridPos.y) > currentItem.itemUseRadius)
        {
            SetCursorInValid();
            return;
        }

        TileDetails currentTile = GridMapManager.Instance.GetTileDetailsOnMousePosition(mouseGridPos);
        if (currentTile != null)
        {
            // WORKFLOW: 补充所有物品类型的判断
            switch (currentItem.itemType)
            {
                case ItemType.Seed:
                    if (currentTile.daysSinceDig > -1 && currentTile.seedItemID == -1) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.Commodity:
                    if (currentTile.canDropItem && currentItem.canDropped) SetCursorValid(); else SetCursorInValid();
                    break;
                case ItemType.HoeTool:
                    if (currentTile.canDig) SetCursorValid(); else SetCursorInValid(); 
                    break;
                case ItemType.WaterTool:
                    if (currentTile.daysSinceDig > -1 && currentTile.daysSinceWatered == -1)
                        SetCursorValid(); else SetCursorInValid();
                    break;
            }
        }
        else
        {
            SetCursorInValid();
        }
    }

    private bool InteractWithUI()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        return false;
    }
}
