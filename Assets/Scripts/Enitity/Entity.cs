using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public float speed;
    public float walkAnimationSpeed;
    public float walkAnimationY;
    public float maxUnitSize;
    public int maxDisplayedUnits;

    public GameObject unitPrefab;

    private List<Vector3> remainingPath;
    private Building destination;
    private int player;
    private int units;

    public void Init(Path path, int unitCount, int controllingPlayer, Building destinationBuilding) {
        remainingPath = new List<Vector3>();
        units = unitCount;
        destination = destinationBuilding;
        player = controllingPlayer;

        foreach (Vector2Int position in path.hexagons) {
            remainingPath.Add(GetWorldPositionWithHeight(position));
        }

        GenerateUnits(Mathf.Min(units, maxDisplayedUnits), controllingPlayer);
    }

    private void GenerateUnits(int unitCount, int controllingPlayer) {
        GameObject temp = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity, transform);
        Material material = new Material(temp.GetComponent<MeshRenderer>().sharedMaterial) {
            color = GameController.instance.playerColors[controllingPlayer]
        };
        Destroy(temp);


        for (int i = 0; i < unitCount; ++i) {
            float size = maxUnitSize * Mathf.Pow(2, -Random.Range(0.0f, 2.0f));
            Vector3 position = new Vector3(Random.Range(-0.5f, 0.5f), size / 2.0f, Random.Range(-0.5f, 0.5f));

            GameObject obj = Instantiate(unitPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.transform.localPosition = position;
            obj.transform.localScale = Vector3.one * size;

            obj.GetComponent<MeshRenderer>().sharedMaterial = material;
        }
    }

    public static Vector3 GetWorldPositionWithHeight(Vector2Int position) {
        Vector3 worldPos = Hexagon.CalculateHexagonWorldPosition(position);
        worldPos.y = 1.0f + HexagonMap.GetHeightAt(position);
        return worldPos;
    }

    private void Update() {
        if (remainingPath.Count == 0) {
            destination.UnitsArrive(units, player);
            Destroy(gameObject);
            return;
        }

        Vector3 delta = remainingPath[0] - transform.position;

        if (delta.magnitude < 0.2f) {
            remainingPath.RemoveAt(0);
        }

        transform.Translate(delta.normalized * (Time.deltaTime * speed));

        for (int i = 0; i < transform.childCount; ++i) {
            float sinOffset = Time.time * walkAnimationSpeed + 2.0f * i * Mathf.PI / transform.childCount * i;
            float yDelta = (1.0f + Mathf.Sin(sinOffset)) * 0.5f * Mathf.PerlinNoise(i * 0.2f, 0) * walkAnimationY;
            Transform unit = transform.GetChild(i);
            unit.localPosition =
                new Vector3(unit.localPosition.x, unit.localScale.y * 0.5f + yDelta, unit.localPosition.z);
        }
    }

}