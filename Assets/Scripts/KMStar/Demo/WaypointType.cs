using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointType : MonoBehaviour {
    [System.Serializable]
    public enum WPType
    {
        Null = 0,
        Grass,
        Water,
        Sand,
        Snow,
        Mountain,
        Bridge,
        Town
    }

    [SerializeField]
    private WPType value = WPType.Null;

    public WPType Value
    {
        get { return value; }
    }
}
