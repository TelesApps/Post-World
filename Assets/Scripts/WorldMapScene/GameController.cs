using CharacterCreator2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static scenePersistence;

public class GameController : MonoBehaviour
{
    public bool isSummerSeason = true;
    Dictionary<(int, int), TileData> AllTiles;
    UIController UI;
    [SerializeField] GameObject ColonistPrefab;
    // Conolist Spawn Grid is the world location where each Colonist in existance stay.
    [SerializeField] Transform ColonistSpawnGrid;
    [SerializeField] GameObject PartyIconPrefab;
    // All Colonists Alive in Game.
    List<Colonist> AllColonist = new List<Colonist>();
    // All Buildings Ever Constructed in Game
    List<Building> AllBuildings = new List<Building>();
    private void Awake()
    {
        // Instantiate All Databases here.
        ResourceDatabase rDB = new ResourceDatabase();
        BuildingsDatabase bDB = new BuildingsDatabase();
        UI = GetComponent<UIController>();
    }
    void Start()
    {
        //#TODO Add Logic for generating all the regions with load screen/bar
        MapCreation mapCreation = GetComponent<MapCreation>();
        List<TileData> startingLocations;
        GameObject SPObj = GameObject.Find("ScenePersistenceObj");
        if (SPObj == null) Debug.LogWarning("Scene PersistanceObj not found");
        else
        {
            scenePersistence SP = SPObj.GetComponent<scenePersistence>();
            (AllTiles, startingLocations) = mapCreation.generateMap(SP.mapSize, SP.mapType, SP.seaLvl, SP.topography,
                SP.temperatureLvl, SP.rainLvl, SP.forestry);
            this.setStartingLocation(startingLocations);
        }
        //Test Code Bellow So you don't have to constantly load game from main menu scene
        (AllTiles, startingLocations) = mapCreation.generateMap(MapCreation.MapSize.tiny, MapCreation.MapType.pangaea,
            MapCreation.SeaLvl.medium, MapCreation.HillsLvl.average, 1, 1, MapCreation.Forestry.Medium);
        //mapCreation.InstanciateAllRegions(AllTiles);
        this.setStartingLocation(startingLocations);
        //End of Test COde
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setStartingLocation(List<TileData> locations)
    {
        // #TODO Add more logic to select starting location based on user selection for easier settings.
        TileData startingTile = locations[Random.Range(0, locations.Count -1)];
        if (startingTile != null)
        {
            startingTile.isVisible = true;
            UI.WorldMapCam.transform.position = new Vector3(startingTile.worldLocation.x, startingTile.worldLocation.y, -10);
            createColonist(startingTile);
            GameObject tileObj = getTileObjFromID(startingTile.ID);
            Tile tile = tileObj.GetComponent<Tile>();
            createParty(AllColonist, tile);
            // Send to UI the starting selected tile.
            Colony colony = tile.getColony();
            colony.setResourceAmount(ResourceDatabase.ResourceSlug.wood, 4);
            colony.setResourceAmount(ResourceDatabase.ResourceSlug.berries, 10);
            UI.onHexTileSelected(tile, colony);

        }
        else Debug.LogError("startingTile Not Found");
    }

    public void createColonist(TileData startingTile)
    {
        GameObject colonist = Instantiate<GameObject>(ColonistPrefab, ColonistSpawnGrid);
        AllColonist.Add(colonist.GetComponent<Colonist>());

        // TODO Set Colonists Background according to the tile he is in.
    }

    /// <summary>
    ///  Create a party of colonist to explore the world map
    /// </summary>
    /// <param name="Colonists">The list of COlonists in this party</param>
    /// <param name="startingTile">The hex Tile that the party will be formed at</param>
    public void createParty(List<Colonist> colonists, Tile startingTile)
    {
        GameObject partyIcon = Instantiate(PartyIconPrefab);
        Party party = partyIcon.GetComponent<Party>();
        party.OnInit(colonists, startingTile);
        startingTile.addPartyToTile(party);
        partyIcon.transform.position = startingTile.getTileData().inTilePartyLocations[startingTile.getOnTilePartiesCount() - 1];
    }

    /// <summary>
    /// Creates a new Colony on a tile
    /// </summary>
    /// <param name="colonyName">The Colony Name</param>
    /// <param name="tile">the tile the new colony is being established</param>
    public void createColony(string colonyName, Tile tile)
    {
        tile.getColony().onColonySettled(colonyName);
    }

    
    /// <summary>
    /// Settles a party into an existing colony, if there is no colony then it simply returns false;
    /// </summary>
    /// <param name="party">The party setteling into this colony</param>
    /// <param name="colony">The colony that party is settling into</param>
    /// <returns>True if colony already exists, and False if it does not</returns>
    public bool settlePartyIntoColony(Party party, Colony colony)
    {
        if(!colony.getColonyData().IsColonized)
            return false;
        foreach (Colonist col in party.PartyMembers)
        {
            colony.addColonist(col);
            col.onSettleInColony(colony);
        }
        colony.getColonyData().IsColonized = true;
        party.partyLocation.removePartyFromTile(party);
        Destroy(party.gameObject);

        return true;
    }

    public void toggleSeason()
    {
        this.isSummerSeason = !this.isSummerSeason;
    }

    public Tile GetTileFromDictionary((int, int) location)
    {
        AllTiles.TryGetValue(location, out TileData tileData);
        if (tileData != null)
        {
            List<GameObject> TilesObj = new List<GameObject>(GameObject.FindGameObjectsWithTag("HexTile"));
            Tile tile = TilesObj.Find(t => t.GetComponent<Tile>().getTileData().ID == tileData.ID).GetComponent<Tile>();
            return tile;
        }
        else return null;
    }

    public GameObject getTileObjFromID(string ID)
    {
        GameObject[] TilesObj = GameObject.FindGameObjectsWithTag("HexTile");
        foreach (GameObject obj in TilesObj)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile.getTileData().ID == ID)
            {
                return obj;
            }
        }
        return null;
    }

    public TileData getTileDataFromDic((int, int) location)
    {
        AllTiles.TryGetValue(location, out TileData tile);
        return tile;
    }

    public void AddToAllBuildingsList(Building building)
    {
        if(!AllBuildings.Exists(b => b.bData.BuildingID == building.bData.BuildingID))
            this.AllBuildings.Add(building);
    }

    public Building getBuildingFromID(string id)
    {
        Building building = this.AllBuildings.Find(b => b.bData.BuildingID == id);
        return building;
    }
}
