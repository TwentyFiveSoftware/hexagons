using System;
using UnityEngine;

public enum HexagonType {
    BUILDING,
    PATH
}

[Serializable]
public struct HexagonTypeData {
    public HexagonType type;
    public Material material;
}