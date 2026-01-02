using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    public Tower currentTower;
    Sprite sprite;
    Upgrade upgrade;
    Image img;
    TMP_Text buttonText;
    public void InitUpgradeButton(Tower tower, Upgrade upgrade)
    {
        img = GetComponent<Image>();
        buttonText = GetComponentInChildren<TMP_Text>();
        currentTower = tower;
        UpdateButton(upgrade);
    }
    void UpdateButton(Upgrade upgrade)
    {
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
    public void BuyUpgrade()
    {
        if (upgrade == null)
            return;
        else if (GameManager.Instance.money >= upgrade.cost)
        {
            currentTower.ApplyUpgrade(upgrade);
            GameManager.Instance.SpendMoney(upgrade.cost);
            upgrade = null;
            UpdateButton(upgrade);
        }
        else
        {
            //Not enough money
        }

    }


}
