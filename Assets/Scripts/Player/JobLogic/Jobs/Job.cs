using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Job", menuName = "Job")]
public class Job : ScriptableObject
{
    public string JobName = "jobless";
    public int Health = 100;
    public int Resource = 0;
    public float PhysicalRes = 0;
    public float MagicRes = 0;
    public List<Spell> Spells;
}
