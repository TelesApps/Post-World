using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Building : MonoBehaviour
{
    public BuildingData bData { get; set; }
    BuildingDisplay bDisplay;
    BuildingActions bActions;
    BuildingControlsUI ControlsUI;
    UIController UI;
    private void Awake()
    {
        bDisplay = GetComponent<BuildingDisplay>();
        bActions = GetComponent<BuildingActions>();
        GameObject obj = GameObject.FindGameObjectWithTag("GameController");
        UI = obj.GetComponent<UIController>();
    }
    public void onInit(BuildingData data)
    {
        this.bData = data;
        bDisplay.setBuildingImg(data.imgSprite);
        GameObject ControlsUIObbj = Instantiate(Resources.Load<GameObject>(data.ControlsUIPrefab), UI.RegionViewUI.transform);
        ControlsUIObbj.SetActive(false);
        ControlsUI = ControlsUIObbj.GetComponent<BuildingControlsUI>();
        ControlsUI.OnInit(this);
    } 

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Tree")
        {
            Destroy(collision.gameObject);
        }
    }

    private void OnMouseDown()
    {
        //#TODO Add Logic for pulling up contruction UI for when a building is under construction.
        if(bData.isComplete)
        {
            ControlsUI.gameObject.SetActive(true);
        }
    }


    public void onAssignColonist(Colonist colonist)
    {
        if(!bData.isComplete)
        {
            bActions.addColonistsToConstruction(colonist);
        }
    }

    public BuildingControlsUI getControlsUI()
    {
        return this.ControlsUI;
    }


}
