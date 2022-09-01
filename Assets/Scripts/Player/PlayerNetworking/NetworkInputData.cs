using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

/*
I think this can be optimized but I'm lazy. I'm not sure how big a vector is compared to
an int or if that makes a difference, but as of right now this performs..adequately..
*/
public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public Vector3 aimVector;
    public NetworkBool isJumpPressed;
    public NetworkBool isWalkHeld;
    public NetworkId targettedEntity;
}
