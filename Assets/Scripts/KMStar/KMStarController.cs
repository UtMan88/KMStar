using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple controller set up to move along a Path returned from <c>KMStar</c>
/// </summary>
public class KMStarController : MonoBehaviour {

    public KMStar pathfinding;
    public KMStar.HeuristicAlgorithm pathFindType = KMStar.HeuristicAlgorithm.Vector3Distance;
    public bool ignoreWaypointHeuristic = false;
    public float recalculateDelay = 5.0f;
    public float moveSpeed = 10.0f;
    public Waypoint currentWaypoint;
    public Waypoint previousWaypoint;
    public Waypoint targetWaypoint;
    private float currentDelay = 0.0f;
    private bool isMoving = false;
    private bool isCalculating = true;

    private List<Waypoint> currentPath;

	// Use this for initialization
	void Start () {
        RecalculatePath();
    }
	
	// Update is called once per frame
	void Update () {
        if (currentPath == null)
            return;
        if (isCalculating == false)
        {
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.transform.position, moveSpeed * MoveSpeedModifier() * Time.deltaTime);
            if (Vector3.Distance(transform.position, targetWaypoint.transform.position) > Mathf.Epsilon)
            {
                if (Vector3.Distance(transform.position, currentWaypoint.transform.position) <= Mathf.Epsilon)
                {
                    isMoving = false;
                    if (currentPath.Count > 1)
                    {
                        currentPath.RemoveAt(0);
                        previousWaypoint = currentWaypoint;
                        currentWaypoint = currentPath[0];
                        OnNextWaypoint();
                    }
                    else
                    {
                        RecalculatePath();
                        return;
                    }
                }
                else
                {
                    isMoving = true;
                }
            }
            else
            {
                isMoving = false;
            }
        }
        currentDelay += Time.deltaTime;

        if (isCalculating == false && isMoving == false && currentDelay >= recalculateDelay)
        {
            RecalculatePath();
        }
		
	}

    /// <summary>
    /// Gets called when we reach our <c>recalculateDelay</c>, or when we reach the <c>targetWaypoint</c>
    /// </summary>
    public void RecalculatePath()
    {
        isCalculating = true;
        currentDelay = 0.0f;
        PreFindPath();
        StartCoroutine(pathfinding.FindPath(currentWaypoint, targetWaypoint, (path) => GetPath(path), pathFindType, ignoreWaypointHeuristic));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    void GetPath(List<Waypoint> path)
    {
        OnGetPath(path);
        currentPath = path;
        if (currentPath.Count > 1)
        {
            currentWaypoint = currentPath[0];
        }
        isCalculating = false;
    }

    /// <summary>
    /// In the member definitions for this class, we have <c>moveSpeed</c>, and that acts as our base movement speed. 
    /// When we traverse over certain <c>Waypoints</c>, we want to change the speed based on the "terrain" we're crossing.
    /// </summary>
    /// <returns></returns>
    protected virtual float MoveSpeedModifier()
    {
        return 1f;
    }

    /// <summary>
    /// This is not our callback to <c>KMStar.FindPath</c>. This is just an overridable function that gets called inside of the callback, 
    /// so that if our object wanted to check/modify the path before it gets used, we have the opportunity to do so.
    /// </summary>
    /// <param name="path"></param>
    protected virtual void OnGetPath(List<Waypoint> path)
    {

    }

    /// <summary>
    /// This gets called when our KMStarController reaches a Waypoint, and is about to start heading to the next.
    /// </summary>
    protected virtual void OnNextWaypoint()
    {

    }

    /// <summary>
    /// Called before the object begins looking for a new path to the target.
    /// </summary>
    protected virtual void PreFindPath()
    {

    }
}
