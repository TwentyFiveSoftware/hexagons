using UnityEngine;

public class Hexagon : MonoBehaviour {

    private void Awake() {
        GetComponent<MeshFilter>().mesh = HexagonMesh.GenerateHexagonMesh(0.2f);
    }

}