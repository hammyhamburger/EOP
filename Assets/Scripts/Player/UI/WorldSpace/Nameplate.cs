using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Nameplate : MonoBehaviour
{
    void Update()
    {
        Camera localCamera = Camera.main;

        transform.LookAt(transform.position + localCamera.transform.rotation * Vector3.forward,
                localCamera.transform.rotation * Vector3.up);
    }
}
