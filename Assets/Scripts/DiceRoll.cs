using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapCreation;
using static TerrainFetcher;

public class DiceRoll
{
    public enum NeighborPercentType { HighRationToCenter, AvoidEdges, KeepToQuadron, KeepToHalf }
    public static BiomeType getBiomeType(TileData tile)
    {
        float sum = tile.coldBiome + tile.plainBiome + tile.desertBiome + tile.tropicalBiome;
        float diceRoll = Random.Range(0f, sum);
        
        if (diceRoll <= tile.plainBiome)
            return BiomeType.Plain;
        if (diceRoll > tile.plainBiome && diceRoll <= tile.plainBiome + tile.coldBiome)
            return BiomeType.Cold;
        if (diceRoll > tile.plainBiome + tile.coldBiome && diceRoll <= tile.plainBiome + tile.coldBiome + tile.desertBiome)
            return BiomeType.Desert;
        if (diceRoll > tile.plainBiome + tile.coldBiome + tile.desertBiome)
            return BiomeType.Tropical;
        return BiomeType.Plain;
    }

    public static Topography setTopography(TileData tile)
    {
        float sum = tile.flat + tile.hill + tile.mountain;
        float diceRoll = Random.Range(0f, sum);

        if (diceRoll <= tile.flat)
            return Topography.Flat;
        if (diceRoll > tile.flat && diceRoll <= tile.flat + tile.hill)
            return Topography.Hills;
        if (diceRoll > tile.flat + tile.hill)
            return Topography.Mountains;

        return Topography.Flat;
    }

    public static Weather setWeather(TileData tile)
    {
        float sum = tile.dry + tile.temperate + tile.wet;
        float diceRoll = Random.Range(0, sum);

        if (diceRoll <= tile.dry)
            return Weather.Dry;
        if (diceRoll > tile.dry && diceRoll <= tile.dry + tile.temperate)
            return Weather.Temperate;
        if (diceRoll > tile.dry + tile.temperate)
            return Weather.Wet;

        return Weather.Temperate;
    }

    public static Forestry setForestry(TileData tile)
    {
        float sum = tile.lowPlants + tile.medPlants + tile.highPlants;
        float diceRoll = Random.Range(0, sum);

        if (diceRoll <= tile.lowPlants)
            return Forestry.Low;
        if (diceRoll > tile.lowPlants && diceRoll <= tile.lowPlants + tile.medPlants)
            return Forestry.Medium;
        if (diceRoll > tile.lowPlants + tile.medPlants)
            return Forestry.High;

        return Forestry.Medium;
    }

    public static int rollSixSides()
    {
        return Random.Range(0, 6);
    }

    public static int rollSixSides(int s0, int s1, int s2, int s3, int s4, int s5)
    {
        float sum = s0 + s1 + s2 + s3 + s4 + s5;
        float diceRoll = Random.Range(0, sum);

        if (diceRoll <= s0)
            return 0;
        if (diceRoll > s0 && diceRoll <= s0 + s1)
            return 1;
        if (diceRoll > s0 + s1 && diceRoll <= s0 + s1 + s2)
            return 2;
        if (diceRoll > s0 + s1 + s2 && diceRoll <= s0 + s1 + s2 + s3)
            return 3;
        if (diceRoll > s0 + s1 + s2 + s3 && diceRoll <= s0 + s1 + s2 + s3 + s4)
            return 4;
        if (diceRoll > s0 + s1 + s2 + s3 + s4)
            return 5;

        Debug.LogError("Calculation error returned -1");
        return -1;
    }

    /// <summary>
    /// Rolls the chance of 3 diferent results and returns one of the results
    /// </summary>
    /// <param name="s0">% chance of result being 0</param>
    /// <param name="s1">% chance of result being 1</param>
    /// <param name="s2">% chance of result being 2</param>
    /// <returns>The result after dice roll</returns>
    public static int rollThreeSides(int s0, int s1, int s2)
    {
        float sum = s0 + s1 + s2;
        float diceRoll = Random.Range(0, sum);

        if (diceRoll <= s0)
            return 0;
        if (diceRoll > s0 && diceRoll <= s0 + s1)
            return 1;
        if (diceRoll > s0 + s1 && diceRoll <= s0 + s1 + s2)
            return 2;
        Debug.LogError("Calculation error returned -1");
        return -1;
    }

    /// <summary>
    /// Calculates the percent chance of each side of the hex being rolled and returns it
    /// </summary>
    /// <param name="calculateType">The type of calculation to make</param>
    /// <param name="xy">the current hex its calculating, xy being its grid location</param>
    /// <param name="side">the side of the hex to calculate from 0 to 5</param>
    /// <param name="xLimit"></param>
    /// <param name="yLimit"></param>
    /// <returns>The calculated percentage for that side of the hex</returns>
    public static int setNeightborPercentage(NeighborPercentType calculateType, (int, int) xy, int side, int xLimit, int yLimit)
    {
        if (calculateType == NeighborPercentType.HighRationToCenter)
            return highRatioToCenter(xy, side, xLimit, yLimit);
        if(calculateType == NeighborPercentType.AvoidEdges)
            return setSidePercentAvoidEdgesOnly(xy, side, xLimit, yLimit);
        return -1;
    }

    /// <summary>
    /// Sets the chances of the dice roll for each side of a Hex based on that tile's location on the map to favor a centered map
    /// </summary>
    /// <param name="side">the side of the hex that this is rolling for</param>
    /// <param name="xy">the tile grid number</param>
    /// <param name="xLimit">the map size x limit</param>
    /// <param name="yLimit">the map size y limit</param>
    /// <returns>the value chance of that side being rolled</returns>
    public static int highRatioToCenter((int, int) xy, int side, int xLimit, int yLimit)
    {
        if (side == 0 || side == 5)
            return yLimit - xy.Item2;
        if (side == 1)
            return xLimit - xy.Item1;
        if (side == 2 || side == 3)
            return 0 + xy.Item2;
        if (side == 4)
            return 0 + xy.Item1;

        return -1;
    }

    public static int setSidePercentAvoidEdgesOnly((int, int) xy, int side, int xLimit, int yLimit)
    {
        if (side == 0)
        {
            if (xy.Item2 >= yLimit - 2 || xy.Item1 >= xLimit - 2)
                return 0;
            else return 15;
        }
        if (side == 1)
        {
            if (xy.Item1 >= xLimit - 2)
                return 0;
            else return 20;
        }
        if (side == 2)
        {
            if (xy.Item2 <= 2 || xy.Item1 >= xLimit - 2)
                return 0;
            else return 15;
        }
        if (side == 3)
        {
            if (xy.Item2 <= 1 || xy.Item1 <= 1)
                return 0;
            else return 15;
        }
        if (side == 4)
        {
            if (xy.Item1 <= 1)
                return 0;
            else return 20;
        }
        if (side == 5)
        {
            if (xy.Item2 >= yLimit - 2 || xy.Item1 <= 2)
                return 0;
            else return 15;
        }
        return -1;
    }

    /// <summary>
    /// Returns the chance of a specific side being generated, it avoids it if it connects to another continent
    /// </summary>
    /// <param name="allTiles">a reference to all tiles on the map</param>
    /// <param name="continentNumber">the continent number of the tile being considered</param>
    /// <param name="xy">the xy location of the current tile</param>
    /// <param name="side">the side being considered</param>
    /// <param name="distance">the distance from this tile to look for land to avoid</param>
    /// <param name="chance">Optional value, the default chance it returns with if no land is found</param>
    /// <returns></returns>
    public static int avoidLandmass
        (Dictionary<(int, int), TileData> allTiles, int continentNumber, (int, int) xy, int side, int distance, int chance = 15)
    {
        for (int i = 1; i <= distance; i++)
        {
            if (xy.Item1 == -1) return 0;
            (int, int) target = getTileLocationFromSide(xy, side, i);
            allTiles.TryGetValue(target, out TileData targetTile);
            if (targetTile == null)
                return 0;
            if (targetTile.continentNumber != continentNumber && targetTile.continentNumber != -1)
                return 0;
            //Check respective neighbors of target tile
            if (side == 0)
            {
                allTiles.TryGetValue(targetTile.neighborHexes[5], out TileData neighbor1);
                if (neighbor1 != null && neighbor1.continentNumber != continentNumber && neighbor1.continentNumber != -1)
                    return 0;
                allTiles.TryGetValue(targetTile.neighborHexes[1], out TileData neighbor2);
                if (neighbor2 != null && neighbor2.continentNumber != continentNumber && neighbor2.continentNumber != -1)
                    return 0;
            }
            if(side == 1)
            {
                allTiles.TryGetValue(targetTile.neighborHexes[0], out TileData neighbor1);
                if (neighbor1 != null && neighbor1.continentNumber != continentNumber && neighbor1.continentNumber != -1)
                    return 0;
                allTiles.TryGetValue(targetTile.neighborHexes[2], out TileData neighbor2);
                if (neighbor2 != null && neighbor2.continentNumber != continentNumber && neighbor2.continentNumber != -1)
                    return 0;
            }
            if (side == 2)
            {
                allTiles.TryGetValue(targetTile.neighborHexes[1], out TileData neighbor1);
                if (neighbor1 != null && neighbor1.continentNumber != continentNumber && neighbor1.continentNumber != -1)
                    return 0;
                allTiles.TryGetValue(targetTile.neighborHexes[3], out TileData neighbor2);
                if (neighbor2 != null && neighbor2.continentNumber != continentNumber && neighbor2.continentNumber != -1)
                    return 0;
            }
            if (side == 3)
            {
                allTiles.TryGetValue(targetTile.neighborHexes[2], out TileData neighbor1);
                if (neighbor1 != null && neighbor1.continentNumber != continentNumber && neighbor1.continentNumber != -1)
                    return 0;
                allTiles.TryGetValue(targetTile.neighborHexes[4], out TileData neighbor2);
                if (neighbor2 != null && neighbor2.continentNumber != continentNumber && neighbor2.continentNumber != -1)
                    return 0;
            }
            if (side == 4)
            {
                allTiles.TryGetValue(targetTile.neighborHexes[3], out TileData neighbor1);
                if (neighbor1 != null && neighbor1.continentNumber != continentNumber && neighbor1.continentNumber != -1)
                    return 0;
                allTiles.TryGetValue(targetTile.neighborHexes[5], out TileData neighbor2);
                if (neighbor2 != null && neighbor2.continentNumber != continentNumber && neighbor2.continentNumber != -1)
                    return 0;
            }
            if (side == 5)
            {
                allTiles.TryGetValue(targetTile.neighborHexes[0], out TileData neighbor1);
                if (neighbor1 != null && neighbor1.continentNumber != continentNumber && neighbor1.continentNumber != -1)
                    return 0;
                allTiles.TryGetValue(targetTile.neighborHexes[4], out TileData neighbor2);
                if (neighbor2 != null && neighbor2.continentNumber != continentNumber && neighbor2.continentNumber != -1)
                    return 0;
            }
        }
        if (side == 1 || side == 4)
            return chance;
        else return chance;
    }

    /// <summary>
    /// returns a location of a tile based on how many spaces from a specific side of a hex
    /// </summary>
    /// <param name="xy">The tile to count from</param>
    /// <param name="side">the side of the tile that is measuring from (0 to 5)</param>
    /// <param name="distance">the distance from the tile</param>
    /// <returns></returns>
    public static (int, int) getTileLocationFromSide((int, int) xy, int side, int distance)
    {
        int x = xy.Item1;
        int y = xy.Item2;
        if(side == 0)
        {
            if (y % 2 == 0)
                x += 0 + (Mathf.FloorToInt(distance / 2));
            else x += 1 + (Mathf.FloorToInt(distance / 2));
            y += distance;
        }
        if(side == 1)
        {
            x += distance;
        }
        if(side == 2)
        {
            if (y % 2 == 0)
                x += 0 + (Mathf.FloorToInt(distance / 2));
            else x += 1 + (Mathf.FloorToInt(distance / 2));
            y -= distance;
        }
        if (side == 3)
        {
            if (y % 2 != 0)
                x -= 0 + (Mathf.FloorToInt(distance / 2));
            else x -= 1 + (Mathf.FloorToInt(distance / 2));
            y -= distance;
        }
        if (side == 4)
        {
            x -= distance;
        }
        if (side == 5)
        {
            if (y % 2 != 0)
                x -= 0 + (Mathf.FloorToInt(distance / 2));
            else x -= 1 + (Mathf.FloorToInt(distance / 2));
            y += distance;
        }
        return (x, y);
    }

    /// <summary>
    /// Returns an (int, int) tileLocation within a quadron of the map, 5 returns center of map
    /// </summary>
    /// <param name="xLimit">xLimit of the map</param>
    /// <param name="yLimit">yLimit of the map</param>
    /// <param name="quad">Optional paramater from 0 to 5 representing the MapQuadron enum</param>
    /// <returns></returns>
    public static (int, int) getTileLocationWithinQuadron(int xLimit, int yLimit, string quad = null)
    {
        MapQuadron quadron;
        if (quad == null)
            quadron = (MapQuadron)Random.Range(0, 6);
        else
        {
            int.TryParse(quad, out int quadronInt);
            if (quadronInt > 5 || quadronInt < 0) Debug.LogError("Quadron number is out of range");
            quadron = (MapQuadron)quadronInt;
        }
        //Debug.Log("Starting at: " + quadron);
        if (quadron == MapQuadron.topLeft)
            return (Random.Range(1, xLimit /2), Random.Range(yLimit/2, yLimit -1));
        if (quadron == MapQuadron.topRight)
            return (Random.Range(xLimit /2, xLimit -1), Random.Range(yLimit /2, yLimit -1));
        if (quadron == MapQuadron.bottomLeft)
            return (Random.Range(1, xLimit/2), Random.Range(1, yLimit /2));
        if (quadron == MapQuadron.bottomRight)
            return (Random.Range(xLimit/2, xLimit -1), Random.Range(1, yLimit /2));
        // Returns Center if roll is 5
        return (Mathf.RoundToInt(xLimit / 2), Mathf.RoundToInt(yLimit / 2));
    }

    public static (int, int) getTileLocationInCorner(int xLimit, int yLimit, string quad = null)
    {
        MapQuadron quadron;
        if (quad == null)
            quadron = (MapQuadron)Random.Range(0, 6);
        else
        {
            int.TryParse(quad, out int quadronInt);
            if (quadronInt > 5 || quadronInt < 0) Debug.LogError("Quadron number is out of range");
            quadron = (MapQuadron)quadronInt;
        }
        //Debug.Log("Starting at: " + quadron);
        if (quadron == MapQuadron.topLeft)
            return (Mathf.RoundToInt(xLimit * 0.1f), Mathf.RoundToInt(yLimit * 0.9f));
        if (quadron == MapQuadron.topRight)
            return (Mathf.RoundToInt(xLimit * 0.9f), Mathf.RoundToInt(yLimit * 0.9f));
        if (quadron == MapQuadron.bottomLeft)
            return (Mathf.RoundToInt(xLimit * 0.1f), Mathf.RoundToInt(yLimit * 0.1f));
        if (quadron == MapQuadron.bottomRight)
            return (Mathf.RoundToInt(xLimit * 0.9f), Mathf.RoundToInt(yLimit * 0.1f));
        // Returns Center if roll is 5
        return (Mathf.RoundToInt(xLimit / 2), Mathf.RoundToInt(yLimit / 2));
    }

    public static (int, int) getRandomLocationInEntireMap(int xLimit, int yLimit)
    {
        return (Random.Range(0, xLimit - 1), Random.Range(0, yLimit - 1));
    }

    public static int getRiverDirection(int incoming, bool isFiveDir = false)
    {
        if(isFiveDir)
        {
            // Default is incoming == 0;
            int[] dir = { 1, 2, 3, 4, 5 };
            if (incoming == 1)
                dir = new int[] { 2, 3, 4, 5, 0 };
            if (incoming == 2)
                dir = new int[] { 3, 4, 5, 0, 1 };
            if (incoming == 3)
                dir = new int[] { 4, 5, 0, 1, 2 };
            if (incoming == 4)
                dir = new int[] { 5, 0, 1, 2, 3 };
            if (incoming == 5)
                dir = new int[] { 0, 1, 2, 3, 4 };
            return dir[Random.Range(0, 5)];
        }
        else
        {
            // Default is incoming == 0;
            int[] dir = { 2, 3, 4 };
            if (incoming == 1)
                dir = new int[] { 3, 4, 5 };
            if (incoming == 2)
                dir = new int[] { 4, 5, 0 };
            if (incoming == 3)
                dir = new int[] { 5, 0, 1 };
            if (incoming == 4)
                dir = new int[] { 0, 1, 2 };
            if (incoming == 5)
                dir = new int[] { 1, 2, 3 };
            // Roll dice so higher chance the river goes straight vs turning.
            return dir[rollThreeSides(25, 50, 25)];
        }
    }

}
