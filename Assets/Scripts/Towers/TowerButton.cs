using UnityEngine;

public class TowerButton : MonoBehaviour
{
    public GameObject towerPrefab;
    Sprite towerBaseSprite;
    Sprite towerMidSprite;
    Sprite towerTopSprite;
    TowerData towerData;

    public void Init(TowerData data)
    {
        towerData = data;
        towerBaseSprite = data.botSprite;
        towerMidSprite = data.midSprite;
        towerTopSprite = data.topSprite;
    }

    // Called by UI Button OnPointerDown
    public void BeginDrag()
    {
        Debug.Log("BeginDrag fired");
        if (towerData == null) return;
        InputManager.Instance.StartTowerDrag(towerPrefab, towerData, towerBaseSprite, towerMidSprite, towerTopSprite);
    }

    // Called by UI Button OnPointerUp
    public void EndDrag()
    {
        Debug.Log("EndDrag fired");
        if (towerData == null) return;
        InputManager.Instance.EndTowerDrag();
    }
}
