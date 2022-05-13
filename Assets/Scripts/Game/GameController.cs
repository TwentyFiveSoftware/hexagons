using UnityEngine;

public class GameController : MonoBehaviour {

    public void HandleBuildingDragAndDrop(Building buildingFrom, Building buildingTo) {
        Debug.Log("Drag from building " + buildingFrom.location + " to " + buildingTo.location);
    }

}