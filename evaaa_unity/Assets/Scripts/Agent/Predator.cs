using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Predator : MonoBehaviour
{
    public enum PredatorState { Resting, Searching, Chasing, Attacking }
    public PredatorState currentState;
    public bool isInLandmarkArea;

    [Header("Movement Settings")]
    [SerializeField] public float walkSpeed = 3.0f;
    [SerializeField] public float turnSpeed = 180.0f;

    private Vector3 destination;
    private bool isWalking;

    [Header("Field of View Settings")]
    [SerializeField] public float viewAngle = 120f;
    [SerializeField] public float viewDistance = 10f;
    [SerializeField] private LayerMask targetMask;
    private Transform detectedAgent;

    [Header("Damage Settings")]
    [SerializeField] public float damageAmount = 1f;
    [SerializeField] public float maxDamage = 5f;
    [SerializeField] public float attackInterval = 1f;

    [Header("State Timing Settings")]
    [SerializeField] public int maxRestingSteps = 50;
    [SerializeField] public int maxSearchingSteps = 150;
    [SerializeField] public int searchingActionInterval = 60;

    private int restingStepCounter = 0;
    private int searchingStepCounter = 0;
    private float lastDamageTime;

    [Header("Components")]
    [SerializeField] private Animator anim;
    private NavMeshAgent nav;
    private DayAndNight dayAndNight;

    private bool isInitialized = false;
    private bool isNavMeshInitialized = false;
    private List<Vector3> landmarkPositions = new List<Vector3>();
    private bool landmarksInitialized = false;
    private PredatorState pendingState = PredatorState.Searching;
    private MeshCollider landmarkAreaMeshCollider; // Reference to the convex hull MeshCollider

    private LandmarkSpawner landmarkSpawner;
    private int outsideAreaStepCounter = 0;
    private int maxOutsideSteps = 1;

    public void InitializePredator()
    {
        if (isInitialized) return;
        
        nav = GetComponent<NavMeshAgent>();
        if (nav == null)
        {
            Debug.LogError($"Predator {gameObject.name} is missing NavMeshAgent component!");
            return;
        }

        nav.enabled = false; // Disable NavMesh until it's properly initialized
        
        // Random initialization of counters to prevent synchronized behavior
        searchingStepCounter = Random.Range(0, searchingActionInterval);
        restingStepCounter = Random.Range(0, maxRestingSteps / 2);

        // Set initial state without activating movement
        pendingState = PredatorState.Searching;
        currentState = PredatorState.Searching;
        isInitialized = true;
    }

    public void InitializeNavMesh()
    {
        InitializeLandmarks();
        if (!isInitialized)
        {
            Debug.LogError($"Predator {gameObject.name} must be initialized before NavMesh initialization!");
            return;
        }

        if (isNavMeshInitialized) return;

        if (nav != null)
        {
            nav.enabled = true;
            nav.speed = walkSpeed;
            nav.angularSpeed = turnSpeed;
            isNavMeshInitialized = true;
            
            // Now that NavMesh is initialized, properly set the initial state
            ChangeState(pendingState);
            Debug.Log($"Predator {gameObject.name} NavMesh initialized and state activated");
        }
    }

    // Called by MasterInitializer after DayAndNight is initialized
    public void SetDayAndNight(DayAndNight dayAndNightSystem)
    {
        dayAndNight = dayAndNightSystem;
        if (dayAndNight == null)
        {
            Debug.LogError($"Predator {gameObject.name} received null DayAndNight reference");
        }
    }

    private void InitializeLandmarks()
    {
        if (landmarksInitialized) return;
        landmarkSpawner = FindObjectOfType<LandmarkSpawner>();
        if (landmarkSpawner != null)
        {
            var landmarks = landmarkSpawner.GetLandmarks();
            landmarkPositions.Clear();
            foreach (var lm in landmarks)
            {
                if (lm != null) landmarkPositions.Add(lm.transform.position);
            }
            // Get the MeshCollider for area logic
            var meshObj = landmarkSpawner.transform.Find("LandmarkAreaMesh");
            if (meshObj != null)
                landmarkAreaMeshCollider = meshObj.GetComponent<MeshCollider>();
            landmarksInitialized = true;
            Debug.Log($"[{gameObject.name}] Initialized with {landmarkPositions.Count} landmarks and MeshCollider");
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] could not find LandmarkSpawner in the scene.");
        }
    }

    /// <summary>
    /// Checks if the predator's XZ position is inside the convex hull polygon defined by the landmark area.
    /// This uses a 2D point-in-convex-polygon test (cross product method).
    /// For each edge of the convex hull, it checks if the point is always on the same side (left) of all edges.
    /// If the point is ever on the right side of any edge, it is outside the polygon.
    /// This is O(N) where N is the number of convex hull points (very fast for small N).
    /// </summary>
    private bool IsInsideLandmarkArea()
    {
        if (landmarkSpawner == null) return false;
        var hull = landmarkSpawner.GetConvexHullPoints();
        if (hull == null || hull.Count < 3) return false;
        Vector2 p = new Vector2(transform.position.x, transform.position.z);
        int n = hull.Count;
        for (int i = 0; i < n; i++)
        {
            Vector2 a = new Vector2(hull[i].x, hull[i].z);
            Vector2 b = new Vector2(hull[(i + 1) % n].x, hull[(i + 1) % n].z);
            Vector2 edge = b - a;
            Vector2 toPoint = p - a;
            // Cross product: if negative, point is to the right of the edge (outside for CCW hull)
            if (edge.x * toPoint.y - edge.y * toPoint.x < 0)
                return false;
        }
        return true;
    }

    public void TakeAction()
    {
        if (!isInitialized || !isNavMeshInitialized)
        {
            return;
        }

        // Check if it's night or dawn - force resting state
        if (dayAndNight != null && 
            (dayAndNight.CurrentDayNightState == DayAndNight.DayNightState.Night ||
             dayAndNight.CurrentDayNightState == DayAndNight.DayNightState.DeepNight ||
             dayAndNight.CurrentDayNightState == DayAndNight.DayNightState.Dawn))
        {
            if (currentState != PredatorState.Resting)
            {
                ChangeState(PredatorState.Resting);
                StopMovement();
                return;
            }
            return; // Skip other behaviors during night/dawn
        }

        // Use 2D convex hull check for containment
        bool isOutside = !IsInsideLandmarkArea();
        isInLandmarkArea = !isOutside;
        if (isOutside)
        {
            outsideAreaStepCounter++;
            if (outsideAreaStepCounter >= maxOutsideSteps)
            {
                Debug.Log($"[{gameObject.name}] Outside landmark area for {outsideAreaStepCounter} steps, redirecting...");
                ChooseRandomDestination();
                outsideAreaStepCounter = 0; // Reset after redirect
            }
        }
        else
        {
            outsideAreaStepCounter = 0; // Reset if inside
        }

        switch (currentState)
        {
            case PredatorState.Resting:
                restingStepCounter++;
                if (View())
                {
                    ChangeState(PredatorState.Chasing);
                }
                else if (restingStepCounter >= maxRestingSteps)
                {
                    restingStepCounter = 0;
                    ChangeState(PredatorState.Searching);
                }
                break;

            case PredatorState.Searching:
                searchingStepCounter++;
                if (searchingStepCounter % searchingActionInterval == 0)
                {
                    ChooseRandomDestination();
                }

                if (View())
                {
                    ChangeState(PredatorState.Chasing);
                }
                else if (searchingStepCounter >= maxSearchingSteps)
                {
                    searchingStepCounter = 0;
                    ChangeState(PredatorState.Resting);
                }
                break;

            case PredatorState.Chasing:
                if (detectedAgent != null && nav.enabled)
                {
                    // Check if predator is inside the landmark area
                    if (!IsInsideLandmarkArea())
                    {
                        Debug.Log($"[{gameObject.name}] Chasing interrupted: outside landmark area, switching to Searching.");
                        detectedAgent = null;
                        ChangeState(PredatorState.Searching);
                        break;
                    }
                    nav.SetDestination(detectedAgent.position);
                    if (!View())
                    {
                        Debug.Log($"[{gameObject.name}] Lost sight of agent, returning to Searching state");
                        detectedAgent = null;
                        ChangeState(PredatorState.Searching);
                    }
                }
                else
                {
                    ChangeState(PredatorState.Searching);
                }
                break;

            case PredatorState.Attacking:
                if (detectedAgent != null)
                {
                    ApplyDamage();

                    // If the agent is no longer in view, transition to Searching
                    if (!View())
                    {
                        Debug.Log($"[{gameObject.name}] Agent escaped attack range, returning to Searching state");
                        detectedAgent = null;
                        ChangeState(PredatorState.Searching);
                    }
                }
                else
                {
                    ChangeState(PredatorState.Searching);
                }
                break;
        }

        UpdateMovementStatus();
    }

    private void ApplyDamage()
    {
        if (detectedAgent == null) return;
        
        InteroceptiveAgent agentScript = detectedAgent.GetComponent<InteroceptiveAgent>();
        if (agentScript != null)
        {
            float damage = Mathf.Min(damageAmount, maxDamage);
            agentScript.resourceLevels[3] -= damage;
            Debug.Log($"[{gameObject.name}] Applied {damage} damage to agent");
        }
    }

    private void ChangeState(PredatorState newState)
    {
        var oldState = currentState;
        currentState = newState;
        
        // If NavMesh isn't initialized yet, just store the state
        if (!isNavMeshInitialized)
        {
            pendingState = newState;
            Debug.Log($"[{gameObject.name}] State change pending (waiting for NavMesh): {oldState} -> {newState}");
            return;
        }

        switch (newState)
        {
            case PredatorState.Resting:
                StopMovement();
                restingStepCounter = 0;
                break;

            case PredatorState.Searching:
                ResumeMovement();
                searchingStepCounter = 0;
                break;

            case PredatorState.Chasing:
                ResumeMovement();
                break;

            case PredatorState.Attacking:
                StopMovement();
                break;
        }
        
        Debug.Log($"[{gameObject.name}] State changed from {oldState} to {newState}");
    }

    private void StopMovement()
    {
        if (nav == null || !isNavMeshInitialized) return;
        
        isWalking = false;
        if (anim != null) anim.SetBool("Walking", false);
        nav.ResetPath();
        nav.isStopped = true;
    }

    private void ResumeMovement()
    {
        if (nav == null || !isNavMeshInitialized || !nav.enabled || !nav.isOnNavMesh) return;
        nav.isStopped = false;
        nav.speed = walkSpeed;
    }

    private void ChooseRandomDestination()
    {
        if (nav == null || !nav.enabled) return;

        if (landmarkPositions.Count > 0)
        {
            // Choose a random landmark as destination
            int randomIndex = Random.Range(0, landmarkPositions.Count);
            destination = landmarkPositions[randomIndex];
            nav.SetDestination(destination);
            isWalking = true;
            if (anim != null) anim.SetBool("Walking", true);
            Debug.Log($"[{gameObject.name}] Set new destination to landmark at {destination}");
        }
        else
        {
            // Fallback to random point if no landmarks available
            Vector3 randomDirection = Random.insideUnitSphere * viewDistance;
            randomDirection.y = 0;
            Vector3 targetPosition = transform.position + randomDirection;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, viewDistance, NavMesh.AllAreas))
            {
                destination = hit.position;
                nav.SetDestination(destination);
                isWalking = true;
                if (anim != null) anim.SetBool("Walking", true);
                Debug.Log($"[{gameObject.name}] Set fallback random destination at {destination}");
            }
        }
    }

    private void UpdateMovementStatus()
    {
        if (nav == null || !isWalking || !isNavMeshInitialized) return;

        if (!nav.pathPending && nav.hasPath && nav.remainingDistance <= nav.stoppingDistance)
        {
            if (!nav.hasPath || nav.velocity.sqrMagnitude == 0f)
            {
                isWalking = false;
                if (anim != null) anim.SetBool("Walking", false);
                Debug.Log($"[{gameObject.name}] Reached destination");
            }
        }
    }

    private bool View()
    {
        Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewDistance, targetMask);
        foreach (Collider target in targetsInView)
        {
            Transform targetTransform = target.transform;
            if (targetTransform.CompareTag("player"))
            {
                Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
                float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

                if (angleToTarget < viewAngle * 0.5f)
                {
                    detectedAgent = targetTransform;
                    return true;
                }
            }
        }

        detectedAgent = null;
        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider is BoxCollider && collision.gameObject.CompareTag("player"))
        {
            Debug.Log($"[{gameObject.name}] Collision with player, switching to Attacking state");
            detectedAgent = collision.transform;
            ChangeState(PredatorState.Attacking);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider is BoxCollider && collision.gameObject.CompareTag("player"))
        {
            Debug.Log($"[{gameObject.name}] Player left collision range, switching to Searching state");
            detectedAgent = null;
            ResumeMovement();
            ChangeState(PredatorState.Searching);
        }
    }
}