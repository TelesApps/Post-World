using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static MapCreation;
using static scenePersistence;

public class TerrainFetcher
{
    public enum BiomeType { Plain, Cold, Desert, Tropical }
    public enum Topography { Flat, Hills, Mountains }
    public enum Weather { Dry, Temperate, Wet }

    public enum TileType
    {
        DesertDune, Dirt, ForestBroadleaf, Highlands, Hills, Marsh, Mountain, Ocean, Plains, Scrubland, Woodlands,
        DirtCold, ForestPine, ForestPineTransition, ForestPineSnow, HillCold, HillCave, HillTransition, HillTransitionCave, HillSnow, HillSnowCave,
        MountainSnow, MountainSnowCave, OceanIceberg, PlainsCold, PlainsPond, PlainsTransition, PlainsTransitionPond, PlainsSnow, PlainsSnowPond, 
        Snowfield, RedDirt, RedForest, RedForestOasis, RedGrass, RedGrassDunes, RedGrassOasis, RedHills, RedHillsOasis, RedMesa, RedMesaCave,
        RedMountain, RedMountainsCave, YellowCacti, YellowCrater, YellowDirt, YellowDunes, YellowHills, YellowHillsOasis, YellowMesa, YellowMesaCave,
        YellowMesaOasis, YellowMountains, YellowMountainsCave, YellowSaltFlat, Bog, GrassySand, GrassySandPalms, Jungle, Sand, SandPalms, 
        Swamp, PlainsTropical, Wetlands
    }

    /// Dictionary Values are (Temperature, Rain, Forestry) Or Header, Column, Row in the spreadsheet
    /// Dictionary based on spreadsheet here https://docs.google.com/spreadsheets/d/1VBJSokbVjQ9jIHxldFEhlm-SFj1q4zgzkouRp8YfSbQ/edit#gid=377689169
    static Dictionary<(int, int, int), TileType[]> terrainCombos = new Dictionary<(int, int, int), TileType[]>
    {
        // FROZEN HEADER
        [(0, 0, 0)] = new TileType[] { TileType.PlainsSnow },
        [(0, 1, 0)] = new TileType[] { TileType.Snowfield },
        [(0, 2, 0)] = new TileType[] { TileType.Snowfield },
        [(0, 3, 0)] = new TileType[] { TileType.PlainsSnowPond },
        [(0, 4, 0)] = new TileType[] { TileType.PlainsSnowPond },
        [(0, 0, 1)] = new TileType[] { TileType.Snowfield },
        [(0, 1, 1)] = new TileType[] { TileType.PlainsSnow },
        [(0, 2, 1)] = new TileType[] { TileType.Snowfield },
        [(0, 3, 1)] = new TileType[] { TileType.PlainsSnowPond },
        [(0, 4, 1)] = new TileType[] { TileType.PlainsSnowPond },
        [(0, 0, 2)] = new TileType[] { TileType.Snowfield },
        [(0, 1, 2)] = new TileType[] { TileType.PlainsSnow },
        [(0, 2, 2)] = new TileType[] { TileType.PlainsSnow },
        [(0, 3, 2)] = new TileType[] { TileType.PlainsSnowPond, TileType.ForestPineSnow },
        [(0, 4, 2)] = new TileType[] { TileType.PlainsSnowPond, TileType.ForestPineSnow },

        // COLD HEADER
        [(1, 0, 0)] = new TileType[] { TileType.DirtCold },
        [(1, 1, 0)] = new TileType[] { TileType.DirtCold },
        [(1, 2, 0)] = new TileType[] { TileType.PlainsCold, TileType.PlainsPond },
        [(1, 3, 0)] = new TileType[] { TileType.PlainsPond, TileType.PlainsCold },
        [(1, 4, 0)] = new TileType[] { TileType.PlainsPond, TileType.PlainsCold, TileType.Wetlands },
        [(1, 0, 1)] = new TileType[] { TileType.PlainsCold },
        [(1, 1, 1)] = new TileType[] { TileType.PlainsCold },
        [(1, 2, 1)] = new TileType[] { TileType.PlainsCold, TileType.PlainsPond, TileType.ForestPine },
        [(1, 3, 1)] = new TileType[] { TileType.PlainsPond, TileType.ForestPine },
        [(1, 4, 1)] = new TileType[] { TileType.PlainsPond, TileType.ForestPine, TileType.Wetlands, TileType.Marsh },
        [(1, 0, 2)] = new TileType[] { TileType.PlainsCold, TileType.ForestPine },
        [(1, 1, 2)] = new TileType[] { TileType.PlainsCold, TileType.ForestPine },
        [(1, 2, 2)] = new TileType[] { TileType.ForestPine },
        [(1, 3, 2)] = new TileType[] { TileType.ForestPine },
        [(1, 4, 2)] = new TileType[] { TileType.ForestPine, TileType.Wetlands, TileType.Marsh },

        // TEMPERATE HEADER
        [(2, 0, 0)] = new TileType[] { TileType.Sand, TileType.RedDirt },
        [(2, 1, 0)] = new TileType[] { TileType.DesertDune, TileType.RedDirt, TileType.RedGrass, TileType.RedGrassDunes },
        [(2, 2, 0)] = new TileType[] { TileType.Scrubland, TileType.Plains, TileType.PlainsTropical},
        [(2, 3, 0)] = new TileType[] { TileType.Plains, TileType.Bog },
        [(2, 4, 0)] = new TileType[] { TileType.Bog, TileType.Wetlands },
        [(2, 0, 1)] = new TileType[] { TileType.GrassySand, TileType.RedDirt, TileType.RedGrassDunes },
        [(2, 1, 1)] = new TileType[] { TileType.RedGrass, TileType.Scrubland, TileType.SandPalms, TileType.RedForest },
        [(2, 2, 1)] = new TileType[] { TileType.Plains, TileType.Woodlands, TileType.ForestBroadleaf, TileType.RedForest},
        [(2, 3, 1)] = new TileType[] { TileType.Jungle, TileType.Marsh, TileType.Swamp },
        [(2, 4, 1)] = new TileType[] { TileType.Swamp, TileType.Marsh, TileType.Wetlands },
        [(2, 0, 2)] = new TileType[] { TileType.RedGrassDunes, TileType.GrassySandPalms, TileType.Scrubland, TileType.RedGrass, TileType.YellowSaltFlat },
        [(2, 1, 2)] = new TileType[] { TileType.Scrubland, TileType.RedForest, TileType.YellowSaltFlat },
        [(2, 2, 2)] = new TileType[] { TileType.ForestBroadleaf, TileType.Jungle },
        [(2, 3, 2)] = new TileType[] { TileType.Jungle, TileType.Swamp },
        [(2, 4, 2)] = new TileType[] { TileType.Swamp },

        // HOT HEADER
        [(3, 0, 0)] = new TileType[] { TileType.Sand, TileType.DesertDune, TileType.YellowDunes, TileType.RedDirt, TileType.YellowDirt },
        [(3, 1, 0)] = new TileType[] { TileType.DesertDune, TileType.YellowDunes, TileType.YellowDirt, TileType.RedDirt },
        [(3, 2, 0)] = new TileType[] { TileType.Scrubland, TileType.YellowDirt },
        [(3, 3, 0)] = new TileType[] { TileType.Plains, TileType.PlainsTropical },
        [(3, 4, 0)] = new TileType[] { TileType.Bog, TileType.Wetlands },
        [(3, 0, 1)] = new TileType[] { TileType.GrassySand, TileType.SandPalms, TileType.YellowCacti, TileType.RedGrassDunes },
        [(3, 1, 1)] = new TileType[] { TileType.GrassySand, TileType.YellowCacti, TileType.RedGrassDunes },
        [(3, 2, 1)] = new TileType[] { TileType.Scrubland, TileType.Plains, TileType.YellowCacti, TileType.PlainsTropical },
        [(3, 3, 1)] = new TileType[] { TileType.Jungle, TileType.Swamp, TileType.RedForestOasis },
        [(3, 4, 1)] = new TileType[] { TileType.Swamp, TileType.Marsh, TileType.Wetlands },
        [(3, 0, 2)] = new TileType[] { TileType.YellowCacti, TileType.GrassySandPalms, TileType.Scrubland, TileType.YellowSaltFlat, TileType.RedGrassDunes },
        [(3, 1, 2)] = new TileType[] { TileType.YellowCacti, TileType.YellowSaltFlat, TileType.RedGrass },
        [(3, 2, 2)] = new TileType[] { TileType.Plains, TileType.YellowCacti, TileType.Woodlands, TileType.ForestBroadleaf, TileType.RedForest },
        [(3, 3, 2)] = new TileType[] { TileType.Jungle, TileType.Swamp, TileType.RedForestOasis },
        [(3, 4, 2)] = new TileType[] { TileType.Swamp },

        // SCORCHING HEADER
        [(4, 0, 0)] = new TileType[] { TileType.Sand, TileType.DesertDune, TileType.YellowDunes, TileType.RedDirt },
        [(4, 1, 0)] = new TileType[] { TileType.DesertDune, TileType.YellowDunes, TileType.YellowDirt, TileType.RedDirt },
        [(4, 2, 0)] = new TileType[] { TileType.Scrubland, TileType.YellowDirt, TileType.Sand },
        [(4, 3, 0)] = new TileType[] { TileType.Scrubland, TileType.Plains, TileType.PlainsTropical },
        [(4, 4, 0)] = new TileType[] { TileType.Bog, TileType.Wetlands },
        [(4, 0, 1)] = new TileType[] { TileType.DesertDune, TileType.YellowDunes, TileType.RedDirt, TileType.RedGrassDunes },
        [(4, 1, 1)] = new TileType[] { TileType.DesertDune, TileType.YellowDunes, TileType.YellowCacti, TileType.RedGrass },
        [(4, 2, 1)] = new TileType[] { TileType.Scrubland, TileType.YellowCacti },
        [(4, 3, 1)] = new TileType[] { TileType.Jungle, TileType.Swamp, TileType.ForestBroadleaf, TileType.RedForest },
        [(4, 4, 1)] = new TileType[] { TileType.Swamp, TileType.Marsh, TileType.Wetlands },
        [(4, 0, 2)] = new TileType[] { TileType.YellowCacti, TileType.GrassySandPalms, TileType.Scrubland, TileType.YellowSaltFlat, TileType.RedGrassDunes },
        [(4, 1, 2)] = new TileType[] { TileType.YellowCacti, TileType.YellowSaltFlat, TileType.RedGrassDunes },
        [(4, 2, 2)] = new TileType[] { TileType.Plains, TileType.YellowCacti, TileType.Woodlands, TileType.Scrubland },
        [(4, 3, 2)] = new TileType[] { TileType.Jungle, TileType.Swamp, TileType.ForestBroadleaf },
        [(4, 4, 2)] = new TileType[] { TileType.Swamp },
    };

    // Dictionary Values are(Rain, Temperature) Or Column, Row in the spreadsheet
    static Dictionary<(int, int), TileType[]> HillsCombos = new Dictionary<(int, int), TileType[]>
    {
        // Hills
        [(0, 0)] = new TileType[] { TileType.HillSnow },
        [(1, 0)] = new TileType[] { TileType.HillSnow },
        [(2, 0)] = new TileType[] { TileType.HillSnow },
        [(3, 0)] = new TileType[] { TileType.HillSnow },
        [(4, 0)] = new TileType[] { TileType.HillSnow, TileType.HillSnowCave },
        [(0, 1)] = new TileType[] { TileType.YellowHills, TileType.RedHills },
        [(1, 1)] = new TileType[] { TileType.YellowHills, TileType.RedHills },
        [(2, 1)] = new TileType[] { TileType.HillCold, TileType.Hills, TileType.Highlands},
        [(3, 1)] = new TileType[] { TileType.Highlands, TileType.Hills },
        [(4, 1)] = new TileType[] { TileType.Hills },
        [(0, 2)] = new TileType[] { TileType.YellowHills, TileType.RedHills },
        [(1, 2)] = new TileType[] { TileType.YellowHills, TileType.RedHills },
        [(2, 2)] = new TileType[] { TileType.YellowHills, TileType.RedHills },
        [(3, 2)] = new TileType[] { TileType.YellowHillsOasis, TileType.RedHillsOasis },
        [(4, 2)] = new TileType[] { TileType.Hills, TileType.YellowHillsOasis, TileType.RedHillsOasis },
    };

    // Dictionary Values are(Rain, Temperature) Or Column, Row in the spreadsheet
    static Dictionary<(int, int), TileType[]> MountainCombos = new Dictionary<(int, int), TileType[]>
    {
        // Hills
        [(0, 0)] = new TileType[] { TileType.MountainSnow },
        [(1, 0)] = new TileType[] { TileType.MountainSnow },
        [(2, 0)] = new TileType[] { TileType.MountainSnow },
        [(3, 0)] = new TileType[] { TileType.MountainSnow },
        [(4, 0)] = new TileType[] { TileType.MountainSnow, TileType.MountainSnowCave },
        [(0, 1)] = new TileType[] { TileType.YellowMountains, TileType.YellowMesa, TileType.RedMountain, TileType.RedMesa },
        [(1, 1)] = new TileType[] { TileType.RedMountain, TileType.RedMesa },
        [(2, 1)] = new TileType[] { TileType.Mountain, TileType.RedMountainsCave, TileType.YellowMesaOasis },
        [(3, 1)] = new TileType[] { TileType.Mountain },
        [(4, 1)] = new TileType[] { TileType.Mountain },
        [(0, 2)] = new TileType[] { TileType.YellowMountains, TileType.YellowMesa, TileType.RedMountain, TileType.RedMesa },
        [(1, 2)] = new TileType[] { TileType.YellowMountains, TileType.YellowMesa, TileType.RedMountain, TileType.RedMesa },
        [(2, 2)] = new TileType[] { TileType.RedMountain, TileType.RedMountainsCave, TileType.YellowMesaOasis },
        [(3, 2)] = new TileType[] { TileType.YellowMesaOasis, TileType.RedMesa },
        [(4, 2)] = new TileType[] { TileType.Mountain, TileType.YellowMesaOasis  },
    };

    // The regionType will determin which Region Prefab to instanciate (for the region view of the game where you build your colony).
    public enum RegionType { PlainsRiver_N_S }

    public static TileType setTileType(TileData tile)
    {
        TileType[] possibleTiles;
        if (tile.temperatureType == TileTemperatureType.Frozen && tile.tileType == TileType.Ocean ||
            (tile.temperatureType == TileTemperatureType.Cold && tile.isColder))
            return TileType.OceanIceberg;
        else if (tile.tileType == TileType.Ocean)
            return TileType.Ocean;
        int highTerrainTemps = 0;
        if (tile.temperatureType == TileTemperatureType.Cold || tile.temperatureType == TileTemperatureType.Temperate)
            highTerrainTemps = 1;
        else if (tile.temperatureType == TileTemperatureType.Hot || tile.temperatureType == TileTemperatureType.Scorching)
            highTerrainTemps = 2;
        if (tile.isMountain)
        {
            MountainCombos.TryGetValue(((int)tile.rainfallType, highTerrainTemps), out TileType[] mountainPossibles);
            possibleTiles = mountainPossibles;
        }
        else if (tile.isHill)
        {
            HillsCombos.TryGetValue(((int)tile.rainfallType, highTerrainTemps), out TileType[] hillPossibles);
            possibleTiles = hillPossibles;
        }
        else
        {
            terrainCombos.TryGetValue(((int)tile.temperatureType, (int)tile.rainfallType, (int)tile.forestry), out TileType[] plainPossibles);
            possibleTiles = plainPossibles;
        }

        if(possibleTiles == null)
        {
            Debug.LogError("No tiles with those paramaters were found in the dictionary");
            return TileType.Ocean;
        }

        return possibleTiles[Random.Range(0, possibleTiles.Length)];
            // Delete bellow if everything is working
            //if (possibleTiles.Length > 1)
            //return possibleTiles[Random.Range(0, possibleTiles.Length)];
            //else return possibleTiles[0];
    }

    public static TileType avoidPondsAndOasis(TileType type)
    {
        if (type == TileType.PlainsPond)
            return TileType.PlainsCold;
        if (type == TileType.RedForestOasis || type == TileType.RedGrassOasis)
            return TileType.RedForest;
        if (type == TileType.RedHillsOasis)
            return TileType.RedHills;
        if (type == TileType.YellowHillsOasis)
            return TileType.YellowHills;
        if (type == TileType.YellowSaltFlat)
            return TileType.YellowDirt;
        else return type;
    }

    /// <summary>
    /// Sets the Winter Sprite for each Tile that has seasons
    /// </summary>
    /// <param name="tile">THe TileData of the Tile</param>
    /// <returns>The Winter Sprite for that tile</returns>
    public static Sprite setWinterTileType(TileData tile)
    {
        // Sets the Forest Winter Tiles
        if(tile.summerImg.name == "hexForestPine00") 
        { 
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransition00");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered00");
        }
        if (tile.summerImg.name == "hexForestPine01")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransition01");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered01");
        }
        if (tile.summerImg.name == "hexForestPine02")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransition02");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered02");
        }
        if (tile.summerImg.name == "hexForestPine03")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransition03");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered03");
        }
        if (tile.summerImg.name == "hexForestPineClearing00")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransitionClearing00");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCoveredClearing00");
        }
        if (tile.summerImg.name == "hexForestPineSnowTransition00")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered00");
        if (tile.summerImg.name == "hexForestPineSnowTransition01")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered01");
        if (tile.summerImg.name == "hexForestPineSnowTransition02")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered02");
        if (tile.summerImg.name == "hexForestPineSnowTransition03")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered03");
        if (tile.summerImg.name == "hexForestPineSnowTransitionClearing00")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCoveredClearing00");
        // Sets the Hills Winter Tiles
        if (tile.summerImg.name == "hexHillsCold00")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransition00");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered00");
        }
        if (tile.summerImg.name == "hexHillsCold01")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransition01");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered01");
        }
        if (tile.summerImg.name == "hexHillsCold02")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransition02");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered02");
        }
        if (tile.summerImg.name == "hexHillsCold03")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransition03");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered03");
        }
        if (tile.summerImg.name == "hexHillsColdCave00")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransitionCave00");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCoveredCave00");
        }
        if (tile.summerImg.name == "hexHillsColdSnowTransition00")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered00");
        if (tile.summerImg.name == "hexHillsColdSnowTransition01")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered01");
        if (tile.summerImg.name == "hexHillsColdSnowTransition02")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered02");
        if (tile.summerImg.name == "hexHillsColdSnowTransition03")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered03");
        if (tile.summerImg.name == "hexHillsColdSnowTransitionCave00")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCoveredCave00");
        // Sets the Plains Winter Tiles
        if (tile.summerImg.name == "hexPlainsCold00")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition00");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexSnowField00");
        }
        if (tile.summerImg.name == "hexPlainsCold01")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition01");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexSnowField01");
        }
        if (tile.summerImg.name == "hexPlainsCold02")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition02");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexSnowField02");
        }
        if (tile.summerImg.name == "hexPlainsCold03")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition03");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexSnowField03");
        }
        if (tile.summerImg.name == "hexPlainsColdPond00")
        {
            if (tile.isWarm) return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition03");
            else return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCoveredPond00");
        }
        if (tile.summerImg.name == "hexPlainsColdSnowTransition00")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCovered00");
        if (tile.summerImg.name == "hexPlainsColdSnowTransition01")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCovered01");
        if (tile.summerImg.name == "hexPlainsColdSnowTransition02")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCovered02");
        if (tile.summerImg.name == "hexPlainsColdSnowTransition03")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCovered03");
        if (tile.summerImg.name == "hexPlainsColdSnowTransitionPond00")
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCoveredPond00");

        return tile.summerImg;
    }

    public static RegionType setRegionType(TileData tile)
    {
        // #TODO set all of the different RegionTipes based on the data from tile here
        // exp: if(tile.tileTpe == var), tile.hasRiver, tile.isCoast, return RegionType.
        return RegionType.PlainsRiver_N_S;
    }
    public static Sprite getTileImg(TileType type)
    {
        //return Resources.Load<Sprite>("HexTiles/TropicalTiles/hexBog03");
        List<Sprite> SL = new List<Sprite>();
        // Oceans 
        if (type == TileType.Ocean)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexOcean00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexOcean01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexOcean02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexOcean03"));
            return SL[Random.Range(0, 4)];
        }
        if (type == TileType.OceanIceberg)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexOceanIceBergs00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexOceanIceBergs01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexOceanIceBergs02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexOceanIceBergs03"));
            return SL[Random.Range(0, 4)];
        }
        if (type == TileType.Bog)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexBog00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexBog01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexBog02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexBog03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.DesertDune)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexDesertDunes00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexDesertDunes01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexDesertDunes02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexDesertDunes03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Dirt)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexDirt00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexDirt01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexDirt02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexDirt03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.DirtCold)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexDirtCold00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexDirtCold01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexDirtCold02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexDirtCold03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.ForestBroadleaf)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexForestBroadleaf00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexForestBroadleaf01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexForestBroadleaf02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexForestBroadleaf03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.ForestPine)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPine00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPine01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPine02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPine03"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineClearing00"));
            return SL[Random.Range(0, 5)];
        }
        if(type == TileType.ForestPineSnow)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCovered03"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowCoveredClearing00"));
            return SL[Random.Range(0, 5)];
        }
        if(type == TileType.ForestPineTransition)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransition00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransition01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransition02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransition03"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexForestPineSnowTransitionClearing00"));
            return SL[Random.Range(0, 5)];
        }
        if(type == TileType.GrassySand)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexGrassySand00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexGrassySand01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexGrassySand02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexGrassySand03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.GrassySandPalms)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexGrassySandPalms00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexGrassySandPalms01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexGrassySandPalms02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexGrassySandPalms03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Highlands)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexHighlands00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexHighlands01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexHighlands02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexHighlands03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.HillCave)
        {
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdCave00");
        }
        if(type == TileType.HillCold)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsCold00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsCold01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsCold02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsCold03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Hills)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexHills00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexHills01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexHills02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexHills03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.HillSnow)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCovered03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.HillSnowCave)
        {
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowCoveredCave00");
        }
        if( type == TileType.HillTransition)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransition00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransition01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransition02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransition03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.HillTransitionCave)
        {
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexHillsColdSnowTransitionCave00");
        }
        if(type == TileType.Jungle)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexJungle00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexJungle01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexJungle02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexJungle03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Marsh)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexMarsh00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexMarsh01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexMarsh02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexMarsh03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Mountain)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexMountain00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexMountain01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexMountain02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexMountain03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.MountainSnow)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexMountainSnow00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexMountainSnow01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexMountainSnow02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexMountainSnow03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.MountainSnowCave)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexMountainSnowCave00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexMountainSnowCave01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexMountainSnowCave02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexMountainSnowCave03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Plains)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexPlains00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexPlains01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexPlains02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexPlains03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.PlainsCold)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsCold00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsCold01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsCold02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsCold03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.PlainsPond)
        {
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdPond00");
        }
        if(type == TileType.PlainsSnow)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCovered00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCovered01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCovered02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCovered03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.PlainsSnowPond)
        {
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowCoveredPond00");
        }
        if(type == TileType.PlainsTransition)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdSnowTransition03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.PlainsTransitionPond)
        {
            return Resources.Load<Sprite>("HexTiles/ColdTiles/hexPlainsColdPond00");
        }
        if(type == TileType.PlainsTropical)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexTropicalPlains00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexTropicalPlains01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexTropicalPlains02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexTropicalPlains03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.RedDirt)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedDirt00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedDirt01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedDirt02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedDirt03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.RedForest)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedForest00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedForest01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedForest02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedForest03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.RedForestOasis)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedForestOasis00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedForestOasis01"));
            return SL[Random.Range(0, 2)];
        }
        if(type == TileType.RedGrass)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrass00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrass01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrass02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrass03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.RedGrassDunes)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrassDunes00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrassDunes01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrassDunes02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrassDunes03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.RedGrassOasis)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrassOasis00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedGrassOasis01"));
            return SL[Random.Range(0, 2)];
        }
        if(type == TileType.RedHills)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedHills00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedHills01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedHills02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedHills03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.RedHillsOasis)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedHillsOasis00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedHillsOasis01"));
            return SL[Random.Range(0, 2)];
        }
        if(type == TileType.RedMesa)
        {
            return Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMesaLarge00");
        }
        if(type == TileType.RedMesaCave)
        {
            return Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMesaLargeCave00");
        }
        if(type == TileType.RedMountain)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMountains00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMountains01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMountains02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMountains03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.RedMountainsCave)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMountainsCave00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMountainsCave01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMountainsCave02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertRedMountainsCave03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Sand)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSand00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSand01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSand02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSand03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.SandPalms)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSandPalms00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSandPalms01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSandPalms02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSandPalms03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Scrubland)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexScrublands00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexScrublands01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexScrublands02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexScrublands03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Snowfield)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexSnowField00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexSnowField01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexSnowField02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/ColdTiles/hexSnowField03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Swamp)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSwamp00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSwamp01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSwamp02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexSwamp03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Wetlands)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexWetlands00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexWetlands01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexWetlands02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/TropicalTiles/hexWetlands03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.Woodlands)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexWoodlands00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexWoodlands01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexWoodlands02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/PlainTiles/hexWoodlands03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.YellowCacti)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowCactiForest00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowCactiForest01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowCactiForest02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowCactiForest03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.YellowCrater)
        {
            return Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowCrater00");
        }
        if(type == TileType.YellowDirt)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowDirt00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowDirt01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowDirt02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowDirt03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.YellowDunes)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowDirtDunes00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowDirtDunes01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowDirtDunes02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowDirtDunes03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.YellowHills)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowHills00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowHills01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowHills02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowHills03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.YellowHillsOasis)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowHillsOasis00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowHillsOasis01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowHillsOasis02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowHillsOasis03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.YellowMesa)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesaLarge00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesaLarge01"));
            return SL[Random.Range(0, 2)];
        }
        if(type == TileType.YellowMesaCave)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesaLargeCave00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesaLargeCave01"));
            return SL[Random.Range(0, 2)];
        }
        if(type == TileType.YellowMesaOasis)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesaLargeOasis00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesaLargeOasis01"));
            return SL[Random.Range(0, 2)];
        }
        if(type == TileType.YellowMountains)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesas00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesas01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesas02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesas03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.YellowMountainsCave)
        {
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesasCave00"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesasCave01"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesasCave02"));
            SL.Add(Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowMesasCave03"));
            return SL[Random.Range(0, 4)];
        }
        if(type == TileType.YellowSaltFlat)
        {
            return Resources.Load<Sprite>("HexTiles/DesertTiles/hexDesertYellowSaltFlat00");
        }

        Debug.LogError("Tile Tipe not found, returned default resource");
        return Resources.Load<Sprite>("HexTiles/PlainTiles/hexForestBroadleaf00");
    }

    public static Sprite setRiverImg((int, int, int) river)
    {
        if(river.Item1 >= 0 && river.Item2 >= 0 && river.Item3 >= 0)
        {

        }
        List<string> list = new List<string>();
        // Starting Location of River
        if (river == (-1, 0, -1))
        {
            list.Add("Rivers/hexRoad-010000-00");
            list.Add("Rivers/hexRoad-010000-01");
        }
        if (river == (-1, 1, -1))
        {
            list.Add("Rivers/hexRoad-001000-00");
            list.Add("Rivers/hexRoad-001000-01");
        }
        if (river == (-1, 2, -1))
        {
            list.Add("Rivers/hexRoad-000100-00");
            list.Add("Rivers/hexRoad-000100-01");
        }
        if (river == (-1, 3, -1))
        {
            list.Add("Rivers/hexRoad-000010-00");
            list.Add("Rivers/hexRoad-000010-01");
        }
        if (river == (-1, 4, -1))
        {
            list.Add("Rivers/hexRoad-000001-00");
            list.Add("Rivers/hexRoad-000001-01");
        }
        if (river == (-1, 5, -1))
        {
            list.Add("Rivers/hexRoad-100000-00");
            list.Add("Rivers/hexRoad-100000-01");
        }

        // 2 sided rivers
        if (river == (0, 1, -1) || river == (1, 0, -1))
        {
            list.Add("Rivers/hexRoad-011000-00");
            list.Add("Rivers/hexRoad-011000-01");
        }
        if (river == (0, 2, -1) || river == (2, 0, -1))
        {
            list.Add("Rivers/hexRoad-010100-00");
            list.Add("Rivers/hexRoad-010100-01");
        }
        if (river == (0, 3, -1) || river == (3, 0, -1))
        {
            list.Add("Rivers/hexRoad-010010-00");
            list.Add("Rivers/hexRoad-010010-01");
        }
        if (river == (0, 4, -1) || river == (4, 0, -1))
        {
            list.Add("Rivers/hexRoad-010001-00");
            list.Add("Rivers/hexRoad-010001-01");
        }
        if (river == (0, 5, -1) || river == (5, 0, -1))
        {
            list.Add("Rivers/hexRoad-110000-00");
            list.Add("Rivers/hexRoad-110000-01");
        }
        if (river == (1, 2, -1) || river == (2, 1, -1))
        {
            list.Add("Rivers/hexRoad-001100-00");
            list.Add("Rivers/hexRoad-001100-01");
        }
        if (river == (1, 3, -1) || river == (3, 1, -1))
        {
            list.Add("Rivers/hexRoad-001010-00");
            list.Add("Rivers/hexRoad-001010-01");
        }
        if (river == (1, 4, -1) || river == (4, 1, -1))
        {
            list.Add("Rivers/hexRoad-001001-00");
            list.Add("Rivers/hexRoad-001001-01");
        }
        if (river == (1, 5, -1) || river == (5, 1, -1))
        {
            list.Add("Rivers/hexRoad-101000-00");
            list.Add("Rivers/hexRoad-101000-01");
        }
        if (river == (2, 3, -1) || river == (3, 2, -1))
        {
            list.Add("Rivers/hexRoad-000110-00");
            list.Add("Rivers/hexRoad-000110-01");
        }
        if (river == (2, 4, -1) || river == (4, 2, -1))
        {
            list.Add("Rivers/hexRoad-000101-00");
            list.Add("Rivers/hexRoad-000101-01");
        }
        if (river == (2, 5, -1) || river == (5, 2, -1))
        {
            list.Add("Rivers/hexRoad-100100-00");
            list.Add("Rivers/hexRoad-100100-01");
        }
        if (river == (3, 4, -1) || river == (4, 3, -1))
        {
            list.Add("Rivers/hexRoad-000011-00");
            list.Add("Rivers/hexRoad-000011-01");
        }
        if (river == (3, 5, -1) || river == (5, 3, -1))
        {
            list.Add("Rivers/hexRoad-100010-00");
            list.Add("Rivers/hexRoad-100010-00");
        }
        if (river == (4, 5, -1) || river == (5, 4, -1))
        {
            list.Add("Rivers/hexRoad-100001-00");
            list.Add("Rivers/hexRoad-100001-01");
        }

        // Splitted Rivers
        List<int> directions = new List<int> { river.Item1, river.Item2, river.Item3 };
        if (directions.TrueForAll(d => d == 0 || d == 1 || d == 2))
            return Resources.Load<Sprite>("Rivers/hexRoad-011100-00");
        if (directions.TrueForAll(d => d == 0 || d == 1 || d == 3))
            return Resources.Load<Sprite>("Rivers/hexRoad-011010-00");
        if (directions.TrueForAll(d => d == 0 || d == 1 || d == 4))
            return Resources.Load<Sprite>("Rivers/hexRoad-011001-00");
        if (directions.TrueForAll(d => d == 0 || d == 1 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-111000-00");
        if (directions.TrueForAll(d => d == 0 || d == 2 || d == 3))
            return Resources.Load<Sprite>("Rivers/hexRoad-010110-00");
        if (directions.TrueForAll(d => d == 0 || d == 2 || d == 4))
            return Resources.Load<Sprite>("Rivers/hexRoad-010101-00");
        if (directions.TrueForAll(d => d == 0 || d == 2 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-110100-00");
        if (directions.TrueForAll(d => d == 0 || d == 3 || d == 4))
            return Resources.Load<Sprite>("Rivers/hexRoad-010011-00");
        if (directions.TrueForAll(d => d == 0 || d == 3 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-110010-00");
        if (directions.TrueForAll(d => d == 0 || d == 4 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-110001-00");
        if (directions.TrueForAll(d => d == 1 || d == 2 || d == 3))
            return Resources.Load<Sprite>("Rivers/hexRoad-001110-00");
        if (directions.TrueForAll(d => d == 1 || d == 2 || d == 4))
            return Resources.Load<Sprite>("Rivers/hexRoad-001101-00");
        if (directions.TrueForAll(d => d == 1 || d == 2 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-101100-00");
        if (directions.TrueForAll(d => d == 1 || d == 3 || d == 4))
            return Resources.Load<Sprite>("Rivers/hexRoad-001011-00");
        if (directions.TrueForAll(d => d == 1 || d == 3 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-101010-00");
        if (directions.TrueForAll(d => d == 1 || d == 4 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-101001-00");
        if (directions.TrueForAll(d => d == 2 || d == 3 || d == 4))
            return Resources.Load<Sprite>("Rivers/hexRoad-000111-00");
        if (directions.TrueForAll(d => d == 2 || d == 3 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-100110-00");
        if (directions.TrueForAll(d => d == 2 || d == 4 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-100101-00");
        if (directions.TrueForAll(d => d == 3 || d == 4 || d == 5))
            return Resources.Load<Sprite>("Rivers/hexRoad-100011-00");
        
        if (list.Count != 2)
        {
            return null;
        }
        return Resources.Load<Sprite>(list[Random.Range(0,2)]);
    }
}
