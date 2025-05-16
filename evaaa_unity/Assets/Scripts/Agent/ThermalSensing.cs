using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThermalSensing : MonoBehaviour
{
    [Header("Thermal Sensing Settings")]
    public float sensingRange = 10.0f; // Range of thermal sensing
    private float fieldTemp = -60.0f;  // Default field temperature

    private ThermoGridSpawner thermoGridSpawner;

    private void Start()
    {
        // Find the ThermoGridSpawner in the scene
        thermoGridSpawner = FindObjectOfType<ThermoGridSpawner>();
        if (thermoGridSpawner == null)
        {
            Debug.LogError("ThermoGridSpawner not found in the scene. Thermal sensing will not function.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Ensure the collider is tagged as a thermal grid cube
        if (other.CompareTag("thermalGridCube"))
        {
            // Debug.Log($"Collided with thermal grid cube: {other.name}");

            // Extract x, z coordinates from the grid cube's name
            string[] cubeCoords = other.name.Split(',');
            if (cubeCoords.Length == 3 && int.TryParse(cubeCoords[0], out int x) && int.TryParse(cubeCoords[2], out int z))
            {
                // Get the temperature from ThermoGridSpawner
                if (thermoGridSpawner != null)
                {
                    fieldTemp = thermoGridSpawner.GetAreaTemp(x, z);
                }
                else
                {
                    Debug.LogError("ThermoGridSpawner reference is null. Unable to retrieve temperature.");
                }
            }
            else
            {
                Debug.LogError($"Invalid thermal grid cube name format: {other.name}");
            }
        }
    }

    public float GetThermalSense()
    {
        return fieldTemp;
    }

    public void SetThermalSense(float value)
    {
        fieldTemp = value;
    }
}