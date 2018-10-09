using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Kind-of like a debug class, <c>KMStarPathDraw</c> will draw a path returned from <c>KMStar</c>
/// using a <c>LineRenderer</c>.
/// </summary>
public class KMStarPathDraw : MonoBehaviour {
    public LineRenderer line;

    [SerializeField]
    KMStar algorithm;
    [SerializeField]
    KMStar.HeuristicAlgorithm algorithmType = KMStar.HeuristicAlgorithm.Vector3Distance;
    [SerializeField]
    Waypoint start;
    [SerializeField]
    Waypoint end;

    private void Start()
    {
        if(algorithm != null && start != null && end != null)
        {
            StartCoroutine(algorithm.FindPath(start, end, (path) => DrawPath(path), algorithmType));
        }
    }

    /// <summary>
    /// This is the callback to FindPath that, once the goal is found, will be called so we can draw the path.
    /// </summary>
    /// <param name="path"></param>
    public void DrawPath(List<Waypoint> path)
    {
        line.positionCount = path.Count;
        Vector3[] pos = new Vector3[path.Count];
        for(int i = 0; i < path.Count; ++i)
        {
            pos[i] = path[i].transform.position;
        }
        line.SetPositions(pos);
    }

}
