using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPatrol : MonoBehaviour {

    [SerializeField]
    private List<Vector3> waypoints = new List<Vector3>();

    [SerializeField]
    private int currentWaypoint = 0;

    public enum PatrolBehaviourMode
    {
        ReturnToStart,
        Reverse
    };

    [SerializeField]
    private PatrolBehaviourMode behaviour = PatrolBehaviourMode.ReturnToStart;

    private int direction = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public Vector3 GetCurrentWaypoint ()
    {
        return waypoints[currentWaypoint];
    }

    public Vector3 GetNextWaypoint ()
    {
        currentWaypoint+=direction;
        if (currentWaypoint >= waypoints.Count)
        {
            switch(behaviour)
            {
                case PatrolBehaviourMode.ReturnToStart:
                    currentWaypoint = 0;
                    break;
                case PatrolBehaviourMode.Reverse:
                    currentWaypoint = waypoints.Count - 1;
                    direction = -1;
                    break;
                default:
                    Debug.LogError("Unknown waypoint behaviour");
                    break;
            }
        }
        if (currentWaypoint < 0)
        {
            currentWaypoint = 0;
            direction = 1;
        }
        return waypoints[currentWaypoint];
    }
}
