using UnityEngine;

public class BuildingData : MonoBehaviour {

    public Building building;

    private Color defaultColor, currentColor;

    private void Awake() {
        currentColor = GetComponent<MeshRenderer>().sharedMaterial.color;
        defaultColor = currentColor;
    }

    private void Update() {
        UpdateColor();
    }

    private void UpdateColor() {
        if (building.controllingPlayer < 0) {
            if (currentColor != defaultColor) {
                GetComponent<MeshRenderer>().sharedMaterial.color = defaultColor;
                currentColor = defaultColor;
            }

            return;
        }

        if (GameController.instance.playerColors.Count <= building.controllingPlayer) {
            Debug.LogError("GameController's playerColor list has no color for player " + building.controllingPlayer);
            return;
        }

        Color color = GameController.instance.playerColors[building.controllingPlayer];

        if (currentColor != color) {
            currentColor = color;
            GetComponent<MeshRenderer>().sharedMaterial.color = color;
        }
    }

}