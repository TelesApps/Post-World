using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ResourceDatabase;

public class BuildingUIPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI NameTxt;
    [SerializeField] GameObject ResourceUIPrefab;
    [SerializeField] Transform RequiredResourcePanel;
    [SerializeField] Color32 redColor;
    [SerializeField] Color32 whiteColor;
    [SerializeField] Image buildingImg;
    BuildingData BD;
    List<Resource> ColonyResources;
    List<GameObject> ResourcePanels = new List<GameObject>();
    public bool hasEnoughResource = false;

    GameObject ParentUI;
    ColonyUI colonyUI;

    public void onInitPanel(GameObject parentUI, Colony colony, BuildingData data)
    {
        this.ParentUI = parentUI;
        this.ColonyResources = colony.getColonyData().AllResources;
        this.BD = data;
        this.BD.InColony = colony;
        buildingImg.sprite = data.imgSprite;
        NameTxt.text = data.Name;
        foreach (Resource res in data.RequiredResources)
        {
            GameObject resourcepanel = Instantiate(ResourceUIPrefab, RequiredResourcePanel);
            ResourceUI resUI = resourcepanel.GetComponent<ResourceUI>();
            resUI.onInitResourcePanel(res);
            ResourcePanels.Add(resourcepanel);
        }
    }
    void Start()
    {
        GameObject GC = GameObject.FindGameObjectWithTag("GameController");
        colonyUI = GC.GetComponent<ColonyUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.activeSelf)
        {
            this.hasEnoughResource = true;
            foreach (GameObject obj in ResourcePanels)
            {
                ResourceUI resUI = obj.GetComponent<ResourceUI>();
                Resource colonyRes = ColonyResources.Find(r => r.NameSlug == resUI.getResource().NameSlug);
                if(colonyRes == null || colonyRes.Amount < resUI.getResource().Amount)
                {
                    this.hasEnoughResource = false;
                    resUI.getTextMeshProRef().color = redColor;
                } else
                {
                    resUI.getTextMeshProRef().color = whiteColor;
                }
            }
        }
    }

    public void onClickBuilding()
    {
        Debug.Log("onclicked pushed");
        if(this.hasEnoughResource)
        {

            ParentUI.SetActive(false);
            BuildingData newBuildingData = new BuildingData(BD);
            colonyUI.startSelectBuildingLocationMode(newBuildingData);
        }
        else
        {
            Debug.Log("Not enough resources");
        }
    }
}
