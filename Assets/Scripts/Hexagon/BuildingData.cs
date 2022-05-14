using UnityEngine;

public class BuildingData : MonoBehaviour {

    public Building building;

    private Color defaultColor, currentColor;
    private TMPro.TextMeshProUGUI unitText;

    private void Awake() {
        currentColor = GetComponent<MeshRenderer>().sharedMaterial.color;
        defaultColor = currentColor;

        unitText = transform.GetChild(0).GetComponentInChildren<TMPro.TextMeshProUGUI>();
    }

    private void Start() {
        UpdateUnitText();
        UpdateUnitTextVisible();
    }

    private void Update() {
        UpdateColor();
    }

    private void FixedUpdate() {
        UpdateUnitText();
    }

    private void UpdateColor() {
        if (building.controllingPlayer < 0) {
            if (currentColor != defaultColor) {
                GetComponent<MeshRenderer>().sharedMaterial.color = defaultColor;
                currentColor = defaultColor;
                UpdateUnitTextVisible();
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
            UpdateUnitTextVisible();
        }
    }

    private void UpdateUnitText() {
        if (building == null || building.controllingPlayer < 0) {
            return;
        }

        unitText.text = building.units.ToString();
    }

    private void UpdateUnitTextVisible() {
        bool visible = building != null && building.controllingPlayer >= 0;
        transform.GetChild(0).gameObject.SetActive(visible);
    }

}