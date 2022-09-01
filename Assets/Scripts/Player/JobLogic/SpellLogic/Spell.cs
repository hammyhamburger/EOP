using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Spell : MonoBehaviour
{
    public string spellName;
    public float damageValue = 0f;
    public float healingValue = 0f;
    public float buffMultiplier = 1f;
    public float cooldown = 0f;
    public float castTime = 0f;
    public float range = 0f;
    public bool needsTarget = true;
}
