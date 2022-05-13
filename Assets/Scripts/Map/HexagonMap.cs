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
        List<Path> paths = new List<Path>();

        foreach (Building building in buildings) {
            GenerateHexagonAt(building.location, HexagonType.BUILDING);

            foreach (Path pathToDestination in building.destinations.Values) {
                if (!paths.Contains(pathToDestination)) {
                    paths.Add(pathToDestination);
                }
            }
        }

        foreach (Path path in paths) {
            foreach (Vector2Int pathHexagon in path.hexagons) {
                GenerateHexagonAt(pathHexagon, HexagonType.PATH);
            }
        }
    }

    private void GenerateHexagonAt(Vector2Int position, HexagonType type) {
        GameObject hexagon = Instantiate(hexagonPrefab, Hexagon.CalculateHexagonWorldPosition(position),
            Quaternion.Euler(Vector3.up * 90));
        hexagon.transform.SetParent(transform);
        hexagon.GetComponent<Hexagon>().Init(1, type);
    }

}