// Evan Pezent (epezent@rice.edu)
// 9/15/2020

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Valve.VR;
using Valve.VR.InteractionSystem;

//=============================================================================
// BRACELET COMPONENT
//=============================================================================

[RequireComponent(typeof(TactorArray))]
[RequireComponent(typeof(ControlDisplay))]
[DisallowMultipleComponent]
public class Bracelet : MonoBehaviour
{
    /// Indicates which wrist a Bracelet on
    public enum Handedness {
        Left,
        Right
    }

    [Header("SteamVR")]
    [Tooltip("This Steam VR Hand this Bracelet is associated with.")]
    public Hand hand;
    [Tooltip("The Steam VR action set you want to enable with this bracelet.")]
    public SteamVR_ActionSet actionSet;

    [Header("References")]
    [Tooltip("This Bracelet's tactors.")]
    public TactorArray tactors;
    [Tooltip("This Bracelet's control/display offset helper.")]
    public ControlDisplay controlDisplay;
    [Tooltip("this Bracelet's velocity/acceleration estimator.")]
    public VelocityEstimator estimator;

    [Header("Info")]
    [Tooltip("This Bracelet's handedness")]
    [ReadOnly]
    public Handedness handedness;

    [Tooltip("The other hand's Bracelet.")]
    [ReadOnly]
    public Bracelet otherBracelet;

    /// Dictionary of Bracelets keyed by SteamVR_Input_Sources (i.e. left hand, right hand)
    static Dictionary<SteamVR_Input_Sources, Bracelet> bracelets = new Dictionary<SteamVR_Input_Sources, Bracelet>();

    //=========================================================================
    void Awake()
    {
        if (actionSet != null)
            actionSet.Activate(hand.handType);
        if (hand == null)
            Debug.LogWarning("Bracelet component does not have reference to a SteamVR Hand component.");
        else
        {
            if (bracelets.ContainsKey(this.hand.handType))
                bracelets[this.hand.handType] = this;
            else
                bracelets.Add(this.hand.handType, this);
            if (this.hand.handType == SteamVR_Input_Sources.LeftHand)
                handedness = Handedness.Left;
            else if (this.hand.handType == SteamVR_Input_Sources.RightHand)
                handedness = Handedness.Right;
        }
    }

    //=========================================================================
    void Start() {
        if (otherBracelet == null)
        {
            if (hand)
                otherBracelet = GetByHand(hand.otherHand);
            else
                Debug.Log("Bracelet found reference to other hand Bracelet.");
        }
    }

    //=========================================================================
    public Vector3 GetVelocityEstimate() {
        return estimator.GetVelocityEstimate();
    }

    //=========================================================================
    public static Bracelet GetByInputSource(SteamVR_Input_Sources type) {
        Bracelet ret;
        if (bracelets.TryGetValue(type, out ret))
            return ret;
        else
            return null;
    }

    //=========================================================================
    public static Bracelet GetByHand(Hand hand) {
        return GetByInputSource(hand.handType);
    }
}

//=============================================================================
// HAND EXTENSION METHODS
//=============================================================================

/// Injects functionality into SteamVR Hand class
public static class HandExtensions
{
    public static Bracelet GetBracelet(this Hand hand) {
        return Bracelet.GetByHand(hand);
    }
}