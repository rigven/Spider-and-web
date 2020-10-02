using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] private Vector3 _force = new Vector3(0f, -0.2f, 0f);

    public Vector3 GetForceGravity()
    {
        return _force;
    }
}
