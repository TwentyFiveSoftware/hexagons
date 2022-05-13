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

        List<Vector2Int> buildings = CalculateBuildingLocations(4, 10, 1);
    }

    private void GenerateHexagonAt(int x, int z) {
        Vector3 position = new Vector3(2.0f * INNER_RADIUS * x + INNER_RADIUS * z, 0, 1.5f * z);
        GameObject hexagon = Instantiate(hexagonPrefab, position, Quaternion.Euler(Vector3.up * 90));
        hexagon.transform.SetParent(transform);
        hexagon.GetComponent<Hexagon>().Init(1, new Vector2Int(x, z));
    }

    private List<Vector2Int> CalculatePathBetweenHexagons(Vector2Int a, Vector2Int b) {
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

        return path;
    }

    private Vector3 CalculateHexagonWorldPosition(int x, int z) {
        return new Vector3(2.0f * INNER_RADIUS * x + INNER_RADIUS * z, 0, 1.5f * z);
    }

    private List<Vector2Int> CalculateBuildingLocations(int a, int baseDistance, float avgOffset) {
        List<Vector2Int> buildings = new List<Vector2Int>();

        Vector2Int[,] grid = new Vector2Int[a, a];

        for (int x = 0; x < a; x++) {
            for (int z = 0; z < a; z++) {
                float radius = Random.Range(avgOffset / 2, avgOffset * 2);
                float degree = Random.Range(0, 2 * Mathf.PI);

                Vector2Int offset = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(degree) * radius),
                    Mathf.RoundToInt(Mathf.Sin(degree) * radius));

                Vector2Int pos = new Vector2Int((x - z / 2), z) * baseDistance + offset;
                grid[x, z] = pos;
                // GenerateHexagonAt(pos.x, pos.y);
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

                if (destinations.Count == 0) {
                    continue;
                }

                foreach (Vector2Int destination in destinations) {
                    List<Vector2Int> path =
                        CalculatePathBetweenHexagons(grid[x, z], grid[destination.x, destination.y]);
                    foreach (Vector2Int hexagon in path) {
                        GenerateHexagonAt(hexagon.x, hexagon.y);
                    }
                }

                buildings.Add(grid[x, z]);
            }
        }

        return buildings;
    }

}