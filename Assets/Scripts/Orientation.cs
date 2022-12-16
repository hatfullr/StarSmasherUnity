using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Orientation : MonoBehaviour
{


    [SerializeField] private Transform all;
    


    void Update()
    {
        all.transform.localRotation = Camera.main.transform.rotation;
        
    }

}
