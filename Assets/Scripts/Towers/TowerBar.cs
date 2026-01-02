using UnityEngine;
using System.Collections.Generic;

public class TowerBar: MonoBehaviour
{
    public List<TowerData> towers;     
    public List<TowerButton> towerButtons;
    public List<UpgradeButton>  upgradeButtons;

    void Start()
    {
        for (int i = 0; i < towerButtons.Count; i++)
        {
            TowerButton button = towerButtons[i];
            button.InitTowerButton((i < towers.Count) ? towers[i] : null);

        }
    }

    public void ShowUpgradeMenu(Tower tower)
    {
        for (int i = 0; i < towerButtons.Count; i++)
        {
            GameObject child = towerButtons[i].gameObject;
            child.SetActive(false); 
        }
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            GameObject child = upgradeButtons[i].gameObject;
            child.SetActive(true);
            UpgradeButton button = upgradeButtons[i];

            /*

            Upgrade upgrade;
            if (i < tower.upgrades.Count)
            {
                upgrade = tower.upgrades[i];
            }
            else
            {
                upgrade = null;
            }*/

            Upgrade upgrade = (i < tower.upgrades.Count && tower.upgrades[i].available) ? tower.upgrades[i] : null;
            button.InitUpgradeButton(tower, upgrade);

        }


    }
    public void ShowTowerMenu()
    {
        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            GameObject child = upgradeButtons[i].gameObject;
            child.SetActive(false); 
        }
        for (int i = 0; i < towerButtons.Count; i++)
        {
            GameObject child = towerButtons[i].gameObject;
            child.SetActive(true); 
        }

    }
}
