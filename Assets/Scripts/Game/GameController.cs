using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class GameController : MonoBehaviour {

    public GameObject entityPrefab;
    public List<Color> playerColors;
    public int playerCount;

    public static GameController instance { get; private set; }
    public List<Building> buildings = new();

    public void StartGame() {
        if (playerCount < 2 || playerCount > playerColors.Count) {
            Debug.LogError("Invalid player count!");
            return;
        }

        for (int player = 0; player < playerCount; player++) {
            Building startBuilding = DetermineStartingPosition();

            if (startBuilding == null) {
                Debug.Log("Unable to determine suitable starting position for player " + player);
                return;
            }

            startBuilding.controllingPlayer = player;
        }
    }

    private Building DetermineStartingPosition() {
        if (buildings.Count == 0) {
            return null;
        }

        for (int i = 1; i <= 4; i++) {
            foreach (Building building in buildings) {
                if (building.controllingPlayer < 0 && building.destinations.Count == i) {
                    bool hasDirectConnectionToOtherPlayer =
                        building.destinations.Keys.Any(b => b.controllingPlayer >= 0);

                    if (hasDirectConnectionToOtherPlayer) {
                        continue;
                    }

                    return building;
                }
            }
        }

        return null;
    }

    public void HandleBuildingDragAndDrop(Building buildingFrom, Building buildingTo) {
        Debug.Log("Drag from building " + buildingFrom.location + " to " + buildingTo.location);

        Path path = PathFinder.FindPath(buildingFrom, buildingTo);

        if (path == null) {
            Debug.LogError("No path found!");
            return;
        }

        GameObject entityObject = Instantiate(entityPrefab, Entity.GetWorldPositionWithHeight(buildingFrom.location),
            Quaternion.identity);
        entityObject.GetComponent<Entity>().Init(path);
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    }
}