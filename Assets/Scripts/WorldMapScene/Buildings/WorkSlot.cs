using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ColonistData;
using static ResourceDatabase;
using static Skill;

public class WorkSlot
{
    public string ParentBuildingID;
    public int SlotNumber { get; set; }
    public bool isActive = false;
    public string taskDescription { get; set; }
    public Colonist ColonistAssigned { get; set; }
    public ColonistStatus ActivityStatus { get; set; }
    public HashSet<Resource> ProductionInputRequired = new HashSet<Resource>();
    public HashSet<Resource> ProductionInputReceived = new HashSet<Resource>();
    public bool isInputFromColony = false;
    public HashSet<Resource> ProductionOutput = new HashSet<Resource>();
    public float OutputRPCRequired { get; set; }
    public float OutputRPCReceived { get; set; }
    // SkillAffected is the Skill required to Perform this task and the skills colonist gains completing the task.
    // the first SkillAffected[0] is always the required skill for the task.
    public List<SkillSlug> SkillAffected = new List<SkillSlug>();

    public WorkSlot(string parentBuilding, int slotNumber) 
    {
        this.ParentBuildingID = parentBuilding;
        this.SlotNumber = slotNumber;
    }
    public WorkSlot (string parentBuilding, int slotNumber, HashSet<Resource> productionInputRequired, bool isInputFromColony, 
        HashSet<Resource> productionOutput, float outputRPCRequired, ColonistStatus activityStatus, List<SkillSlug> skillAffected, 
        string taskDescription = "")
    {
        this.ParentBuildingID = parentBuilding;
        this.SlotNumber = slotNumber;
        this.ProductionInputRequired = productionInputRequired;
        this.isInputFromColony = isInputFromColony;
        this.ProductionOutput = productionOutput;
        this.OutputRPCRequired = outputRPCRequired;
        this.taskDescription = taskDescription;
        this.SkillAffected = skillAffected;
        this.ActivityStatus = activityStatus;
    }

    public void AssignColonist(Colonist colonist, string parentBuildingId)
    {
        this.ParentBuildingID = parentBuildingId;
        colonist.setColonistStatus(ActivityStatus, taskDescription);
        this.ColonistAssigned = colonist;
        this.ColonistAssigned.getColonistData().AssignedWorkslot = this;
        this.isActive = true;
    }

    public Colonist removeColonist()
    {
        Colonist col = this.ColonistAssigned;
        this.ColonistAssigned = null;
        this.isActive = false;
        return col;
    }

    public void setResourceInputRequirement((ResourceSlug, int)[] requiredResources)
    {
        foreach ((ResourceSlug, int) req in requiredResources)
        {
            this.ProductionInputRequired.Add(new Resource(ResourceDatabase.getRes(req.Item1), req.Item2));
        }
    }

    public void setResourceOutput((ResourceSlug, int)[] requiredResources)
    {
        foreach ((ResourceSlug, int) req in requiredResources)
        {
            this.ProductionOutput.Add(new Resource(ResourceDatabase.getRes(req.Item1), req.Item2));
        }
    }
}
