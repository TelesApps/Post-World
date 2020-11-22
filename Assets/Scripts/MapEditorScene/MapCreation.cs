using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static DiceRoll;
using static TerrainFetcher;

public class MapCreation : MonoBehaviour
{
    // TileSize X and Y is based on the image size of the png file.
    static float tileSizeX = 2.56f;
    static float tileSizeY = 1.92f;
    public enum MapSize { tiny, small, medium, large, huge }
    public enum MapType { pangaea, continents, archipelago }
    public enum SeaLvl { low, medium, high }
    public enum HillsLvl { flat, average, hills }
    public enum MapQuadron { topLeft, topRight, bottomLeft, bottomRight, center }
    public enum Forestry { Low, Medium, High }
    public enum TileTemperatureType { Frozen, Cold, Temperate, Hot, Scorching }
    public enum TileRainfallType { Sand, Arid, Average, Wet, Flooded }

    [SerializeField] GameObject DevRegionArea;

    [SerializeField] GameObject RegionPrefab;
    [SerializeField] Transform RegionAreaSpawnLocation;
    [SerializeField] GameObject TilePrefab;
    [SerializeField] Transform HexTilesGrid;
    [SerializeField] float seaLvlLow;
    [SerializeField] float seaLvlMedium;
    [SerializeField] float seaLvlHigh;
    [SerializeField] int tinySizeX;
    [SerializeField] int tinySizeY;
    [SerializeField] int smallSizeX;
    [SerializeField] int smallSizeY;
    [SerializeField] int mediumSizeX;
    [SerializeField] int mediumSizeY;
    [SerializeField] int largeSizeX;
    [SerializeField] int largeSizeY;
    [SerializeField] int hugeSizeX;
    [SerializeField] int hugeSizeY;
    [SerializeField] float mountainPercent;
    [SerializeField] float hillsFlat;
    [SerializeField] float hillsAverage;
    [SerializeField] float hillsHigh;
    [SerializeField] float RiverAmountArid;
    [SerializeField] float RiverAmountAverage;
    [SerializeField] float RiverAmountWet;
    [SerializeField] int[] lowForestryPercentages = new int[3];
    [SerializeField] int[] mediumForestryPercentages = new int[3];
    [SerializeField] int[] highForestryPercentages = new int[3];

    Dictionary<(int, int), TileData> AllTiles;
    List<TileData> PossibleStarting = new List<TileData>();

    float hillsPercent;
    MapSize mapSize;
    int xLimit = 0;
    int yLimit = 0;
    
    public (Dictionary<(int, int), TileData>, List<TileData>) generateMap(MapSize mapSize, MapType mapType, SeaLvl seaLvl, HillsLvl topography, 
        int temperatureLvl, int rainLvl, Forestry forestry)
    {
        AllTiles = GenerateTileDictionary(mapSize);
        TestAllTiles = AllTiles;
        int landSum = calculateLandSum(xLimit * yLimit, seaLvl);

        if(topography == HillsLvl.flat)
            hillsPercent = hillsFlat;
        else if (topography == HillsLvl.average)
            hillsPercent = hillsAverage;
        else hillsPercent = hillsHigh;

        if (mapType == MapType.pangaea)
            generatePangea(landSum);
        else if (mapType == MapType.continents)
            generateContinentMap(landSum);
        else if (mapType == MapType.archipelago)
            generateArchipelagoMap(landSum);

        generateTopography(mountainPercent);
        setWorldConditions(temperatureLvl, rainLvl, forestry);
        generateRivers(mapSize, rainLvl);
        generateBiomes();
        InstanciateAllRegions(AllTiles);
        InstanciateTiles(AllTiles.Values.ToList());
        

        return (AllTiles, PossibleStarting);

        //testgenerateStartingLocation(AllTiles, landSum);
        //InvokeRepeating("TestplaceTile", 1, 0.1f);
    }
    Dictionary<(int, int), TileData> TestAllTiles;
    public void TestplaceTile()
    {
        testGenerate(TestAllTiles);
    }
    void TestInstanciateTile(TileData Tile)
    {
        // Test Code bellow, move this assignment of img to another function
        Tile.summerImg = TerrainFetcher.getTileImg(Tile.tileType);
        // end of test code
        GameObject tileObj = Instantiate(TilePrefab, HexTilesGrid);
        tileObj.name = Tile.tileType.ToString();
        tileObj.transform.position = Tile.worldLocation;
        tileObj.GetComponent<Tile>().setTileData(Tile);

    }

    private Dictionary<(int, int), TileData> GenerateTileDictionary(MapSize size)
    {
        foreach (Transform child in HexTilesGrid)
        {
            Destroy(child.gameObject);
        }
        Dictionary<(int, int), TileData> AllTiles = new Dictionary<(int, int), TileData>();
        int tileSum = 0;
        setMapSize(size);
        tileSum = xLimit * yLimit;
        // x and y grid int
        int xTile = 0;
        int yTile = 0;
        // x and y transform location
        float xt = 0;
        float yt = 0;
        // First create all tiles with only grid and world location as its data and add to dictionary
        for (int i = 0; i < tileSum; i++)
        {
            if (xTile == xLimit)
            {
                yTile++;
                yt += tileSizeY;
                xTile = 0;
                if (yTile % 2 != 0) { xt = 1.28f; }
                else { xt = 0; }
            }
            TileData tile = new TileData(xTile, yTile, xt, yt);
            // Add neighborTiles if not on an edge
            this.addNeightboringTiles(tile, xTile, yTile, xLimit, yLimit);
            
            AllTiles.Add((xTile, yTile), tile);
            xTile++;
            xt += tileSizeX;
        }

        return AllTiles;
    }

    private void setMapSize(MapSize size)
    {
        this.mapSize = size;
        if (this.mapSize == MapSize.tiny)
        {
            xLimit = tinySizeX;
            yLimit = tinySizeY;
        }
        else if (this.mapSize == MapSize.small)
        {
            xLimit = smallSizeX;
            yLimit = smallSizeY;
        }
        else if (this.mapSize == MapSize.medium)
        {
            xLimit = mediumSizeX;
            yLimit = mediumSizeY;
        }
        else if (this.mapSize == MapSize.large)
        {
            xLimit = largeSizeX;
            yLimit = largeSizeY;
        }
        else if (this.mapSize == MapSize.huge)
        {
            xLimit = hugeSizeX;
            yLimit = hugeSizeY;
        }
    }

    /// <summary>
    /// Calculates how much land there will be on the map based on sea level
    /// </summary>
    /// <param name="tileSum">the size of the map afte multiplying x and y</param>
    /// <param name="lvl">the seaLvl</param>
    /// <returns></returns>
    private int calculateLandSum(int tileSum, SeaLvl lvl)
    {
        if (lvl == SeaLvl.low)
            return tileSum - (Mathf.RoundToInt(tileSum * seaLvlLow));
        if (lvl == SeaLvl.medium)
            return tileSum - (Mathf.RoundToInt(tileSum * seaLvlMedium));
        return tileSum - (Mathf.RoundToInt(tileSum * seaLvlHigh));
    }
    #region Testing Functions Generates tile slowly
    public void testgenerateStartingLocation(Dictionary<(int, int), TileData> allTiles, int landTileSum)
    {
        // Determine how many continents there will be, generally between 2 and 4
        TestcontinentNumber = Random.Range(2, 5);
        Debug.Log("Continents: " + TestcontinentNumber);
        TestlandMasses = new Dictionary<int, (int, int)>();
        for (int i = 0; i < TestcontinentNumber; i++)
        {
            // starting location for each landmass will always follow order topLeft, topRight, bottomLeft, bottomRight, center
            (int, int) start = DiceRoll.getTileLocationWithinQuadron(xLimit, yLimit, i.ToString());
            TestlandMasses.Add(i, start);
            addnewLandTile(testLandTiles, i, start);
            allTiles.TryGetValue(start, out TileData tile);
            TestInstanciateTile(tile);
        }
    }
    //TEST VALUES HERE UNCOMMENT THIS TO USE THE TEST FUNCTIONS ABOVE AND BELLOW HERE
    int TestcontinentNumber;
    Dictionary<int, (int, int)> TestlandMasses = new Dictionary<int, (int, int)>();
    HashSet<(int, int)> testLandTiles = new HashSet<(int, int)>();
    public void testGenerate(Dictionary<(int, int), TileData> allTiles)
    {
        // For each landmass, select the next tile that will be created.
        for (int i = 0; i < TestcontinentNumber; i++)
        {
            // Add logic to determine weather this landmass gets a new tile or not
            TestlandMasses.TryGetValue(i, out (int, int) current);
            allTiles.TryGetValue(current, out TileData currentTile);
            generateAllNeighbors(testLandTiles, currentTile, 3);
            (int, int) nextMove = (-1, -1);
            while (nextMove.Item1 == -1 || nextMove.Item2 == -1)
            {
                int s0 = DiceRoll.avoidLandmass(allTiles, i, current, 0, 4);
                int s1 = DiceRoll.avoidLandmass(allTiles, i, current, 1, 4);
                int s2 = DiceRoll.avoidLandmass(allTiles, i, current, 2, 4);
                int s3 = DiceRoll.avoidLandmass(allTiles, i, current, 3, 4);
                int s4 = DiceRoll.avoidLandmass(allTiles, i, current, 4, 4);
                int s5 = DiceRoll.avoidLandmass(allTiles, i, current, 5, 4);

                nextMove = currentTile.neighborHexes[DiceRoll.rollSixSides(s0, s1, s2, s3, s4, s5)];
            }
            //if (canPlace)
            //{
            //    addnewLandTile(allTiles, testLandTiles, i, nextMove);
            //    TestlandMasses[i] = nextMove;
            //    allTiles.TryGetValue(nextMove, out TileData tile);
            //    TestInstanciateTile(tile);
            //}
            //else
            //{
            //    TestlandMasses[i] = nextMove;
            //}

            addnewLandTile(testLandTiles, i, nextMove);
            TestlandMasses[i] = nextMove;
            allTiles.TryGetValue(nextMove, out TileData tile);
            TestInstanciateTile(tile);
        }
    }

    #endregion

    #region Generate MapType, Pangea, Continents and Achipelago

    /// <summary>
    /// Pangea will have one landmass, it starts on the center and build from there
    /// Or in the center and build from there
    /// </summary>
    /// <param name="landTileSum"></param>
    private void generatePangea(int landTileSum)
    {
        HashSet<(int, int)> landTiles = new HashSet<(int, int)>();
        int startingPoints = 1;
        Dictionary<int, (int, int)> landMasses = new Dictionary<int, (int, int)>();
        for (int i = 0; i < startingPoints; i++)
        {
            (int, int) start = DiceRoll.getTileLocationWithinQuadron(xLimit, yLimit, "5");
            landMasses.Add(i, start);
            addnewLandTile(landTiles, i, start);
        }
        int abort = 0;
        while (landTiles.Count < landTileSum)
        {
            // For each startingPoints, select the next tile that will be created.
            for (int i = 0; i < startingPoints; i++)
            {
                landMasses.TryGetValue(i, out (int, int) current);
                AllTiles.TryGetValue(current, out TileData currentTile);
                generateAllNeighbors(landTiles, currentTile, 6);
                (int, int) nextMove = (-1, -1);
                while (nextMove.Item1 == -1 || nextMove.Item2 == -1)
                {
                    int s0 = DiceRoll.setNeightborPercentage(NeighborPercentType.AvoidEdges, current, 0, xLimit, yLimit);
                    int s1 = DiceRoll.setNeightborPercentage(NeighborPercentType.AvoidEdges, current, 1, xLimit, yLimit);
                    int s2 = DiceRoll.setNeightborPercentage(NeighborPercentType.AvoidEdges, current, 2, xLimit, yLimit);
                    int s3 = DiceRoll.setNeightborPercentage(NeighborPercentType.AvoidEdges, current, 3, xLimit, yLimit);
                    int s4 = DiceRoll.setNeightborPercentage(NeighborPercentType.AvoidEdges, current, 4, xLimit, yLimit);
                    int s5 = DiceRoll.setNeightborPercentage(NeighborPercentType.AvoidEdges, current, 5, xLimit, yLimit);

                    nextMove = currentTile.neighborHexes[DiceRoll.rollSixSides(s0, s1, s2, s3, s4, s5)];
                }
                addnewLandTile(landTiles, 1, nextMove);
                landMasses[i] = nextMove;

            }
            abort++;
            if (abort > 10000)
            {
                Debug.LogError("Broke out of endless loop");
                break;
            }
        }
    }

    private void generateContinentMap(int landTileSum)
    {
        HashSet<(int, int)> landTiles = new HashSet<(int, int)>();
        // Determine how many continents there will be, generally between 2 and 4
        int continentNumber = Random.Range(2, 6);
        if (mapSize == MapSize.tiny || mapSize == MapSize.small)
            continentNumber = Random.Range(2, 4);
        Debug.Log("Continents: " + continentNumber);
        Dictionary<int, (int, int)> landMasses = new Dictionary<int, (int, int)>();
        for (int i = 0; i < continentNumber; i++)
        {
            // starting location for each landmass will always follow order topLeft, topRight, bottomLeft, bottomRight, center
            (int, int) start = DiceRoll.getTileLocationInCorner(xLimit, yLimit, i.ToString());
            landMasses.Add(i, start);
            addnewLandTile(landTiles, i, start);
        }
        int abort = 0;
        while (landTiles.Count < landTileSum)
        {
            // For each landmass, select the next tile that will be created.
            for (int i = 0; i < continentNumber; i++)
            {
                // Break out of this loop for this continent if chance to develop is not met
                int chanceToDevelop = Random.Range(0, 4);
                if (i > 1 && chanceToDevelop != 0)
                    break;
                landMasses.TryGetValue(i, out (int, int) current);
                AllTiles.TryGetValue(current, out TileData currentTile);
                //generateAllNeighbors(landTiles, currentTile, 3);
                (int, int) nextMove = (-1, -1);
                while (nextMove.Item1 == -1 || nextMove.Item2 == -1)
                {
                    int s0 = DiceRoll.avoidLandmass(AllTiles, i, current, 0, 3);
                    int s1 = DiceRoll.avoidLandmass(AllTiles, i, current, 1, 3);
                    int s2 = DiceRoll.avoidLandmass(AllTiles, i, current, 2, 3);
                    int s3 = DiceRoll.avoidLandmass(AllTiles, i, current, 3, 3);
                    int s4 = DiceRoll.avoidLandmass(AllTiles, i, current, 4, 3);
                    int s5 = DiceRoll.avoidLandmass(AllTiles, i, current, 5, 3);

                    nextMove = currentTile.neighborHexes[DiceRoll.rollSixSides(s0, s1, s2, s3, s4, s5)];
                }
                addnewLandTile(landTiles, i, nextMove);
                landMasses[i] = nextMove;
            }
            abort++;
            if (abort > 2000)
            {
                Debug.LogError("Broke out of endless loop");
                break;
            }
        }

    }

    private void generateArchipelagoMap(int landTileSum)
    {
        int continentNumber = 0;
        if (mapSize == MapSize.huge || mapSize == MapSize.large)
            continentNumber = Random.Range(10, 16);
        if (mapSize == MapSize.medium || mapSize == MapSize.small)
            continentNumber = Random.Range(5, 10);
        if (mapSize == MapSize.medium || mapSize == MapSize.small || mapSize == MapSize.tiny)
            continentNumber = Random.Range(4, 7);
        HashSet<(int, int)> landTiles = new HashSet<(int, int)>();
        // Determine how many continents there will be, generally between 2 and 4
        Debug.Log("Continents: " + continentNumber);
        Dictionary<int, (int, int)> landMasses = new Dictionary<int, (int, int)>();
        int quadron = 0;
        for (int i = 0; i < continentNumber; i++)
        {
            // starting location for each landmass will always follow order topLeft, topRight, bottomLeft, bottomRight, center
            (int, int) start = DiceRoll.getRandomLocationInEntireMap(xLimit, yLimit);
            landMasses.Add(i, start);
            addnewLandTile(landTiles, i, start);
            AllTiles.TryGetValue(start, out TileData startTile);
            generateAllNeighbors(landTiles, startTile, 3);
            quadron++;
            if (quadron > 5) quadron = 0;
        }
        int abort = 0;
        while (landTiles.Count < landTileSum)
        {
            // For each landmass, select the next tile that will be created.
            for (int i = 0; i < continentNumber; i++)
            {
                // Break out of this loop for this continent if chance to develop is not met
                int chanceToDevelop = Random.Range(0, 4);
                if (chanceToDevelop == 0)
                    break;
                landMasses.TryGetValue(i, out (int, int) current);
                AllTiles.TryGetValue(current, out TileData currentTile);
                (int, int) nextMove = (-1, -1);
                int abort2 = 0;
                //int distance = 2;
                //if (mapSize == MapSize.large || mapsize == MapSize.huge)
                //    distance = 3;
                while (nextMove.Item1 == -1 || nextMove.Item2 == -1)
                {
                    int s0 = DiceRoll.avoidLandmass(AllTiles, i, current, 0, 3);
                    int s1 = DiceRoll.avoidLandmass(AllTiles, i, current, 1, 3);
                    int s2 = DiceRoll.avoidLandmass(AllTiles, i, current, 2, 3);
                    int s3 = DiceRoll.avoidLandmass(AllTiles, i, current, 3, 3);
                    int s4 = DiceRoll.avoidLandmass(AllTiles, i, current, 4, 3);
                    int s5 = DiceRoll.avoidLandmass(AllTiles, i, current, 5, 3);

                    nextMove = currentTile.neighborHexes[DiceRoll.rollSixSides(s0, s1, s2, s3, s4, s5)];
                    abort2++;
                    if (abort2 == 1000)
                    {
                        Debug.LogError("Broke out of endless loop");
                        break;
                    }
                }
                addnewLandTile(landTiles, i, nextMove);
                landMasses[i] = nextMove;

            }
            abort++;
            if (abort > 2000)
            {
                Debug.LogError("Broke out of endless loop");
                break;
            }
        }
    }

    #endregion

    #region helper methods for generating Map Types

    /// <summary>
    /// Adds all of the neighboring tiles of a specific tile
    /// </summary>
    /// <param name="tile">The tile to add the neighbors to</param>
    /// <param name="xTile">the x number of the tile</param>
    /// <param name="yTile">the y number of the tile</param>
    /// <param name="xLimit">the x size limit of the map</param>
    /// <param name="yLimit">the y size limit of the map</param>
    private void addNeightboringTiles(TileData tile, int xTile, int yTile, int xLimit, int yLimit)
    {
        // #TODO Not sure why this function is so big, need to review this and likely change it
        // neightbor[0]
        if (yTile != yLimit - 1)
        {
            if (xTile == xLimit - 1 && yTile % 2 != 0) { }
            else
            {
                if (yTile % 2 == 0)
                    tile.neighborHexes[0] = (xTile, yTile + 1);
                else tile.neighborHexes[0] = (xTile + 1, yTile + 1);
            }
        }
        // neightbor[1]
        if (xTile != xLimit - 1)
        {
            tile.neighborHexes[1] = (xTile + 1, yTile);
        }
        // neightbor[2]
        if (yTile != 0)
        {
            if (xTile == xLimit - 1 && yTile % 2 != 0) { }
            else
            {
                if (yTile % 2 == 0)
                    tile.neighborHexes[2] = (xTile, yTile - 1);
                else tile.neighborHexes[2] = (xTile + 1, yTile - 1);
            }
        }
        // neightbor[3]
        if (yTile != 0)
        {
            if (xTile == 0 && yTile % 2 == 0) { }
            else
            {
                if (yTile % 2 == 0)
                    tile.neighborHexes[3] = (xTile - 1, yTile - 1);
                else tile.neighborHexes[3] = (xTile, yTile - 1);
            }
        }
        // neightbor[4]
        if (xTile != 0)
        {
            tile.neighborHexes[4] = (xTile - 1, yTile);
        }
        // neightbor[5]
        if (yTile != yLimit - 1)
        {
            if (xTile == 0 && yTile % 2 == 0) { }
            else
            {
                if (yTile % 2 == 0)
                    tile.neighborHexes[5] = (xTile - 1, yTile + 1);
                else tile.neighborHexes[5] = (xTile, yTile + 1);
            }
        }
    }

    /// <summary>
    /// Notifies al the neighbors of a specific tile what type of tile they are next to
    /// </summary>
    /// <param name="neighbors">the Array of neighbors for that are being notified</param>
    /// <param name="type">The type of tile they are being notified about</param>
    private void notifyNeighbors((int, int)[] neighbors, TileType type)
    {
        foreach ((int, int) nei in neighbors)
        {
            if (nei.Item1 == -1) break;
            AllTiles.TryGetValue(nei, out TileData neighborTile);
            if(type == TileType.Ocean || type == TileType.OceanIceberg)
            {
                neighborTile.isCoast = true;
            }
        }
    }

    /// <summary>
    /// Chance to generate all neighbors of a specific tile, set it to 1 for 100% certanty
    /// </summary>
    /// <param name="landTiles">the Hashset of land tiles to be added to</param>
    /// <param name="tile">The current tile that all neighbors are being generated from</param>
    /// <param name="upperInt">The upper int of Random.Range(0, int) chances are 1 in whatever the odds</param>
    private void generateAllNeighbors(HashSet<(int, int)> landTiles, TileData tile, int upperInt)
    {
        if (Random.Range(0, upperInt) == 0)
        {
            foreach ((int, int) location in tile.neighborHexes)
            {
                if (location == (-1, -1)) break;
                AllTiles.TryGetValue(location, out TileData neighborTile);
                if(!isNeighborsAnotherContinent(neighborTile))
                {
                    neighborTile.tileType = TileType.Plains;
                    neighborTile.continentNumber = tile.continentNumber;
                    notifyNeighbors(neighborTile.neighborHexes, TileType.Plains);
                    landTiles.Add(neighborTile.gridLocation);
                }
            }
        }
    }

    private bool isNeighborsAnotherContinent(TileData tile)
    {
        foreach ((int, int) nei in tile.neighborHexes)
        {
            AllTiles.TryGetValue(nei, out TileData neighbor);
            if (neighbor !=  null && neighbor.continentNumber != -1 && neighbor.continentNumber != tile.continentNumber)
                return false;
            else return true;
        }
        return false;
    }

    #endregion

    /// <summary>
    /// Adds a new land tile to the Hashset landtiles
    /// </summary>
    /// <param name="landTiles">the hashset of all land tiles</param>
    /// <param name="continentNumber">the continent this tile belongs to</param>
    /// <param name="hexLocation">the hex location of the new tile being added</param>
    private void addnewLandTile(HashSet<(int, int)> landTiles, int continentNumber, (int, int) hexLocation)
    {
        if(continentNumber == -1)
        {
            Debug.LogError("Error on continent number");
        }
        AllTiles.TryGetValue(hexLocation, out TileData tile);
        if (tile != null)
        {
            tile.tileType = TileType.Plains;
            tile.continentNumber = continentNumber;
            notifyNeighbors(tile.neighborHexes, TileType.Plains);
            landTiles.Add(tile.gridLocation);
        }

    }

    private void generateTopography(float percentOfMountains)
    {
        List<TileData> allTileData = new List<TileData>(AllTiles.Values.ToList());
        List<TileData> AllLandTiles = allTileData.FindAll(t => t.tileType != TileType.Ocean);
        int mountainCount = Mathf.RoundToInt(AllLandTiles.Count * percentOfMountains);
        HashSet<(int, int)> allMountains = new HashSet<(int, int)>();
        int abort = 0;
        while (allMountains.Count < mountainCount)
        {
            int mountainRange = Random.Range(1, 11);
            int rangeCount = 0;
            (int, int) current = AllLandTiles[Random.Range(0, AllLandTiles.Count)].gridLocation;
            int cancelCount = 0;
            while (rangeCount < mountainRange)
            {
                TileData currentTile = setTileType(current, TileType.Mountain);
                currentTile.isMountain = true;
                allMountains.Add(current);
                (int, int) nextTile = currentTile.neighborHexes[Random.Range(0, 6)];
                AllTiles.TryGetValue(nextTile, out TileData nextTileData);
                if (nextTileData == null  || nextTileData.tileType == TileType.Ocean || nextTileData.tileType == TileType.OceanIceberg)
                { rangeCount--; }
                else current = nextTile;
                rangeCount++;
                cancelCount++;
                if (cancelCount > 100) break;
            }
            abort++;
            if (abort == 10000) break;
        }
        generateHills(allMountains, AllLandTiles);
    }

    private void generateHills(HashSet<(int, int)> allMountains, List<TileData> AllLandTiles)
    {
        int hillsCount = Mathf.RoundToInt(AllLandTiles.Count * (hillsPercent));
        HashSet<(int, int)> allHills = new HashSet<(int, int)>();
        int abort = 0;
        while (allHills.Count < hillsCount)
        {
            int hillsRange = Random.Range(1, 4);
            int rangeCount = 0;
            (int, int) current = (-1, -1);
            // 1 in 4 chances it will spawn somewhere not neat a mountain
            if (Random.Range(0,4) == 0)
            {
                current = AllLandTiles[Random.Range(0, AllLandTiles.Count)].gridLocation;
            } else
            {
                (int, int) mountain = allMountains.ToArray()[Random.Range(0, allMountains.Count)];
                AllTiles.TryGetValue(mountain, out TileData tile);
                current = tile.neighborHexes[(Random.Range(0, 6))];
                if (current.Item1 == -1) break;
            }
            int cancelCount = 0;
            while (rangeCount < hillsRange)
            {
                TileData currentTile = setTileType(current, TileType.Hills);
                currentTile.isHill = true;
                allHills.Add(current);
                (int, int) nextTile = currentTile.neighborHexes[Random.Range(0, 6)];
                if (nextTile.Item1 == -1) break;
                AllTiles.TryGetValue(nextTile, out TileData nextTileData);
                if (nextTileData == null || nextTileData.tileType == TileType.Ocean || nextTileData.tileType == TileType.OceanIceberg)
                { rangeCount--; }
                else current = nextTile;
                rangeCount++;
                cancelCount++;
                if (cancelCount > 1000) break;
            }
            abort++;
            if (abort == 10000) break;
        }
    }

    /// <summary>
    /// Sets the tileData for a specific tile from an (int, int) location and returns the tileData
    /// </summary>
    /// <param name="tileLocation">the x and y grid location of the tile</param>
    /// <param name="type">The type being assigned to this tile</param>
    /// <returns>The TileData from the tile</returns>
    private TileData setTileType((int, int) tileLocation, TileType type)
    {
        AllTiles.TryGetValue(tileLocation, out TileData tileData);
        if(tileData != null)
        {
            tileData.tileType = type;
        }
        return tileData;
    }

    private void setWorldConditions(int temperature, int rainSelection, Forestry forestry)
    {
        // Forestry
        (int, int, int) lowMedHigh = (mediumForestryPercentages[0], mediumForestryPercentages[1], mediumForestryPercentages[2]);
        if (forestry == Forestry.Low) lowMedHigh = (lowForestryPercentages[0], lowForestryPercentages[1], lowForestryPercentages[2]);
        if (forestry == Forestry.High) lowMedHigh = (highForestryPercentages[0], highForestryPercentages[1], highForestryPercentages[2]);
        // Temperature result 0 for Cold, 1 for Temperate and 2 for Hot
        int temp = -1 + temperature;
        // Rain Result is 0 for Arid, 1 for Average and 2 for Wet
        int rain = -1 + rainSelection;

        List<TileData> allTileData = new List<TileData>(AllTiles.Values.ToList());
        foreach (TileData tile in allTileData)
        {
            // Set Forestry based on user input.
            int tileForestry = DiceRoll.rollThreeSides(lowMedHigh.Item1, lowMedHigh.Item2, lowMedHigh.Item3);
            tile.forestry = (Forestry)tileForestry;
            // Set Default Temperature
            int tileY = tile.gridLocation.Item2;
            if (tileY == 0 || tileY == yLimit -1)
            {
                tile.temperatureType = TileTemperatureType.Frozen;
                if(temp > 0 && Random.Range(0,2) == 0)
                {
                    tile.temperatureType = TileTemperatureType.Cold;
                    tile.isColder = true;
                }
            }
            else if (tileY < yLimit * 0.1f || tileY > yLimit * 0.9f)
            {
                tile.temperatureType = TileTemperatureType.Cold;
                if (Random.Range(0, 2) == 0) tile.isColder = true;
                if (temp < 0) tile.temperatureType = TileTemperatureType.Frozen;
                if (temp > 0) tile.isColder = false;
            }
            else if (tileY < yLimit * 0.2f || tileY > yLimit * 0.8f)
            {
                tile.temperatureType = TileTemperatureType.Cold;
                if(temp != 0 && Random.Range(0, 2) == 0)
                    tile.temperatureType = TileTemperatureType.Cold + temp;
            }
            else if (tileY < yLimit * 0.3f || tileY > yLimit * 0.7f)
            {
                // 50% chance to return Cold or Temperate.
                tile.temperatureType = (TileTemperatureType)Random.Range(1, 3);
                tile.temperatureType += temp;
                if (tile.temperatureType == 0) tile.temperatureType = TileTemperatureType.Cold;
            }
            else if(tileY < yLimit * 0.4f || tileY > yLimit * 0.6f)
            {
                tile.temperatureType = TileTemperatureType.Temperate;
                if(Random.Range(0, 2) == 0)
                    tile.temperatureType += temp;
            }
            else if (tileY < yLimit * 0.45f || tileY > yLimit * 0.55f)
            {
                // 50% chance to return Temperate or Hot.
                tile.temperatureType = (TileTemperatureType)Random.Range(2, 4);
                tile.temperatureType += temp;
            }
            else
            {
                // 50% chance to return Hot or Scortching.
                int hotLimit = Random.Range(3, 5) + temp;
                if (hotLimit > 4) hotLimit = 4;
                tile.temperatureType = (TileTemperatureType)hotLimit;
            }

            // Set Rain Level
            if (tileY < yLimit * 0.2f || tileY > yLimit * 0.8f)
            {
                // equal chance of Arid, Temperate and Wet
                int rainLvl = Random.Range(1, 4) + rain;
                tile.rainfallType = (TileRainfallType)rainLvl;
            }
            else if (tileY < yLimit * 0.3f || tileY > yLimit * 0.7f)
            {
                // equal chance of Sand, Arid and Temperate
                int rainLvl = Random.Range(0, 3) + rain;
                if (rainLvl < 0) rainLvl = 0;
                tile.rainfallType = (TileRainfallType)rainLvl;
            }
            else
            {
                // equal chance of Temperate, Wet and Flooded
                int rainLvl = Random.Range(2, 5) + rain;
                if (rainLvl > 4) rainLvl = 4;
                tile.rainfallType = (TileRainfallType)rainLvl;
            }
        }
    }

    private void generateBiomes()
    {
        List<TileData> allTileData = new List<TileData>(AllTiles.Values.ToList());

        foreach (TileData tile in allTileData)
        {
            tile.tileType = TerrainFetcher.setTileType(tile);
        }
        // Pass Through all the tiles and makes it into one of its neighbor, used to make map more consistent

        this.Passthrough();

    }

    private void generateRivers(MapSize mapSize, int rainLvl)
    {
        // #TODO Brain not working when wrote this, write better code for Numbers of rivers based on mapsize and Rain amount
        int numberOfRivers = 4;
        if (mapSize == MapSize.small) numberOfRivers = 6;
        if (mapSize == MapSize.medium) numberOfRivers = 9;
        if (mapSize == MapSize.large) numberOfRivers = 12;
        if (mapSize == MapSize.huge) numberOfRivers = 16;
        if (rainLvl == 0) numberOfRivers = Mathf.RoundToInt(numberOfRivers * RiverAmountArid);
        if (rainLvl == 1) numberOfRivers = Mathf.RoundToInt(numberOfRivers * RiverAmountAverage);
        if (rainLvl == 2) numberOfRivers = Mathf.RoundToInt(numberOfRivers * RiverAmountWet);
        Debug.Log("number of rivers = " + numberOfRivers);
        List<TileData> allTileData = new List<TileData>(AllTiles.Values.ToList());
        List<TileData> allHighGround = new List<TileData>(allTileData.FindAll(t => t.isMountain || t.isHill));
        List<River> allRivers = new List<River>();
        // Create starting locations for all rivers
        for (int i = 0; i < numberOfRivers; i++)
        {
            (int, int) starting = allHighGround[Random.Range(0, allHighGround.Count)].neighborHexes[Random.Range(0, 5)];
            AllTiles.TryGetValue(starting, out TileData sTile);
            if(sTile == null || sTile.isMountain || sTile.tileType == TileType.Ocean || 
                sTile.tileType == TileType.OceanIceberg || sTile.riverNumber != -1)
            {
                i--;
                continue;
            } else
            {
                River river = new River(sTile);
                if (!sTile.isHill)
                    river.canPassHills = false;
                river.riverNumber = i;
                allRivers.Add(river);
                sTile.rainfallType = TileRainfallType.Wet;
                sTile.riverNumber = i;
            }
        }

        int abort = 0;
        while (allRivers.Any(r => r.isFinished == false))
        {
            foreach (River river in allRivers)
            {
                AllTiles.TryGetValue(river.currentLocation, out TileData currentTile);

                if (river.lastDirection == -1)
                {
                    // Start the flow of a new river.
                    int flowDirection = Random.Range(0, 6);
                    int opositeDirection = 3;
                    if (flowDirection >= 3) opositeDirection = -3;
                    AllTiles.TryGetValue(currentTile.neighborHexes[flowDirection], out TileData nextTile);
                    if (nextTile != null && !nextTile.isMountain && nextTile.riverNumber == -1 && 
                        nextTile.tileType != TileType.Ocean && nextTile.tileType != TileType.OceanIceberg)
                    {
                        if (nextTile.isHill && !river.canPassHills)
                            continue;
                        if (!nextTile.isHill)
                            river.canPassHills = false;
                        // Since river is passing through make that tile Wet
                        nextTile.rainfallType = TileRainfallType.Wet;
                        if (nextTile.rainfallType == TileRainfallType.Wet) nextTile.rainfallType = TileRainfallType.Flooded;

                        river.lastDirection = flowDirection + opositeDirection;
                        currentTile.river.Item2 = flowDirection;
                        nextTile.riverNumber = river.riverNumber;
                        nextTile.river.Item1 = flowDirection + opositeDirection;
                        river.currentLocation = nextTile.gridLocation;
                    }
                    else continue;
                } else
                {
                    // End river if this is an ocean tile
                    if(currentTile.tileType == TileType.Ocean || currentTile.tileType == TileType.OceanIceberg)
                    {
                        currentTile.riverNumber = -1;
                        river.isFinished = true;
                    }
                    else if(!river.isFinished)
                    {
                        int nextDirection = -1;
                        // if is a hill then river can go in 5 directions, also 1 in 5 chances to go in 5 directions
                        if(river.canPassHills || Random.Range(0, 5) == 0)
                            nextDirection = DiceRoll.getRiverDirection(river.lastDirection, true);
                        else nextDirection = DiceRoll.getRiverDirection(river.lastDirection);
                        int opositeDirection = 3;
                        if (nextDirection >= 3) opositeDirection = -3;
                        AllTiles.TryGetValue(currentTile.neighborHexes[nextDirection], out TileData nextTile);
                        if (nextTile != null && !nextTile.isMountain && nextTile.riverNumber != river.riverNumber)
                        {
                            if (nextTile.isHill && !river.canPassHills)
                                continue;
                            if (!nextTile.isHill)
                                river.canPassHills = false;
                            // Add this tile into list of possible starting locations
                            if(nextTile.tileType != TileType.Ocean || nextTile.tileType != TileType.OceanIceberg )
                                this.PossibleStarting.Add(nextTile);
                            // Since river is passing through make that tile Wet
                            nextTile.rainfallType = TileRainfallType.Wet;

                            // #TODO Add logic for rivers splitting, currently it only merges into other rivers
                            river.lastDirection = nextDirection + opositeDirection;
                            currentTile.river.Item2 = nextDirection;
                            // If next tile is river, merge the rivers, if Its a starting pointmerge into river.item1 not item3
                            if (nextTile.riverNumber != -1)
                            {
                                if(nextTile.river.Item1 == -1)
                                    nextTile.river.Item1 = nextDirection + opositeDirection;
                                else
                                {
                                    nextTile.river.Item3 = nextDirection + opositeDirection;
                                    river.isFinished = true;
                                }
                            } else
                            {
                                nextTile.riverNumber = river.riverNumber;
                                nextTile.river.Item1 = nextDirection + opositeDirection;
                                river.currentLocation = nextTile.gridLocation;
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                abort++;
                if (abort > 20000)
                {
                    Debug.LogWarning("Aborted from while loop, tiles converted to Ocean tiles");
                    List<River> unfinishedRivers = allRivers.FindAll(r => !r.isFinished);
                    foreach (River ur in unfinishedRivers)
                    {
                        AllTiles.TryGetValue(ur.currentLocation, out TileData tile);
                        TileData toRemove = PossibleStarting.Find(t => t.ID == tile.ID);
                        if (toRemove != null)
                            PossibleStarting.Remove(toRemove);
                        tile.tileType = TileType.Ocean;
                    }
                    break;
                }
            }
            if (abort > 20000)
            {
                break;
            }
        }


    }


    /// <summary>
    /// // Pass Through all the tiles and makes it into one of its neighbor, used to make map more consistent triggered via button
    /// </summary>
    public void Passthrough()
    {
        foreach (Transform child in HexTilesGrid)
        {
            Destroy(child.gameObject);
        }
        List<TileData> allTileData = new List<TileData>(AllTiles.Values.ToList());
        List<TileData> allLandTiles = allTileData.FindAll(t => t.tileType != TileType.Ocean && t.tileType != TileType.OceanIceberg);
        foreach (TileData tile in allLandTiles)
        {
            if (tile.isMountain || tile.isHill || tile.tileType == TileType.Ocean || tile.tileType == TileType.OceanIceberg)
                continue;
            int diceRoll = Random.Range(0, 7);
            // If diceroll is 6, tile stays whatever it is.
            if (diceRoll == 6) continue;
            else
            {
                AllTiles.TryGetValue(tile.neighborHexes[diceRoll], out TileData neighbor);
                if (neighbor == null) continue;
                // If neightbor is a specific type of tile, avoid becoming it, including if its a Sand Or Arid and tile is a river
                if (neighbor.isMountain || neighbor.isHill || neighbor.tileType == TileType.Ocean || neighbor.tileType == TileType.OceanIceberg
                    || (neighbor.rainfallType == TileRainfallType.Sand && tile.riverNumber != -1)
                    || (neighbor.rainfallType == TileRainfallType.Arid && tile.riverNumber != -1))
                    continue;
                else tile.tileType = TerrainFetcher.avoidPondsAndOasis(neighbor.tileType);
            }
        }
        //AllTiles
    }

    public void mapEditorPassThrough()
    {
        this.Passthrough();
        this.InstanciateTiles(AllTiles.Values.ToList());
    }

    void InstanciateTiles(List<TileData> allTiles)
    {
        // Fix bug so this wont be nessesary, but for now iterate through all possible starting location and remove any that are ocean
        TileData[] badLocations = PossibleStarting.FindAll(t => t.tileType == TileType.Ocean || t.tileType == TileType.OceanIceberg).ToArray();
        foreach (TileData badTile in badLocations)
        {
            PossibleStarting.Remove(badTile);
        }
        // Instanciate all the tiles in the worldscene based on its data
        foreach (TileData data in allTiles)
        {
            // Test Code bellow, move this assignment of img to another function
            data.summerImg = TerrainFetcher.getTileImg(data.tileType);
            // end of test code
            GameObject tileObj = Instantiate(TilePrefab, HexTilesGrid);
            tileObj.name = data.tileType.ToString();
            tileObj.transform.position = data.worldLocation;
            tileObj.GetComponent<Tile>().setTileData(data);
        }
    }

    /// <summary>
    /// // Instanciate all the regions related to this tile
    /// </summary>
    /// <param name="AllTiles"></param>
    public void InstanciateAllRegions(Dictionary<(int, int), TileData> AllTiles)
    {
        List<TileData> allData = new List<TileData>(AllTiles.Values.ToList());
        foreach (TileData data in allData)
        {
            if(Debug.isDebugBuild)
            {
                Debug.LogWarning("In Dev Mode");
                data.RegionGameObj = DevRegionArea;
            } else
            {
                data.regionType = TerrainFetcher.setRegionType(data);
                // #TODO eventually put this logic elsewhere, Instantiate the specific region prefab according to regionType
                GameObject regionObj = Instantiate(RegionPrefab, RegionAreaSpawnLocation);
                data.RegionGameObj = regionObj;
                regionObj.SetActive(false);
            }
        }
    }

}
