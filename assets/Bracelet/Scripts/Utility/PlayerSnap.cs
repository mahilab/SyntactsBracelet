using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class PlayerSnap : MonoBehaviour
{

    public SteamVR_ActionSet actionSet;
    public SteamVR_Action_Vector2 joystickAction;
    public Transform vrCamera;
    public float snapAngle = 60.0f;
    public float snapTime = 0.1f;
    public float snapThreshold = 0.9f;
    public float resetThreshold = 0.1f;

    bool rightReset = true;
    bool leftReset = true;
    bool snapping = false;

    // Update is called once per frame
    void Update()
    {
        // right hand
        Vector2 rightAxis = joystickAction.GetAxis(SteamVR_Input_Sources.RightHand);
        Vector2 leftAxis  = joystickAction.GetAxis(SteamVR_Input_Sources.LeftHand);
        if (rightAxis.x > snapThreshold && rightReset && !snapping) {
            StartCoroutine(Snap(snapAngle));
            rightReset = false;
        }
        else if (rightAxis.x < -snapThreshold && rightReset && !snapping) {
            StartCoroutine(Snap(-snapAngle));
            rightReset = false;
        }
        else if (rightAxis.x > -resetThreshold && rightAxis.x < resetThreshold) {
            rightReset = true;
        }      
        // left hand
        if (leftAxis.x > snapThreshold && leftReset && !snapping) {
            StartCoroutine(Snap(snapAngle));
            leftReset = false;
        }
        else if (leftAxis.x < -snapThreshold && leftReset && !snapping) {
            StartCoroutine(Snap(-snapAngle));
            leftReset = false;
        }
        else if (leftAxis.x > -resetThreshold && rightAxis.x < resetThreshold) {
            leftReset = true;
        }        
    }

    IEnumerator Snap(float angle) {
        snapping = true;
        SteamVR_Fade.Start( Color.clear, 0 );
        SteamVR_Fade.Start( Color.black, snapTime );
        yield return new WaitForSeconds(snapTime);
        transform.RotateAround(vrCamera.position, Vector3.up, angle);
        SteamVR_Fade.Start( Color.clear, snapTime );
        snapping = false;
    }

}
