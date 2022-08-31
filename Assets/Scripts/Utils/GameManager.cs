using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int maxTargetID = 0;
    public List<GameObject> targetList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetNextTargetId()
    {
        maxTargetID += 1;
        return maxTargetID;
    }
}
