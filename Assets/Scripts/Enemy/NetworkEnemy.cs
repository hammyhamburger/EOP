using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Fusion;

public class NetworkEnemy : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {
        transform.name = "Boss 1";
    }
}
