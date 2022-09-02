using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class EntityStats : MonoBehaviour
{
    public enum EntityType {Player, Enemy}
    public EntityType entityType;

    public float MoveSpeed;
    public int Health;
    public int Mana;

    // Targeting
    [Networked]
    public NetworkObject Target { get; set; }

    [Networked]
    public NetworkObject TargetOfTarget { get; set; }

    void Start()
    {
        FindObjectOfType<GameManager>().targetNetworkObjDict.Add(
            GetComponentInChildren<NetworkObject>().Id, GetComponentInChildren<NetworkObject>());
    }

    void Update()
    {
    }

    public void TargetEntity(NetworkId targettedEntity)
    {
        Target = FindObjectOfType<GameManager>().targetNetworkObjDict[targettedEntity];
        if (Target != null) // Target is targettable
        {
            if (Target.GetComponent<EntityStats>()) // Target is a player
            {
                if (Target.GetComponent<EntityStats>().Target != null) // Target has a target
                {
                    TargetOfTarget = Target.GetComponent<EntityStats>().Target;
                }
            }
            else TargetOfTarget = null; // Null out old target of target
        }
        else TargetOfTarget = null; // No target means no target of target
            
    }

}
