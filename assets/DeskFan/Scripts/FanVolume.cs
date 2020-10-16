using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Syntacts;

public class FanVolume : MonoBehaviour
{
    public float volumeRadius = 0.2f;
    public float volumeHeight = 0.5f;

    public DeskFan fan;
    public Transform origin;

    BraceletCollider bc;

    void Update() {
        if (bc) {
            UpdateSpatializer();
        }
    }

    void UpdateSpatializer() {
        var bracelet_pos = bc.transform.position;
        Plane plane = new Plane(origin.forward, origin.position);
        float dist_axis = Vector3.Cross(transform.forward, bracelet_pos - transform.position).magnitude;
        float dist_origin = plane.GetDistanceToPoint(bracelet_pos);
        float v = Mathf.Clamp01(1.25f - dist_axis / volumeRadius) * Mathf.Clamp01(1.25f - dist_origin / volumeHeight);

        var projected = Vector3.ProjectOnPlane(origin.position - bc.bracelet.transform.position, bc.bracelet.transform.forward).normalized;
        var localDir = bc.bracelet.transform.InverseTransformDirection(projected);
        float angle = Mathf.Atan2(localDir.y, localDir.x) * Mathf.Rad2Deg - 90;
        bc.bracelet.tactors.SpatialVolume(v);
        bc.bracelet.tactors.SpatialTarget(angle, 135);

        //Debug.DrawLine(origin.position, bracelet_pos, Color.cyan);
        //BraceletUtility.DrawPlane(bc.bracelet.transform.position, bc.bracelet.transform.forward, Color.green, 0.1f);
        //Debug.DrawLine(bc.bracelet.transform.position, bc.bracelet.transform.position + projected * 0.1f, Color.red);
    }

    private void OnTriggerEnter(Collider other)
    {
        var try_bc = other.gameObject.GetComponent<BraceletCollider>();
        if (try_bc) {
            bc = try_bc;
            bc.bracelet.tactors.SpatialEnable();
            bc.bracelet.tactors.SpatialVolume(0);
            Signal sig = new Sine(175) * new Sine(fan.bladeSpeed) + new Noise() * 0.1;
            bc.bracelet.tactors.SpatialVibrate(sig);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (bc && other.gameObject == bc.gameObject)
        {
            bc.bracelet.tactors.SpatialDisable();
            bc = null;
        }
    }
}
