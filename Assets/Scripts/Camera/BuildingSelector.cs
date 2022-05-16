using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingSelector : MonoBehaviour {

    private Camera cameraComponent;
    private List<GameObject> selectedSrcBuildings = new();
    private GameObject currentlyHighlightedObj;

    private void Start() {
        cameraComponent = GetComponent<Camera>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            MouseDown();
        }

        if (Input.GetMouseButton(0) && selectedSrcBuildings.Count > 0) {
            MouseDown();
        }

        if (Input.GetMouseButtonUp(0)) {
            MouseUp();
        }
    }

    private void MouseDown() {
        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            GameObject obj = hit.transform.gameObject;
            if (!obj.CompareTag("BUILDING")) {
                return;
            }

            HighlightObject(obj);

            if (selectedSrcBuildings.Contains(obj)) {
                return;
            }

            if (obj.GetComponent<BuildingData>().building.controllingPlayer != GameController.instance.myPlayerId) {
                return;
            }

            selectedSrcBuildings.Add(obj);
            SetBuildingSelected(obj, true);
        } else {
            UnhighlightObject();
        }

        if (selectedSrcBuildings.Count > 0) {
            for (int i = selectedSrcBuildings.Count - 1; i >= 0; i--) {
                if (selectedSrcBuildings[i].GetComponent<BuildingData>().building.controllingPlayer !=
                    GameController.instance.myPlayerId) {
                    SetBuildingSelected(selectedSrcBuildings[i], false);
                    selectedSrcBuildings.RemoveAt(i);
                }
            }
        }
    }

    private void MouseUp() {
        UnhighlightObject();

        if (selectedSrcBuildings.Count == 0) {
            return;
        }

        Ray ray = cameraComponent.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit)) {
            GameObject obj = hit.transform.gameObject;

            if (!obj.CompareTag("BUILDING")) {
                DeselectBuildings();
                return;
            }

            if (selectedSrcBuildings.Contains(obj)) {
                SetBuildingSelected(obj, false);
                selectedSrcBuildings.Remove(obj);
            }

            List<Building> srcBuildings = selectedSrcBuildings
                                          .Select(building => building.GetComponent<BuildingData>().building).ToList();
            Building dstBuilding = hit.transform.gameObject.GetComponent<BuildingData>().building;
            GameController.instance.InitiateUnitMove(srcBuildings, dstBuilding, GameController.instance.myPlayerId);
        }

        DeselectBuildings();
    }

    private void DeselectBuildings() {
        foreach (GameObject building in selectedSrcBuildings) {
            SetBuildingSelected(building, false);
        }

        selectedSrcBuildings.Clear();
    }

    private void SetBuildingSelected(GameObject building, bool selected) {
        building.transform.GetChild(1).gameObject.SetActive(selected);
    }

    private void HighlightObject(GameObject obj) {
        UnhighlightObject();

        if (selectedSrcBuildings.Count == 0 ||
            (selectedSrcBuildings.Count == 1 && selectedSrcBuildings.Contains(obj))) {
            return;
        }

        currentlyHighlightedObj = obj;

        BuildingArrow.ShowArrowAbove(obj);
    }

    private void UnhighlightObject() {
        if (currentlyHighlightedObj == null) {
            return;
        }

        foreach (LineRenderer line in currentlyHighlightedObj.GetComponentsInChildren<LineRenderer>()) {
            Destroy(line.gameObject);
        }

        currentlyHighlightedObj = null;
    }

}