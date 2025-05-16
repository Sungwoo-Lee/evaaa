using UnityEngine;

public class AgentTrackBlackDot : MonoBehaviour
{
    public GameObject agent; // The agent whose position we are tracking
    public CourtSpawner courtSpawner; // Reference to the CourtSpawner
    public RectTransform heatMapRect; // The RectTransform of the HeatMap UI element
    public RectTransform blackDotRect; // The RectTransform of the black dot representing the agent

    private void Awake()
    {
        if (agent == null)
        {
            Debug.LogError("Agent is not assigned.");
        }

        if (courtSpawner == null)
        {
            Debug.LogError("CourtSpawner is not assigned.");
        }

        if (heatMapRect == null)
        {
            Debug.LogError("HeatMap RectTransform is not assigned.");
        }

        if (blackDotRect == null)
        {
            Debug.LogError("BlackDot RectTransform is not assigned.");
        }
    }

    private void FixedUpdate()
    {
        if (agent == null || courtSpawner == null || heatMapRect == null || blackDotRect == null)
        {
            return;
        }

        // Get the agent's relative position within the court
        Vector3 relativePosition = GetAgentRelativePosition();
        // Debug.Log($"Agent's relative position: {relativePosition}");

        // Move the black dot based on the agent's relative position
        MoveBlackDot(relativePosition);
    }

    private Vector3 GetAgentRelativePosition()
    {
        // Get the court floor's position and size
        Transform courtFloorTransform = courtSpawner.CourtFloorTransform;
        if (courtFloorTransform == null)
        {
            Debug.LogError("Court floor transform is not found.");
            return Vector3.zero;
        }

        Vector3 courtPosition = courtFloorTransform.position;
        Vector3 courtSize = new Vector3(courtSpawner.courtConfig.floorSize.x, 1, courtSpawner.courtConfig.floorSize.z);

        // Calculate the agent's relative position within the court
        Vector3 agentPosition = agent.transform.position;
        Vector3 relativePosition = new Vector3(
            (agentPosition.x - courtPosition.x) / courtSize.x,
            (agentPosition.y - courtPosition.y) / courtSize.y,
            (agentPosition.z - courtPosition.z) / courtSize.z
        );

        return relativePosition;
    }

    private void MoveBlackDot(Vector3 relativePosition)
    {

        Vector2 localPosition = new Vector2(heatMapRect.anchoredPosition.x, heatMapRect.anchoredPosition.y);
        Vector2 relativePosition2D = new Vector2(relativePosition.x * heatMapRect.rect.width, relativePosition.z* heatMapRect.rect.height);

        // Debug.Log($"heatMapRect's anchoredPosition position: {localPosition}");
        // Debug.Log($"agent's relativePosition2D position: {relativePosition2D}");

        // Update the position of the black dot on the HeatMap
        blackDotRect.anchoredPosition = localPosition + relativePosition2D;
    }
}