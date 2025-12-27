using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public Tower currentTower;
    Sprite sprite;
    [SerializeField] Upgrade upgrade;

    public void InitUpgradeButton(Tower tower, Upgrade upgrade)
    {
        Image img = GetComponent<Image>();
        TMP_Text buttonText = GetComponentInChildren<TMP_Text>();
        currentTower = tower;
        if (upgrade != null)
        {
            this.upgrade = upgrade;
            // img.sprite = combinedSprite;
            img.preserveAspect = true;
            buttonText.text = upgrade.upgradeName;
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

    public void buyUpgrade()
    {
        currentTower.ApplyUpgrade(upgrade);
    }


}
