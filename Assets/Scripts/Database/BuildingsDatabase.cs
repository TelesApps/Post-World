using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceDatabase;

public class BuildingsDatabase
{
    public enum BuildingType { residence, production, refinery, farming, storage }
    public enum BuildingSlug { storageField, shack }
    public enum BuildingStatus { working, inactive, lacking_Worker, lacking_Resources, storage_Full }

    public static List<BuildingData> AllBuildings = new List<BuildingData>();

    // The storage field is a unique type building so here we define specific workslot data for that building
    public static WorkSlot getFieldStorageWorkSlotData(int index)
    {
        WorkSlot workSlot = new WorkSlot("", 0);
        workSlot.isInputFromColony = false;
        workSlot.OutputRPCRequired = 8;
        workSlot.ActivityStatus = ColonistData.ColonistStatus.scavanging;
        workSlot.SkillAffected.Add(Skill.SkillSlug.Scavanging);
        (ResourceSlug, int)[] resInputOutput = new (ResourceSlug, int)[] { };
        //Gathering Wood
        if(index == 0)
        {
            resInputOutput = new (ResourceSlug, int)[] { (ResourceSlug.berries, 1) };
            workSlot.taskDescription = "Scavaging for food";
        }    
        else if (index == 1)
        {
            resInputOutput = new (ResourceSlug, int)[] { (ResourceSlug.wood, 1) };
            workSlot.taskDescription = "Collecting wood";
        }    
        else if (index == 2)
        {
            resInputOutput = new (ResourceSlug, int)[] { (ResourceSlug.rock, 1) };
            workSlot.taskDescription = "Collecting rocks";
        }    
        else if (index == 3)
        {
            resInputOutput = new (ResourceSlug, int)[] { (ResourceSlug.clay, 1) };
            workSlot.taskDescription = "Collecting clay";
        }    
        workSlot.setResourceInputRequirement(resInputOutput);
        workSlot.setResourceOutput(resInputOutput);
        return workSlot;
    }

    public BuildingsDatabase()
    {
        // StorageField
        {
            BuildingData bd = new BuildingData();
            bd.buildingType = BuildingType.storage;
            bd.ControlsUIPrefab = "Buildings/BuildingControls/StorageField";
            bd.imgSprite = Resources.Load<Sprite>("Buildings/Storage");
            bd.slugName = BuildingSlug.storageField;
            bd.Name = "Storage Field";
            bd.OutputCapacity = 10;
            // REQUIREMENTS TO BUILD
            bd.ConstructionRPCRequired = 1;
            (ResourceSlug, int)[] requiredResources =
            {
                (ResourceSlug.wood, 1),
            };
            bd.setResourceRequirement(requiredResources);
            // BUILDING PRODUCTION
            bd.workSlotLimit = 10;
            bd.isStaticInputOutput = false;
            AllBuildings.Add(bd);
        }

        // *********** LUMBER MILL ************
        {
            BuildingData bd = new BuildingData();
            bd.buildingType = BuildingType.storage;
            bd.ControlsUIPrefab = "Buildings/BuildingControls/BuildingControls-4Slots";
            bd.imgSprite = Resources.Load<Sprite>("Buildings/Sawmill");
            bd.slugName = BuildingSlug.storageField;
            bd.Name = "Lumber Mill";
            bd.OutputCapacity = 4;
            // Requirements to build
            bd.ConstructionRPCRequired = 1;
            (ResourceSlug, int)[] requiredResources =
            {
                (ResourceSlug.wood, 2),
            };
            bd.setResourceRequirement(requiredResources);
            // BUILDING PRODUCTION
            bd.workSlotLimit = 4;
            bd.StaticWorkSlotInfo.ActivityStatus = ColonistData.ColonistStatus.working;
            bd.StaticWorkSlotInfo.taskDescription = "Producing LUMBER in LUMBER MILL";
            bd.StaticWorkSlotInfo.SkillAffected.Add(Skill.SkillSlug.Lumber_Mill);
            bd.StaticWorkSlotInfo.OutputRPCRequired = 8;
            bd.StaticWorkSlotInfo.isInputFromColony = true;
            (ResourceSlug, int)[] requiredInput =
            {
                //LIST OF INPUT RESOURCES, AND AMOUNT REQUIRED
                (ResourceSlug.wood, 1),
            };
            bd.StaticWorkSlotInfo.setResourceInputRequirement(requiredInput);
            (ResourceSlug, int)[] resourceOutput =
            {
                //LIST OF OUTPUT RESOURCES, AND AMOUNT YELD
                (ResourceSlug.lumber, 1),
            };
            bd.StaticWorkSlotInfo.setResourceOutput(resourceOutput);

            AllBuildings.Add(bd);
        }
    } 


}
