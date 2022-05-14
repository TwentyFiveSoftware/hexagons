using UnityEngine;

public class BuildingData : MonoBehaviour {

    public Building building;

    private Color defaultColor, currentColor;
    private TMPro.TextMeshProUGUI unitText;

    private void Awake() {
        currentColor = GetComponent<MeshRenderer>().sharedMaterial.color;
        defaultColor = currentColor;

        unitText = transform.GetChild(0).GetComponentInChildren<TMPro.TextMeshProUGUI>();
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void Start() {
        UpdateUnitText();
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

    private void UpdateUnitText() {
        if (building == null) {
            return;
        }

        unitText.text = building.units.ToString();
    }

}