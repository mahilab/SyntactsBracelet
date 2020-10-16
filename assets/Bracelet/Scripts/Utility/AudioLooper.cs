using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioLooper : MonoBehaviour
{
    public AudioSource[] sources;
    public AudioClip[] clips;
    public bool randomize = false;
    public float lookAheadTime = 1.0f;

    int nextClip;
    int toggle;
    double nextStartTime;

    // Start is called before the first frame update
    void Start()
    {
        toggle = 0;
        nextStartTime = AudioSettings.dspTime + 0.2;
    }

    // Update is called once per frame
    void Update()
    {
        if(AudioSettings.dspTime > nextStartTime - lookAheadTime) {

            AudioClip clipToPlay = clips[nextClip];

            // Loads the next Clip to play and schedules when it will start
            sources[toggle].clip = clipToPlay;
            sources[toggle].PlayScheduled(nextStartTime);

            // Checks how long the Clip will last and updates the Next Start Time with a new value
            double duration = (double)clipToPlay.samples / clipToPlay.frequency;
            nextStartTime = nextStartTime + duration;

            // Switches the toggle to use the other Audio Source next
            toggle = 1 - toggle;

            // Increase the clip index number, reset if it runs out of clips
            if (randomize)
                nextClip = Random.Range(0, clips.Length);
            else
                nextClip = nextClip < clips.Length - 1 ? nextClip + 1 : 0;
        }
    }
}
