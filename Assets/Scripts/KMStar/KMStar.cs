using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KMStar : MonoBehaviour {

    /// <summary>
    /// Different algorithms to use for calculating the heuristic between nodes.
    /// <para><see cref="HeuristicAlgorithm.Vector3Distance"/> - The cost of a straight line from <c>w1</c> to <c>w2</c></para>
    /// <para><see cref="HeuristicAlgorithm.Manhattan"/> - Best algorithm for travelling along a 4-directional grid (North, South, etc.)</para>
    /// <para><see cref="HeuristicAlgorithm.CostOfMovement"/> - A little amalgamation I made up - takes the largest value of subtracting the 2 Waypoint's heuristic values and adds that to the <c>Vector3Distance</c> algorithm</para>
    /// <para><see cref="HeuristicAlgorithm.InverseCOM"/> - </para>
    /// <para><see cref="HeuristicAlgorithm.DiagonalDistanceUniform"/> - </para>
    /// <para><see cref="HeuristicAlgorithm.DiagonalDistance"/> - </para>
    /// <para><see cref="HeuristicAlgorithm.HScore"/> - Simply just the Heuristic of <c>w1</c></para>
    /// <para><see cref="HeuristicAlgorithm.Incremental"/> - </para>
    /// <para><see cref="HeuristicAlgorithm.Drunk"/> - Go home, KMSTar...</para>
    /// </summary>
    public enum HeuristicAlgorithm
    {
        Vector3Distance,
        Manhattan,
        CostOfMovement,
        InverseCOM,
        DiagonalDistanceUniform,
        DiagonalDistance,
        HScore,
        Incremental,
        Drunk
    }

    private class Node
    {
        public Waypoint waypoint;
        public Node from;
        public float score;

        public Node(Waypoint w, Node n, float s)
        {
            waypoint = w;
            from = n;
            score = s;
        }

        public override string ToString()
        {
            return string.Format("Node({0},{1})", waypoint.gameObject.name, score);
        }

        public string DebugPath()
        {
            return ToString() + " <- " + (from != null ? from.DebugPath() : "START");
        }
    }

    public Waypoint[] wayPoints;

    /// <summary>
    /// 
    /// </summary>
    private void Awake()
    {
        wayPoints = GetComponentsInChildren<Waypoint>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="pathCallback"></param>
    /// <param name="heuristicAlgorithm">See <see cref="HeuristicAlgorithm"/> for more information.</param>
    /// <param name="ignoreWaypointHeuristic"></param>
    /// <returns></returns>
    public IEnumerator FindPath(Waypoint start, 
                                Waypoint end, 
                                System.Action< List<Waypoint> > pathCallback, 
                                HeuristicAlgorithm heuristicAlgorithm = HeuristicAlgorithm.Vector3Distance, 
                                bool ignoreWaypointHeuristic = false)
    {
        // Return Value (in Node form)
        List<Node> path = new List<Node>();
        // Open Waypoints
        List<Node> open = new List<Node>();
        // Closed Waypoints
        Queue<Waypoint> closed = new Queue<Waypoint>();
        // Start with adding our start waypoint to the list of open waypoints
        open.Add(new Node(start, null, CalculateHeuristic(heuristicAlgorithm, null, start, end, ignoreWaypointHeuristic)));

        while(open.Count > 0)
        {
            // Step 1: Get the next waypoint to process, based on the heuristic
            Node nextWaypoint = null;
            // Our psuedo-Priority Queue Linq Query will order everything in the open list by score.
            // Meaning the top is the best score. Meaning no loop required to search.
            IEnumerable<Node> query = open.OrderBy(node => node.score);
            nextWaypoint = query.First();
            if(nextWaypoint.waypoint == end)
            {
                path.Add(nextWaypoint);
                break;
            }
            open.Remove(nextWaypoint);
            if (nextWaypoint == null)
            {
                yield return null;
                continue;
            }
            closed.Enqueue(nextWaypoint.waypoint);

            // Step 2: Add the connections that haven't been visited to the open queue
            foreach(Waypoint c in nextWaypoint.waypoint.connections)
            {
                if(closed.Contains(c) == false)
                {
                    open.Add(new Node(c, nextWaypoint, CalculateHeuristic(heuristicAlgorithm, nextWaypoint, c, end, ignoreWaypointHeuristic)));
                }
            }
            yield return null;
        }
        pathCallback(BuildFinalPath(path));
    }

    /// <summary>
    /// Calculates the Heuristic Score based on several different Algorithms.
    /// </summary>
    /// <param name="algorithm">See <see cref="HeuristicAlgorithm"/> for more information.</param>
    /// <param name="from">The Node we're coming from. Can be null if this is the start.</param>
    /// <param name="w1">First waypoint (typically "start" or your Current)</param>
    /// <param name="w2">Second waypoint (the "goal" or "target")</param>
    /// <returns></returns>
    // References:
    // - https://brilliant.org/wiki/a-star-search/
    // - http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
    // - https://www.growingwiththeweb.com/2012/06/a-pathfinding-algorithm.html
    private float CalculateHeuristic(HeuristicAlgorithm algorithm, Node from, Waypoint w1, Waypoint w2, bool ignoreWaypointHeuristic = false)
    {
        float cost = 0, max = 0, min = 0, h1 = 0, h2 = 0;
        switch (algorithm)
        {
            case HeuristicAlgorithm.Vector3Distance:
                return Vector3.Distance(w1.transform.position, w2.transform.position) + HScore(w1, ignoreWaypointHeuristic);
            case HeuristicAlgorithm.Manhattan:
                return (Mathf.Abs(w1.transform.position.x - w2.transform.position.x) +
                        Mathf.Abs(w1.transform.position.y - w2.transform.position.y) +
                        Mathf.Abs(w1.transform.position.z - w2.transform.position.z)) 
                        + HScore(w1, ignoreWaypointHeuristic);
            case HeuristicAlgorithm.CostOfMovement:
                h1 = HScore(w1, ignoreWaypointHeuristic);
                h2 = HScore(w2, ignoreWaypointHeuristic);
                cost = Mathf.Max(Mathf.Abs(h2 - h1),
                                 Mathf.Abs(h1 - h2));
                return Vector3.Distance(w1.transform.position, w2.transform.position) + cost;
            case HeuristicAlgorithm.InverseCOM:
                h1 = HScore(w1, ignoreWaypointHeuristic) * -1;
                h2 = HScore(w2, ignoreWaypointHeuristic) * -1;
                cost = Mathf.Min(h1 - h2, h2 - h1);
                return Vector3.Distance(w1.transform.position, w2.transform.position) + cost;
            case HeuristicAlgorithm.DiagonalDistanceUniform:
                max = Mathf.Max(
                    Mathf.Max(Mathf.Abs(w1.transform.position.x - w2.transform.position.x),
                    Mathf.Abs(w1.transform.position.y - w2.transform.position.y)),
                    Mathf.Abs(w1.transform.position.z - w2.transform.position.z));
                return HScore(w1, ignoreWaypointHeuristic) * max;
            case HeuristicAlgorithm.DiagonalDistance:
                max = Mathf.Max(
                    Mathf.Max(Mathf.Abs(w1.transform.position.x - w2.transform.position.x),
                    Mathf.Abs(w1.transform.position.y - w2.transform.position.y)),
                    Mathf.Abs(w1.transform.position.z - w2.transform.position.z));
                min = Mathf.Min(
                    Mathf.Min(Mathf.Abs(w1.transform.position.x - w2.transform.position.x),
                    Mathf.Abs(w1.transform.position.y - w2.transform.position.y)),
                    Mathf.Abs(w1.transform.position.z - w2.transform.position.z));
                return (HScore(w1, ignoreWaypointHeuristic) * 1.414f) * (max - min);
            case HeuristicAlgorithm.HScore:
                return HScore(w1, ignoreWaypointHeuristic);
            case HeuristicAlgorithm.Incremental:
                return (from != null ? from.score : 0) + HScore(w1, ignoreWaypointHeuristic);
            case HeuristicAlgorithm.Drunk:
                h1 = HScore(w1, ignoreWaypointHeuristic);
                h2 = HScore(w2, ignoreWaypointHeuristic);
                return Random.Range(Mathf.Min(h1, h2), Mathf.Max(h1, h2));
        }
        return -1; // what!?
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="w"></param>
    /// <param name="ignoreWaypointHeuristic"></param>
    /// <returns></returns>
    private float HScore(Waypoint w, bool ignoreWaypointHeuristic = false)
    {
        return (ignoreWaypointHeuristic ? 0 : w.heuristic);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="nodes"></param>
    /// <returns></returns>
    private List<Waypoint> BuildFinalPath(List<Node> nodes)
    {
        List<Waypoint> path = new List<Waypoint>();
        Node n = nodes[nodes.Count - 1];
        //Debug.Log(n.DebugPath());
        while(n != null)
        {
            //Debug.Log("Adding " + n + " to the path!");
            path.Add(n.waypoint);
            n = n.from;
        }
        path.Reverse();
        return path;
    }
}
