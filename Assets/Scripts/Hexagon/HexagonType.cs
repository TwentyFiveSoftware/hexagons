using System;
using UnityEngine;

public enum HexagonType {
    DEFAULT,
    BUILDING,
    PATH,
}

[Serializable]
public struct HexagonTypeData {
    public HexagonType type;
    public Material material;
}