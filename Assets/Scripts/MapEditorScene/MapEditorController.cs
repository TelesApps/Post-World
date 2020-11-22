using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static MapCreation;

public class MapEditorController : MonoBehaviour
{
    [SerializeField] TMP_Dropdown sizeDrp;
    [SerializeField] TMP_Dropdown typeDrp;
    [SerializeField] TMP_Dropdown sealvlDrp;
    [SerializeField] TMP_Dropdown topographyDrp;
    [SerializeField] TMP_Dropdown temperatureDrp;
    [SerializeField] TMP_Dropdown rainDrp;
    [SerializeField] TMP_Dropdown forestryDrp;
    [SerializeField] MapCreation mapCreation;
    void Start()
    {
        //mapCreation.generateMap(MapSize.small, MapType.pangaea, SeaLvl.medium);
        //Debug.Log(DiceRoll.getTileLocationFromSide((5, 0), 5, 3));

    }

    public void onGenerateMap()
    {
        MapSize size = (MapSize)sizeDrp.value;
        MapType mapType = (MapType)typeDrp.value;
        SeaLvl seaLvl = (SeaLvl)sealvlDrp.value;
        HillsLvl topography = (HillsLvl)topographyDrp.value;
        Forestry forestry = (Forestry)forestryDrp.value;
        //Debug.Log(size);
        //Debug.Log(mapType);
        mapCreation.generateMap(size, mapType, seaLvl, topography, temperatureDrp.value, rainDrp.value, forestry);
    }

    
}
