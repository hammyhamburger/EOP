using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spell", menuName = "Spell")]
public class Spell : ScriptableObject
{
    public enum Type {Magical, Physical};
    public Type SpellType;
    public string SpellName;
    public float BaseDamageValue = 0f;
    public float HealingValue = 0f;
    public float BuffMultiplier = 1f;
    public float Cooldown = 0f;
    public float CastTime = 0f;
    public float Range = 0f;
    
    public bool NeedsTarget = true;
    // Add animation variable?
}
