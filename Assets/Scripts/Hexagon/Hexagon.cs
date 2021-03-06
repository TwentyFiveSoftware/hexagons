using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

    public List<HexagonTypeData> hexagonTypeData;
    public float buildingSizeMultiplier;
    public float unitTextYOffset;

    private float hexagonHeight;

    public void Init(float height, HexagonType type) {
        hexagonHeight = height;
        GetComponent<MeshFilter>().mesh = HexagonMesh.GenerateHexagonMesh(height);

        if (hexagonTypeData.Exists(data => data.type == type)) {
            HexagonTypeData data = hexagonTypeData.Find(data => data.type == type);
            GetComponent<MeshRenderer>().sharedMaterial = data.materials[Random.Range(0, data.materials.Count)];
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

        transform.localScale *= buildingSizeMultiplier;

        Material material = new Material(GetComponent<MeshRenderer>().sharedMaterial);
        GetComponent<MeshRenderer>().sharedMaterial = material;
        transform.GetChild(0).localPosition = Vector3.up * (hexagonHeight + unitTextYOffset);

        transform.GetChild(1).gameObject.SetActive(true);
        GetComponentInChildren<BuildingRing>()
            .Init((1.5f + HexagonMap.GetHeightAt(building.location)) / transform.localScale.y, material);
    }

    public static Vector3 CalculateHexagonWorldPosition(Vector2Int position) {
        float innerRadius = Mathf.Sin(Mathf.Deg2Rad * 60);
        return new Vector3(2.0f * innerRadius * position.x + innerRadius * position.y, 0, 1.5f * position.y);
    }

}