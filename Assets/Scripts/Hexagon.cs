using UnityEngine;

public class Hexagon : MonoBehaviour {

    public void Init(float height) {
        GetComponent<MeshFilter>().mesh = HexagonMesh.GenerateHexagonMesh(height);
    }

}