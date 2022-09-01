using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Job))]
[RequireComponent(typeof(Spell))]
public class Mage : MonoBehaviour
{
    Spell fireball = new Spell();
    void Start()
    {
        Debug.Log(fireball.spellName);
    }
}
