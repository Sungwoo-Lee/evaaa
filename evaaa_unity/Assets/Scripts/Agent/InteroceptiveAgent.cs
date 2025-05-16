using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Assets.Scripts.Utility;

public class InteroceptiveAgent : Agent
{
        [Header("Configuration")]
        public string configFileName = "agentConfig.json";
        private ConfigLoader configLoader;
        private AgentConfig agentConfig;

        public static bool isEnvironmentReady = false; // Global readiness flag
        private bool isFirstEpisode = true;
        private Academy academy;
        public int episodeCount;

        protected EnvironmentParameters m_ResetParams;
        protected ResourceProperty[] FoodObjects;
        protected Rigidbody m_AgentRb;
        protected bool isAgentActionEat = false;
        public bool IsAgentActionEat { get { return this.isAgentActionEat; } set { this.isAgentActionEat = value; } }
        protected bool eatenResource = false;
        public bool EatenResource { get { return this.eatenResource; } set { this.eatenResource = value; } }
        protected string eatenResourceTag;
        public string EatenResourceTag { get { return this.eatenResourceTag; } set { this.eatenResourceTag = value; } }
        // protected float bodyTemp;
        protected GameObject[] agents;

        public bool isAIControlled; // Default to true for AI control

        [Header("Game Ojects for script")]
        public GameObject heatMap;
        public GameObject playRecorder;
        public CameraSwitcher camareManager;
        public DataRecorder dataRecorder;

        [Header("Environment settings")]
        public bool singleTrial;
        public bool initRandomAgentPosition;
        public PositionRange randomPositionRange;
        public ThreeDVector initAgentPosition;
        public ThreeDVector initAgentAngle;

        [Header("Actions")]
        public float moveSpeed;
        public float turnSpeed;
        public float eatingDistance;
        public bool autoEat;
        public Vector3 agentRotation;
        public Vector3 agentPosition;

        [Header("Observations")]
        public bool useTouchObs;
        public float touchObservation;
        public bool isTouched;
        public bool useOlfactoryObs;
        public float olfactorySensorLength;
        public int olfactoryFeatureSize;
        public float[] olfactoryObservation;
        public bool useThermalObs;
        public bool relativeThermalObs;
        public float[] thermoObservation;
        public GameObject thermoSensorCenter; 
        public GameObject thermoSensorForward;
        public GameObject thermoSensorBackward;
        public GameObject thermoSensorLeft;
        public GameObject thermoSensorRight;
        public GameObject thermoSensorForwardLeft;
        public GameObject thermoSensorForwardRight;
        public GameObject thermoSensorBackwardLeft;
        public GameObject thermoSensorBackwardRight;
        public ObjectRaycast objectRaycast;
        public bool isCollisionDetected;
        public bool useCollisionObs;
        public float[] collisionObservation;
        public int collisionFeatureSize;
        private Queue<float> recentRewards; // Queue for storing recent rewards
        public int rewardWindowSize; // Size of the moving average window
        public float averageReward; // Calculated average reward
        public float currentReward; // Current reward for this step

        [Header("Essential variables (EV)")]
        public int countEV;
        public float[] resourceLevels;
        private float[] oldResourceLevels;
        public bool resourceConsumedInStep;  // Track if any resource was consumed in this step
        public string consumedResourceType;  // Track which resource was consumed (using actual tag from eatenResourceTag)

        [Header("Food")]
        public EVRange foodLevelRange;
        public float resourceFoodValue;
        public float startFoodLevel;
        public float countFood;

        // blue
        [Header("Water")]
        public EVRange waterLevelRange;
        public float resourceWaterValue;
        public float startWaterLevel;
        public float countWater;

        // yellow
        [Header("Temperature")]
        public EVRange thermoLevelRange;
        public float startThermoLevel;
        public float countCollision;

        // hp
        [Header("Health")]
        public EVRange healthLevelRange;
        public float startHealthLevel;

        // Food Function Coefficient
        // Index 0 : Constant Decay
        // Index 1 : Food Effect
        // Index 2 : Water Effect
        // Index 3 : Thermo Effect
        // Index 4 : Interaction Effect
        // Index 5 : Discrete Change (Eating)
        [Header("Coefficient for EV dynamics")]
        public Coefficient foodCoefficient;
        public Coefficient waterCoefficient;
        public Coefficient thermoCoefficient;
        public Coefficient healthCoefficient;


        [Header("Collision System")]
        public float raysPerDirection;
        public float maxDistance;
        public float radialRange;
        public float damageConstant;

        private int maxSteps=0;
        private int stepsTaken = 0;

        public void InitializeAgent(ConfigLoader loader)
        {
                academy = Academy.Instance;

                configLoader = loader;
                if (configLoader == null)
                {
                        Debug.LogError("ConfigLoader is not set. Ensure ConfigLoader is initialized.");
                        return;
                }

                // Initialize DataRecorder
                dataRecorder = FindObjectOfType<DataRecorder>();
                if (dataRecorder != null)
                {
                        Debug.Log("InteroceptiveAgent: DataRecorder found.");
                        dataRecorder.targetAgent = this;
                        string configFolderName = FindObjectOfType<ConfigLoader>().mainConfig.configFolderName;
                        Debug.Log("InteroceptiveAgent: configFolderName: " + configFolderName);
                        dataRecorder.experimentType = configFolderName;
                        dataRecorder.episodeNumber = episodeCount;
                }

                // Load the agent configuration
                agentConfig = configLoader.LoadConfig<AgentConfig>(configFileName);
                if (agentConfig == null)
                {
                        Debug.LogError("Failed to load AgentConfig.");
                        return;
                }

                // Use the loaded configuration
                isAIControlled = configLoader.mainConfig.isAIControlled;
                singleTrial = agentConfig.singleTrial;
                initRandomAgentPosition = agentConfig.initRandomAgentPosition;
                initAgentPosition = agentConfig.initAgentPosition;
                initAgentAngle = agentConfig.initAgentAngle;
                randomPositionRange = agentConfig.randomPositionRange;
                moveSpeed = agentConfig.moveSpeed;
                turnSpeed = agentConfig.turnSpeed;
                autoEat = agentConfig.autoEat;
                eatingDistance = agentConfig.eatingDistance;
                rewardWindowSize = agentConfig.rewardWindowSize;
                averageReward = agentConfig.averageReward;
                currentReward = agentConfig.currentReward;
                countEV = agentConfig.countEV;
                foodLevelRange = agentConfig.foodLevelRange;
                resourceFoodValue = agentConfig.resourceFoodValue;
                startFoodLevel = agentConfig.startFoodLevel;
                waterLevelRange = agentConfig.waterLevelRange;
                resourceWaterValue = agentConfig.resourceWaterValue;
                startWaterLevel = agentConfig.startWaterLevel;
                thermoLevelRange = agentConfig.thermoLevelRange;
                startThermoLevel = agentConfig.startThermoLevel;
                healthLevelRange = agentConfig.healthLevelRange;
                startHealthLevel = agentConfig.startHealthLevel;
                useTouchObs = agentConfig.useTouchObs;
                useCollisionObs = agentConfig.useCollisionObs;
                useOlfactoryObs = agentConfig.useOlfactoryObs;
                olfactorySensorLength = agentConfig.olfactorySensorLength;
                useThermalObs = agentConfig.useThermalObs;
                relativeThermalObs = agentConfig.relativeThermalObs;
                foodCoefficient = agentConfig.foodCoefficient;
                waterCoefficient = agentConfig.waterCoefficient;
                thermoCoefficient = agentConfig.thermoCoefficient;
                healthCoefficient = agentConfig.healthCoefficient;
                raysPerDirection = agentConfig.raysPerDirection;
                maxDistance = agentConfig.maxDistance;
                radialRange = agentConfig.radialRange;
                damageConstant = agentConfig.damageConstant;
                maxSteps = agentConfig.maxSteps;  // Set the maximum number of steps per episode

                m_ResetParams = Academy.Instance.EnvironmentParameters; 
                // if (playRecorder.GetComponent<CaptureScreenShot>().recordEnable)
                // {
                //         playRecorder.GetComponent<CaptureScreenShot>().CreateRecordDirectory();
                // }

                // Update the CameraSwitcher if available
                if (camareManager != null)
                {
                        camareManager.isAIControlled = isAIControlled;
                }
                // Set initial position and rotation
                if (initRandomAgentPosition)
                {
                        transform.position = RandomPosition(randomPositionRange);
                        transform.eulerAngles = RandomRotation();
                }
                else
                {
                        transform.position = initAgentPosition.ToVector3();
                        transform.eulerAngles = initAgentAngle.ToVector3();
                }

                m_AgentRb = GetComponent<Rigidbody>();
                if (m_AgentRb == null)
                {
                        Debug.LogError("Rigidbody component not found.");
                        return;
                }
                m_AgentRb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                eatenResource = false;

                this.agentPosition = this.transform.position;
                this.agentRotation = this.transform.eulerAngles;

                this.resourceLevels = new float[this.countEV];
                this.oldResourceLevels = new float[this.countEV];

                if (this.useOlfactoryObs)
                {
                        this.olfactoryObservation = new float[this.olfactoryFeatureSize];
                }
                if (this.useThermalObs)
                {
                        this.thermoObservation = new float[8];
                }
                if (this.useCollisionObs)
                {
                        // isCollided = true;
                        this.collisionObservation = new float[this.collisionFeatureSize];
                }
                if (this.useTouchObs)
                {
                        this.touchObservation = 0.0f;
                }
                
                recentRewards = new Queue<float>(rewardWindowSize);
                
                ResetAgent();
                // Debug.Log("InteroceptiveAgent: InitializeAgent");
        }
        public override void OnEpisodeBegin()
        {
                // IMPORTANT: Always ensure dataRecorder.OnEpisodeEnd() was called for the previous episode before calling OnEpisodeBegin() for a new one.
                stepsTaken = 1;
                if (dataRecorder != null)
                {
                        dataRecorder.OnEpisodeBegin();
                }
                episodeCount = academy.EpisodeCount;
                if (!isEnvironmentReady)
                {
                        Debug.LogWarning("Environment is not ready. Skipping OnEpisodeBegin.");
                        return;
                }

                if (isFirstEpisode)
                {
                        isFirstEpisode = false;  
                        // Debug.Log("InteroceptiveAgent: The First Episode Started.");
                        return;
                }

                // Find the MasterInitializer and reset the scene
                var masterInitializer = FindObjectOfType<MasterInitializer>();
                if (masterInitializer != null)
                {
                        masterInitializer.ResetScene();
                }
                else
                {
                        Debug.LogError("MasterInitializer not found in the scene.");
                }

                var dayAndNight = FindObjectOfType<DayAndNight>();
                var thermoGridSpawner = FindObjectOfType<ThermoGridSpawner>();
                // Adjust temperature based on the current day/night state
                if (dayAndNight != null)
                {
                        if (dayAndNight.CurrentDayNightState == DayAndNight.DayNightState.Night)
                        {
                                float temperatureChange = dayAndNight.nightTemperatureChange;
                                if (thermoGridSpawner != null)
                                {
                                        thermoGridSpawner.AdjustTemperature(temperatureChange);
                                }
                                else
                                { 
                                        Debug.LogError("ThermoGridSpawner not found in the scene.");
                                }
                        }
                }
                else
                {
                        Debug.LogError("DayAndNight not found in the scene.");
                }

                ResetAgent();
                Debug.Log("InteroceptiveAgent: OnEpisodeBegin");
                // Debug.Log("InteroceptiveAgent: " + episodeCount + " Episode Started.");
        }
        public void ResetAgent()
        {
                 // Reset agent
                m_AgentRb.velocity = Vector3.zero;

                eatenResource = false;

                // Set initial position and rotation
                if (initRandomAgentPosition)
                {
                        // transform.position = RandomPosition(randomPositionRange);
                        SetRandomPosition();
                        transform.eulerAngles = RandomRotation();
                }
                else
                {
                        transform.position = initAgentPosition.ToVector3();
                        transform.eulerAngles = initAgentAngle.ToVector3();
                }

                // Reset energy
                for (int i = 0; i < this.countEV; i++)
                {
                        if (i == 0)
                        {
                                this.resourceLevels[i] = startFoodLevel;
                                this.oldResourceLevels[i] = this.resourceLevels[i];
                        }
                        else if (i == 1)
                        {
                                this.resourceLevels[i] = startWaterLevel;
                                this.oldResourceLevels[i] = this.resourceLevels[i];
                        }
                        else if (i == 2)
                        {
                                this.resourceLevels[i] = startThermoLevel;
                                this.oldResourceLevels[i] = this.resourceLevels[i];
                        }
                        else if (i == 3)
                        {
                                this.resourceLevels[i] = startHealthLevel;
                                this.oldResourceLevels[i] = this.resourceLevels[i];
                        }
                }

                // Reset olfactory
                if (this.useOlfactoryObs)
                {
                        for (int i = 0; i < olfactoryFeatureSize; i++)
                        {
                                this.olfactoryObservation[i] = 0;
                        }
                }
                

                if (this.useThermalObs)
                {
                        for (int i = 0; i < 8; i++)
                        {
                                this.thermoObservation[i] = 0;
                        }

                        // bodyTemp = 0;
                        // bodyTemp = startThermoLevel;

                        // thermoSensorCenter.GetComponent<ThermalSensing>().SetThermalSense(0);
                        thermoSensorForward.GetComponent<ThermalSensing>().SetThermalSense(0);
                        thermoSensorBackward.GetComponent<ThermalSensing>().SetThermalSense(0);
                        thermoSensorLeft.GetComponent<ThermalSensing>().SetThermalSense(0);
                        thermoSensorRight.GetComponent<ThermalSensing>().SetThermalSense(0);
                        thermoSensorForwardLeft.GetComponent<ThermalSensing>().SetThermalSense(0);
                        thermoSensorForwardRight.GetComponent<ThermalSensing>().SetThermalSense(0);
                        thermoSensorBackwardLeft.GetComponent<ThermalSensing>().SetThermalSense(0);
                        thermoSensorBackwardRight.GetComponent<ThermalSensing>().SetThermalSense(0);
                        // // Find all spotlight hotzones and set their hotzones
                        // SpotlightHotzone[] spotlightHotzones = FindObjectsOfType<SpotlightHotzone>(); // Get all spotlight hotzones
                        // foreach (var spotlightHotzone in spotlightHotzones)
                        // {
                        //         spotlightHotzone.ApplySpotlightHotzone(); // Set hotzone for each spotlight
                        // }
                        // Reset heatmap
                        // heatMap.GetComponent<HeatMap>().EpisodeHeatMap();
                }

                if (useCollisionObs)
                {
                        // this.collisionObservation = 0.0f;
                        for (int i = 0; i < 10; i++)
                        {
                                this.collisionObservation[i] = 0;
                        }                                            

                }

                if (useTouchObs)
                {
                        this.touchObservation = 0.0f;
                }
        }

        private void SetRandomPosition()
        {
                Vector3 position;
                int attempts = 0;
                bool validPosition = false;

                do
                {
                        position = RandomPosition(randomPositionRange);
                        attempts++;
                        validPosition = !OverlapUtility.IsOverlapping(position, gameObject, transform.localScale);
                } while (!validPosition && attempts < 100);

                if (validPosition)
                {
                        transform.position = position;
                }
                else
                {
                        Debug.LogWarning($"Could not find a valid position for agent after {attempts} attempts.");
                        transform.position = initAgentPosition.ToVector3();
                }
        }

        public override void CollectObservations(VectorSensor sensor)
        {
                sensor.AddObservation(resourceLevels);
                if (useOlfactoryObs)
                {
                        OlfactorySensingRangeVisualize();
                        sensor.AddObservation(olfactoryObservation);
                }
                if (useThermalObs)
                {
                        sensor.AddObservation(thermoObservation);
                }
                if (useCollisionObs)
                {
                        sensor.AddObservation(collisionObservation);
                }
                if (useTouchObs)
                {
                        sensor.AddObservation(touchObservation);
                }
                sensor.AddObservation(agentPosition);
                sensor.AddObservation(agentRotation);
                
                // Debug.Log("Eat Food : " + countFood);
                sensor.AddObservation(countFood);
                
                // Debug.Log("Drink Water : " + countWater);
                sensor.AddObservation(countWater);

                // Debug.Log("Collide Obstacle : " + countCollision);
                sensor.AddObservation(countCollision);
        }

        // Method to execute actions received from the brain (policy)
        public override void OnActionReceived(ActionBuffers actions)
        {       
                // Advance day/night cycle per ML-Agents step
                var dayAndNight = FindObjectOfType<DayAndNight>();
                if (dayAndNight != null)
                {
                        dayAndNight.StepUpdate();
                }
                // Debug.Log("stepsTaken: " + stepsTaken);

                stepsTaken++;
                countFood = 0.0f;
                countWater = 0.0f;
                countCollision = 0.0f;
                resourceConsumedInStep = false;  // Reset consumption tracking
                consumedResourceType = "none";   // Reset resource type

                if (playRecorder.GetComponent<CaptureScreenShot>().recordEnable)
                {
                        playRecorder.GetComponent<CaptureScreenShot>().CaptureImage();
                }

                this.agentPosition = this.transform.position;
                this.agentRotation = this.transform.eulerAngles;

                if (eatenResource)
                {
                        if (eatenResourceTag.ToLower() == "food")
                        {
                                foodCoefficient.change_5 = 1.0f;
                                countFood = 1.0f;
                                resourceConsumedInStep = true;
                                consumedResourceType = eatenResourceTag;  // Use actual tag
                                if (dataRecorder != null)
                                {
                                        dataRecorder.RecordAction("Eat_Food");
                                        dataRecorder.RecordFoodConsumed();
                                }
                        }
                        if (eatenResourceTag.ToLower() == "water" || eatenResourceTag.ToLower() == "pond")
                        {
                                waterCoefficient.change_5 = 1.0f;
                                countWater = 1.0f;
                                resourceConsumedInStep = true;
                                consumedResourceType = eatenResourceTag;  // Use actual tag
                                if (dataRecorder != null)
                                {
                                        dataRecorder.RecordAction("Drink_Water");
                                        dataRecorder.RecordWaterConsumed();
                                }
                        }

                        if (singleTrial)
                        {
                                if (dataRecorder != null)
                                {
                                        dataRecorder.SetEpisodeEndType("ResourceConsumed");  // Set episode end type
                                        // dataRecorder.RecordFinalStep();
                                        dataRecorder.OnEpisodeEnd();
                                }
                                EndEpisode();
                        }
                }

                // EV (Food, Water, Thermo) Update
                FoodUpdate(foodCoefficient.change_0, foodCoefficient.change_1, foodCoefficient.change_2, foodCoefficient.change_3, foodCoefficient.change_4, foodCoefficient.change_5);
                WaterUpdate(waterCoefficient.change_0, waterCoefficient.change_1, waterCoefficient.change_2, waterCoefficient.change_3, waterCoefficient.change_4, waterCoefficient.change_5);
                HealthUpdate(healthCoefficient.change_0, healthCoefficient.change_1, healthCoefficient.change_2, healthCoefficient.change_3, healthCoefficient.change_4, healthCoefficient.change_5);

                // Olfactory Observation
                if (this.useOlfactoryObs)
                {
                        OlfactoryObserving();
                }

                if (this.useThermalObs)
                {
                        // ThermalChanging();
                        ThermalObserving();
                        ThermoUpdate(thermoCoefficient.change_0, thermoCoefficient.change_1, thermoCoefficient.change_2, thermoCoefficient.change_3, thermoCoefficient.change_4);
                }

                if (this.useCollisionObs)
                {
                        CollisionObserving(); 
                }

                if (this.useTouchObs)
                {
                        TouchObserving();
                }

                bool checkFoodLevel = (this.foodLevelRange.max < this.resourceLevels[0] || this.resourceLevels[0] < this.foodLevelRange.min);
                bool checkWaterLevel = (this.waterLevelRange.max < this.resourceLevels[1] || this.resourceLevels[1] < this.waterLevelRange.min);
                bool checkThermoLevel = false;
                if (this.useThermalObs)
                {
                        checkThermoLevel = (this.thermoLevelRange.max < this.resourceLevels[2] || this.resourceLevels[2] < this.thermoLevelRange.min);
                }

                // bool checkHealth = (this.resourceLevels[3] < this.healthLevelRange.min);
                bool checkHealth = (this.resourceLevels[3] > this.healthLevelRange.max);

                if (checkFoodLevel || checkWaterLevel || checkThermoLevel || checkHealth)
                {        
                        if (dataRecorder != null)
                        {
                                // Determine the specific reason for episode end
                                if (checkFoodLevel) dataRecorder.SetEpisodeEndType("FoodLevelOutOfRange");
                                else if (checkWaterLevel) dataRecorder.SetEpisodeEndType("WaterLevelOutOfRange");
                                else if (checkThermoLevel) dataRecorder.SetEpisodeEndType("ThermoLevelOutOfRange");
                                else if (checkHealth) dataRecorder.SetEpisodeEndType("HealthLevelTooLow");
                                
                                // IMPORTANT: Always call OnEpisodeEnd before starting a new episode
                                // dataRecorder.RecordFinalStep();
                                dataRecorder.OnEpisodeEnd();
                        }
                        EndEpisode();
                }
                if (this.resourceLevels[3] > healthLevelRange.max)
                {
                        this.resourceLevels[3] = healthLevelRange.max;
                }

                int action = actions.DiscreteActions[0];
                MoveAgent(action);

                // Record action in metrics
                if (dataRecorder != null)
                {
                        string actionName = action switch
                        {
                                0 => "None",
                                1 => "Forward",
                                2 => "Left",
                                3 => "Right",
                                4 => "Eat",
                                _ => "Unknown"
                        };
                        dataRecorder.RecordAction(actionName);
                        dataRecorder.RecordStep(); // Record the step after all updates
                }

                // Synchronize Predator's action
                Predator[] predators = FindObjectsOfType<Predator>();
                foreach (var predator in predators)
                {
                    predator.TakeAction();
                }

                // Calculate reward
                currentReward = CalculateReward();
                // Track recent rewards for moving average
                if (recentRewards.Count >= rewardWindowSize)
                {
                        recentRewards.Dequeue(); // Remove oldest reward
                }
                recentRewards.Enqueue(currentReward);
                // Update average reward
                averageReward = recentRewards.Average();
                // Apply the reward to ML-Agent
                AddReward(currentReward);

                // Reset eating state as default
                eatenResource = false;
                eatenResourceTag = "none";
                foodCoefficient.change_5 = 0.0f;
                waterCoefficient.change_5 = 0.0f;

                for (int i = 0; i < this.countEV; i++)
                {
                        oldResourceLevels[i] = resourceLevels[i];
                }

                if (maxSteps>0 && stepsTaken >= maxSteps)
                {
                        if (dataRecorder != null)
                        {
                                dataRecorder.SetEpisodeEndType("MaxStepReached");
                        // dataRecorder.RecordFinalStep();
                        // IMPORTANT: Always call OnEpisodeEnd before starting a new episode
                        dataRecorder.OnEpisodeEnd();
                        }
                        EndEpisode();
                        Debug.Log("MaxStep reached");
                }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
                var discreteActionsOut = actionsOut.DiscreteActions;
                discreteActionsOut[0] = 0;
                if (Input.GetKey(KeyCode.UpArrow))
                {
                        discreteActionsOut[0] = 1;
                }
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                        discreteActionsOut[0] = 2;
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                        discreteActionsOut[0] = 3;
                }
                if (Input.GetKey(KeyCode.Space))
                {
                        discreteActionsOut[0] = 4;
                }
        }

        public void MoveAgent(int action)
        {
                var dirToGo = Vector3.zero;
                var rotateDir = Vector3.zero;

                // Get the action index for movement
                // int action = Mathf.FloorToInt(act[0]);
                /*** Action Category
                 * 0 : None 
                 * 1 : Forward  
                 * 2 : Left
                 * 3 : Right
                 * 4 : Eat
                 * ***/

                this.isAgentActionEat = false;
                switch (action)
                {
                        case 0:
                                m_AgentRb.velocity = Vector3.zero;
                                break;
                        case 1:
                                dirToGo = transform.forward;
                                m_AgentRb.velocity = dirToGo * moveSpeed;
                                break;
                        case 2:
                                transform.Rotate(-transform.up, Time.fixedDeltaTime * turnSpeed);
                                m_AgentRb.velocity = Vector3.zero;
                                break;
                        case 3:
                                transform.Rotate(transform.up, Time.fixedDeltaTime * turnSpeed);
                                m_AgentRb.velocity = Vector3.zero;
                                break;
                        case 4:
                                this.isAgentActionEat = true;
                                m_AgentRb.velocity = Vector3.zero;
                                break;
                }
        }

        private void OlfactoryObserving()
        {
                for (int i = 0; i < olfactoryFeatureSize; i++)
                {
                        olfactoryObservation[i] = 0;
                }

                // If the agent size changes, check the z value
                Vector3 SpherePos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + 0.5f);
                // Detect layer number 8
                Collider[] olfactoryTargets = Physics.OverlapSphere(SpherePos, olfactorySensorLength, 1 << 8);

                int j = 0;
                foreach (Collider other in olfactoryTargets)
                {
                        ResourceProperty resource = other.gameObject.GetComponent<ResourceProperty>();
                        if (resource.CompareTag("pond") || resource.CompareTag("water") || resource.CompareTag("food"))
                        {
                                j += 1;
                                float resourceDistance = Vector3.Distance(SpherePos, resource.transform.position);
                                for (int i = 0; i < olfactoryFeatureSize; i++)
                                {
                                        olfactoryObservation[i] += resource.ResourceP[i] * (1 / resourceDistance);
                                }
                        }
                }
        }

        private void OlfactorySensingRangeVisualize()
        {
                GameObject olfactorySensingRange = this.transform.Find("SensingRange").gameObject;
                Vector3 newScale = new Vector3(this.olfactorySensorLength, this.olfactorySensorLength, this.olfactorySensorLength);
                olfactorySensingRange.transform.localScale = newScale;
        }

        public float[] ThermalObserving()
        {
                if (relativeThermalObs)
                {
                        thermoObservation[0] = thermoSensorForward.GetComponent<ThermalSensing>().GetThermalSense() - this.resourceLevels[2];
                        thermoObservation[1] = thermoSensorBackward.GetComponent<ThermalSensing>().GetThermalSense() - this.resourceLevels[2];
                        thermoObservation[2] = thermoSensorLeft.GetComponent<ThermalSensing>().GetThermalSense() - this.resourceLevels[2];
                        thermoObservation[3] = thermoSensorRight.GetComponent<ThermalSensing>().GetThermalSense() - this.resourceLevels[2];
                        thermoObservation[4] = thermoSensorForwardLeft.GetComponent<ThermalSensing>().GetThermalSense() - this.resourceLevels[2];
                        thermoObservation[5] = thermoSensorForwardRight.GetComponent<ThermalSensing>().GetThermalSense() - this.resourceLevels[2];
                        thermoObservation[6] = thermoSensorBackwardLeft.GetComponent<ThermalSensing>().GetThermalSense() - this.resourceLevels[2];
                        thermoObservation[7] = thermoSensorBackwardRight.GetComponent<ThermalSensing>().GetThermalSense() - this.resourceLevels[2];
                }
                else
                {
                        thermoObservation[0] = thermoSensorForward.GetComponent<ThermalSensing>().GetThermalSense();
                        thermoObservation[1] = thermoSensorBackward.GetComponent<ThermalSensing>().GetThermalSense();
                        thermoObservation[2] = thermoSensorLeft.GetComponent<ThermalSensing>().GetThermalSense();
                        thermoObservation[3] = thermoSensorRight.GetComponent<ThermalSensing>().GetThermalSense();
                        thermoObservation[4] = thermoSensorForwardLeft.GetComponent<ThermalSensing>().GetThermalSense();
                        thermoObservation[5] = thermoSensorForwardRight.GetComponent<ThermalSensing>().GetThermalSense();
                        thermoObservation[6] = thermoSensorBackwardLeft.GetComponent<ThermalSensing>().GetThermalSense();
                        thermoObservation[7] = thermoSensorBackwardRight.GetComponent<ThermalSensing>().GetThermalSense();
                }
                return thermoObservation;
        }


        public void CollisionObserving()
        {
                objectRaycast = GetComponent<ObjectRaycast>();

                collisionObservation[0] = objectRaycast.collisionObservation[0];
                collisionObservation[1] = objectRaycast.collisionObservation[1];
                collisionObservation[2] = objectRaycast.collisionObservation[2];
                collisionObservation[3] = objectRaycast.collisionObservation[3];
                collisionObservation[4] = objectRaycast.collisionObservation[4];
                collisionObservation[5] = objectRaycast.collisionObservation[5];
                collisionObservation[6] = objectRaycast.collisionObservation[6];
                collisionObservation[7] = objectRaycast.collisionObservation[7];
                collisionObservation[8] = objectRaycast.collisionObservation[8];
                collisionObservation[9] = objectRaycast.collisionObservation[9];
                
                // this.resourceLevels[3] -= objectRaycast.damage;
        }
        public void TouchObserving()
        {
                if (isTouched)
                {
                        touchObservation = 0.0f;
                }
                else
                {
                        touchObservation = 0.0f;
                }
        }

        // Update considering the interaction between EVs (Essential Variables)
        public void FoodUpdate(float changeFood_0, float changeFood_1, float changeFood_2, float changeFood_3, float changeFood_4, float changeFood_5)
        {
                this.resourceLevels[0] = this.resourceLevels[0] +
                                        changeFood_0 * foodLevelRange.max * Time.fixedDeltaTime +
                                        changeFood_1 * (this.oldResourceLevels[0] + 15) * Time.fixedDeltaTime +
                                        changeFood_2 * (this.oldResourceLevels[1] + 15) * Time.fixedDeltaTime +
                                        changeFood_3 * (this.oldResourceLevels[2] + 15) * Time.fixedDeltaTime +
                                        changeFood_4 * (CalculateInteraction(oldResourceLevels[0], oldResourceLevels[1], oldResourceLevels[2])) * Time.fixedDeltaTime +
                                        changeFood_5 * resourceFoodValue;
        }

        public void WaterUpdate(float changeWater_0, float changeWater_1, float changeWater_2, float changeWater_3, float changeWater_4, float changeWater_5)
        {
                this.resourceLevels[1] = this.resourceLevels[1] +
                                        changeWater_0 * waterLevelRange.max * Time.fixedDeltaTime +
                                        changeWater_1 * (this.oldResourceLevels[0] + 15) * Time.fixedDeltaTime +
                                        changeWater_2 * (this.oldResourceLevels[1] + 15) * Time.fixedDeltaTime +
                                        changeWater_3 * (this.oldResourceLevels[2] + 15) * Time.fixedDeltaTime +
                                        changeWater_4 * (CalculateInteraction(oldResourceLevels[0], oldResourceLevels[1], oldResourceLevels[2])) * Time.fixedDeltaTime +
                                        changeWater_5 * resourceWaterValue;
        }

        public void ThermoUpdate(float changeBody_0, float changeBody_1, float changeBody_2, float changeBody_3, float changeBody_4)
        {
                float surroundTemp = 0.0f;
                for (int i = 0; i < thermoObservation.Length; i++)
                {
                        if (relativeThermalObs)
                        { surroundTemp += thermoObservation[i]; }
                        else
                        { surroundTemp += thermoObservation[i] - this.resourceLevels[2]; }

                }

                this.resourceLevels[2] = this.resourceLevels[2] +
                            changeBody_0 * surroundTemp * Time.fixedDeltaTime +
                            changeBody_1 * (this.oldResourceLevels[0] + 15) * Time.fixedDeltaTime +
                            changeBody_2 * (this.oldResourceLevels[1] + 15) * Time.fixedDeltaTime +
                            changeBody_3 * (this.oldResourceLevels[2] + 15) * Time.fixedDeltaTime +
                            changeBody_4 * (CalculateInteraction(oldResourceLevels[0], oldResourceLevels[1], oldResourceLevels[2])) * Time.fixedDeltaTime;
                // this.resourceLevels[2] = this.bodyTemp;
        }

        public void HealthUpdate(float changeHealth_0, float changeHealth_1, float changeHealth_2, float changeHealth_3, float changeHealth_4, float changeHealth_5)
        {
                this.resourceLevels[3] = this.resourceLevels[3] +
                                        changeHealth_0 * healthLevelRange.max * Time.fixedDeltaTime +
                                        changeHealth_1 * (this.oldResourceLevels[0] + 15) * Time.fixedDeltaTime +
                                        changeHealth_2 * (this.oldResourceLevels[1] + 15) * Time.fixedDeltaTime +
                                        changeHealth_3 * (this.oldResourceLevels[2] + 15) * Time.fixedDeltaTime +
                                        changeHealth_4 * (CalculateInteraction(oldResourceLevels[0], oldResourceLevels[1], oldResourceLevels[2])) * Time.fixedDeltaTime +
                                        changeHealth_5 * resourceWaterValue;
        }

        public float CalculateInteraction(float food, float water, float bodyTemp)
        {
                return 1.0f;
        }

        private float CalculateReward()
        {
                float[] setPoints = new float[] { 0f, 0f, 0f, 0f };
                // setPoints[0] = startFoodLevel;
                // setPoints[1] = startWaterLevel;
                // setPoints[2] = startThermoLevel;
                // setPoints[3] = startHealthLevel;

                setPoints[0] = 0;
                setPoints[1] = 0;
                setPoints[2] = 0;
                setPoints[3] = 0;

                // float[] maxDeviations = new float[] { startFoodLevel - foodLevelRange.min, 
                //                                 startWaterLevel - waterLevelRange.min, 
                //                                 startThermoLevel - thermoLevelRange.min, 
                //                                 healthLevelRange.max - healthLevelRange.min };

                float[] maxDeviations = new float[] { foodLevelRange.max,
                                                     waterLevelRange.max,
                                                     thermoLevelRange.max,
                                                     healthLevelRange.max };

                // Calculate normalized Euclidean distance
                float sumSquares = 0f;
                for (int i = 0; i < resourceLevels.Length; i++)
                {
                        float deviation = resourceLevels[i] - setPoints[i];
                        float normalizedDeviation = deviation / maxDeviations[i];
                        sumSquares += normalizedDeviation * normalizedDeviation;
                }

                float reward = Mathf.Sqrt(sumSquares);

                // Invert reward (higher distance = lower reward)
                return -reward;
        }

        private Vector3 RandomPosition(PositionRange positionRange)
        {
        return new Vector3(
                Random.Range(positionRange.xMin, positionRange.xMax),
                Random.Range(positionRange.yMin, positionRange.yMax),
                Random.Range(positionRange.zMin, positionRange.zMax)
        );
        }

        private Vector3 RandomRotation()
        {
        return new Vector3(
                0, // Assuming we only want to randomize the y-axis rotation
                Random.Range(0f, 360f),
                0
        );
        }
}

[System.Serializable]
public class AgentConfig
{
    public bool singleTrial;
    public bool initRandomAgentPosition;
    public ThreeDVector initAgentPosition;
    public ThreeDVector initAgentAngle;
    public PositionRange randomPositionRange;
    public float moveSpeed;
    public float turnSpeed;
    public bool autoEat;
    public float eatingDistance;
    public int rewardWindowSize;
    public float averageReward;
    public float currentReward;
    public int countEV;
    public EVRange foodLevelRange;
    public float resourceFoodValue;
    public float startFoodLevel;
    public EVRange waterLevelRange;
    public float resourceWaterValue;
    public float startWaterLevel;
    public EVRange thermoLevelRange;
    public float startThermoLevel;
    public EVRange healthLevelRange;
    public float startHealthLevel;
    public bool useTouchObs;
    public bool useCollisionObs;
    public bool useOlfactoryObs;
    public float olfactorySensorLength;
    public bool useThermalObs;
    public bool relativeThermalObs;
    public Coefficient foodCoefficient;
    public Coefficient waterCoefficient;
    public Coefficient thermoCoefficient;
    public Coefficient healthCoefficient;
    public float raysPerDirection;
    public float maxDistance;
    public float radialRange;
    public float damageConstant;
    public int maxSteps;  // Maximum number of steps per episode
}
