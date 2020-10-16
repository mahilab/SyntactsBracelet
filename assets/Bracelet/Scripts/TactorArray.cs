// Evan Pezent (epezent@rice.edu)
// 9/15/2020

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Syntacts;

//=============================================================================
// TACTOR ARRAY COMPONENT
//=============================================================================

[DisallowMultipleComponent]
public class TactorArray : MonoBehaviour
{
    [Tooltip("The channels to initialize each Tactor on.")]
    public Tactor.AudioChannel[] channels;
    
    [Header("References")]
    public Bracelet bracelet;
    public Tactor[] tactors;
    public GameObject spatializerDial;

    [Header("Info")]
    [ReadOnly]
    public SyntactsHub hub;

    public Tactor this[int index] { get => tactors[index]; }

    public Tactor Nearest(Vector3 worldPosition, int nth = 0) {
        float[] distances = new float[8];
        int[] indices = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
        for (int i = 0; i < 8; ++i)
            distances[i] = (worldPosition - tactors[i].transform.position).magnitude;
        System.Array.Sort(distances, indices);
        return tactors[indices[nth]];
    }

    public Tactor Farthest(Vector3 worldPosition, int nth = 0) {
        return Nearest(worldPosition, 7 - nth);
    }

    void Start() {
        hub = FindObjectOfType<SyntactsHub>();
        spat = new Spatializer(hub.session);
        for (int i = 0; i < 8; ++i)  {
            tactors[i].channel = channels[i];
            tactors[i].index = i;
            tactors[i].bracelet = bracelet;
        }
        spatializerDial.SetActive(false);
    }

    public void VibrateAll() {
        for (int i = 0; i < 8; ++i)
            tactors[i].Vibrate();
    }

    public void VibrateAll(Signal signal) {
        for (int i = 0; i < 8; ++i)
            tactors[i].Vibrate(signal);
    }

    void OnEnable() {
        foreach (var tactor in tactors)
            tactor.enabled = true;
    }

    void OnDisable() {
        foreach (var tactor in tactors)
            tactor.enabled = false;
    }

    Syntacts.Spatializer spat;

    public void SpatialEnable()
    {
        spat.Clear();
        for (int i = 0; i < 8; ++i) {
            var pos = new Point(45.0f + i * 315.0f / 7.0f, 0);
            spat.SetPosition((int)tactors[i].channel, pos);
        }
        spat.wrapInterval = new Point(360,0);
        spat.target = new Point(0, 0);
        spat.volume = 1.0f;
        spat.rollOff = Curve.Linear;
        spatializerDial.SetActive(true);
    }

    public void SpatialDisable()
    {
        spat.Clear();
        spatializerDial.SetActive(false);
    }

    public void SpatialVibrate(Signal sig)
    {
        spat.Play(sig);
    }

    public void SpatialStop()
    {
        spat.Stop();
    }

    public void SpatialVolume(float v)
    {
        spat.volume = v;
    }

    public void SpatialTarget(float pos, float ang)
    {
        spat.target = new Point(pos,0);
        spat.radius = ang;
        spatializerDial.transform.localEulerAngles = new Vector3(0, 0, pos);
    }

    private void OnApplicationQuit()
    {
        if (spat.valid)
            spat.Dispose();
    }

} // TactorArray
