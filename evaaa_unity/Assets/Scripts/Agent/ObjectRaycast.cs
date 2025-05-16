using UnityEngine;

public class ObjectRaycast : MonoBehaviour
{
    public GameObject agent;
    public RaycastHit hit;
    public float damage;
    public float maxDamage = 30f; // Maximum damage cap
    private float impulseMagnitude;

    public float[] collisionObservation;

    void Start()
    {
        collisionObservation = new float[10];
    }
    void FixedUpdate()
    {
        DetectObstacle();
    }

    void DetectObstacle()
    {    
        int agentLayer = LayerMask.NameToLayer("Player");
        int layerMask = ~(1 << agentLayer);

        for (int j = 0; j < collisionObservation.Length; j++)
        {
            collisionObservation[j] = 0;
        }

        int raysPerGroup = 100 / 10; // Number of rays per group
        
        for (int i = 0; i < 100; i++)
        {
            int groupIndex = i / raysPerGroup; // Index of the group to which the current ray belongs
            float angle = i * 360f / 100; // Divide 360 degrees into 100 vectors
            Quaternion rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 direction = rotation * transform.forward;

            if (Physics.Raycast(transform.position, direction, out hit, agent.GetComponent<InteroceptiveAgent>().maxDistance, layerMask))
            {
                Debug.DrawRay(transform.position, direction * agent.GetComponent<InteroceptiveAgent>().maxDistance, Color.red);
                collisionObservation[groupIndex] = 1 + impulseMagnitude;
            }
            else
            {
                Debug.DrawRay(transform.position, direction * agent.GetComponent<InteroceptiveAgent>().maxDistance, Color.green);
            }
        }
        // string collisionObservationString = string.Join(", ", collisionObservation);
        // Debug.Log("collisionObservation : " + collisionObservationString);
        // Debug.Log("Observation : " + agent.GetComponent<InteroceptiveAgent>().collisionObservation);
    }

    void OnCollisionStay(Collision collision)
    {   
        // int agentLayer = LayerMask.NameToLayer("Court");
        // if (collision.gameObject.layer == agentLayer)
        int courtLayer = LayerMask.NameToLayer("Court");
        if (collision.gameObject.layer == courtLayer)
        {
            return;
        }
        impulseMagnitude = collision.impulse.magnitude;
        // agent.GetComponent<InteroceptiveAgent>().isCollisionDetected = true;
        damage = Mathf.Exp(0.07f * (impulseMagnitude-60)) * agent.GetComponent<InteroceptiveAgent>().damageConstant;
        damage = Mathf.Min(damage, maxDamage); // Cap the damage to maxDamage
        // agent.GetComponent<InteroceptiveAgent>().ApplyDamage(damage);

        InteroceptiveAgent agentScript = agent.GetComponent<InteroceptiveAgent>();
        // agentScript.resourceLevels[3] -= damage;
        agentScript.resourceLevels[3] += damage;
        if (damage > 0.5)
        {
            agentScript.countCollision = 1.0f;
            agentScript.dataRecorder.RecordCollision();
            // Debug.Log("Collided with: " + collision.gameObject.name);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        damage = 0;
        InteroceptiveAgent agentScript = agent.GetComponent<InteroceptiveAgent>();
        agentScript.countCollision = 0;
    }
}
