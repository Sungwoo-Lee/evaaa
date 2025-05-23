﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attached to the ResourceLevelSetting GameObject, which is a child of the Canvas GameObject
public class ResourceUI : MonoBehaviour
{
    public Slider FoodLevel;
    public Slider WaterLevel;
    public Slider ThermoLevel;
    public Slider HpLevel;

    public Image Foodhandle;
    public Image Waterhandle;
    public Image Thermohandle;
    public Image Hphandel;

    public GameObject FoodText;
    public GameObject WaterText;
    public GameObject ThermoText;
    public GameObject HpText;

    public InteroceptiveAgent agent;

    protected float foodLevel;
    protected float waterLevel;
    protected float thermoLevel;
    private float hpLevel;

    protected void Start()
    {
        WaterLevel.value = 0;
        FoodLevel.value = 0;
        Waterhandle.color = Color.blue;
        Foodhandle.color = Color.red;

        HpLevel.value = 100.0f;
        Hphandel.color = Color.green;

        if (agent.useThermalObs)
        {
            ThermoLevel.value = 0;
            Thermohandle.color = Color.yellow;
        }
        else
        {
            ThermoLevel.gameObject.SetActive(false);
            ThermoText.GetComponent<TextMeshProUGUI>().enabled = false;
        }

    }

    // protected void Update()
    protected void FixedUpdate()
    {
        foodLevel = agent.resourceLevels[0];
        waterLevel = agent.resourceLevels[1];

        WaterLevel.value = waterLevel;
        FoodLevel.value = foodLevel;

        hpLevel = agent.resourceLevels[3];
        HpLevel.value = hpLevel;

        if (agent.useThermalObs)
        {
            thermoLevel = agent.resourceLevels[2];
            ThermoLevel.value = thermoLevel;
        }

        // if (blueLevel >= 0) { Bluehandle.color = Color.green; } else { Bluehandle.color = Color.red; }
        // if (redLevel >= 0) { Redhandle.color = Color.green; } else { Redhandle.color = Color.red; }
        // if (yellowLevel >= 0) { Yellowhandle.color = Color.green; } else { Yellowhandle.color = Color.red; }
    }
}
