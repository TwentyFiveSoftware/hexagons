using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Hexagon : MonoBehaviour {

    public Vector2Int coordinates;

    public List<HexagonTypeData> hexagonTypeData;

    public void Init(float height, HexagonType type, Vector2Int hexCoords) {
        this.coordinates = hexCoords;
        GetComponent<MeshFilter>().mesh = HexagonMesh.GenerateHexagonMesh(height);

        if (hexagonTypeData.Exists(data => data.type == type)) {
            HexagonTypeData data = hexagonTypeData.Find((data) => data.type == type);
            GetComponent<MeshRenderer>().sharedMaterial = data.material;
        } else {
            Debug.LogError("HexagonTypeData not found for type '" + type + "'");
        }
    }

}