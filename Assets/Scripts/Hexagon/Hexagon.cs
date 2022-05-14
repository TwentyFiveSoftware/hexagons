using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

    public List<HexagonTypeData> hexagonTypeData;
    public float buildingSizeMultiplier;

    public void Init(float height, HexagonType type) {
        GetComponent<MeshFilter>().mesh = HexagonMesh.GenerateHexagonMesh(height);

        if (hexagonTypeData.Exists(data => data.type == type)) {
            HexagonTypeData data = hexagonTypeData.Find(data => data.type == type);
            GetComponent<MeshRenderer>().sharedMaterial = data.material;
        } else {
            Debug.LogError("HexagonTypeData not found for type '" + type + "'");
        }
    }

    public void InitBuilding(Building building) {
        gameObject.AddComponent<BuildingData>().building = building;

        gameObject.tag = "BUILDING";

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = GetComponent<MeshFilter>().sharedMesh;
        meshCollider.convex = true;
        meshCollider.isTrigger = true;

        transform.localScale = new Vector3(transform.localScale.x * buildingSizeMultiplier, transform.localScale.y,
            transform.localScale.z * buildingSizeMultiplier);
        
        GetComponent<MeshRenderer>().sharedMaterial = new Material(GetComponent<MeshRenderer>().sharedMaterial);
    }

    public static Vector3 CalculateHexagonWorldPosition(Vector2Int position) {
        float innerRadius = Mathf.Sin(Mathf.Deg2Rad * 60);
        return new Vector3(2.0f * innerRadius * position.x + innerRadius * position.y, 0, 1.5f * position.y);
    }

}