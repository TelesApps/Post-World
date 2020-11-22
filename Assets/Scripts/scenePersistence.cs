using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MapCreation;

public class scenePersistence : MonoBehaviour
{
    public MapSize mapSize;
    public MapType mapType;
    public SeaLvl seaLvl;
    public HillsLvl topography;
    public int temperatureLvl;
    public int rainLvl;
    public Forestry forestry;
    private void Awake()
    {
        DontDestroyOnLoad(this.transform.gameObject);
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void setMapProperties(MapSize mapSize, MapType mapType, SeaLvl seaLvl, HillsLvl topography,
        int temperatureLvl, int rainLvl, Forestry forestry)
    {
        this.mapSize = mapSize;
        this.mapType = mapType;
        this.seaLvl = seaLvl;
        this.topography = topography;
        this.temperatureLvl = temperatureLvl;
        this.rainLvl = rainLvl;
        this.forestry = forestry;
    }
}
