using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LookAtCamera : MonoBehaviour
{
    public bool reverse = false;

    void Update()
    {
        if (!reverse)
            transform.LookAt(Camera.main.transform.position);
        else
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
