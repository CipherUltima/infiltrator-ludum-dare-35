using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIEnemy : MonoBehaviour {

    public LayerMask mask;

    private const float PATROL_SPEED = 0.04f;
    private const float INVESTIGATE_SPEED = 0.05f;
    private const float CHASE_SPEED = 0.075f;
    private const float VISION_MAX_RANGE = 5f;
    private const float INVESTIGATE_TIME = 5f;
    private const float CLOSE_ENOUGH = 0.1f;

    public GameObject gibbedEnemyObject;

    [SerializeField]
    private AIPatrol myPatrol;

    public enum AIBehaviourMode
    {
        Patrol,
        ChasePlayer,
        Investigate,
        Inactive
    };

    [SerializeField]
    private AIBehaviourMode behaviour = AIBehaviourMode.Patrol;


    private Vector3 investigatePosition = new Vector3(); // Last known player position, sighted dead body, etc
    private Vector3 nextWaypointPosition = new Vector3();

    private Transform thisTransform;
    private Rigidbody2D thisRigidbody;

    private bool isChasing = false;

    private bool isInvestigating = false;
    private float investigateStartedAt = float.NegativeInfinity;

    private AStarNavList aStarNavList;
    private List<Vector3> navList = new List<Vector3>();
    private Vector3 destination = new Vector3();

	// Use this for initialization
    void Start()
    {
        thisTransform = GetComponent<Transform>();
        thisRigidbody = GetComponent<Rigidbody2D>();
        destination = myPatrol.GetCurrentWaypoint();
        aStarNavList = new AStarNavList();
        SwitchToPatrol();
	}
	
	// Update is called once per frame
	void Update () {
        // Handle movement
	    switch(behaviour)
        {
            case AIBehaviourMode.Patrol:
                DoPatrol();
                break;
            case AIBehaviourMode.ChasePlayer:
                DoChase();
                break;
            case AIBehaviourMode.Investigate:
                DoInvestigate();
                break;
            case AIBehaviourMode.Inactive:
                break;
        }
        // TODO: Consider guns for enemies?

        if (Player.IsDead)
        {
            behaviour = AIBehaviourMode.Inactive;
        }
        else if (Player.HasWon)
        {
            behaviour = AIBehaviourMode.Inactive;
        }
        else if (behaviour == AIBehaviourMode.Inactive)
        {
            behaviour = AIBehaviourMode.Patrol;
        }
	}

    void FixedUpdate ()
    {
        if (behaviour == AIBehaviourMode.Inactive)
        {
            return;
        }
        if (CheckVision(PlayerMovement.GetPosition()))
        {
            SwitchToChase();
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                investigatePosition = PlayerMovement.GetPosition();
                SwitchToInvestigate();
            }
        }
        if (navList.Count == 0)
            navList = aStarNavList.BuildPath(thisTransform.position, destination);
    }

    void DoPatrol ()
    {
        nextWaypointPosition = myPatrol.GetCurrentWaypoint();
        destination = nextWaypointPosition;
        Move(PATROL_SPEED);
        if (Vector3.Distance(thisTransform.position, nextWaypointPosition) < CLOSE_ENOUGH)
        {
            nextWaypointPosition = myPatrol.GetNextWaypoint();
            destination = nextWaypointPosition;
        }
    }

    void DoChase ()
    {
        destination = PlayerMovement.GetPosition();
        Move(CHASE_SPEED);
        if (Vector3.Distance(thisTransform.position, PlayerMovement.GetPosition()) < 0.3f)
        {
            Player.Kill();
            SwitchToPatrol();
        }
    }

    void DoInvestigate ()
    {
        destination = investigatePosition;
        Move(INVESTIGATE_SPEED);
        if (Vector3.Distance(thisTransform.position, investigatePosition) < CLOSE_ENOUGH && !isInvestigating)
        {
            investigateStartedAt = Time.time;
            isInvestigating = true;
        }
        if (isInvestigating && Time.time >= investigateStartedAt + INVESTIGATE_TIME)
        {
            isInvestigating = false;
            SwitchToPatrol();
        }
    }

    void Move (float speed)
    {
        if (navList.Count == 0)
        {
            navList = aStarNavList.BuildPath(thisTransform.position, destination);
        }
        Vector3 position = navList[0];

        // Attempt to simplify pathfinding

        RaycastHit2D hit = Physics2D.Raycast(position,
            (navList[navList.Count - 1] - position), (navList[navList.Count - 1] - position).magnitude,
            LayerMask.GetMask("Default"));
        if (hit.collider == null)
        {
            position = navList[navList.Count - 1];
        }

        Vector3 diff = (position - thisTransform.position);
        diff.Normalize();

        Vector3 destPosition = thisTransform.position + (diff * speed);



        if ((destPosition - thisTransform.position).magnitude > (position - thisTransform.position).magnitude)
        {
            destPosition = position;
        }
        thisRigidbody.MovePosition(destPosition);
        FaceTowards(destPosition);

        if (Vector3.Distance(thisTransform.position, position) < CLOSE_ENOUGH)
        {
            navList.RemoveAt(0);
        }
    }

    void FaceTowards (Vector3 position)
    {
        Vector3 diff = position - thisTransform.position;
        float x = diff.x;
        float y = diff.y;
        float angleRad = Mathf.Atan2(x, y);
        float angleDeg = (Mathf.Rad2Deg * angleRad);
        thisTransform.rotation = Quaternion.AngleAxis(angleDeg, Vector3.back);
    }

    bool CheckVision (Vector3 position)
    {
        // Step 1: Check raw distance
        Vector3 diff = position - thisTransform.position;
        if (diff.magnitude > VISION_MAX_RANGE)
        {
            return false;
        }

        // Step 2: Check angle between us and player

        float x = diff.x;
        float y = diff.y;
        float angleRad = Mathf.Atan2(x, y);
        float angleDeg = (Mathf.Rad2Deg * angleRad);
        angleDeg = Quaternion.AngleAxis(angleDeg, Vector3.back).eulerAngles.z;
        // 1a: Adjust angle by our rotation
        angleDeg -= thisTransform.rotation.eulerAngles.z;

        bool canSeeTarget = (angleDeg > -90 && angleDeg < 90);
        canSeeTarget = canSeeTarget && (PlayerShift.GetMode() == PlayerShift.Mode.Circle);
        canSeeTarget = canSeeTarget || behaviour == AIBehaviourMode.ChasePlayer;

        if (!canSeeTarget)
        {
            return false;
        }

        // Step 3: If step 2 was successful, begin raytrace

        RaycastHit2D hit = Physics2D.Raycast(thisTransform.position, (position - thisTransform.position).normalized, VISION_MAX_RANGE, mask);
        if (hit.collider != null)
        {
            if (Vector3.Distance((Vector3)hit.point, position) < 0.126f)
            {
                canSeeTarget = true;
            }
            else
            {
                canSeeTarget = false;
            }
        }

        return canSeeTarget;
    }

    public void SwitchToChase ()
    {
        if (behaviour != AIBehaviourMode.ChasePlayer)
        {
            isChasing = true;
            behaviour = AIBehaviourMode.ChasePlayer;
            GetComponentInChildren<AIInvestigatePopup>(true).Reset();
            GetComponentInChildren<AIAlertPopup>(true).Alert();
            destination = PlayerMovement.GetPosition();
            navList = aStarNavList.BuildPath(thisTransform.position, destination);
        }
    }

    public void SwitchToPatrol ()
    {
        if (behaviour != AIBehaviourMode.Patrol)
        {
            behaviour = AIBehaviourMode.Patrol;
            GetComponentInChildren<AIInvestigatePopup>(true).Reset();
            GetComponentInChildren<AIAlertPopup>(true).Reset();
            destination = myPatrol.GetCurrentWaypoint();
            navList = aStarNavList.BuildPath(thisTransform.position, destination);
        }
    }

    public void SwitchToInvestigate ()
    {
        if (behaviour != AIBehaviourMode.Investigate)
        {
            behaviour = AIBehaviourMode.Investigate;
            GetComponentInChildren<AIInvestigatePopup>(true).Alert();
            GetComponentInChildren<AIAlertPopup>(true).Reset();
            destination = investigatePosition;
            navList = aStarNavList.BuildPath(thisTransform.position, destination);
        }
    }

    public void Kill ()
    {
        GameObject gibs = Instantiate(gibbedEnemyObject);
        gibs.transform.position = thisTransform.position;
        Destroy(gameObject);
    }
}
