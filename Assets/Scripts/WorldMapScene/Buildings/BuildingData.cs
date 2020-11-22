using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static BuildingsDatabase;
using static ResourceDatabase;

public class BuildingData
{
    public string BuildingID { get; set; }
    public string ControlsUIPrefab { get; set; }
    public Colony InColony { get; set; }
    public BuildingType buildingType { get; set; }
    public BuildingStatus buildingStatus { get; set; }
    public Sprite imgSprite { get; set; }
    public Vector3 worldLocation { get; set; }
    public BuildingSlug slugName { get; set; }
    public string Name { get; set; }
    // CONSTRUCTION OF BUILDING RELATED
    public List<Resource> RequiredResources = new List<Resource>();
    public float ConstructionRPCRequired { get; set; }
    public float ConstructionRPCReceived = 0;
    public bool isComplete = false;
    // PRODUCTION RELATED
    public bool isActive = true;
    public int workSlotLimit { get; set; }
    public int OutputCapacity { get; set; }
    public List<Resource> ProductionOutputStorage = new List<Resource>();
    public WorkSlot[] WorkSlots { get; set; }
    public bool isStaticInputOutput = true;
    public WorkSlot StaticWorkSlotInfo = new WorkSlot("", 0);

    // Basic Constructor
    public BuildingData() { }
    // Constructor that receives the information for this instance of BuildingData. 
    // This instance becomes a copy of an existing data.
    public BuildingData(BuildingData data)
    {
        if (data.BuildingID == "" || data.BuildingID == null) { this.BuildingID = Guid.NewGuid().ToString(); }
        else { this.BuildingID = data.BuildingID; }
        this.ControlsUIPrefab = data.ControlsUIPrefab;
        this.InColony = data.InColony;
        this.buildingType = data.buildingType;
        this.buildingStatus = data.buildingStatus;
        this.imgSprite = data.imgSprite;
        this.slugName = data.slugName;
        this.Name = data.Name;
        this.RequiredResources = data.RequiredResources;
        this.OutputCapacity = data.OutputCapacity;
        this.ConstructionRPCRequired = data.ConstructionRPCRequired;
        this.workSlotLimit = data.workSlotLimit;
        this.WorkSlots = new WorkSlot[workSlotLimit];
        this.isStaticInputOutput = data.isStaticInputOutput;
        for (int i = 0; i < workSlotLimit; i++)
        {
            if (isStaticInputOutput)
            {
                WorkSlots[i] = new WorkSlot(this.BuildingID, i, data.StaticWorkSlotInfo.ProductionInputRequired, 
                    data.StaticWorkSlotInfo.isInputFromColony,
                    data.StaticWorkSlotInfo.ProductionOutput, data.StaticWorkSlotInfo.OutputRPCRequired,
                    data.StaticWorkSlotInfo.ActivityStatus, data.StaticWorkSlotInfo.SkillAffected);
                this.StaticWorkSlotInfo = data.StaticWorkSlotInfo;
            }
            else WorkSlots[i] = new WorkSlot(this.BuildingID, i);
        }
    }

    public void setResourceRequirement((ResourceSlug, int)[] requiredResources)
    {
        foreach ((ResourceSlug, int) req in requiredResources)
        {
            this.RequiredResources.Add(new Resource(ResourceDatabase.getRes(req.Item1), req.Item2));
        }
    }
}
