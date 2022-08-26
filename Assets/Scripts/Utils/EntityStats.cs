using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStats : MonoBehaviour
{
    [SerializeField] public int maxHealth;
    public int health;
    public int maxMana;
    public int mana;

    void Awake()
    {
        health = maxHealth;
        mana = maxMana;
    }
    void Update()
    {
        
    }

    public void SubtractHealth(int damage)
    {
        health -= damage;
    }

    public void SubtractMana(int manaBurn)
    {
        mana -= manaBurn;
    }

}
