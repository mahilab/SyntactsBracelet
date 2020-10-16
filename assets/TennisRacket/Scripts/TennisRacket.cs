using Syntacts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class TennisRacket : MonoBehaviour
{
    [EnumFlags]
    public Hand.AttachmentFlags attachmentFlags = 0;
    public AudioClip[] hitSounds;
    public Transform center;

    new Rigidbody rigidbody;
    private AudioSource audioSource;

    float lastHitTime = 0;
    float audioCoolDownTime = 0.25f;

    Bracelet bracelet;

    float releaseVelocityTimeOffset = -0.011f;
    protected RigidbodyInterpolation hadInterpolation = RigidbodyInterpolation.None;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnAttachedToHand(Hand hand) {
        hadInterpolation = this.rigidbody.interpolation;
        bracelet = hand.GetBracelet();
        Signal sig = new Sine(100) * new Envelope(0.1,0.1);
        bracelet.tactors.VibrateAll(sig);
    }

    private void OnDetachedFromHand(Hand hand) {
        rigidbody.interpolation = hadInterpolation;
        rigidbody.velocity = hand.GetTrackedObjectVelocity(releaseVelocityTimeOffset);
        rigidbody.angularVelocity = hand.GetTrackedObjectAngularVelocity(releaseVelocityTimeOffset);
        bracelet = null;
    }

    private void HandHoverUpdate( Hand hand ) {
        GrabTypes startingGrabType = hand.GetGrabStarting();
        if (startingGrabType != GrabTypes.None) {
            PhysicsAttach(hand, startingGrabType);
        }
    }

    private void HandAttachedUpdate( Hand hand ) {
        if (hand.IsGrabEnding(this.gameObject))
            PhysicsDetach(hand);
    }

    private void PhysicsAttach(Hand hand, GrabTypes startingGrabType) {
        Rigidbody handRigidbody = Util.FindOrAddComponent<Rigidbody>(hand.gameObject);
        handRigidbody.isKinematic = true;
        FixedJoint handJoint = hand.gameObject.AddComponent<FixedJoint>();
        handJoint.connectedBody = rigidbody;
        hand.HoverLock(null);
        hand.AttachObject( gameObject, startingGrabType, attachmentFlags, null );
    }

    private void PhysicsDetach( Hand hand )
    {
        hand.DetachObject(gameObject, false);
        hand.HoverUnlock(null);
        Destroy( hand.GetComponent<FixedJoint>() );
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == 14 && (UnityEngine.Time.time - lastHitTime) > audioCoolDownTime) {
            audioSource.PlayRandom(hitSounds);
            lastHitTime = UnityEngine.Time.time;
            if (bracelet) {
                Signal sig = new Sine(175) * new ExponentialDecay();
                bracelet.tactors.VibrateAll(sig);
            }
        }

    }

    // Vector3 point;

    // void FixedUpdate() {
    //     Plane plane = new Plane(center.right, center.position); 
    //     Utility.DrawPlane(center.position, center.right);
    //     Ray ray = new Ray(center.position - center.right, center.right);
    //     float enter;
    //     if (plane.Raycast(ray, out enter)) {
    //         point = center.position - center.right + center.right * enter;
    //     }
    // }

    // void OnDrawGizmosSelected() {
    //     Gizmos.DrawWireSphere(point, 0.01f);
    // }

}
