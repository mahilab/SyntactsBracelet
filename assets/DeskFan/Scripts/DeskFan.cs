using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskFan : MonoBehaviour
{
    public const float minSpeed = 10; // rps
    public const float maxSpeed = 30; // rps
    public const float minAngle = -30;
    public const float maxAngle =  30;

    [Header("Settings")]
    public bool lockYaw = false;    
    [Range(minAngle, maxAngle)]
    public float pitchAngle = 0;
    [Range(minAngle, maxAngle)]
    public float yawAngle   = 0;
    [Range(minSpeed,maxSpeed)]
    public float bladeSpeed = 20;
    public float speedGain  = 360;
    [Range(0, 0.5f)]
    public float yawFreq = 0.25f;

    [Header("Audio Settings")]
    public AnimationCurve angleVolumeCurve;
    public AnimationCurve speedVolumeCurve;
    public AnimationCurve speedPitchCurve;

    [Header("Monitor")]
    public float audioAngle;

    [Header("References")]
    public Transform pitchAxis;
    public Transform yawAxis;
    public Transform bladeAxis;
    public Knob speedKnob;
    public Knob pitchKnob;
    public FanVolume volume;
    public Transform front;
    float yawTime;

    AudioSource audioSource;
    AudioListener listener;

    // Start is called before the first frame update
    void Awake()
    {
        yawTime = 0;
        audioSource = GetComponentInChildren<AudioSource>();
        listener = FindObjectOfType<AudioListener>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateAxes();
        UpdateAudio();

    }

    public void ToggleLock() {
        lockYaw = !lockYaw;
    }

    void OnValidate() {
        pitchAxis.localEulerAngles = new Vector3(-pitchAngle, 0,0);
        yawAxis.localEulerAngles = new Vector3(0,yawAngle,0);
    }

    void UpdateAxes() {
        if (!lockYaw) {
            yawAngle = 30 * Mathf.Sin(Mathf.PI * 2 * yawFreq * yawTime);
            yawTime += Time.deltaTime;
        }
        bladeSpeed = BraceletUtility.Remap(speedKnob.KnobPercent(), 0, 1, minSpeed, maxSpeed);
        pitchAngle = BraceletUtility.Remap(pitchKnob.KnobPercent(), 0, 1, minAngle, maxAngle);
        pitchAxis.localEulerAngles = new Vector3(-pitchAngle, 0,0);
        yawAxis.localEulerAngles = new Vector3(0,yawAngle,0);
        bladeAxis.localEulerAngles += new Vector3(0,0,bladeSpeed*speedGain) * Time.deltaTime;
    }

    void UpdateAudio() {
        Vector3 direction = (listener.transform.position - audioSource.transform.position).normalized;
        audioAngle = Mathf.Abs(Vector3.Angle(audioSource.transform.forward, direction) - 90);

        float tAngle = Mathf.Clamp01(audioAngle / 90.0f);
        float tSpeed = BraceletUtility.Remap01Clamped(bladeSpeed, minSpeed, maxSpeed);

        audioSource.volume = angleVolumeCurve.Evaluate(tAngle) * speedVolumeCurve.Evaluate(tSpeed);;
        audioSource.pitch = speedPitchCurve.Evaluate(tSpeed);;
    }

}
