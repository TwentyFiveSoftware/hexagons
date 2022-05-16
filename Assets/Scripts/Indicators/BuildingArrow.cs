using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class BuildingArrow {

    public static void ShowArrowAbove(GameObject obj) {
        float y = 0.2f * obj.GetComponent<Renderer>().bounds.size.y;

        float lineWidth = 0.4f;
        float arrowWidth = 0.5f;
        float arrowHeight = 1.3f;

        foreach (Vector3[] linePoints in new[] {
                     new[] { new Vector3(0, y + arrowHeight, 0), new Vector3(0, y + lineWidth * 0.5f, 0) },
                     new[] {
                         new Vector3(arrowWidth, y + arrowWidth, 0), new Vector3(0, y, 0),
                         new Vector3(-arrowWidth, y + arrowWidth, 0)
                     }
                 }) {
            GameObject lineObj = new GameObject();
            lineObj.transform.SetParent(obj.transform.GetChild(0), false);

            LineRenderer line = lineObj.AddComponent<LineRenderer>();
            line.startWidth = lineWidth;
            line.endWidth = lineWidth;
            line.useWorldSpace = false;
            line.generateLightingData = true;
            line.receiveShadows = false;
            line.shadowCastingMode = ShadowCastingMode.Off;
            line.sharedMaterial = obj.GetComponent<MeshRenderer>().sharedMaterial;
            line.positionCount = linePoints.Length;
            line.numCapVertices = 10;
            line.numCornerVertices = 10;
            line.SetPositions(linePoints);
        }
    }

}