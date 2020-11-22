using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColonyUI : MonoBehaviour
{
    Colony Colony;
    [SerializeField] GameObject ColonistsPanel;
    [SerializeField] TextMeshProUGUI TotalColonistTxt;
    [SerializeField] TextMeshProUGUI IdleColonistTxt;
    [SerializeField] GameObject ColonyControlsPanel;
    [SerializeField] GameObject BuildingShadowPrefab;
    
    GameObject buildingShadow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Colony != null && Colony.getColonyData().AllColonists != null)
        {
            TotalColonistTxt.text = Colony.getColonyData().AllColonists.Count.ToString();
            // For better performance add logic to check if the following function needs to be called here or elsewhere
            IdleColonistTxt.text = Colony.getIdleColonists().Count.ToString();
        }
    }

    public void setColonyData(Colony colony)
    {
        this.Colony = colony;
        if(colony.getColonyData().IsColonized)
        {
            ColonistsPanel.SetActive(true);
            ColonyControlsPanel.SetActive(true);
        } else
        {
            ColonistsPanel.SetActive(false);
            ColonyControlsPanel.SetActive(false);
        }
    }

    public void startSelectBuildingLocationMode(BuildingData buildingData)
    {
        buildingShadow = Instantiate(BuildingShadowPrefab);
        buildingShadow.GetComponent<BuildingShadow>().onInit(Colony, buildingData);
    }
    

    public Colony getColony()
    {
        return Colony;
    }

}
