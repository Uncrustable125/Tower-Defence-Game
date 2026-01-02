using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    private Tower selectedTower;
    private Controls controls;

    // Drag state
    private GameObject dragPreview;
    private GameObject towerPrefab;
    private TowerData towerData;
    [SerializeField] TowerBar towerBar;
    private Sprite dragBase, dragMid, dragTop;
    [Range(0f, 1f)] public float previewAlpha = 0.5f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        controls = new Controls();
        controls.Gameplay.Click.performed += ctx => OnClick();
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    // Called when starting drag from button
    public void StartTowerDrag(GameObject prefab, TowerData data)
    {
        towerPrefab = prefab;
        towerData = data;

        dragPreview = new GameObject("TowerPreview");

        // Get the prefab's sprite renderers to read Y offsets
        SpriteRenderer[] prefabLayers = prefab.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < data.baseSprites.Length; i++)
        {
            float yOffset = 0f;

            // Use prefab's sprite Y position if it exists
            if (i < prefabLayers.Length)
                yOffset = prefabLayers[i].transform.localPosition.y;
            else
                yOffset = i * 0.32f; // fallback if prefab has fewer layers

            CreatePreviewLayer(data.baseSprites[i], i, new Vector2(0, yOffset), dragPreview.transform);
        }
    }

    // Called when releasing drag
    public void EndTowerDrag()
    {
        if (dragPreview == null) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        if (CanPlaceHere(mousePos))
        {
            if (GameManager.Instance.money >= towerData.cost)
            {
                GameManager.Instance.SpendMoney(towerData.cost);
                GameObject newTower = Instantiate(towerPrefab, mousePos, Quaternion.identity);
                newTower.GetComponent<Tower>().Init(towerData);
            }
            else
            {
                Debug.Log("Not enough money!");
            }
        }

        Destroy(dragPreview);
        dragPreview = null;
        towerPrefab = null;
        towerData = null;
    }

    private void Update()
    {
        // Move drag preview with mouse
        if (dragPreview != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            dragPreview.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);
        }
    }

    private void OnClick()
    {
        if (dragPreview != null) return;

        // If clicking UI, do NOT raycast world
        if (IsPointerOverUI())
            return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Tower tower = hit.collider.GetComponent<Tower>();
            if (tower != null)
            {
                if (selectedTower != null && selectedTower != tower)
                    selectedTower.SetSelected(false);

                selectedTower = tower;
                selectedTower.SetSelected(true);
                towerBar.ShowUpgradeMenu(tower);
            }
        }
        else
        {
            if (selectedTower != null)
            {
                selectedTower.SetSelected(false);
                selectedTower = null;
                towerBar.ShowTowerMenu();
            }
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Mouse.current.position.ReadValue()
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }



    private void CreatePreviewLayer(Sprite sprite, int order, Vector2 offset, Transform parent)
    {
        if (sprite == null) return;

        GameObject layer = new GameObject("Layer_" + order);
        layer.transform.SetParent(parent, false);
        layer.transform.localPosition = offset;

        SpriteRenderer sr = layer.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 999 + order;

        // All layers use same alpha
        Color c = sr.color;
        c.a = previewAlpha;
        sr.color = c;
    }



    private bool CanPlaceHere(Vector2 pos)
    {
        // TODO: collision/grid checks
        return true;
    }
}
