using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River
{
    public int riverNumber { get; set; }
    public (int, int) currentLocation = (-1, -1);
    public int lastDirection = -1;

    public bool canPassHills = true;
    public bool isFinished = false;

    public River(TileData startingTile)
    {
        this.currentLocation = startingTile.gridLocation;
    }

}
