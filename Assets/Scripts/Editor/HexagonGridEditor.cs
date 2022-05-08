using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexagonGrid))]
public class HexagonGridEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate Grid")) {
            HexagonGrid hexagonGrid = (HexagonGrid)target;
            hexagonGrid.GenerateGrid();
        }
    }
}