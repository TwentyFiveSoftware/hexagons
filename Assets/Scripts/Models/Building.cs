using System.Collections.Generic;
using UnityEngine;

public class Building {
    public Vector2Int location;
    public Dictionary<Building, Path> destinations;
}