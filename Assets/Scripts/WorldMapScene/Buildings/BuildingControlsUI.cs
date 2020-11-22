using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingControlsUI : MonoBehaviour
{
    [SerializeField] GameObject ColonistTaskProgressPrefab;
    [SerializeField] Transform SelectedSpawnParent;
    [SerializeField] Transform[] TaskProgressSpawnParents;
    GameController GC;
    GameObject selectColonistPanelObj;
    Building building;
    List<(Colonist, Transform)> assignedColonists = new List<(Colonist, Transform)>(); 
    int selectedSlotIndex = 0;

    private void Awake()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GameController");
        this.GC = obj.GetComponent<GameController>();
        selectColonistPanelObj = obj.GetComponent<UIController>().SelectColonistPanel;
    }
    public void OnInit(Building building)
    {
        this.building = building;
    } 

    void Start()
    {

    }

    public void setStorageFieldWorkSlot(int selection)
    {
        WorkSlot workSlot = BuildingsDatabase.getFieldStorageWorkSlotData(selection);
        selectedSlotIndex = getAvailableSlotIndex();
        workSlot.SlotNumber = selectedSlotIndex;
        if (selectedSlotIndex != -1)
            building.bData.WorkSlots[selectedSlotIndex] = workSlot;
        callSelectColonist(Skill.SkillSlug.Scavanging);
    }

    public void onSelectColonistForSlot(int slotIndex)
    {
        this.selectedSlotIndex = slotIndex;
        this.SelectedSpawnParent = TaskProgressSpawnParents[slotIndex];
        callSelectColonist(building.bData.StaticWorkSlotInfo.SkillAffected[0]);
    }
    /// <summary>
    /// Subscribes to the function to select a colonist and listens to the return selection
    /// </summary>
    /// <param name="skill">The skill required to perform this function, displays this skill level for each colonist in the list</param>
    public void callSelectColonist(Skill.SkillSlug skill)
    {
        selectColonistPanelObj.SetActive(true);
        SelectColonistPanel selectColonistPanel = selectColonistPanelObj.GetComponent<SelectColonistPanel>();
        selectColonistPanel.onStartSelection(building.bData.InColony.getAllColonists(), skill);
        SelectColonistPanel.onSelected += ColonistSelected;
    }

    public void onCancelColonistTask(int slotNumber, Transform taskProgressPanel = null)
    {
        Colonist colonist = building.bData.WorkSlots[slotNumber].removeColonist();
        colonist.setColonistStatus(ColonistData.ColonistStatus.idle, "Idle");
        (Colonist, Transform) taskPanel =
                this.assignedColonists.Find(obj => obj.Item1.getColonistData().ColonistId == colonist.getColonistData().ColonistId);
        this.assignedColonists.Remove(taskPanel);
        Destroy(taskPanel.Item2.gameObject);
    }

    private int getAvailableSlotIndex()
    {
        int index = new List<WorkSlot>(building.bData.WorkSlots).FindIndex(ws => ws.isActive == false);
        return index;
    }

    private void ColonistSelected(Colonist colonist)
    {
        // Unsubscribe;
        SelectColonistPanel.onSelected -= ColonistSelected;
        if(colonist != null)
        {
            // If colonist is already working elsewhere, cancel that assignent and assign him to new location
            if(colonist.getColonistData().colonistStatus != ColonistData.ColonistStatus.idle)
            {
                WorkSlot workSlot = colonist.getColonistData().AssignedWorkslot;
                Building building = GC.getBuildingFromID(workSlot.ParentBuildingID);
                if(building.bData.BuildingID == this.building.bData.BuildingID)
                {
                    Debug.Log("Building is the same as before");
                }
                building.getControlsUI().onCancelColonistTask(workSlot.SlotNumber);
                assignColonistToWorkSlot(colonist);
            }
            else
            {
                assignColonistToWorkSlot(colonist);
            }
        }
    }

    private void assignColonistToWorkSlot(Colonist colonist)
    {
        GameObject taskProgressPanel = Instantiate(ColonistTaskProgressPrefab, SelectedSpawnParent);
        this.assignedColonists.Add((colonist, taskProgressPanel.transform));
        taskProgressPanel.GetComponent<TaskProgressPanel>().OnInit(colonist, building.bData.WorkSlots[selectedSlotIndex]);
        if (selectedSlotIndex != -1)
            building.bData.WorkSlots[selectedSlotIndex].AssignColonist(colonist, building.bData.BuildingID);
    }

}
