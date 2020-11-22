using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Party : MonoBehaviour
{
    public enum PartyStatus { Scavaging_Food, Exploring }
    public PartyStatus partyStatus;
    public Tile partyLocation;
    public float partyMoveSpeed = 0.1f;

    public List<Colonist> PartyMembers = new List<Colonist>();

    UIController UI;

    public void OnInit(List<Colonist> colonists, Tile tile)
    {
        this.partyStatus = PartyStatus.Scavaging_Food;
        this.PartyMembers = colonists;
        this.partyLocation = tile;
    }

    void Start()
    {
        UI = GameObject.Find("GameController").GetComponent<UIController>();
        InvokeRepeating("updateEverySecond", 1, 1);
    }

    void Update()
    {

    }

    void updateEverySecond()
    {
        if(partyStatus == PartyStatus.Exploring)
        {
            //#TODO Add further logic here, maybe include party Skill level at exploring etc.
            partyLocation.addToExploration(partyMoveSpeed);
        }
    }

    public void setPartyToExplore()
    {
        this.partyStatus = PartyStatus.Exploring;
        foreach (Colonist colonist in PartyMembers)
        {
            colonist.setAnimation("Walk");
        }
    } 


    public void addColonistToMember(Colonist colonist)
    {
        this.PartyMembers.Add(colonist);
    }

    private void OnMouseDown()
    {
        // Check if Mouse is over an UI Panel, if its not then perform click action
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            List<Party> partiesList = new List<Party>();
            partiesList.Add(this);
            UI.onPartiesSelected(partiesList);
        }
    }
}
