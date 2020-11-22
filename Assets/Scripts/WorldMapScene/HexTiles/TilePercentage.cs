using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilePercentage
{
    public float plainBiome { get; set; }
    public float coldBiome { get; set; }
    public float desertBiome { get; set; }
    public float tropicalBiome { get; set; }

    public float flat { get; set; }
    public float hill { get; set; }
    public float mountain { get; set; }

    public float dry { get; set; }
    public float temperate { get; set; }
    public float wet { get; set; }

    public float lowPlants { get; set; }
    public float medPlants { get; set; }
    public float highPlants { get; set; }

    public TilePercentage()
    {
        this.plainBiome = .25f;
        this.coldBiome = .25f;
        this.desertBiome = .25f;
        this.tropicalBiome = .25f;
        this.flat = .65f;
        this.hill = .2f;
        this.mountain = .15f;
        this.dry = .3f;
        this.temperate = .4f;
        this.wet = .3f;
        this.lowPlants = .3f;
        this.medPlants = .4f;
        this.highPlants = .3f;
    }
    /// <summary>
    /// Sets the chance of Biome being plain, cold desert or tropical
    /// </summary>
    /// <param name="isAdding">true if adding to chance, false if setting the chance</param>
    /// <param name="plain">plains chance</param>
    /// <param name="cold">cold chance</param>
    /// <param name="desert">desert chance</param>
    /// <param name="tropical">tropical chance</param>
    public void setBiomePercent(bool isAdding, float plain, float cold, float desert, float tropical)
    {
        if(isAdding)
        {
            this.plainBiome += plain;
            this.coldBiome += cold;
            this.desertBiome += desert;
            this.tropicalBiome += tropical;
        } else
        {
            this.plainBiome = plain;
            this.coldBiome = cold;
            this.desertBiome = desert;
            this.tropicalBiome = tropical;
        }
    }
    
    public void setTopographyPercent(bool isAdding, float flat, float hill, float mountain)
    {
        if(isAdding)
        {
            this.flat += flat;
            this.hill += hill;
            this.mountain += mountain;
        } else
        {
            this.flat = flat;
            this.hill = hill;
            this.mountain = mountain;
        }
    }

    public void setWeatherPercent(bool isAdding, float dry, float temperate, float wet)
    {
        if(isAdding)
        {
            this.dry += dry;
            this.temperate += temperate;
            this.wet += wet;
        } else
        {
            this.dry = dry;
            this.temperate = temperate;
            this.wet = wet;
        }
    }

    public void setForestry(bool isAdding, float low, float med, float high)
    {
        if(isAdding)
        {
            this.lowPlants += low;
            this.medPlants += med;
            this.highPlants += high;
        } else
        {
            this.lowPlants = low;
            this.medPlants = med;
            this.highPlants = high;
        }
    }
}
