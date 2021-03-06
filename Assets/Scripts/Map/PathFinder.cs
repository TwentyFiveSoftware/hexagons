using System.Collections.Generic;
using UnityEngine;

public static class PathFinder {

    public static Path FindPath(Building buildingFrom, Building buildingTo) {
        List<Building> buildingPath = FindBuildingPath(buildingFrom, buildingTo);

        if (buildingPath == null) {
            return null;
        }

        Path path = new Path { hexagons = new List<Vector2Int>() };

        for (int i = 0; i < buildingPath.Count - 1; i++) {
            Building building = buildingPath[i];
            path.hexagons.Add(building.location);

            Path pathToNextBuilding = building.destinations[buildingPath[i + 1]];
            List<Vector2Int> hexagons = new List<Vector2Int>(pathToNextBuilding.hexagons);
            if (pathToNextBuilding.b == building.location) {
                hexagons.Reverse();
            }

            path.hexagons.AddRange(hexagons);
        }

        path.hexagons.Add(buildingPath[^1].location);

        return path;
    }

    private static List<Building> FindBuildingPath(Building buildingFrom, Building buildingTo) {
        List<List<Building>> queue = new List<List<Building>> { new() { buildingFrom } };

        while (queue.Count > 0) {
            List<Building> currentPath = queue[0];
            queue.RemoveAt(0);

            if (currentPath[^1] == buildingTo) {
                return currentPath;
            }

            foreach (Building adjacent in currentPath[^1].destinations.Keys) {
                queue.Add(new List<Building>(currentPath) { adjacent });
            }
        }

        return null;
    }
}