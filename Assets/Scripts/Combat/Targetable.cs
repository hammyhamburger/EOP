using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Targetable : MonoBehaviour
{
    private GameManager _gameManager;
    public int targetId;
    
    private void Awake()
    {
        _gameManager = FindObjectOfType<GameManager>();
        targetId = _gameManager.GetNextTargetId();
        _gameManager.targetList.Add(this.gameObject);
    }

    public enum EntityType {Player, Enemy}
    public EntityType entityType;
}
