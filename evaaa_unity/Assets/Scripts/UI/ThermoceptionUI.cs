using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // Ensure you have TextMeshPro imported if using it for labels

public class ThermoreceptionUI : MonoBehaviour
{
    [Header("UI Settings")]
    [Tooltip("Reference to the ThermoreceptionGrid GameObject.")]
    public GameObject thermoreceptionGrid;

    [Tooltip("Reference to the InteroceptiveAgent.")]
    public InteroceptiveAgent interoceptiveAgent;

    [Tooltip("List of UI Image components representing the grid cells.")]
    public List<Image> gridCells = new List<Image>();

    [Tooltip("List of TextMeshProUGUI components representing the temperature labels.")]
    public List<TextMeshProUGUI> temperatureLabels = new List<TextMeshProUGUI>();

    [Header("Temperature Settings")]
    [Tooltip("The set-point temperature.")]
    public float setPoint = 0f;

    [Tooltip("Maximum deviation for color scaling.")]
    public float maxDeviation = 15f;

    private float[] thermoreceptors;

    void Start()
    {
        // Validate references
        if (thermoreceptionGrid == null)
        {
            Debug.LogError("ThermoreceptionGrid is not assigned in the Inspector.");
            return;
        }

        if (interoceptiveAgent == null)
        {
            Debug.LogError("InteroceptiveAgent is not assigned in the Inspector.");
            return;
        }

        // Initialize grid cells if not manually assigned
        if (gridCells.Count == 0)
        {
            gridCells.AddRange(thermoreceptionGrid.GetComponentsInChildren<Image>());
        }

        // Initialize temperature labels if using labels
        if (temperatureLabels.Count == 0)
        {
            temperatureLabels.AddRange(thermoreceptionGrid.GetComponentsInChildren<TextMeshProUGUI>());
        }

        // Retrieve thermoreceptor data from the InteroceptiveAgent
        thermoreceptors = interoceptiveAgent.thermoObservation;

        // Optional: Check if the number of grid cells matches the number of thermoreceptors
        if (gridCells.Count != thermoreceptors.Length)
        {
            Debug.LogWarning("Number of grid cells does not match the number of thermoreceptors.");
        }
    }

    void Update()
    {
        // Update thermoreceptor data from the InteroceptiveAgent
        if (interoceptiveAgent != null)
        {
            thermoreceptors = interoceptiveAgent.thermoObservation;
            UpdateThermoreceptionUI();
        }
    }

    /// <summary>
    /// Updates the UI grid based on the thermoreceptor temperatures.
    /// </summary>
    void UpdateThermoreceptionUI()
    {
        for (int i = 0; i < gridCells.Count; i++)
        {
            if (i < thermoreceptors.Length)
            {
                float temp = thermoreceptors[i]; // Corrected: thermoreceptors[i] is a float
                Color cellColor = GetColorFromTemperature(temp);
                gridCells[i].color = cellColor;

                // Update the temperature label if assigned
                if (i < temperatureLabels.Count && temperatureLabels[i] != null)
                {
                    temperatureLabels[i].text = temp.ToString("F1"); // One decimal place
                }
            }
            else
            {
                // Handle extra grid cells without thermoreceptors
                gridCells[i].color = Color.gray;

                // Clear or set default text
                if (i < temperatureLabels.Count && temperatureLabels[i] != null)
                {
                    temperatureLabels[i].text = "--";
                }
            }
        }
    }

    /// <summary>
    /// Maps a temperature value to a color.
    /// </summary>
    /// <param name="temperature">Temperature relative to set-point.</param>
    /// <returns>Color representing the temperature.</returns>
    Color GetColorFromTemperature(float temperature)
    {
        // Calculate deviation from set-point
        float deviation = temperature - setPoint;

        // Normalize deviation based on maxDeviation
        float normalizedDeviation = Mathf.Clamp(deviation / maxDeviation, -1f, 1f);

        // Map normalized deviation to color
        if (normalizedDeviation > 0)
        {
            // Above set-point: Green to Red
            return Color.Lerp(Color.green, Color.red, normalizedDeviation);
        }
        else if (normalizedDeviation < 0)
        {
            // Below set-point: Green to Blue
            return Color.Lerp(Color.green, Color.blue, -normalizedDeviation);
        }
        else
        {
            // At set-point: Green
            return Color.green;
        }
    }
}
