using UnityEngine;

public class BuildingSelector : MonoBehaviour {

    public GameController gameController;

    private Camera cameraComponent;
    private GameObject selectedBuildingA;

    private void Start() {
        cameraComponent = GetComponent<Camera>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            MouseDown();
        }

        if (Input.GetMouseButtonUp(0)) {
            MouseUp();
        }
    }

    private void MouseDown() {
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (hit.transform.gameObject.CompareTag("BUILDING")) {
                selectedBuildingA = hit.transform.gameObject;
            }
        }
    }

    private void MouseUp() {
        if (selectedBuildingA == null) {
            return;
        }

        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            if (!hit.transform.gameObject.Equals(selectedBuildingA) &&
                hit.transform.gameObject.CompareTag("BUILDING")) {
                Building buildingFrom = selectedBuildingA.GetComponent<BuildingData>().building;
                Building buildingTo = hit.transform.gameObject.GetComponent<BuildingData>().building;

                gameController.HandleBuildingDragAndDrop(buildingFrom, buildingTo);
            }
        }

        selectedBuildingA = null;
    }

}