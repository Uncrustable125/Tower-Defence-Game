using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

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
    [Range(0f, 1f)] float previewAlpha = 0.5f;


   // private float previewYOffset = 0.25f; // adjust as needed
    private string[] forbiddenTags = { "UI", "NoPlace" };
    [SerializeField] private float towerRadius;
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
            dragPreview.transform.localScale = new Vector3(.8f, 1f, 1f); // or some fixed scale

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
                GameObject newTowerObject = Instantiate(towerPrefab, mousePos, Quaternion.identity);
                Tower newTower = newTowerObject.GetComponent<Tower>();
                newTower.Init(towerData);
                CapsuleCollider2D capsule = newTower.GetComponent<CapsuleCollider2D>();
                if (newTower.towerData.midSprite  != null) //LARGE TOWER
                {
                    capsule.size = new Vector2(0.801920295f, 1.80445623f);
                    capsule.offset = new Vector2(0.0196710229f, 0.539305568f);
                }
                else if(newTower.towerData.midSprite2 != null) //MEDIUM TOQWER
                {
                    capsule.size = new Vector2(0.801920295f, 1.43710947f);
                    capsule.offset = new Vector2(0.0196710229f, 0.347979069f);
                }
                else   //SMALL TOWER   
                {
                    //not implemented yet
                }
                
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
        if (dragPreview != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            dragPreview.transform.position = new Vector3(mousePos.x, mousePos.y, 0f);

            // Check if placement is valid
            bool canPlace = CanPlaceHere(mousePos);

            // Update all preview layers
            SpriteRenderer[] layers = dragPreview.GetComponentsInChildren<SpriteRenderer>();
            foreach (var sr in layers)
            {
                Color c = sr.color;
                if (canPlace)
                    c = new Color(1f, 1f, 1f, previewAlpha); // normal (white)
                else
                    c = new Color(1f, 0f, 0f, previewAlpha); // red if invalid
                sr.color = c;
            }
        }
    }



    private void OnClick()
    {
        if (dragPreview != null) return;

        if (IsPointerOverUI()) return;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        // Raycast all colliders at mouse position
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        Tower towerHit = null;

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            Tower tower = hit.collider.GetComponent<Tower>();
            if (tower != null)
            {
                towerHit = tower;
                break; // stop at first tower found
            }
        }

        if (towerHit != null)
        {
            // Select tower
            if (selectedTower != null && selectedTower != towerHit)
                selectedTower.SetSelected(false);

            selectedTower = towerHit;
            selectedTower.SetSelected(true);
            towerBar.ShowUpgradeMenu(towerHit);
            return;
        }

        // Clicked empty space or non-tower
        if (selectedTower != null)
        {
            selectedTower.SetSelected(false);
            selectedTower = null;
            towerBar.ShowTowerMenu();
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
        sr.sortingLayerName = "UI";   // must exist in Sorting Layers
        sr.sortingOrder = 999 + order;

        // All layers use same alpha
        Color c = sr.color;
        c.a = previewAlpha;
        sr.color = c;
    }


    private bool CanPlaceHere(Vector2 pos)
    {
        // 2. Check for colliders at the mouse position using tags
        Collider2D[] colliders = Physics2D.OverlapCircleAll(pos, towerRadius); // radius ~ tower footprint - capsule collider
        foreach (var col in colliders)
        {
            foreach (var tag in forbiddenTags)
            {
                if (col.CompareTag(tag))
                    return false;
            }
        }

        // 3. Check if position is inside camera bounds
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(pos);
        if (viewportPos.x < 0 || viewportPos.x > 1 || viewportPos.y < 0 || viewportPos.y > 1)
            return false;

        return true;
    }

}
