using System.Collections.Generic;
using UnityEngine;

public class Building {
    public Vector2Int location;
    public Vector2Int gridCoordinates;
    public Dictionary<Building, Path> destinations;
}