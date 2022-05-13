using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexagonGrid : MonoBehaviour {

    public Vector3Int size;
    public GameObject hexagonPrefab;
    public Vector3 heightMapScale;

    private static readonly float INNER_RADIUS = Mathf.Sin(Mathf.Deg2Rad * 60);

    private void Start() {
        GenerateGrid();
    }

    public void GenerateGrid() {
        for (int i = transform.childCount - 1; i >= 0; --i) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

        List<Building> buildings = CalculateBuildingLocations(4, 10, 1);
        GenerateMap(buildings);
    }

    private void GenerateHexagonAt(Vector2Int pos, HexagonType type) {
        Vector3 position = new Vector3(2.0f * INNER_RADIUS * pos.x + INNER_RADIUS * pos.y, 0, 1.5f * pos.y);
        GameObject hexagon = Instantiate(hexagonPrefab, position, Quaternion.Euler(Vector3.up * 90));
        hexagon.transform.SetParent(transform);
        hexagon.GetComponent<Hexagon>().Init(1, type, pos);
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

    private Path CalculatePathBetweenHexagons(Vector2Int a, Vector2Int b) {
        List<Vector2Int> sideOffsets = new List<Vector2Int> {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
        };

        List<Vector2Int> path = new List<Vector2Int> { a };
        Vector3 targetWorldPos = CalculateHexagonWorldPosition(b.x, b.y);

        while (true) {
            if (path[^1] == b) {
                break;
            }

            Vector2Int bestNextHexagon = path[^1];
            float bestDistance = Single.PositiveInfinity;

            foreach (Vector2Int side in sideOffsets) {
                Vector2Int currentHexagon = path[^1];
                Vector3 pos = CalculateHexagonWorldPosition(currentHexagon.x + side.x, currentHexagon.y + side.y);

                float distance = (targetWorldPos - pos).sqrMagnitude;
                if (distance < bestDistance) {
                    bestDistance = distance;
                    bestNextHexagon = currentHexagon + side;
                }
            }

            path.Add(bestNextHexagon);
        }

        path.RemoveAt(0);
        path.RemoveAt(path.Count - 1);

        return new Path { hexagons = path };
    }

    private Vector3 CalculateHexagonWorldPosition(int x, int z) {
        return new Vector3(2.0f * INNER_RADIUS * x + INNER_RADIUS * z, 0, 1.5f * z);
    }

    private List<Building> CalculateBuildingLocations(int a, int baseDistance, float avgOffset) {
        Dictionary<Vector2Int, Dictionary<Vector2Int, Path>> buildingPositions =
            new Dictionary<Vector2Int, Dictionary<Vector2Int, Path>>();

        Vector2Int[,] grid = new Vector2Int[a, a];

        for (int x = 0; x < a; x++) {
            for (int z = 0; z < a; z++) {
                float radius = Random.Range(avgOffset / 2, avgOffset * 2);
                float degree = Random.Range(0, 2 * Mathf.PI);

                Vector2Int offset = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(degree) * radius),
                    Mathf.RoundToInt(Mathf.Sin(degree) * radius));

                Vector2Int pos = new Vector2Int((x - z / 2), z) * baseDistance + offset;
                grid[x, z] = pos;
            }
        }

        for (int x = 0; x < a; x++) {
            for (int z = 0; z < a; z++) {
                List<Vector2Int> destinations = new List<Vector2Int>();

                for (int i = 0; i < Mathf.FloorToInt(Random.Range(0, 8)); i++) {
                    int xOffset = Mathf.RoundToInt(Random.Range(-1, 1));
                    int zOffset = Mathf.RoundToInt(Random.Range(-1, 1));

                    if (xOffset == 0 && zOffset == 0) {
                        continue;
                    }

                    if (x + xOffset < 0 || x + xOffset >= a || z + zOffset < 0 || z + zOffset >= a) {
                        continue;
                    }

                    Vector2Int destination = new Vector2Int(x + xOffset, z + zOffset);
                    if (!destinations.Contains(destination)) {
                        destinations.Add(destination);
                    }
                }

                Dictionary<Vector2Int, Path> paths = new Dictionary<Vector2Int, Path>();

                foreach (Vector2Int destination in destinations) {
                    paths.Add(grid[destination.x, destination.y],
                        CalculatePathBetweenHexagons(grid[x, z], grid[destination.x, destination.y]));
                }

                buildingPositions.Add(grid[x, z], paths);
            }
        }


        List<Building> buildings = new List<Building>();

        foreach (Vector2Int location in buildingPositions.Keys) {
            buildings.Add(new Building { location = location, destinations = new Dictionary<Building, Path>() });
        }

        foreach (Building building in buildings) {
            foreach (Vector2Int destination in buildingPositions[building.location].Keys) {
                if (buildings.Exists(b => b.location == destination)) {
                    Building destBuilding = buildings.Find(b => b.location == destination);
                    Path path = buildingPositions[building.location][destination];

                    building.destinations.Add(destBuilding, path);
                    destBuilding.destinations.Add(building, path);
                }
            }
        }

        for (int i = buildings.Count - 1; i >= 0; i--) {
            if (buildings[i].destinations.Count == 0) {
                buildings.RemoveAt(i);
            }
        }

        return buildings;
    }

    class Path {
        public List<Vector2Int> hexagons;
    }

    class Building {
        public Vector2Int location;
        public Dictionary<Building, Path> destinations;
    }

}