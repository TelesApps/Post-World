using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingActions : MonoBehaviour
{
    Building building;
    BuildingDisplay bDisplay;
    BuildingData bData;

    HashSet<Colonist> ConstructionColonists = new HashSet<Colonist>();
    void Start()
    {
        bDisplay = GetComponent<BuildingDisplay>();
        building = GetComponent<Building>();
        bData = building.bData;
        InvokeRepeating("updateEverySecond", 1, 1);
    }

    void Update()
    {
        // GRAB RESOURCES IF AVAILABLE
        if(bData.isActive && new List<WorkSlot>(bData.WorkSlots).Exists(ws => ws.isActive))
        {
            List<WorkSlot> activeSlots = new List<WorkSlot>(bData.WorkSlots).FindAll(ws => ws.isActive == true);
            foreach (WorkSlot slot in activeSlots)
            {
                bool hasGathered = true;
                if (slot.ProductionInputReceived.Count != slot.ProductionInputRequired.Count)
                    hasGathered = gatherResourcesForInput(slot, new List<Resource>(slot.ProductionInputRequired), slot.isInputFromColony);
                if(hasGathered == false)
                {
                    Debug.Log("Not Enough Resource");
                    slot.isActive = false;
                    Debug.LogWarning("made slot inactive, but need further logic here");
                }
            }
        }
        // Transfer resource from building output to colony storage if possible.
        if(bData.ProductionOutputStorage.Count > 0)
        {
            for (int i = bData.ProductionOutputStorage.Count - 1; i >= 0; i--)
            {
                bool isSuccess = bData.InColony.setResourceAmount(bData.ProductionOutputStorage[i].NameSlug, bData.ProductionOutputStorage[i].Amount);
                if(isSuccess)
                {
                    bData.ProductionOutputStorage.RemoveAt(i);
                    if (bData.buildingStatus == BuildingsDatabase.BuildingStatus.storage_Full)
                        bData.buildingStatus = BuildingsDatabase.BuildingStatus.working;
                }
            }

        }
    }

    public void updateEverySecond()
    {
        if(building.bData.isActive && bData.buildingStatus != BuildingsDatabase.BuildingStatus.storage_Full)
        {
            List<WorkSlot> activeSlots = new List<WorkSlot>(bData.WorkSlots).FindAll(ws => ws.isActive == true);
            foreach (WorkSlot WS in activeSlots)
            {
                bool isFinished = addToProduction(WS);
                if(isFinished)
                {
                    if(bData.ProductionOutputStorage.Count + WS.ProductionOutput.Count <= bData.OutputCapacity)
                    {
                        foreach (Resource res in WS.ProductionOutput)
                        {
                            bData.ProductionOutputStorage.Add(res);
                        }
                    } else
                    {
                        bData.buildingStatus = BuildingsDatabase.BuildingStatus.storage_Full;
                    }
                }
            }
        }
        // CONSTRUCTION OF BUILDING
        if(!building.bData.isComplete)
        {
            foreach (Colonist colonist in ConstructionColonists)
            {
                bData.ConstructionRPCReceived += colonist.getProduction(Skill.SkillSlug.Construction);
                colonist.addToColonistSkill(Skill.SkillSlug.Construction);
                if (building.bData.ConstructionRPCReceived > building.bData.ConstructionRPCRequired)
                {
                    // Building construction is done
                    bData.ConstructionRPCReceived = bData.ConstructionRPCRequired;
                    onFinishConstruction();
                }
            }
        }
        else if(ConstructionColonists.Count > 0)
        {
            foreach (Colonist colonist in ConstructionColonists)
            {
                colonist.setColonistStatus(ColonistData.ColonistStatus.idle, "Idle");
            }
            ConstructionColonists.Clear();
        }
    }

    public void addColonistsToConstruction(Colonist colonist)
    {
        if (!building.bData.isComplete)
        {
            ConstructionColonists.Add(colonist);
        }
    }

    void onFinishConstruction()
    {
        building.bData.isComplete = true;
        bDisplay.CompleteConstruction();
    }

    /// <summary>
    /// Gathers a resource into this building's WorkSlot Input
    /// </summary>
    /// <param name="slot">The WorkSLot the resource is being collected for</param>
    /// <param name="resources">The resource gathered</param>
    /// <param name="isFromColony">true if the resource is being gathered from the Colony's storage, False if gathered from the wild</param>
    /// <returns>true if the resource was successfully gathered and False if it was not</returns>
    bool gatherResourcesForInput(WorkSlot slot, List<Resource> resources, bool isFromColony)
    {
        Debug.Log("Gathering resource");
        if(isFromColony)
        {
            bool isAllGathered = true;
            foreach (Resource res in resources)
            {
                if (!slot.ProductionInputReceived.Contains(res))
                {
                    bool isGathered = bData.InColony.setResourceAmount(res.NameSlug, -res.Amount);
                    if (isGathered)
                        slot.ProductionInputReceived.Add(res);
                    else isAllGathered = false;
                }
                else continue;
            }
            return isAllGathered;
        } else
        {
            bool isAllGathered = true;
            foreach (Resource res in resources)
            {
                if (!slot.ProductionInputReceived.Contains(res))
                {
                    bool isGathered = bData.InColony.collectResourceFromWild(res.NameSlug, res.Amount);
                    if (isGathered)
                        slot.ProductionInputReceived.Add(res);
                    else isAllGathered = false;
                }
                else continue;
            }
            return isAllGathered;
        }
    }

    public bool addToProduction(WorkSlot workSlot)
    {
        bool isFinished = false; 
        Colonist colonist = workSlot.ColonistAssigned;
        workSlot.OutputRPCReceived += colonist.getProduction(workSlot.SkillAffected[0]);
        if (workSlot.OutputRPCReceived >= workSlot.OutputRPCRequired)
        {
            workSlot.OutputRPCReceived -= workSlot.OutputRPCRequired;
            isFinished = true;
        }
        return isFinished;
    }

}
