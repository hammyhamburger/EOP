using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 MovementInput;
    public Vector3 AimVector;
    public NetworkBool IsJumpPressed;
    public NetworkBool IsWalkHeld;
    public NetworkId TargetId;
}
