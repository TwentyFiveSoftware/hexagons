using System.Collections.Generic;
using UnityEngine;

public class Building {
    public Vector2Int location;
    public Vector2Int gridCoordinates;
    public Dictionary<Building, Path> destinations;

    public int controllingPlayer = -1;
    public int units = 10;
    public int unitGeneration = 1;

    public void GenerateUnits() {
        if (controllingPlayer < 0) {
            return;
        }

        units += unitGeneration;
    }
}