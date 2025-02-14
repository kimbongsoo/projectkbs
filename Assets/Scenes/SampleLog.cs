using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleLog : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Hello World!");
        Debug.LogWarning("This is Warning");
        Debug.LogError("This is Error");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Pressed 1 Key !!");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Pressed 2 Key !!");
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Space Key is pressed !!");
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Debug.Log("Left Control Key is  !!");
        }
    }
}
