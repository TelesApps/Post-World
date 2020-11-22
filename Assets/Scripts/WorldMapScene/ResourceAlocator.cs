using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TerrainFetcher;

public class ResourceAlocator
{
    //static ResourceDatabase DB = new ResourceDatabase();
    
    public static List<Resource> alocateResource(TileType type)
    {
        List<Resource> tileHexResource = new List<Resource>();
        // Add logic to return resources according to TileTypes here
        // For now return a copy of every resource in database with an amount of 100
        foreach (Resource res in ResourceDatabase.AllResources)
        {
            Resource r = new Resource(res, amount:100);
            tileHexResource.Add(r);
        }
        return tileHexResource;
    }
}
