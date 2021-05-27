using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class Shootable : MonoBehaviour
{
    public abstract void OnBeingShot();
    public abstract void Respawn();

    private void Awake()
    {
        gameObject.tag = "Shootable";
        gameObject.layer = LayerMask.NameToLayer("Goal");
    }
}
