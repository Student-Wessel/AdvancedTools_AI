using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnviornemntSetGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject enviornemntPrefab;

    [SerializeField]
    [Range(1, 10)]
    private int _rows = 1, _cols = 1;

    [SerializeField]
    private float margin = 50f;

    public void GenerateSets()
    {
        RemoveSets();

        if (enviornemntPrefab == null)
        {
            Debug.LogError("Prefab is null");
            return;
        }

        for (int rows = 0; rows < _rows; rows++)
        {
            for (int cols = 0; cols < _cols; cols++)
            {
                float x = rows * margin;
                float z = cols * margin;

                Vector3 position = new Vector3(x, 0, z);
                GameObject enviornemnt = (GameObject)PrefabUtility.InstantiatePrefab(enviornemntPrefab, this.transform);
                enviornemnt.transform.localPosition = position;
            }
        }
    }

    private void RemoveSets()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
}
