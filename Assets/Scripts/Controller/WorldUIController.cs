using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUIController : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3 WorldPos
    {
        set
        {
            worldPos = value;
            transform.position = Camera.main.WorldToScreenPoint(worldPos);
        }
    }

    private Vector3 worldPos;

    // Update is called once per frame
    void Update()
    {
        transform.position = Camera.main.WorldToScreenPoint(worldPos);
    }
}
