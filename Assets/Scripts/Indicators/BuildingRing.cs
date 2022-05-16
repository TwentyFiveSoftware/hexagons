using System.Collections.Generic;
using UnityEngine;

public class BuildingRing : MonoBehaviour {

    public float radius;
    public float width;

    public void Init(float height, Material material) {
        GetComponent<MeshFilter>().mesh = GenerateMesh(height, radius, width);
        GetComponent<MeshRenderer>().sharedMaterial = material;
        gameObject.SetActive(false);
    }

    private static Mesh GenerateMesh(float height, float radius, float width) {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();

        for (int i = 0; i <= 6; i++) {
            float rad = 2.0f * Mathf.PI * i / 6.0f;
            vertices.Add(new Vector3(Mathf.Cos(rad) * radius, height, Mathf.Sin(rad) * radius));
            vertices.Add(new Vector3(Mathf.Cos(rad) * (radius + width), height, Mathf.Sin(rad) * (radius + width)));

            if (i == 6) {
                break;
            }

            triangles.Add(i * 2);
            triangles.Add((i + 1) * 2);
            triangles.Add((i + 1) * 2 + 1);

            triangles.Add(i * 2);
            triangles.Add((i + 1) * 2 + 1);
            triangles.Add(i * 2 + 1);
        }

        Mesh mesh = new Mesh { vertices = vertices.ToArray(), triangles = triangles.ToArray() };
        mesh.RecalculateNormals();

        return mesh;
    }
}