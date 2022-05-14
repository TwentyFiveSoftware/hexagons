using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GameController : MonoBehaviour {

    public GameObject entityPrefab;
    public List<Color> playerColors;

    public static GameController instance { get; private set; }
    public List<Building> buildings;

    public void StartGame() {
        if (buildings == null) {
            return;
        }

        foreach (Building building in buildings) {
            building.controllingPlayer = Random.Range(-1, playerColors.Count);
        }
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