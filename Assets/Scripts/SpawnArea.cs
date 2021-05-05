using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnArea : MonoBehaviour
{
    private void Start()
    {
        int layerMask = LayerMask.NameToLayer("SpawnArea");
        gameObject.layer = layerMask;
        GetComponent<Renderer>().material.color = Color.clear;
    }
}
