using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WaveAction
{
    public string type; // Action type (e.g., spawn, wait)
    public string enemyType; // Base enemy type name
    public string prototype; // Optional prototype name
    public Dictionary<string, object> variations; // Collection of fields to update the prototype
    public int amount; // Number of enemies to spawn
    public float duration; // Duration of the action

    public WaveAction(
        string type, 
        string enemyType, 
        string prototype, 
        Dictionary<string, object> variations, 
        int amount, 
        float duration)
    {
        this.type = type;
        this.enemyType = enemyType;
        this.prototype = prototype;
        this.variations = variations ?? new Dictionary<string, object>(); // Initialize if null
        this.amount = amount;
        this.duration = duration;
    }

    // Helper method to check if a prototype is used
    public bool UsesPrototype()
    {
        return !string.IsNullOrEmpty(prototype);
    }
}

