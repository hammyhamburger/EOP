using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GameManager : MonoBehaviour
{
    public Dictionary<NetworkId, NetworkObject> targetNetworkObjDict = 
        new Dictionary<NetworkId, NetworkObject>();

    NetworkObject defaultNo;
    NetworkId defaultId;

    void Awake()
    {
        targetNetworkObjDict.Add(defaultId, defaultNo);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
