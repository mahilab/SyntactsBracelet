using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;
using UnityEditor;
using Syntacts;

public class TennisBall : MonoBehaviour
{

    public bool resetting = false;

    public AudioClip[] bounceSounds;
    private new Rigidbody rigidbody;
    private AudioSource audioSource;
    // public TennisRacket racket;
    private TrailRenderer trail;
    // Vector3 lastPosition;
    private SphereCollider col;

    float bounciness;
    bool held = false;
    Bracelet bracelet;

    Vector3 initialPosition;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        trail = GetComponent<TrailRenderer>();
        col = GetComponentInChildren<SphereCollider>();
        bounciness = col.material.bounciness;
        initialPosition = transform.position;
    }

    private void Start()
    {
        initialPosition = transform.position;
    }

    void OnCollisionEnter(Collision col) {
        if (col.gameObject.layer == 13) {
            trail.enabled = true;
        }
        else {
            audioSource.PlayRandom(bounceSounds);
        }
        if (held)
        {
            var v = bracelet.GetVelocityEstimate().magnitude;
            var a = BraceletUtility.Remap(v, 0, 0.75f, 0.1f, 1);
            bracelet.tactors.VibrateAll(new Sine(175) * new ASR(0.0f,0.1f,0.15f, a));
        }
    }

    void OnAttachedToHand(Hand hand) {
        trail.enabled = false;
        col.material.bounciness = 0;
        held = true;
        bracelet = hand.GetBracelet();
        bracelet.tactors.VibrateAll(new Sine(175) * new ASR(0, 0, 0.02f,0.1f));
     }

    void OnDetachedFromHand(Hand hand) {
        trail.enabled = true;
        trail.Clear();
        col.material.bounciness = bounciness;
        held = false;
        bracelet = null;
    }

    void OnEnabled() {
        trail.Clear();
    }

    void OnDisabled() {
        trail.Clear();
    }

    private void Update()
    {
        if (resetting && transform.position.y < -5)
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            transform.position = initialPosition;
            held = true;
        }
    }
}
