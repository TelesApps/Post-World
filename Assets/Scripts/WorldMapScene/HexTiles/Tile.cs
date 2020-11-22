using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainFetcher;

public class Tile : MonoBehaviour
{
    int revealNeighborsValue = 10;
    [SerializeField] SpriteRenderer river;
    TileData tileData;
    Colony colony;
    //int water;
    List<Resource> AllResources;
    SpriteRenderer spriteRenderer;
    GameController GC;
    UIController UI;

    Camera WorldCamera;

    private void Awake()
    {
        GameObject obj = GameObject.Find("Main Camera");
        WorldCamera = obj.GetComponent<Camera>();
        Canvas canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = WorldCamera;
        colony = GetComponentInChildren<Colony>();
        canvas.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("updateEverySecond", 1, 1);

        GameObject Obj = GameObject.Find("GameController");
        if(Obj != null)
        {
            GC = Obj.GetComponent<GameController>();
            UI = Obj.GetComponent<UIController>();
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = tileData.summerImg;
        spriteRenderer.sortingOrder = -tileData.gridLocation.Item2 * 2;
        if (tileData.isVisible)
        {
            this.revealTile();
        } else
        {
            this.spriteRenderer.color = new Color32(0, 0, 0, 255);
            river.color = new Color32(0, 0, 0, 255);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GC != null)
        {
            if (GC.isSummerSeason) { this.spriteRenderer.sprite = tileData.summerImg; }
            else { this.spriteRenderer.sprite = tileData.winterImg; }
        }
    }

    void updateEverySecond()
    {
        if(!tileData.isNeighborsRevealed && tileData.exploredPercent >= revealNeighborsValue)
        {
            tileData.isNeighborsRevealed = true;
            revealNeighboringTiles();
        }
    }

    public void setTileData(TileData data)
    {
        this.tileData = data;
        this.AllResources = ResourceAlocator.alocateResource(data.tileType);
        setRiverImg(data);
        colony.setRegionTilemap(data.RegionGameObj);
    }

    public TileData getTileData()
    {
        return this.tileData;
    }

    public List<Resource> getTileResources()
    {
        return this.AllResources;
    }

    public Colony getColony()
    {
        return this.colony;
    }

    public void addPartyToTile(Party party)
    {
        this.tileData.PartiesInTile.Add(party);
    }

    public void removePartyFromTile(Party party)
    {
        this.tileData.PartiesInTile.Remove(party);
    }

    public int getOnTilePartiesCount()
    {
        return this.tileData.PartiesInTile.Count;
    }

    public bool addToExploration(float exploreRate)
    {
        //#TODO likely add further logic here
        bool isComplete = false;
        this.tileData.exploredPercent += exploreRate;
        if(tileData.exploredPercent >= 100)
        {
            tileData.exploredPercent = 100;
            isComplete = true;
        }

        return isComplete;
    }

    //Private Functions Bellow
    private void revealTile()
    {
        this.spriteRenderer.color = new Color32(255, 255, 255, 255);
        river.color = new Color32(255, 255, 255, 255);
        tileData.isVisible = true;
    }

    void revealNeighboringTiles()
    {
        foreach ((int, int) location in tileData.neighborHexes)
        {
            Tile tile = GC.GetTileFromDictionary(location);
            tile.revealTile();
        }
    }
    private void setRiverImg(TileData data)
    {
        if(data.riverNumber != -1)
        {
            river.sprite = TerrainFetcher.setRiverImg((data.river.Item1, data.river.Item2, data.river.Item3));
            // #TODO add logic to make sure river is a lower layer then the tiles bellow it if they are mountains.
            //river.sortingOrder = (-tileData.gridLocation.Item2 * 2) + 1;
        }
    }

    // Possibly move this into its own script
    private void OnMouseDown()
    {
        // Check if Mouse is over an UI Panel, if its not then perform click action
        if(!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            if(tileData.isVisible)
            {
                UI.onHexTileSelected(this, this.colony);
            }
        }
    }

}
