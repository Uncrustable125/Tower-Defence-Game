using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    public GameObject towerPrefab;
    Sprite[] towerSprites;
    [SerializeField] TowerData towerData;

    public void InitTowerButton(TowerData data)
    {
        TMP_Text buttonText = GetComponentInChildren<TMP_Text>();

        Image img = null;
        foreach (Transform child in transform)
        {
            img = child.GetComponent<Image>();
            if (img != null) break; // stops at the first (and only) child Image
        }

        if (data != null)
        {
            towerData = data;
            towerSprites = new Sprite[]
            {
        data.botSprite,
        data.midSprite,
        data.midSprite2,
        data.topSprite
            };
            Sprite combinedSprite =
                SpriteCombiner.Instance.CombineSpritesToSprite(towerSprites);

            img.sprite = combinedSprite;
            img.preserveAspect = true;
            buttonText.text = data.towerName;
            Color c = img.color;
            c.a = 1f;
            img.color = c;

            
            //Scales image to sprite size
            RectTransform rt = img.rectTransform;
            if (data.midSprite == null) // for small towers not implemented yet
            {

            }
            else if(data.midSprite2 == null) // reg towers
            {
                rt.sizeDelta = new Vector2(rt.sizeDelta.x, 130f);
                rt.anchoredPosition += new Vector2(0f, 8f);
            }
            else // big towers
            {
                rt.sizeDelta = new Vector2(160f, rt.sizeDelta.y);
            }
            
        }
        else
        {
            Color c = img.color;
            c.a = 0f;  // set alpha to 0 (fully transparent)
            img.color = c;

            buttonText.text = "";
        }

    }
    void FitSpriteToImage(Image img)
    {
        RectTransform rt = img.rectTransform;
        Sprite s = img.sprite;

        if (s == null)
            return;

        float spriteW = s.rect.width;
        float spriteH = s.rect.height;

        RectTransform parentRT = rt.parent as RectTransform;
        if (parentRT == null)
            return;

        // ensure parent rect is valid
        LayoutRebuilder.ForceRebuildLayoutImmediate(parentRT);

        float maxW = parentRT.rect.width;
        float maxH = parentRT.rect.height;

        if (maxW <= 0f || maxH <= 0f)
            return;

        float scale = Mathf.Min(maxW / spriteW, maxH / spriteH);

        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        rt.sizeDelta = new Vector2(
            spriteW * scale,
            spriteH * scale
        );
    }

    // Called by UI Button OnPointerDown
    public void BeginDrag()
    {
        if (towerData == null) return;
        InputManager.Instance.StartTowerDrag(towerPrefab, towerData);
    }

    // Called by UI Button OnPointerUp
    public void EndDrag()
    {
       // if (towerData == null) return;
        EventSystem.current.SetSelectedGameObject(null);
        InputManager.Instance.EndTowerDrag();
    }
}
