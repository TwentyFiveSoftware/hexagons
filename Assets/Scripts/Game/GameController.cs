using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject entityPrefab;

    public void HandleBuildingDragAndDrop(Building buildingFrom, Building buildingTo) {
        Debug.Log("Drag from building " + buildingFrom.location + " to " + buildingTo.location);

        Path path = buildingFrom.destinations[buildingTo];

        GameObject entityObject = Instantiate(entityPrefab, Entity.GetWorldPositionWithHeight(buildingFrom.location),
            Quaternion.identity);
        entityObject.GetComponent<Entity>().Init(path);
    }

}