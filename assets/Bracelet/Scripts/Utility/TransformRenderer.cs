using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformRenderer : MonoBehaviour
{
    public float width  = 0.005f;
    public float length = 0.1f;

    GameObject root;
    LineRenderer xAxis;
    LineRenderer yAxis;
    LineRenderer zAxis;

    Material axisMaterial;

    // Start is called before the first frame update
    void Awake()
    {
        CreateMaterial();
        CreateRoot();
        CreateAxis(ref xAxis, "X-Axis", Color.red);
        CreateAxis(ref yAxis, "Y-Axis", Color.green);
        CreateAxis(ref zAxis, "Z-Axis", Color.blue);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateAxis(ref xAxis, transform.right);
        UpdateAxis(ref yAxis, transform.up);
        UpdateAxis(ref zAxis, transform.forward);
    }

    void CreateMaterial() {
        axisMaterial = new Material(Shader.Find("Valve/VR/Highlight"));
        axisMaterial.SetFloat("_Darken", 1.0f);
        axisMaterial.SetFloat("_SeeThru", 1.0f);
    }

    void CreateRoot() {
        root = new GameObject("TransformRenderer");
        root.transform.parent = this.transform;
        root.transform.localPosition = Vector3.zero;
        root.transform.localEulerAngles = Vector3.zero;
        root.transform.localScale = Vector3.one;
    }

    void CreateAxis(ref LineRenderer axis, string name, Color color) {
        GameObject axisGo = new GameObject(name);
        axisGo.transform.parent = root.transform;
        axisGo.transform.localPosition = Vector3.zero;
        axisGo.transform.localEulerAngles = Vector3.zero;
        axisGo.transform.localScale = Vector3.one;
        axis = axisGo.AddComponent<LineRenderer>();
        axis.positionCount = 2;
        axis.startWidth = width;
        axis.endWidth   = width;
        axis.startColor = color;
        axis.endColor   = color;
        axis.useWorldSpace = false;
        axis.SetPosition(0, Vector3.zero);
        axis.material = axisMaterial;
    }

    void UpdateAxis(ref LineRenderer axis, Vector3 direction) {
        Vector3 endPoint = transform.position + direction * length;
        axis.SetPosition(1, transform.InverseTransformPoint(endPoint));
        axis.startWidth = width;
        axis.endWidth   = width;
    }

    void OnEnable() {
        root.SetActive(true);
    }

    void OnDisable() {
        root.SetActive(false);
    }

    void OnDestroy() {
        Destroy(axisMaterial);
        Destroy(root);
    }
}
