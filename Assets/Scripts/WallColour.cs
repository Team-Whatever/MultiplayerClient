using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColour : MonoBehaviour
{
    void Start()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<MeshRenderer>().material.color = new Color(UnityEngine.Random.Range(0.0f, 1.0f), 
                                                                        UnityEngine.Random.Range(0.0f, 1.0f), 
                                                                        UnityEngine.Random.Range(0.0f, 1.0f));
        }            
    }
}
