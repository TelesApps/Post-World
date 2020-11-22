using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static MapCreation;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] TMP_Dropdown sizeDrp;
    [SerializeField] TMP_Dropdown typeDrp;
    [SerializeField] TMP_Dropdown sealvlDrp;
    [SerializeField] TMP_Dropdown topographyDrp;
    [SerializeField] TMP_Dropdown temperatureDrp;
    [SerializeField] TMP_Dropdown rainDrp;
    [SerializeField] TMP_Dropdown forestryDrp;
    [SerializeField] scenePersistence sPercistance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startGame()
    {
        sPercistance.setMapProperties((MapSize)sizeDrp.value, (MapType)typeDrp.value, (SeaLvl)sealvlDrp.value, 
            (HillsLvl)topographyDrp.value,temperatureDrp.value, rainDrp.value, (Forestry)forestryDrp.value);
        SceneManager.LoadScene("WorldMapScene", LoadSceneMode.Single);
    }

    public void startMapEditor()
    {
        SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
    }

}
