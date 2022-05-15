using System;
using System.Collections.Generic;
using UnityEngine;

public enum HexagonType {
    DEFAULT,
    BUILDING,
    PATH,
}

[Serializable]
public struct HexagonTypeData {
    public HexagonType type;
    public List<Material> materials;
}