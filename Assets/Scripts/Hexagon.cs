using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour {

    private void Start() {
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();

        // top
        vertices.Add(Vector3.up * 0.1f);
        for (int degree = 0; degree < 360; degree += 60) {
            vertices.Add(new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), 0.1f, Mathf.Sin(Mathf.Deg2Rad * degree)));
        }

        for (int i = 0; i < 6; i++) {
            triangles.AddRange(new[] { 0, (i + 1) % 6 + 1, i + 1 });
        }

        for (int i = 0; i < 7; i++) {
            normals.Add(Vector3.up);
        }

        int currentVertexIndexOffset = vertices.Count;

        // bottom
        vertices.Add(Vector3.up * -0.1f);
        for (int degree = 0; degree < 360; degree += 60) {
            vertices.Add(new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), -0.1f, Mathf.Sin(Mathf.Deg2Rad * degree)));
        }

        for (int i = 0; i < 6; i++) {
            triangles.AddRange(new[] {
                currentVertexIndexOffset, currentVertexIndexOffset + i + 1, currentVertexIndexOffset + (i + 1) % 6 + 1
            });
        }

        for (int i = 0; i < 7; i++) {
            normals.Add(Vector3.down);
        }

        currentVertexIndexOffset = vertices.Count;

        // sides
        for (int degree = 30; degree <= 360; degree += 60) {
            vertices.Add(new Vector3(Mathf.Cos(Mathf.Deg2Rad * (degree - 30)), 0.1f,
                Mathf.Sin(Mathf.Deg2Rad * (degree - 30))));
            vertices.Add(new Vector3(Mathf.Cos(Mathf.Deg2Rad * (degree + 30)), 0.1f,
                Mathf.Sin(Mathf.Deg2Rad * (degree + 30))));
            vertices.Add(new Vector3(Mathf.Cos(Mathf.Deg2Rad * (degree + 30)), -0.1f,
                Mathf.Sin(Mathf.Deg2Rad * (degree + 30))));
            vertices.Add(new Vector3(Mathf.Cos(Mathf.Deg2Rad * (degree - 30)), -0.1f,
                Mathf.Sin(Mathf.Deg2Rad * (degree - 30))));
        }

        for (int i = 0; i < 6; i++) {
            triangles.AddRange(new[] {
                currentVertexIndexOffset + i * 4, currentVertexIndexOffset + i * 4 + 1,
                currentVertexIndexOffset + i * 4 + 2, currentVertexIndexOffset + i * 4,
                currentVertexIndexOffset + i * 4 + 2, currentVertexIndexOffset + i * 4 + 3,
            });
        }

        for (int degree = 30; degree <= 360; degree += 60) {
            for (int i = 0; i < 4; i++) {
                normals.Add(new Vector3(Mathf.Cos(Mathf.Deg2Rad * degree), 0.0f, Mathf.Sin(Mathf.Deg2Rad * degree)));
            }
        }

        //
        Mesh mesh = new Mesh {
            vertices = vertices.ToArray(), triangles = triangles.ToArray(), normals = normals.ToArray()
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }

}