using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Targetable : MonoBehaviour
{
    private GameManager _gameManager;
    private NetworkObject _networkObject;
    
    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _networkObject = GetComponentInChildren<NetworkObject>();
    }

    private void Start()
    {
        _gameManager.targetNetworkObjDict.Add(_networkObject.Id, _networkObject);
    }

    public enum EntityType {Player, Enemy}
    public EntityType entityType;
}
