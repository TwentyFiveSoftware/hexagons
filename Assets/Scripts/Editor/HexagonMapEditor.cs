using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexagonMap))]
public class HexagonMapEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate")) {
            ((HexagonMap)target).Generate();
        }

        if (GUILayout.Button("Destroy")) {
            ((HexagonMap)target).DestroyMap();
        }
    }
}