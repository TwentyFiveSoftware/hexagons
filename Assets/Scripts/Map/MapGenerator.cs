using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MapGenerator {

    public static List<Building> GenerateBuildings(int buildingsPerAxis, int baseDistance, float avgOffset) {
        List<Building> buildings = new List<Building>();
        Building[,] grid = new Building[buildingsPerAxis, buildingsPerAxis];

        for (int x = 0; x < buildingsPerAxis; x++) {
            for (int z = 0; z < buildingsPerAxis; z++) {
                float radius = Random.Range(avgOffset / 2, avgOffset * 2);
                float degree = Random.Range(0, 2 * Mathf.PI);

                Vector2Int offset = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(degree) * radius),
                    Mathf.RoundToInt(Mathf.Sin(degree) * radius));

                Vector2Int pos = new Vector2Int(x - z / 2, z) * baseDistance + offset;
                Building building = new Building {
                    location = pos,
                    gridCoordinates = new Vector2Int(x, z),
                    destinations = new Dictionary<Building, Path>()
                };
                buildings.Add(building);
                grid[x, z] = building;
            }
        }


        GeneratePaths(grid, grid[buildingsPerAxis / 2, buildingsPerAxis / 2], buildingsPerAxis / 2 + 1);

        for (int i = buildings.Count - 1; i >= 0; i--) {
            if (buildings[i].destinations.Count == 0) {
                buildings.RemoveAt(i);
            }
        }

        return buildings;
    }

    private static void GeneratePaths(Building[,] grid, Building building, int maxPathLength) {
        List<Building> reachableBuildings = new List<Building>();

        foreach (Vector2Int offset in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right }) {
            if (building.gridCoordinates.x + offset.x < 0 ||
                building.gridCoordinates.x + offset.x >= grid.GetLength(0) ||
                building.gridCoordinates.y + offset.y < 0 ||
                building.gridCoordinates.y + offset.y >= grid.GetLength(1)) {
                continue;
            }

            Building destination = grid[building.gridCoordinates.x + offset.x, building.gridCoordinates.y + offset.y];
            if (!reachableBuildings.Contains(destination) && !building.destinations.ContainsKey(destination)) {
                reachableBuildings.Add(destination);
            }
        }

        if (reachableBuildings.Count == 0 || maxPathLength <= 0) {
            return;
        }

        List<Building> destinations = new List<Building>();

        while (reachableBuildings.Count > 0) {
            Building destination = reachableBuildings[Random.Range(0, reachableBuildings.Count - 1)];
            reachableBuildings.Remove(destination);
            destinations.Add(destination);

            if (building.destinations.ContainsKey(destination)) {
                continue;
            }

            Path path = CalculatePathBetweenHexagons(building.location, destination.location);
            building.destinations.Add(destination, path);
            destination.destinations.Add(building, path);

            if (Random.value < 1.0f / maxPathLength) {
                break;
            }
        }

        foreach (Building destination in destinations) {
            GeneratePaths(grid, destination, maxPathLength - 1);
        }
    }

    private static Path CalculatePathBetweenHexagons(Vector2Int a, Vector2Int b) {
        List<Vector2Int> sideOffsets = new List<Vector2Int> {
            new(1, 0),
            new(-1, 0),
            new(0, 1),
            new(-1, 1),
            new(1, -1),
            new(0, -1),
        };

        List<Vector2Int> path = new List<Vector2Int> { a };
        Vector3 targetWorldPos = Hexagon.CalculateHexagonWorldPosition(b);

        while (true) {
            if (path[^1] == b) {
                break;
            }

            Vector2Int bestNextHexagon = path[^1];
            float bestDistance = Single.PositiveInfinity;

            foreach (Vector2Int side in sideOffsets) {
                Vector2Int currentHexagon = path[^1];
                Vector3 pos = Hexagon.CalculateHexagonWorldPosition(currentHexagon + side);

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

        return new Path { hexagons = path, a = a, b = b };
    }

}