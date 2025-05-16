using System.IO;
using UnityEngine;
using Unity.MLAgents;
using Assets.Scripts.Utility;

// Attach this script to the Sun GameObject (Directional Light)
public class DayAndNight : MonoBehaviour
{
    [Header("Configurable Parameters")]
    public float fogChangeSpeed;
    public float dayFogDensity;
    public float nightFogDensity;
    public float sunsetExponent = 1.5f;
    public float dawnExponent = 1.5f;
    public Color dayFogColor;
    public Color nightFogColor;
    public float dayTemperatureChange;
    public float nightTemperatureChange;
    public float dayFarClip;
    public float nightFarClip;
    public float farClipTransitionSpeed;
    public bool randomSunAngle;
    public int rotationIntervalMultiplier = 1; // 1=360, 2=720, 4=1440, etc.
    public int rotationSpeedSteps = 1; // How many fine-grained intervals to advance per step
    public int temperatureUpdateSteps = 24; // How many coarse steps for temperature/hour
    public float fogExponent = 2.0f;
    public bool enableDayNightCycle = true;

    [Header("References")]
    public ThermoGridSpawner thermoGridSpawner;
    public HeatMap heatMap;
    public Camera mainCamera;
    public Material daySkybox;
    public Material nightSkybox;
    public Color daySkyboxTint = Color.white;
    public Color nightSkyboxTint = Color.gray;

    [Header("Sun/Rotation State (Runtime)")]
    [SerializeField]
    private float sunAngle = 0f;
    private int rotationIntervals = 360;
    private float angleStep = 1f;
    private float temperatureStepAngle = 15f;

    [Header("Temperature/Hour State (Runtime)")]
    [SerializeField]
    private int currentTemperatureStep = -1;
    [SerializeField]
    private int realWorldHour = 0;

    [Header("Fog State (Runtime)")]
    private float currentFogDensity;

    [Header("Camera State (Runtime)")]
    private float targetFarClip;

    [Header("Fog and Camera Step Values")]
    [SerializeField]
    private float[] fogDensitySteps;
    [SerializeField]
    private float[] farClipSteps;

    private ConfigLoader configLoader;
    private string configFileName = "daynightConfig.json";
    private float stepAngleSize = 0f; // (legacy, not used)

    public enum DayNightState { Day, Sunset, Night, DeepNight, Dawn }
    public DayNightState CurrentDayNightState
    {
        get
        {
            float angle = sunAngle;
            if (angle < 0f) angle += 360f;
            return (angle >= 350f || angle <= 170f) ? DayNightState.Day : DayNightState.Night;
        }
    }

    public int CurrentTemperatureStep => currentTemperatureStep;
    public int RealWorldHour => (currentTemperatureStep + 6) % 24;

    private void Start()
    {
        // Calculate intervals and step sizes
        rotationIntervals = 360 * rotationIntervalMultiplier;
        angleStep = 360f / rotationIntervals;
        temperatureStepAngle = 360f / temperatureUpdateSteps;
    }

    private DayNightState GetStateForHour(int hour)
    {
        if (hour >= 7 && hour <= 17)
            return DayNightState.Day;
        if (hour >= 18 && hour <= 20)
            return DayNightState.Sunset;
        if ((hour >= 21 && hour <= 23) || (hour >= 0 && hour <= 2))
            return DayNightState.Night;
        if (hour == 3)
            return DayNightState.DeepNight;
        if (hour >= 4 && hour <= 6)
            return DayNightState.Dawn;
        return DayNightState.Day;
    }

    private void InitializeFogAndCameraSteps()
    {
        int firstHour = 6; // i=0 corresponds to hour 6
        if (fogDensitySteps == null || fogDensitySteps.Length != temperatureUpdateSteps)
            fogDensitySteps = new float[temperatureUpdateSteps];
        if (farClipSteps == null || farClipSteps.Length != temperatureUpdateSteps)
            farClipSteps = new float[temperatureUpdateSteps];

        for (int i = 0; i < temperatureUpdateSteps; i++)
        {
            int hour = (firstHour + i) % 24;
            DayNightState state = GetStateForHour(hour);
            switch (state)
            {
                case DayNightState.Day:
                    fogDensitySteps[i] = dayFogDensity;
                    farClipSteps[i] = dayFarClip;
                    break;
                case DayNightState.Sunset:
                    // Sunset: 18-20 (using sunsetExponent)
                    float tSunset = (hour - 18) / 3f; // t=0 at 18, t=1 at 20
                    float nonlinearTSunset = Mathf.Pow(tSunset, sunsetExponent);
                    fogDensitySteps[i] = Mathf.Lerp(dayFogDensity, nightFogDensity, nonlinearTSunset);
                    farClipSteps[i] = Mathf.Lerp(dayFarClip, nightFarClip, nonlinearTSunset);
                    break;
                case DayNightState.Night:
                    // Night: 21-2 (using fogExponent)
                    float tNight = hour >= 21 ? (hour - 21) / 6f : (hour + 3) / 6f; // t=0 at 21, t=1 at 2
                    float nonlinearTNight = Mathf.Pow(tNight, fogExponent);
                    fogDensitySteps[i] = Mathf.Lerp(nightFogDensity * 0.5f, nightFogDensity, nonlinearTNight);
                    farClipSteps[i] = Mathf.Lerp((dayFarClip + nightFarClip) * 0.5f, nightFarClip, nonlinearTNight);
                    break;
                case DayNightState.DeepNight:
                    fogDensitySteps[i] = nightFogDensity;
                    farClipSteps[i] = nightFarClip;
                    break;
                case DayNightState.Dawn:
                    // Dawn: 4-6 (using dawnExponent)
                    float tDawn = (hour - 4) / 3f; // t=0 at 4, t=1 at 6
                    float nonlinearTDawn = 1f - Mathf.Pow(1f - tDawn, dawnExponent);
                    fogDensitySteps[i] = Mathf.Lerp(nightFogDensity, dayFogDensity, nonlinearTDawn);
                    farClipSteps[i] = Mathf.Lerp(nightFarClip, dayFarClip, nonlinearTDawn);
                    break;
            }
        }
    }

    public void InitializeDayAndNight(ConfigLoader loader)
    {
        configLoader = loader;
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        LoadConfig();
        InitializeFogAndCameraSteps();
    }

    private void LoadConfig()
    {
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }

        DayAndNightConfig config = configLoader.LoadConfig<DayAndNightConfig>(configFileName);

        if (config == null)
        {
            Debug.LogError("Invalid day and night configuration.");
            return;
        }

        // Use the loaded configuration
        fogChangeSpeed = config.fogChangeSpeed;
        dayFogDensity = config.dayFogDensity;
        nightFogDensity = config.nightFogDensity;
        // dayFogColor = config.dayFogColor;
        // nightFogColor = config.nightFogColor;
        dayTemperatureChange = config.dayTemperatureChange;
        nightTemperatureChange = config.nightTemperatureChange;
        dayFarClip = config.dayFarClip;
        nightFarClip = config.nightFarClip;
        farClipTransitionSpeed = config.farClipTransitionSpeed;
        randomSunAngle = config.randomSunAngle;
        rotationIntervalMultiplier = config.rotationIntervalMultiplier > 0 ? config.rotationIntervalMultiplier : 1;
        rotationSpeedSteps = config.rotationSpeedSteps > 0 ? config.rotationSpeedSteps : 1;
        temperatureUpdateSteps = config.temperatureUpdateSteps > 0 ? config.temperatureUpdateSteps : 24;
        fogExponent = config.fogExponent > 0 ? config.fogExponent : 2.0f;
        sunsetExponent = config.sunsetExponent > 0 ? config.sunsetExponent : 1.5f;
        dawnExponent = config.dawnExponent > 0 ? config.dawnExponent : 1.5f;
        enableDayNightCycle = config.enableDayNightCycle;

        // Recalculate intervals and step sizes
        rotationIntervals = 360 * rotationIntervalMultiplier;
        angleStep = 360f / rotationIntervals;
        temperatureStepAngle = 360f / temperatureUpdateSteps;

        if (randomSunAngle)
        {
            float randomAngle = Random.Range(0f, 360f);
            sunAngle = randomAngle;
            transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);
        }
        else
        {
            sunAngle = 90f; // 12H (noon)
            transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);
        }

        RenderSettings.fog = true;
        RenderSettings.fogDensity = currentFogDensity;
        RenderSettings.fogColor = dayFogColor;

        if (thermoGridSpawner == null)
        {
            Debug.LogError("ThermoGridSpawner is not assigned.");
        }

        if (heatMap == null)
        {
            Debug.LogError("HeatMap is not assigned.");
        }

        if (mainCamera == null)
        {
            Debug.LogError("MainCamera is not assigned.");
        }
    }

    public void StepUpdate()
    {
        if (enableDayNightCycle)
        {
            // Advance sun angle by fine-grained step(s)
            sunAngle += angleStep * rotationSpeedSteps;
            if (sunAngle >= 360f) sunAngle -= 360f;
            if (sunAngle < 0f) sunAngle += 360f;
        }

        // Apply rotation
        transform.rotation = Quaternion.Euler(sunAngle, 0f, 0f);

        // Only update temperature/hour when crossing next temperature step boundary
        int step = Mathf.FloorToInt(sunAngle / temperatureStepAngle) % temperatureUpdateSteps;
        if (step != currentTemperatureStep)
        {
            currentTemperatureStep = step;
            realWorldHour = (currentTemperatureStep + 6) % 24;
            UpdateTemperatureAndHeatMap();

            // Set fog and far clip instantly for this step
            currentFogDensity = fogDensitySteps[currentTemperatureStep];
            RenderSettings.fogDensity = currentFogDensity;
            if (mainCamera != null)
                mainCamera.farClipPlane = farClipSteps[currentTemperatureStep];
            
            DayNightState state = GetStateForHour(realWorldHour);
            // Debug.Log($"[DayAndNight] Hour: {realWorldHour:00}, State: {state}, SunAngle: {sunAngle:F2}, FogDensity: {currentFogDensity:F5}, FarClip: {mainCamera?.farClipPlane:F2}");
        }

        // Adjust Fog Color and Skybox (still gradual)
        AdjustFogColorAndSkybox();
    }

    private void AdjustFogColorAndSkybox()
    {
        // Use the same logic as before for color and skybox
        float hour = realWorldHour;
        Color fogColorTarget = dayFogColor;
        // Day: 6 <= hour < 17
        if (hour >= 6f && hour < 17f)
        {
            fogColorTarget = dayFogColor;
        }
        // Sunset: 17 <= hour < 20
        else if (hour >= 17f && hour < 20f)
        {
            float t = (hour - 17f) / 3f; // t=0 at 17, t=1 at 20
            float nonlinearT = Mathf.Pow(t, sunsetExponent);
            fogColorTarget = Color.Lerp(dayFogColor, nightFogColor, nonlinearT * 0.5f);
        }
        // Night: 20 <= hour < 4
        else if ((hour >= 20f && hour < 24f) || (hour >= 0f && hour < 4f))
        {
            fogColorTarget = nightFogColor;
        }
        // Dawn: 4 <= hour < 6
        else if (hour >= 4f && hour < 6f)
        {
            float t = (hour - 4f) / 2f; // t=0 at 4, t=1 at 6
            float nonlinearT = 1f - Mathf.Pow(1f - t, dawnExponent);
            fogColorTarget = Color.Lerp(nightFogColor, dayFogColor, nonlinearT);
        }
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, fogColorTarget, fogChangeSpeed * Time.fixedDeltaTime);

        // Skybox update (optional: keep as before)
        float skyboxT = (hour >= 6f && hour < 18f) ? 1f : 0f; // 1 during day, 0 at night
        if (daySkybox != null && nightSkybox != null)
        {
            RenderSettings.skybox = skyboxT > 0.5f ? daySkybox : nightSkybox;
            Color targetTint = Color.Lerp(nightSkyboxTint, daySkyboxTint, skyboxT);
            if (RenderSettings.skybox.HasProperty("_Tint"))
            {
                Color currentTint = RenderSettings.skybox.GetColor("_Tint");
                RenderSettings.skybox.SetColor("_Tint", Color.Lerp(currentTint, targetTint, fogChangeSpeed * Time.fixedDeltaTime));
            }
        }
    }

    /// <summary>
    /// Updates the temperature based on day/night state.
    /// </summary>
    private void UpdateTemperatureAndHeatMap()
    {
        if (thermoGridSpawner != null)
        {
            float minTemp = nightTemperatureChange;
            float maxTemp = dayTemperatureChange;
            float temp;
            float hour = realWorldHour;
            if (hour >= 4 && hour <= 12)
            {
                // 4H to 12H: rise from min to max
                float t = (hour - 4f) / 8f;
                temp = Mathf.Lerp(minTemp, maxTemp, t);
            }
            else
            {
                // 12H to next day's 4H: fall from max to min
                float adjHour = hour < 4 ? hour + 24 : hour;
                float t = (adjHour - 12f) / 16f;
                temp = Mathf.Lerp(maxTemp, minTemp, t);
            }
            thermoGridSpawner.SetDayNightTemperature(temp);
        }

        if (heatMap != null)
        {
            heatMap.SetDayNightTemperature(CurrentDayNightState == DayNightState.Night);
        }
    }

    public void ResetDayAndNight()
    {
        if (configLoader == null)
        {
            Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
            return;
        }
        DayAndNightConfig config = configLoader.LoadConfig<DayAndNightConfig>(configFileName);
        if (config != null && config.randomSunAngle)
        {
            float randomAngle = Random.Range(0f, 360f);
            Vector3 currentEuler = transform.eulerAngles;
            transform.eulerAngles = new Vector3(randomAngle, currentEuler.y, currentEuler.z);
            sunAngle = randomAngle;
        }
        else if (config != null && !config.randomSunAngle)
        {
            sunAngle = 90f; // 12H (noon)
            Vector3 currentEuler = transform.eulerAngles;
            transform.eulerAngles = new Vector3(sunAngle, currentEuler.y, currentEuler.z);
        }
    }
}

[System.Serializable]
public class DayAndNightConfig
{
    public float fogChangeSpeed;
    public float dayFogDensity;
    public float nightFogDensity;
    public float sunsetExponent;
    public float dawnExponent;
    public Color dayFogColor;
    public Color nightFogColor;
    public float dayTemperatureChange;
    public float nightTemperatureChange;
    public float dayFarClip;
    public float nightFarClip;
    public float farClipTransitionSpeed;
    public bool randomSunAngle;
    public int rotationIntervalMultiplier;
    public int rotationSpeedSteps;
    public int temperatureUpdateSteps;
    public float fogExponent;
    public bool enableDayNightCycle = true;
}