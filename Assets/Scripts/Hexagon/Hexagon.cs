using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

    public List<HexagonTypeData> hexagonTypeData;

    public void Init(float height, HexagonType type) {
        GetComponent<MeshFilter>().mesh = HexagonMesh.GenerateHexagonMesh(height);

        if (hexagonTypeData.Exists(data => data.type == type)) {
            HexagonTypeData data = hexagonTypeData.Find((data) => data.type == type);
            GetComponent<MeshRenderer>().sharedMaterial = data.material;
        } else {
            Debug.LogError("HexagonTypeData not found for type '" + type + "'");
        }
    }

    public static Vector3 CalculateHexagonWorldPosition(Vector2Int position) {
        float innerRadius = Mathf.Sin(Mathf.Deg2Rad * 60);
        return new Vector3(2.0f * innerRadius * position.x + innerRadius * position.y, 0, 1.5f * position.y);
    }

}