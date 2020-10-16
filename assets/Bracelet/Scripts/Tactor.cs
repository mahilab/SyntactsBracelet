// Evan Pezent (epezent@rice.edu)
// 9/15/2020

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Syntacts;

//=============================================================================
// TACTOR COMPONENT
//=============================================================================

[DisallowMultipleComponent]
public class Tactor : MonoBehaviour
{
    public enum AudioChannel {
        Channel1 = 0,
        Channel2 = 1,
        Channel3 = 2,
        Channel4 = 3,
        Channel5 = 4,
        Channel6 = 5,
        Channel7 = 6,
        Channel8 = 7,
        Channel9 = 8,
        Channel10 = 9,
        Channel11 = 10,
        Channel12 = 11,
        Channel13 = 12,
        Channel14 = 13,
        Channel15 = 14,
        Channel16 = 15,
        Channel17 = 16,
        Channel18 = 17,
        Channel19 = 18,
        Channel20 = 19,
        Channel21 = 20,
        Channel22 = 21,
        Channel23 = 22,
        Channel24 = 23
    }

    [Tooltip("Tactor Resonance Frequency (Hz)")]
    public float f0 = 175;

    [Space]
    public Color idleColor;
    public Color vibrationColor;

    [Header("References")]
    public MeshRenderer lid;

    [Header("Info")]
    [ReadOnly]
    public int index;
    [ReadOnly]
    public AudioChannel channel;
    [ReadOnly]
    public Bracelet bracelet;
    [ReadOnly]
    public SyntactsHub hub;
   
    public void Vibrate() {
        if (!enabled) return;
        var sig = new Sine(f0) * new ASR(0.1, 0.1, 0.1);
        hub.session.Play((int)channel, sig);
    }

    public void Vibrate(Signal signal) {
        if (!enabled) return;
        hub.session.Play((int)channel, signal);
    }

    public float Level()
    {
        return (float)hub.session.GetLevel((int)channel);
    }

    private void Start()
    {
        hub = FindObjectOfType<SyntactsHub>();
    }

    void Update()
    {
        lid.material.color = Color.Lerp(idleColor, vibrationColor, Level());
    }

} // Tactor
