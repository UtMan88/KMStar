using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Waypoint))]
[CanEditMultipleObjects]
public class WaypointEditor : Editor
{
    float size = 3f;

    protected virtual void OnSceneGUI()
    {
        Handles.color = ((Waypoint)target).DebugColor;
        Transform transform = ((Waypoint)target).transform;
        foreach (Waypoint wp in ((Waypoint)target).connections)
        {
            if (wp == null)
                continue;
            Handles.ArrowHandleCap(
                0,
                transform.position,
                Quaternion.LookRotation(wp.transform.position - transform.position),
                size,
                EventType.Repaint
                );
        }
    }

    private void DebugDraw()
    {
        if (Event.current.type == EventType.Repaint)
        {
            Transform transform = ((Waypoint)target).transform;
            Handles.color = Handles.xAxisColor;
            Handles.ArrowHandleCap(
                0,
                transform.position + new Vector3(3f, 0f, 0f),
                transform.rotation * Quaternion.LookRotation(Vector3.right),
                size,
                EventType.Repaint
                );
            Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(
                0,
                transform.position + new Vector3(0f, 3f, 0f),
                transform.rotation * Quaternion.LookRotation(Vector3.up),
                size,
                EventType.Repaint
                );
            Handles.color = Handles.zAxisColor;
            Handles.ArrowHandleCap(
                0,
                transform.position + new Vector3(0f, 0f, 3f),
                transform.rotation * Quaternion.LookRotation(Vector3.forward),
                size,
                EventType.Repaint
                );
        }
    }
}