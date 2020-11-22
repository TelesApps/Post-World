using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDatabase
{
    public enum ResourceSlug { wood, berries, lumber, rock, clay }

    public static List<Resource> AllResources = new List<Resource>();

    public static Resource getRes(ResourceSlug slug)
    {
        Resource res = AllResources.Find(r => r.NameSlug == slug);
        if (res != null)
            return res;
        else
        {
            Debug.LogError("Resource was not found");
            return null;
        }
    }
    
    public ResourceDatabase()
    {
        // Wood
        {
            Resource res = new Resource();
            res.NameSlug = ResourceSlug.wood;
            res.Name = "Wood";
            res.RestoreRatePerDay = 1;
            res.imgSprite = Resources.Load<Sprite>("ResourceIcons/wood");

            AllResources.Add(res);
        }

        // Lumber
        {
            Resource res = new Resource();
            res.NameSlug = ResourceSlug.lumber;
            res.Name = "Lumber";
            res.RestoreRatePerDay = 0;
            res.imgSprite = Resources.Load<Sprite>("ResourceIcons/planks");

            AllResources.Add(res);
        }

        // Berries
        {
            Resource res = new Resource();
            res.NameSlug = ResourceSlug.berries;
            res.Name = "Berries";
            res.RestoreRatePerDay = 0.5f;
            res.imgSprite = Resources.Load<Sprite>("ResourceIcons/berries");

            AllResources.Add(res);
        }

    }

}
