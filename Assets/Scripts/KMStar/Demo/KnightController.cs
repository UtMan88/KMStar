using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : KMStarController {

    [System.Serializable]
    public class WayPointSpeedModifiers
    {
        public float NullMod = 1.0f;
        public float GrassMod = 1.0f;
        public float WaterMod = 1.0f;
        public float SandMod = 1.0f;
        public float SnowMod = 1.0f;
        public float MountainMod = 1.0f;
        public float BridgeMod = 1.0f;
        public float TownMod = 1.0f;
    }

    public KMStarController dragon;
    public float speedMod = 1.0f;
    public WayPointSpeedModifiers FromModifiers;
    public WayPointSpeedModifiers ToModifiers;

    protected override void OnGetPath(List<Waypoint> path)
    {
        base.OnGetPath(path);
    }

    protected override void OnNextWaypoint()
    {
        if(previousWaypoint != null && currentWaypoint != null)
        {
            WaypointType wptypeFrom = previousWaypoint.GetComponent<WaypointType>();
            WaypointType wptypeTo = currentWaypoint.GetComponent<WaypointType>();
            if (wptypeFrom != null)
            {
                switch (wptypeFrom.Value)
                {
                    case WaypointType.WPType.Null:
                        speedMod = FromModifiers.NullMod;
                        break;
                    case WaypointType.WPType.Grass:
                        speedMod = FromModifiers.GrassMod;
                        break;
                    case WaypointType.WPType.Water:
                        speedMod = FromModifiers.WaterMod;
                        break;
                    case WaypointType.WPType.Sand:
                        speedMod = FromModifiers.SandMod;
                        break;
                    case WaypointType.WPType.Snow:
                        speedMod = FromModifiers.SnowMod;
                        break;
                    case WaypointType.WPType.Mountain:
                        speedMod = FromModifiers.MountainMod;
                        break;
                    case WaypointType.WPType.Bridge:
                        speedMod = FromModifiers.BridgeMod;
                        break;
                    case WaypointType.WPType.Town:
                        speedMod = FromModifiers.TownMod;
                        break;
                }
            }
            else
            {
                speedMod = 1.0f;
            }
            if (wptypeTo != null)
            {
                switch (wptypeTo.Value)
                {
                    case WaypointType.WPType.Null:
                        speedMod *= ToModifiers.NullMod;
                        break;
                    case WaypointType.WPType.Grass:
                        speedMod *= ToModifiers.GrassMod;
                        break;
                    case WaypointType.WPType.Water:
                        speedMod *= ToModifiers.WaterMod;
                        break;
                    case WaypointType.WPType.Sand:
                        speedMod *= ToModifiers.SandMod;
                        break;
                    case WaypointType.WPType.Snow:
                        speedMod *= ToModifiers.SnowMod;
                        break;
                    case WaypointType.WPType.Mountain:
                        speedMod *= ToModifiers.MountainMod;
                        break;
                    case WaypointType.WPType.Bridge:
                        speedMod *= ToModifiers.BridgeMod;
                        break;
                    case WaypointType.WPType.Town:
                        speedMod *= ToModifiers.TownMod;
                        break;
                }
            }
        }
    }

    protected override void PreFindPath()
    {
        this.targetWaypoint = dragon.currentWaypoint;
    }

    protected override float MoveSpeedModifier()
    {
        return speedMod;
    }
}
