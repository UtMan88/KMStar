using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonController : KMStarController
{

    protected override void OnGetPath(List<Waypoint> path)
    {
        
    }

    protected override void PreFindPath()
    {
        Waypoint randTarget = pathfinding.wayPoints[Random.Range(0, pathfinding.wayPoints.Length)];
        if (randTarget != null && randTarget != currentWaypoint)
        {
            targetWaypoint = randTarget;
        }
    
    }

    protected override void OnNextWaypoint()
    {

    }
}
