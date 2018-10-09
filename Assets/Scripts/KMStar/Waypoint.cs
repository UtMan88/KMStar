using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    public Waypoint[] connections;
    public float heuristic = 0.0f;

#if UNITY_EDITOR
    public Color DebugColor = Color.grey;
#endif

    private void OnDrawGizmos()
    {
        Gizmos.color = DebugColor;
        Gizmos.DrawSphere(transform.position, 1f);
    }
}