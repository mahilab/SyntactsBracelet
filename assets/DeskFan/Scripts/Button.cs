using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Button : MonoBehaviour {

    [Header("Options")]
    public bool toggle = false;

    [Header("Haptics")]
    public bool vibrotactorHaptics = true;
    public bool cDHaptics = true;
    public string touchSignal;
    public string pressSignal;

    [Header("Range of Motion")]
    public float zMax;
    public float zMid;
    public float zMin;

    [Header("Stiffness Properties")]
    public float Kc;
    public float Kb;
    public float Ks;

    [Header("Monitor")]
    public float z = 0;
    public float x = 0;
    public float p = 0;
    public float squeeze = 0;
    public bool bottomOut = false;
    public bool isContacted = false;
    public bool isToggled = false;

    [Header("Events")]
    public UnityEvent onContact;
    public UnityEvent onPressed;
    public UnityEvent onRelease;
    public UnityEvent onHeld;
    public UnityEvent onToggled;
    public UnityEvent onUntoggled;

    [Header("References")]
    public Transform surface;
    public GameObject model;

    // private
    FingerTip fingerTip;

    // Use this for initialization
    void Start () {
        z = zMax;
        p = 0.0f;
        x = 0.0f;
        bottomOut = false;
        squeeze = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {

        Plane surfacePlane = new Plane(surface.forward, surface.transform.position);
        Plane fixedPlane = new Plane(model.transform.forward, surface.transform.position +  model.transform.forward * x);

        // Control/Display
        if (cDHaptics)
        {
            x = (zMax - z);
            z -= p * Kc * Time.deltaTime;
            z += x * Kb * Time.deltaTime;
            squeeze = Ks * x;
        }
        else
        {
            x = Mathf.Clamp(p * 0.5f, 0.0f, zMax - zMin);
            squeeze = Ks * x;
            z = zMax - p;
        }

        // Z target position
        float zTarg = zMax;
        if (toggle) {
            if (isToggled)
                zTarg = zMid;
            else
                zTarg = zMax;
        }

        if (z > zTarg) {
            z = zTarg;
            bottomOut = false;
        }
        if (z < zMin)
        {
            z = zMin;
            if (!bottomOut)
            {
                isToggled = !isToggled;
                if (isToggled)
                    onToggled.Invoke();
                else
                    onUntoggled.Invoke();
                if (fingerTip && vibrotactorHaptics)                
                    OnButtonPress();                
                onPressed.Invoke();
            }
            bottomOut = true;
        }
        else
            bottomOut = false;

        // move model
        model.transform.localPosition = new Vector3(0, 0, z);

        // held by finger
        if (isContacted && fingerTip) {

            onHeld.Invoke();

            // finger position
            Vector3 centerPoint = fingerTip.transform.position;
            Vector3 deepestPoint = centerPoint - model.transform.forward * fingerTip.radius;

            // CD haptics
            p = -fixedPlane.GetDistanceToPoint(deepestPoint);
            float pPrime = -surfacePlane.GetDistanceToPoint(deepestPoint);
            if (cDHaptics)
                fingerTip.controlDisplay.offset = model.transform.forward.normalized * pPrime;
            else if (z == zMin)
                fingerTip.controlDisplay.offset = model.transform.forward.normalized * pPrime;

        }     
    }

    private void OnButtonPress() {
        Syntacts.Signal sig;
        if (pressSignal != "" && Syntacts.Library.LoadSignal(out sig, pressSignal))
            fingerTip.bracelet.tactors.VibrateAll(sig);
    }

    private void OnTriggerEnter(Collider other)
    {
        fingerTip = other.gameObject.GetComponent<FingerTip>();
        if (fingerTip) {        
            onContact.Invoke();
            isContacted = true;
            if (vibrotactorHaptics)
            {
                Syntacts.Signal sig;
                if (touchSignal != "" && Syntacts.Library.LoadSignal(out sig, touchSignal))
                    fingerTip.bracelet.tactors.VibrateAll(sig);
            }                
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (fingerTip && other.gameObject == fingerTip.gameObject)
        {
            onRelease.Invoke();
            p = 0.0f;
            fingerTip.controlDisplay.offset = Vector3.zero;
            fingerTip = null;
            isContacted = false;
        }
    }
}
