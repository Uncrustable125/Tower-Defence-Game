using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    public GameObject towerPrefab;
    Sprite towerBaseSprite, towerMidSprite, towerMidSprite2, towerTopSprite;
    Sprite[] towerSprites;
    [SerializeField] TowerData towerData;

    public void InitTowerButton(TowerData data)
    {
        TMP_Text buttonText = GetComponentInChildren<TMP_Text>();
        Image img = GetComponent<Image>();

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
        }
        else
        {
            Color c = img.color;
            c.a = 0f;  // set alpha to 0 (fully transparent)
            img.color = c;

            buttonText.text = "";
        }

    }

    // Called by UI Button OnPointerDown
    public void BeginDrag()
    {
        Debug.Log("BeginDrag fired");
        if (towerData == null) return;
        InputManager.Instance.StartTowerDrag(towerPrefab, towerData);
    }

    // Called by UI Button OnPointerUp
    public void EndDrag()
    {
        Debug.Log("EndDrag fired");
        if (towerData == null) return;
        InputManager.Instance.EndTowerDrag();
    }
}
