using System.Collections.Generic;
using System.Linq;

public class AIController {
    private readonly int playerId;
    private bool eliminated;

    public AIController(int playerId) {
        this.playerId = playerId;
    }

    public void Update() {
        if (eliminated) {
            return;
        }

        List<Building> controlledBuildings = GetControlledBuildings();

        if (controlledBuildings.Count == 0) {
            eliminated = true;
            return;
        }

        List<Building> srcBuildings = controlledBuildings.Where(building => building.units > 10).ToList();
        int unitsAvailable = srcBuildings.Sum(building => building.units - 10);

        Building target = ChooseNextBuildingToConquer(unitsAvailable);
        if (target == null) {
            return;
        }

        if (unitsAvailable >= target.units * 0.5f) {
            GameController.instance.InitiateUnitMove(srcBuildings, target, playerId);
        }
    }

    public bool IsEliminated() {
        return eliminated;
    }

    private List<Building> GetControlledBuildings() {
        return GameController.instance.buildings.Where(building => building.controllingPlayer == playerId).ToList();
    }

    private Building ChooseNextBuildingToConquer(int unitsAvailable) {
        List<Building> controlledBuildings = GetControlledBuildings();

        List<Building> possibleBuildings = controlledBuildings.SelectMany(building => building.destinations.Keys)
                                                              .Where(building => building.controllingPlayer != playerId)
                                                              .OrderBy(building => building.units).ToList();

        if (possibleBuildings.Count == 0) {
            return null;
        }

        if (possibleBuildings.Any(building => building.controllingPlayer >= 0 && building.units < unitsAvailable)) {
            return possibleBuildings.Find(
                building => building.controllingPlayer >= 0 && building.units < unitsAvailable);
        }

        return possibleBuildings[0];
    }

}