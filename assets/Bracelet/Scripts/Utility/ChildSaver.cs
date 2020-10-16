using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildSaver : MonoBehaviour
{
    void OnDestroy()
    {
        transform.DetachChildren();
    }
}
