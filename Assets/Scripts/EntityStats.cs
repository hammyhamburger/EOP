using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EntityStats : NetworkBehaviour
{
    public enum EntityType {Player, Enemy}
    public EntityType entityType;

    public float MoveSpeed;
    
    [Networked]
    public int Health { get; set; }
    public int Mana;

    // Targeting
    [Networked]
    public NetworkObject Target { get; set; }

    [Networked]
    public NetworkObject TargetOfTarget { get; set; }

    void Update()
    {
        if (Target) 
        {
            if (Target.GetComponentInChildren<EntityStats>().Target)
                TargetOfTarget = Target.GetComponentInChildren<EntityStats>().Target; // Get target's target and set it to ours
            else 
                TargetOfTarget = default;
        }
        else 
            TargetOfTarget = default; // No target means no target of target
    }
}
