using System.Collections.Generic;
using UnityEngine;

public class Building {
    public Vector2Int location;
    public Vector2Int gridCoordinates;
    public Dictionary<Building, Path> destinations;

    public int controllingPlayer = -1;
    public int units;
    public int unitGeneration = 1;

    public Building() {
        units = Random.Range(1, 50);
    }

    public void SetAsStartingBuilding(int player) {
        units = 10;
        controllingPlayer = player;
    }

    public void GenerateUnits() {
        if (controllingPlayer < 0) {
            return;
        }

        units += unitGeneration;
    }

    public void UnitsArrive(int arrivingUnits, int player) {
        if (player == controllingPlayer) {
            units += arrivingUnits;
            return;
        }

        units -= arrivingUnits;
        if (units < 0) {
            controllingPlayer = player;
            units = -units;
        }
    }
}