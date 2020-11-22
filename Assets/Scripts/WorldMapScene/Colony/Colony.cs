using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static ResourceDatabase;

public class Colony : MonoBehaviour
{
    [SerializeField] SpriteRenderer colonyImg;
    [SerializeField] GameObject ColonyNameCanvas;
    [SerializeField] TextMeshProUGUI ColonyNameTxt;
    [SerializeField] GameObject BuildingPrefab;
    GameController GC;
    public GameObject RegionTileMap;
    Tile inTile;

    //[SerializeField] Transform DevBuildingSpawnLocation;
    Transform buildingsSpawnLocation;
    //Transform 
    ColonyData CD = new ColonyData();
    private void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GameController");
        this.GC = obj.GetComponent<GameController>();
        inTile = GetComponentInParent<Tile>();
    }

    void Update()
    {
        
    }

    public void setRegionTilemap(GameObject regionTileMap)
    {
        this.RegionTileMap = regionTileMap;
        List<Transform> children = new List<Transform>(regionTileMap.GetComponentsInChildren<Transform>());
        buildingsSpawnLocation = children.Find(c => c.name == "ColonyBuildings");
    }

    public ColonyData getColonyData()
    {
        return this.CD;
    }

    public void onColonySettled(string colonyName)
    {
        ColonyNameCanvas.SetActive(true);
        ColonyNameTxt.text = colonyName;
        this.CD.ColonyName = colonyName;
        this.CD.IsColonized = true;
        colonyImg.sprite = Resources.Load<Sprite>("Decor/house00");
    }

    /// <summary>
    /// Sets or adds a specific resource to the total amount in the colony, to deduct from resources simply pass negative numbers
    /// </summary>
    /// <param name="slug">The resource that is being changed</param>
    /// <param name="amount">The amount it is being changed by</param>
    /// <param name="isAdding">Is the amount to be added or to be set</param>
    /// <returns>Returns true addition is successful and False it it fails due to resource limitations</returns>
    public bool setResourceAmount(ResourceSlug slug, float amount, bool isAdding = true)
    {
        bool isSuccess = CD.setResource(slug, amount, isAdding);
        return isSuccess;
    }

    public bool collectResourceFromWild(ResourceSlug slug, float amount)
    {
        Resource resource = inTile.getTileResources().Find(r => r.NameSlug == slug);
        if(resource != null && resource.Amount + amount > -1)
        {
            resource.Amount -= amount;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Adds a colonist to this colony provided he is not already in this colony
    /// </summary>
    /// <param name="colonist">The Colonist to add to this colony</param>
    public void addColonist(Colonist colonist)
    {
        Colonist col = CD.AllColonists.Find(c => c.getColonistData().ColonistId == colonist.getColonistData().ColonistId);
        if(col == null)
        {
            CD.AllColonists.Add(colonist);
        }
    }

    public void addNewBuilding(BuildingData buildingData)
    {
        foreach (Resource resource in buildingData.RequiredResources)
        {
            CD.setResource(resource.NameSlug, -resource.Amount, true);
        }
        GameObject buildingObj = Instantiate(BuildingPrefab, buildingsSpawnLocation);
        buildingObj.transform.position = buildingData.worldLocation;
        Building building = buildingObj.GetComponent<Building>();
        building.onInit(buildingData);
        CD.AllBuildings.Add(building);
        this.GC.AddToAllBuildingsList(building);
    }

    public List<Colonist> getIdleColonists()
    {
        List<Colonist> idle = new List<Colonist>();
        idle = CD.AllColonists.FindAll(c => c.getColonistData().colonistStatus == ColonistData.ColonistStatus.idle);
        return idle;
    }

    public List<Colonist> getAllColonists()
    {
        return CD.AllColonists;
    }
}
