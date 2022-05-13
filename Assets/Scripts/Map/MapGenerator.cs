using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class MapGenerator {

    public static List<Building> GenerateBuildings(int buildingsPerAxis, int baseDistance, float avgOffset) {
        Dictionary<Vector2Int, Dictionary<Vector2Int, Path>> buildingPositions =
            new Dictionary<Vector2Int, Dictionary<Vector2Int, Path>>();

        Vector2Int[,] grid = new Vector2Int[buildingsPerAxis, buildingsPerAxis];

        for (int x = 0; x < buildingsPerAxis; x++) {
            for (int z = 0; z < buildingsPerAxis; z++) {
                float radius = Random.Range(avgOffset / 2, avgOffset * 2);
                float degree = Random.Range(0, 2 * Mathf.PI);

                Vector2Int offset = new Vector2Int(Mathf.RoundToInt(Mathf.Cos(degree) * radius),
                    Mathf.RoundToInt(Mathf.Sin(degree) * radius));

                Vector2Int pos = new Vector2Int((x - z / 2), z) * baseDistance + offset;
                grid[x, z] = pos;
            }
        }

        for (int x = 0; x < buildingsPerAxis; x++) {
            for (int z = 0; z < buildingsPerAxis; z++) {
                List<Vector2Int> destinations = new List<Vector2Int>();

                for (int i = 0; i < Mathf.FloorToInt(Random.Range(0, 8)); i++) {
                    int xOffset = Mathf.RoundToInt(Random.Range(-1, 1));
                    int zOffset = Mathf.RoundToInt(Random.Range(-1, 1));

                    if (xOffset == 0 && zOffset == 0) {
                        continue;
                    }

                    if (x + xOffset < 0 || x + xOffset >= buildingsPerAxis || z + zOffset < 0 ||
                        z + zOffset >= buildingsPerAxis) {
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

                buildingPositions.TryAdd(grid[x, z], paths);
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

        return new Path { hexagons = path };
    }

}