using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ResourceDatabase;

public class Resource
{
    public Sprite imgSprite { get; set; }
    public ResourceSlug NameSlug { get; set; }
    public string Name { get; set; }
    public float Amount = 0;
    public float RestoreRatePerDay { get; set; }

    // Basic Constructor
    public Resource() { }

    /// <summary>
    /// Constructor that receives the information for this instance of Resource. 
    /// This instance becomes a copy of an existing data.
    /// </summary>
    /// <param name="data">The resource that a copy will be made out of</param>
    /// <param name="amount">Optional paramater to set an amount</param>
    public Resource(Resource data, float amount = 0)
    {
        this.imgSprite = data.imgSprite;
        this.NameSlug = data.NameSlug;
        this.Name = data.Name;
        if (amount != 0) this.Amount = amount;
        else this.Amount = data.Amount;
        this.RestoreRatePerDay = data.RestoreRatePerDay;
    }


    public void restoreResource()
    {
        this.Amount += this.RestoreRatePerDay;
    }

}
