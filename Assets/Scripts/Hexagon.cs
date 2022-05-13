using UnityEngine;

public class Hexagon : MonoBehaviour {

    public Vector2Int coordinates;

    public void Init(float height, Vector2Int hexCoords) {
        this.coordinates = hexCoords;
        GetComponent<MeshFilter>().mesh = HexagonMesh.GenerateHexagonMesh(height);
    }

}