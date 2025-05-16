using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadialMeterController : MonoBehaviour
{
    [Header("State Configuration")]
    [Tooltip("Name of the state (e.g., 'Satiation', 'Hydration', 'Thermal', 'HP')")]
    public string stateName = "Satiation";

    [Tooltip("Minimum value of the state (e.g., -15)")]
    public float minValue = -15f;

    [Tooltip("Maximum value of the state (e.g., +15)")]
    public float maxValue = 15f;

    [Tooltip("Optimal set-point value (e.g., 0)")]
    public float optimalValue = 0f;

    [Tooltip("Fill direction: True for clockwise, False for counter-clockwise")]
    public bool fillClockwise = true;

    [Header("UI Components")]
    [Tooltip("Reference to the Fill Image")]
    public Image fillImage;


    [Header("Color Configuration")]
    public Color optimalColor = Color.green;
    public Color warningColor = Color.yellow;
    public Color criticalColor = Color.red;

    [Header("Thresholds")]
    [Tooltip("Deviation from optimal to trigger warning color")]
    public float warningThreshold = 5f;

    [Tooltip("Deviation from optimal to trigger critical color")]
    public float criticalThreshold = 10f;

    [Header("Transition Settings")]
    [Tooltip("Speed at which the fill amount transitions")]
    public float fillTransitionSpeed = 5f;

    [Tooltip("Speed at which the fill color transitions")]
    public float colorTransitionSpeed = 5f;

    public InteroceptiveAgent agentState; // Reference to your agent's state script
    private float currentStateValue;

    // Initial normalized fill amount based on set-point
    private float initialNormalizedFillAmount;
    private Color initialFillColor;

    // Target values for smooth transitions
    private float targetFillAmount;
    private Color targetFillColor;

    void Start()
    {
        // Validate references
        if (fillImage == null)
        {
            Debug.LogError("Fill Image is not assigned in the Inspector.");
            return;
        }

        if (agentState == null)
        {
            Debug.LogError("InteroceptiveAgent is not assigned in the Inspector.");
            return;
        }
        // Calculate initial normalized fill amount based on set-point
        initialNormalizedFillAmount = CalculateNormalizedFill(optimalValue);
        targetFillAmount = initialNormalizedFillAmount;

        // Store initial fill color
        initialFillColor = optimalColor;
        targetFillColor = initialFillColor;

        // Initialize Fill Image
        fillImage.fillAmount = initialNormalizedFillAmount;
        fillImage.color = initialFillColor;
    }

    void Update()
    {
        if (agentState == null)
            return;

        // Retrieve the current state value from the AgentState script
        switch (stateName.ToLower())
        {
            case "satiation":
                currentStateValue = agentState.resourceLevels[0];
                break;
            case "hydration":
                currentStateValue = agentState.resourceLevels[1];
                break;
            case "thermal":
                currentStateValue = agentState.resourceLevels[2];
                break;
            case "hp":
                currentStateValue = agentState.resourceLevels[3];
                break;
            // Add more cases as needed
            default:
                currentStateValue = 0f;
                break;
        }

        // Clamp the state value to min and max
        currentStateValue = Mathf.Clamp(currentStateValue, minValue, maxValue);

        // Update the Fill Amount
        UpdateFillAmount();

        // Update the Fill Color based on thresholds
        UpdateFillColor();
    }

    /// <summary>
    /// Calculates the normalized fill amount based on a given value.
    /// </summary>
    /// <param name="value">The state value to normalize.</param>
    /// <returns>Normalized value between 0 and 1.</returns>
    float CalculateNormalizedFill(float value)
    {
        return (value - minValue) / (maxValue - minValue);
    }

    /// <summary>
    /// Updates the fill amount of the radial meter based on the current state value.
    /// Implements smooth transitions.
    /// </summary>
    void UpdateFillAmount()
    {
        // Calculate normalized fill based on current state value
        float normalizedFill = CalculateNormalizedFill(currentStateValue);

        // Clamp to ensure it's within [0,1]
        normalizedFill = Mathf.Clamp01(normalizedFill);

        // Set fill direction
        fillImage.fillClockwise = fillClockwise;

        // Update the target fill amount
        targetFillAmount = normalizedFill;

        // Smoothly interpolate to the target fill amount
        fillImage.fillAmount = Mathf.Lerp(fillImage.fillAmount, targetFillAmount, Time.deltaTime * fillTransitionSpeed);
    }

    /// <summary>
    /// Updates the fill color based on how much the state deviates from the optimal value.
    /// Implements smooth transitions.
    /// </summary>
    void UpdateFillColor()
    {
        float deviation = Mathf.Abs(currentStateValue - optimalValue);

        // Normalize deviation based on maxDeviation
        float normalizedDeviation = Mathf.Clamp(deviation / maxValue, 0f, 1f);

        // Map normalized deviation to color
        // Positive deviation: Green to Red
        // Negative deviation: Green to Blue
        // Debug.Log(stateName + ": " + normalizedDeviation);
        if (normalizedDeviation > 0.1 && normalizedDeviation < 0.3)
        {
            targetFillColor = Color.Lerp(optimalColor, warningColor, normalizedDeviation);
        }
        else if (normalizedDeviation > 0.3)
        {
            
            targetFillColor = Color.Lerp(warningColor, criticalColor, normalizedDeviation);
        }
        else
        {
            targetFillColor = optimalColor;
        }

        // Smoothly interpolate to the target color
        fillImage.color = Color.Lerp(fillImage.color, targetFillColor, Time.deltaTime * colorTransitionSpeed);

    }

    /// <summary>
    /// Resets the fill amount and color to their initial values.
    /// </summary>
    public void ResetMeter()
    {
        fillImage.fillAmount = initialNormalizedFillAmount;
        fillImage.color = initialFillColor;
    }

    /// <summary>
    /// Returns the current normalized fill amount (0 to 1).
    /// </summary>
    public float GetCurrentFillAmount()
    {
        return fillImage.fillAmount;
    }

    /// <summary>
    /// Returns the current fill color.
    /// </summary>
    public Color GetCurrentFillColor()
    {
        return fillImage.color;
    }
}
