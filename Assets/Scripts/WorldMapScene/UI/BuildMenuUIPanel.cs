using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildMenuUIPanel : MonoBehaviour
{
    [SerializeField] ColonyUI colonyUI;
    [SerializeField] GameObject BuildingUIPanelPrefab;
    [SerializeField] Transform ResideceContentGrid;
    [SerializeField] Transform StorageContentGrid;
    [SerializeField] Transform ProductionContentGrid;

    // Write a script that populates the Build Menu with buildings based on the Database.
    void Start()
    {
        // #TODO add logic to only show buildings that can be built or gray out the ones that cannot be built yet.
        foreach (BuildingData bData in BuildingsDatabase.AllBuildings)
        {
            // All Storage Type Buildings
            if(bData.buildingType == BuildingsDatabase.BuildingType.storage)
            {
                GameObject UIPanel = Instantiate(BuildingUIPanelPrefab, StorageContentGrid);
                BuildingUIPanel panel = UIPanel.GetComponent<BuildingUIPanel>();
                panel.onInitPanel(this.gameObject, colonyUI.getColony(), bData);
            }
        }
    }

    void Update()
    {
        
    }
}
