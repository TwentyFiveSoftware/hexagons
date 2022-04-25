using UnityEngine;

public class HexagonGrid : MonoBehaviour {

    public Vector3Int size;
    public GameObject hexagonPrefab;

    private static readonly float INNER_RADIUS = Mathf.Sin(Mathf.Deg2Rad * 60);

    private void Start() {
        for (int x = 0; x < size.x; x++) {
            for (int z = 0; z < size.z; z++) {
                // Vector3 position = new Vector3(2.0f * INNER_RADIUS * x + INNER_RADIUS * z, 0, 1.5f * z);
                Vector3 position = new Vector3(2.0f * INNER_RADIUS * x + INNER_RADIUS * (z % 2), 0, 1.5f * z);
                GameObject hexagon = Instantiate(hexagonPrefab, position, Quaternion.Euler(Vector3.up * 90));
                hexagon.transform.SetParent(transform);
            }
        }
    }

}