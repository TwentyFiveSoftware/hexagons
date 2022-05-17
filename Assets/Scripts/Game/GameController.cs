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
    private List<AIController> aiPlayers = new();

    private void StartGame() {
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

            if (player != myPlayerId) {
                aiPlayers.Add(new AIController(player));
            }
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

    public void InitiateUnitMove(List<Building> srcBuildings, Building dstBuilding, int playerId) {
        srcBuildings = srcBuildings.Where(building => building.controllingPlayer == playerId && building.units >= 2)
                                   .ToList();

        foreach (Building srcBuilding in srcBuildings) {
            Path path = PathFinder.FindPath(srcBuilding, dstBuilding);

            if (path == null) {
                Debug.LogError("No path found!");
                return;
            }

            int units = srcBuilding.units / 2;
            srcBuilding.units -= units;

            GameObject entityObject = Instantiate(entityPrefab, Entity.GetWorldPositionWithHeight(srcBuilding.location),
                Quaternion.identity);
            entityObject.GetComponent<Entity>().Init(path, units, playerId, dstBuilding);
        }
    }

    private void CheckForGameOver() {
        if (aiPlayers.All(ai => ai.IsEliminated())) {
            GetComponent<PlayAgainScreenHandler>().GameWon();
        } else if (buildings.All(building => building.controllingPlayer != myPlayerId)) {
            GetComponent<PlayAgainScreenHandler>().GameOver();
        }
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

            foreach (AIController ai in aiPlayers) {
                ai.Update();
            }

            CheckForGameOver();
        }
    }
}