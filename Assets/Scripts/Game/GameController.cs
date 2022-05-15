using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class GameController : MonoBehaviour {

    public GameObject entityPrefab;
    public List<Color> playerColors;
    public int playerCount;
    public int unitGenerationTicks;
    public int myPlayerId = 0;

    public static GameController instance { get; private set; }
    public List<Building> buildings = new();

    private int ticksSinceLastUnitGeneration = 0;

    public void StartGame() {
        if (playerCount < 2 || playerCount > playerColors.Count) {
            Debug.LogError("Invalid player count!");
            return;
        }

        for (int player = 0; player < playerCount; player++) {
            Building startBuilding = DetermineStartingPosition();

            if (startBuilding == null) {
                Debug.LogError("Unable to determine suitable starting position for player " + player);
                return;
            }

            startBuilding.SetAsStartingBuilding(player);
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

        if (buildingFrom.controllingPlayer != myPlayerId || buildingFrom.units < 2) {
            return;
        }

        Path path = PathFinder.FindPath(buildingFrom, buildingTo);

        if (path == null) {
            Debug.LogError("No path found!");
            return;
        }

        int units = buildingFrom.units / 2;
        buildingFrom.units -= units;

        GameObject entityObject = Instantiate(entityPrefab, Entity.GetWorldPositionWithHeight(buildingFrom.location),
            Quaternion.identity);
        entityObject.GetComponent<Entity>().Init(path, units, myPlayerId, buildingTo);
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    private void Start() {
        GetComponent<HexagonMap>().Generate();
        StartGame();
    }

    private void FixedUpdate() {
        ticksSinceLastUnitGeneration++;

        if (ticksSinceLastUnitGeneration >= unitGenerationTicks) {
            ticksSinceLastUnitGeneration = 0;

            foreach (Building building in buildings) {
                building.GenerateUnits();
            }
        }
    }
}