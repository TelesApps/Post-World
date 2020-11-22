using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    GameController GC;
    public GameObject RegionViewUI;
    public GameObject SelectColonistPanel;
    public Camera WorldMapCam;
    public GameObject TooltipPanel;
    [SerializeField] GameObject WorldViewUI;
    [SerializeField] ScreenTopUIPanel screenTopUIPanel;
    [SerializeField] GameObject tileOverviewPanel;
    [SerializeField] GameObject PartiesPanel;
    [SerializeField] GameObject PartiesPanelContent;
    [SerializeField] GameObject PartyPanelPrefab;
    [SerializeField] ColonyUI ColonyUI;
    [SerializeField] GameObject SelectColonyNamePanel;
    [SerializeField] Camera TileRegionCam;
    
    // Private fields bellow
    GameObject selectedRegionTileMap;
    Party partyToSettleInColony;
    // Start is called before the first frame update
    void Start()
    {
        SelectColonistPanel.SetActive(false);
        GC = GetComponent<GameController>();
        WorldMapCam.enabled = true;
        TileRegionCam.enabled = false;
        RegionViewUI.SetActive(false);
        tileOverviewPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onHexTileSelected(Tile tile, Colony col)
    {
        selectedRegionTileMap = col.RegionTileMap;
        tileOverviewPanel.SetActive(true);
        tileOverviewPanel.GetComponent<TileOverviewPanel>().setSelectedTile(tile);
        screenTopUIPanel.onColonySelected(col.getColonyData());
        ColonyUI.setColonyData(col);
    }

    public void switchCamera(bool isWorldMap)
    {
        if(isWorldMap)
        {
            WorldViewUI.SetActive(true);
            WorldMapCam.enabled = true;
            TileRegionCam.enabled = false;
            selectedRegionTileMap.SetActive(false);
        } else
        {
            RegionViewUI.SetActive(true);
            WorldMapCam.enabled = false;
            TileRegionCam.enabled = true;
            selectedRegionTileMap.SetActive(true);
        }
    }

    /// <summary>
    /// Passes a list of PartyPanel UI Prefabs to display in the Parties Panel 
    /// </summary>
    /// <param name="PartyPanels">List of all selected Parties</param>
    public void onPartiesSelected(List<Party> PartiesList)
    {
        PartiesPanel.SetActive(true);
        // First Clear out all Selected Children of Content
        foreach (Transform child in PartiesPanelContent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        // Repopulate Content with Parties
        foreach (Party party in PartiesList)
        {
            GameObject partyPanel = Instantiate(PartyPanelPrefab, PartiesPanelContent.transform);
            partyPanel.GetComponent<PartyPanel>().setColonists(party);
        }
    }

    public void onSettleParty(Party party)
    {
        this.onHexTileSelected(party.partyLocation, party.partyLocation.getColony());
        if(!GC.settlePartyIntoColony(party, party.partyLocation.getColony()))
        {
            this.partyToSettleInColony = party;
            this.SelectColonyNamePanel.SetActive(true);
        }
    }

    public void ConfirmNewColony()
    {
        TMP_InputField input = SelectColonyNamePanel.GetComponentInChildren<TMP_InputField>();
        string colonyName = input.text;
        GC.createColony(colonyName, partyToSettleInColony.partyLocation);
        GC.settlePartyIntoColony(partyToSettleInColony, partyToSettleInColony.partyLocation.getColony());
        onHexTileSelected(partyToSettleInColony.partyLocation, partyToSettleInColony.partyLocation.getColony());
        PartiesPanel.SetActive(false);
        input.text = "";
        this.SelectColonyNamePanel.SetActive(false);
        this.switchCamera(false);
    }
}
