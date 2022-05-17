using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject entityPrefab;
    public List<Color> playerColors;
    public int playerCount;
    public int unitGenerationTicks;
    public int myPlayerId = 0;

    private bool gameOver = false;

    public static GameController instance { get; private set; }
    public List<Building> buildings = new();

    private int ticksSinceLastUnitGeneration = 0;
    private readonly List<AIController> aiPlayers = new();

    public void StartGame() {
        if (playerCount < 2 || playerCount > playerColors.Count) {
            Debug.LogError("Invalid player count!");
            return;
        }

        gameOver = false;
        aiPlayers.Clear();
        buildings.Clear();
        ticksSinceLastUnitGeneration = 0;

        bool validMapGenerated;
        do {
            validMapGenerated = true;
            GetComponent<HexagonMap>().Generate();

            if (buildings.Count == 0) {
                Debug.LogError("Error while generating buildings!");
                return;
            }

            for (int player = 0; player < playerCount; player++) {
                Building startBuilding = DetermineStartingPosition();

                if (startBuilding == null) {
                    validMapGenerated = false;
                    break;
                }

                startBuilding.SetAsStartingBuilding(player);

                if (player != myPlayerId) {
                    aiPlayers.Add(new AIController(player));
                }
            }
        } while (!validMapGenerated);
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
            gameOver = true;
        } else if (buildings.All(building => building.controllingPlayer != myPlayerId)) {
            GetComponent<PlayAgainScreenHandler>().GameOver();
            gameOver = true;
        }
    }

    public bool IsGameOver() {
        return gameOver;
    }

    private void Awake() {
        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }
    }

    private void Start() {
        StartGame();
    }

    private void FixedUpdate() {
        if (gameOver) {
            return;
        }

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