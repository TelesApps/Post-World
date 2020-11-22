using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapCreation;
using static TerrainFetcher;

public class TileData : TilePercentage
{
    // Tile Metadata here
    public string ID { get; set; }
    public (int, int) gridLocation { get; set; }
    public Vector2 worldLocation { get; set; }
    public (int, int)[] neighborHexes = { (-1, -1), (-1, -1), (-1, -1), (-1, -1), (-1, -1), (-1, -1) };
    public int continentNumber = -1;
    public TileType tileType { get; set; }
    public bool isCoast = false;
    public bool isMountain { get; set; }
    public bool isHill { get; set; }
    public bool isColder = false;
    // A tile's river number is -1 if there are no rivers in the tile
    public int riverNumber = -1;
    public (int, int, int) river = (-1, -1, -1);
    public RegionType regionType { get; set; }
    public GameObject RegionGameObj { get; set; }
    public TileTemperatureType temperatureType { get; set; }
    public Forestry forestry { get; set; }
    public TileRainfallType rainfallType { get; set; }

    public bool isIce = false;
    public bool isWarm = false;
    public Sprite summerImg { get; set; }
    public Sprite winterImg { get; set; }

    // Game Specific Data Bellow Here
    public float exploredPercent = 0;
    public bool isVisible = false;
    public bool isNeighborsRevealed = false;
    public List<Vector3> inTilePartyLocations = new List<Vector3>();
    public List<Party> PartiesInTile = new List<Party>();
    public bool isRevealed { get; set; }

    /// <summary>
    /// Constructor for TileData
    /// </summary>
    /// <param name="x">Tile GridLocation X</param>
    /// <param name="y">Tile GridLocation Y</param>
    /// <param name="xt">Transform Location X</param>
    /// <param name="yt">Transform Location Y</param>
    public TileData(int x, int y, float xt, float yt)
    {
        this.tileType = TileType.Ocean;
        this.isRevealed = true;
        this.ID = Guid.NewGuid().ToString();
        this.gridLocation = (x, y);
        this.worldLocation = new Vector2(xt, yt);
        // Set the world locations of units on this tile, stagger them according to how many Parties are on tile
        // #TODO set all of the available locations for a party in tile, have them be centered if only party
        // #TODO and how to manage more them in case of too many parties in one tile.
        this.inTilePartyLocations.Add(new Vector3(worldLocation.x - 0.3f, worldLocation.y + 1.0f, -1));
        this.inTilePartyLocations.Add(new Vector3(worldLocation.x, worldLocation.y + 1.0f, -1));
        this.inTilePartyLocations.Add(new Vector3(worldLocation.x + 0.3f, worldLocation.y + 1.0f, -1));
    }
}
