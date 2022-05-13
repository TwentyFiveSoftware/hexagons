using System.Collections.Generic;
using UnityEngine;

public class HexagonMap : MonoBehaviour {

    public GameObject hexagonPrefab;
    public Vector3 heightMapScale;
    public int buildingsPerAxis;
    public int buildingsBaseDistance;
    public float buildingsAvgOffset;

    private void Start() {
        Generate();
    }

    public void Generate() {
        for (int i = transform.childCount - 1; i >= 0; --i) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        List<Building> buildings =
            MapGenerator.GenerateBuildings(buildingsPerAxis, buildingsBaseDistance, buildingsAvgOffset);
        GenerateMap(buildings);
    }

    private void GenerateMap(List<Building> buildings) {
        List<Vector2Int> generatedHexagonPositions = new List<Vector2Int>();
        List<Vector2Int> mapOutline = new List<Vector2Int>();

        List<Path> paths = new List<Path>();

        foreach (Building building in buildings) {
            float height = 3.0f + GetHeightAt(building.location);
            GenerateHexagonAt(building.location, HexagonType.BUILDING, height);
            generatedHexagonPositions.Add(building.location);
            mapOutline.Add(building.location);

            foreach (Path pathToDestination in building.destinations.Values) {
                if (!paths.Contains(pathToDestination)) {
                    paths.Add(pathToDestination);
                }
            }
        }

        foreach (Path path in paths) {
            for (int i = 0; i < path.hexagons.Count; i++) {
                Vector2Int pathHexagon = path.hexagons[i];
                float height = 1.0f + GetHeightAt(pathHexagon);
                GenerateHexagonAt(pathHexagon, HexagonType.PATH, height);
                generatedHexagonPositions.Add(pathHexagon);

                if (i % 5 == 0) {
                    mapOutline.Add(pathHexagon);
                }
            }
        }

        foreach (Vector2Int outlinePoint in mapOutline) {
            Vector3 pointWorldPosition = Hexagon.CalculateHexagonWorldPosition(outlinePoint);

            for (int x = -buildingsBaseDistance; x < buildingsBaseDistance; x++) {
                for (int z = -buildingsBaseDistance; z < buildingsBaseDistance; z++) {
                    Vector2Int position = outlinePoint + new Vector2Int(x - z / 2, z);

                    if (generatedHexagonPositions.Contains(position)) {
                        continue;
                    }

                    float distance = (Hexagon.CalculateHexagonWorldPosition(position) - pointWorldPosition).magnitude;

                    // if (distance > buildingsBaseDistance * 1.05) {
                    if (distance > buildingsBaseDistance * 1.1) {
                        continue;
                    }

                    generatedHexagonPositions.Add(position);

                    float height = 0.5f + GetHeightAt(position);
                    GenerateHexagonAt(position, HexagonType.DEFAULT, height);
                }
            }
        }
    }

    private float GetHeightAt(Vector2Int position) {
        return heightMapScale.y * Mathf.PerlinNoise(position.x * heightMapScale.x, position.y * heightMapScale.z);
    }

    private void GenerateHexagonAt(Vector2Int position, HexagonType type, float height) {
        GameObject hexagon = Instantiate(hexagonPrefab, Hexagon.CalculateHexagonWorldPosition(position),
            Quaternion.Euler(Vector3.up * 90));
        hexagon.transform.SetParent(transform);
        hexagon.GetComponent<Hexagon>().Init(height, type);
    }

}