using UnityEngine;
using System.Collections.Generic;

public class TowerBar: MonoBehaviour
{
    public List<TowerData> towers;     
    public List<TowerButton> towerButtons;

    void Start()
    {
        for (int i = 0; i < towers.Count; i++)
        {
            TowerButton button = towerButtons[i];
            button.Init(towers[i]);
        }
    }

}
