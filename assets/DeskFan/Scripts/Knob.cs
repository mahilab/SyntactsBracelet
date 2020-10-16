using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class Knob : MonoBehaviour
{
    public enum SpringReturnMode {
        NominalAngle,
        NotchAngle,
        None
    }

    [Header("Options")]
    public bool discreteNotches = false;
    public SpringReturnMode springReturnMode;
    public bool haptics = true;
    public bool acumulateAngle = false;

    [Header("Settings")]
    public float controlDisplay = 1.2f;
    public float angleNom = 0;
    public float angleMin = 0;
    public float angleMax = 135;
    public string touchSignal;
    public string notchSignal;

    [Space]
    public int notchCount       = 7;
    public float notchRadius1   = 0.028f;
    public float notchRadius2   = 0.032f;
    public float notchThickness = 0.001f;
    public float notchTol = 2.0f;
    public float resetTol = 5.0f;

    [Space]
    public float K = 20;

    [Header("References")]
    public Transform notchPlane;
    public Material notchMaterial;
    public GameObject model;

    [Header("Monitor")]
    public float angle;
    public float acumulatedAngle = 0;
    public int revolution;
    public bool held = false;
    public float[] notchAngles;
    public int currentNotch;

    protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand;
    Transform transformHelperHand;
    Transform transformHelperParent;
    Interactable interactable;
    float angleOnGrab;
    float lastHandAngle;
    bool notchReset = false;
    LineRenderer[] notches;

    Bracelet bracelet;

    void Awake() {
        interactable = GetComponent<Interactable>();
        transform.localEulerAngles = transform.localEulerAngles.WithZ(angleNom);
        angle =angleNom;
        CreateNotches();
        currentNotch = InsideNotch();
    }

    // Update is called once per frame
    void Update()
    {
        // NotchDebugDraw();
        if (!held && springReturnMode != SpringReturnMode.None) {

            float targetAngle;
            if (springReturnMode == SpringReturnMode.NominalAngle)
                targetAngle = angleNom;
            else
                targetAngle = notchAngles[currentNotch];
            if (angle != targetAngle) {
                angle -= (angle - targetAngle) *  K * Time.deltaTime;
                transform.localEulerAngles = transform.localEulerAngles.WithZ(angle);
            }
        }
    }

    void CreateNotches() {    
        if (notchCount < 2)
            return;    
        notchAngles = new float[notchCount];
        notches = new LineRenderer[notchCount];
        for (int i = 0; i < notchCount; ++i) {
            GameObject notchGo = new GameObject("notch_"+i.ToString());
            notchGo.transform.parent = notchPlane.transform;
            notchGo.transform.localPosition = Vector3.zero;
            notchGo.transform.localEulerAngles = Vector3.zero;
            LineRenderer notch = notchGo.AddComponent<LineRenderer>();
            notches[i] = notch;
            notch.useWorldSpace = false;
            notch.positionCount = 2;
            notch.material = notchMaterial;
            notch.startWidth = notchThickness;
            notch.endWidth = notchThickness;
            float angle = (angleMin + i * (angleMax-angleMin)/(notchCount-1));
            notchAngles[i] = angle;
            notchGo.transform.localEulerAngles = new Vector3(0,0,angle);
            notch.SetPosition(0, new Vector3(0, notchRadius1, 0));
            notch.SetPosition(1, new Vector3(0, notchRadius2, 0));
        }
    }

    int InsideNotch() {
        for (int i = 0; i < notchCount; ++i) {
            float notchAngle = notchAngles[i];
            if (Between(angle, notchAngle - notchTol, notchAngle + notchTol))
                return i;
        }
        return -1;
    }

    void TryNotchReset() {
        for (int i = 0; i < notchCount -1; ++i) {
            float resetAngle = 0.5f * (notchAngles[i] + notchAngles[i+1]);
            if (Between(angle, resetAngle - resetTol, resetAngle + resetTol))
            {
                notchReset = true;
                break;
            }
        }
    }

    protected virtual void HandHoverUpdate( Hand hand )
    {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
        {
            hand.AttachObject(gameObject, startingGrabType, attachmentFlags);
        }
    }

    bool Between(float angle, float min, float max) {
        return angle >= min && angle <= max;
    }

    protected virtual void HandAttachedUpdate(Hand hand)
    {
        if (hand.IsGrabEnding(this.gameObject))
        {
            hand.DetachObject(gameObject);
            return;
        }

        Vector3 projection = Vector3.ProjectOnPlane(transformHelperHand.right, transformHelperParent.forward);
        Vector3 p1 = transformHelperParent.position;
        Vector3 p2 = transformHelperParent.position + projection;
        Vector2 xy = transformHelperParent.InverseTransformPoint(p2);
        float handAngle = BraceletUtility.WrapTo360(Mathf.Atan2(xy.y,xy.x)*Mathf.Rad2Deg); 

        if (Between(lastHandAngle, 0, 90) && (Between(handAngle, 270, 360)))
            revolution--;
        else if (Between(lastHandAngle, 270, 360) && (Between(handAngle, 0, 90)))
            revolution++;
        lastHandAngle = handAngle;
        float totalHandAngle = revolution * 360 + handAngle;
        angle = angleOnGrab + totalHandAngle / controlDisplay;
        angle = Mathf.Clamp(angle, angleMin - acumulatedAngle, angleMax - acumulatedAngle);

        if (discreteNotches) {
            if (notchReset)
                transform.localEulerAngles = transform.localEulerAngles.WithZ(angle);
        }
        else {
            transform.localEulerAngles = transform.localEulerAngles.WithZ(angle);
        }

        // check if inside notch
        int notch = InsideNotch();
        if (notch != -1) {
            currentNotch = notch;
            if (notchReset) {
                if (haptics)
                    OnKnobNotch();                              
                notchReset = false;
            }
        }
        else {
            TryNotchReset();
        }
    }

    void NotchDebugDraw() {
        if (notchCount < 2)
            return;
        Vector3 p0 = notchPlane.TransformPoint(Vector3.zero);
        float x, y;
        for (int i = 0; i < notchCount; ++i) {
            y = notchRadius2 * Mathf.Cos((notchAngles[i] - notchTol) * Mathf.Deg2Rad);
            x = -notchRadius2 * Mathf.Sin((notchAngles[i] - notchTol) * Mathf.Deg2Rad);
            Debug.DrawLine(p0, notchPlane.TransformPoint(new Vector3(x,y,0)) ,Color.magenta);
            y = notchRadius2 * Mathf.Cos((notchAngles[i] + notchTol) * Mathf.Deg2Rad);
            x = -notchRadius2 * Mathf.Sin((notchAngles[i] + notchTol) * Mathf.Deg2Rad);
            Debug.DrawLine(p0, notchPlane.TransformPoint(new Vector3(x,y,0)) ,Color.magenta);
        }
        for (int i = 0; i < notchCount - 1; ++i) {
            float resetAngle = (notchAngles[i] + notchAngles[i+1]) * 0.5f;
            y = notchRadius2 * Mathf.Cos((resetAngle - resetTol) * Mathf.Deg2Rad);
            x = -notchRadius2 * Mathf.Sin((resetAngle - resetTol) * Mathf.Deg2Rad);
            Debug.DrawLine(p0, notchPlane.TransformPoint(new Vector3(x,y,0)) ,Color.green);
            y = notchRadius2 * Mathf.Cos((resetAngle + resetTol) * Mathf.Deg2Rad);
            x = -notchRadius2 * Mathf.Sin((resetAngle + resetTol) * Mathf.Deg2Rad);
            Debug.DrawLine(p0, notchPlane.TransformPoint(new Vector3(x,y,0)) ,Color.green);
        }
    }

    protected virtual void OnAttachedToHand(Hand hand) {
        CreateTransformHelpers(hand);
        angleOnGrab = angle;
        revolution = 0;
        lastHandAngle = 0;
        held = true;
        bracelet = hand.GetBracelet();
        Syntacts.Signal sig;
        if (touchSignal != "" && Syntacts.Library.LoadSignal(out sig, touchSignal))
            bracelet.tactors.VibrateAll(sig);
    }

    void OnKnobNotch() {
        Syntacts.Signal sig;
        if (notchSignal != "" && Syntacts.Library.LoadSignal(out sig, notchSignal))
            bracelet.tactors.VibrateAll(sig);
    }

    protected virtual void OnDetachedFromHand(Hand hand)
    {
        if (acumulateAngle) {
            acumulatedAngle += angle;
            model.transform.localEulerAngles += new Vector3(0,0,angle);
            transform.localEulerAngles = transform.localEulerAngles.WithZ(angleOnGrab);
            angle = angleOnGrab;
        }
        DestroyTransformHelpers();
        held = false;
        bracelet = null;
    }

    void CreateTransformHelpers(Hand hand, bool debug = false) {
        GameObject transformHelperHandGo = new GameObject("transformHelperHand");
        transformHelperHand = transformHelperHandGo.transform;
        transformHelperHand.transform.parent = hand.transform;
        transformHelperHand.transform.localPosition = Vector3.zero;
        transformHelperHand.transform.rotation = this.transform.rotation;
        GameObject transformHelperParentGo = new GameObject("transformHelperParent");
        transformHelperParent = transformHelperParentGo.transform;
        transformHelperParent.transform.parent = this.transform.parent;
        transformHelperParent.transform.localPosition = this.transform.localPosition;
        transformHelperParent.transform.rotation = this.transform.rotation;
        if (debug) {
            transformHelperHandGo.AddComponent<TransformRenderer>();
            transformHelperParentGo.AddComponent<TransformRenderer>();
        }
    }

    void DestroyTransformHelpers() {
        Destroy(transformHelperHand.gameObject);
        Destroy(transformHelperParent.gameObject);
    }

    public float KnobPercent() {
        return BraceletUtility.Remap01Clamped(angle + acumulatedAngle, angleMin, angleMax);
    }

    public int Notch() {
        return currentNotch;
    }

    public float NotchPercent() {
        return (float)currentNotch / (float)(notchCount - 1);
    }
}
