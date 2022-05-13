using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {

    public float speed;
    public float walkAnimationSpeed;
    public float walkAnimationY;

    private List<Vector3> remainingPath;

    public void Init(Path path) {
        remainingPath = new List<Vector3>();

        foreach (Vector2Int position in path.hexagons) {
            remainingPath.Add(GetWorldPositionWithHeight(position));
        }
    }

    public static Vector3 GetWorldPositionWithHeight(Vector2Int position) {
        Vector3 worldPos = Hexagon.CalculateHexagonWorldPosition(position);
        worldPos.y = 1.0f + HexagonMap.GetHeightAt(position);
        return worldPos;
    }

    private void Update() {
        if (remainingPath.Count == 0) {
            Destroy(gameObject);
            return;
        }

        Vector3 delta = remainingPath[0] - transform.position;

        if (delta.magnitude < 0.2f) {
            remainingPath.RemoveAt(0);
        }

        transform.Translate(delta.normalized * (Time.deltaTime * speed));

        transform.GetChild(0).localPosition =
            Vector3.up * ((Mathf.Sin(Time.time * walkAnimationSpeed) + 1) * walkAnimationY);
    }

}