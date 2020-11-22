using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceDatabase;

public class ColonyData
{
    public string HexTileID { get; set; }
    public string ColonyName { get; set; }
    public bool IsColonized = false;
    public int ColonyStorageLimit = 10;
    public List<Resource> AllResources = new List<Resource>();
    public List<Colonist> AllColonists = new List<Colonist>();
    public List<Building> AllBuildings = new List<Building>();


    public ColonyData()
    {
        this.ColonyName = "Uncolonized";
    }

    /// <summary>
    /// Sets or adds a specific resource to the total amount in the colony, to deduct from resources simply pass negative numbers
    /// </summary>
    /// <param name="slug">The resource that is being changed</param>
    /// <param name="amount">The amount it is being changed by</param>
    /// <param name="isAdding">Is the amount to be added or to be set</param>
    /// <returns>Returns true addition is successful and False it it fails due to resource limitations</returns>
    public bool setResource(ResourceSlug slug, float amount, bool isAdding = true)
    {
        Resource res = AllResources.Find(r => r.NameSlug == slug);
        if (res == null && amount > 0)
        {
            AllResources.Add(new Resource(ResourceDatabase.AllResources.Find(r => r.NameSlug == slug), amount));
        }
        else
        {
            if(isAdding)
            {
                if (res.Amount + amount <= ColonyStorageLimit && res.Amount + amount > -1)
                    res.Amount += amount;
                else return false;
            }
            else
            {
                if (amount <= ColonyStorageLimit && amount > -1)
                    res.Amount = amount;
                else return false;
            }
        }

        return true;
    } 

}
