using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColonistActions : MonoBehaviour
{
    Colonist colonist;
    ColonistData CData;
    void Start()
    {
        colonist = GetComponent<Colonist>();
        CData = colonist.getColonistData();
        InvokeRepeating("updateEverySecond", 1, 1);
    }

    void Update()
    {
        
    }

    public void updateEverySecond()
    {
        if(CData.colonistStatus == ColonistData.ColonistStatus.idle)
        {
            checkForConstruction();
        }
    }

    private void checkForConstruction()
    {
        List<Building> AllBuildings = CData.ColonyResidence.getColonyData().AllBuildings;
        List<Building> inConstruction = AllBuildings.FindAll(b => b.bData.isComplete == false);
        // #TODO Add Logic to assign colonist to prioritised construction project
        if(inConstruction.Count > 0)
        {
            inConstruction[0].onAssignColonist(colonist);
            CData.colonistStatus = ColonistData.ColonistStatus.building;
        }
    }
}
