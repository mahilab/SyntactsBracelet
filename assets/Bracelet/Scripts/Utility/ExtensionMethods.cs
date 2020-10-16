using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {

    //=========================================================================
    // AUDIO SOURCE
    //=========================================================================

    public static void PlayRandom(this AudioSource audioSource, AudioClip[] bank, float volume = 1.0f, bool dontRepeat = true) 
    {
        if (bank.Length > 1) {
            if (dontRepeat) {
                int index = UnityEngine.Random.Range (1, bank.Length);
                audioSource.PlayOneShot(bank[index], volume);
                AudioClip temp = bank[index];
                bank[index] = bank[0];
                bank[0] = temp;
            }
            else {
                int index = UnityEngine.Random.Range (0, bank.Length);
                audioSource.PlayOneShot(bank[index], volume);
            }
        }
        else {
            audioSource.PlayOneShot(bank[0], volume);
        }
    }

    //=========================================================================
    // TRANSFORM
    //=========================================================================

    public static void Reset(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(1, 1, 1);
    }

    public static Vector3 LocalRight(this Transform transform) {
        return transform.worldToLocalMatrix.MultiplyVector(transform.right);
    }

    public static Vector3 LocalUp(this Transform transform) {
        return transform.worldToLocalMatrix.MultiplyVector(transform.up);
    }

    public static Vector3 LocalForward(this Transform transform) {
        return transform.worldToLocalMatrix.MultiplyVector(transform.forward);
    }

    //=========================================================================
    // VECTOR3
    //=========================================================================

    public static Vector3 WithX(this Vector3 v, float x) {
		return new Vector3(x, v.y, v.z);
	}

	public static Vector3 WithY(this Vector3 v, float y) {
		return new Vector3(v.x, y, v.z);
	}

	public static Vector3 WithZ(this Vector3 v, float z) {
		return new Vector3(v.x, v.y, z);
	}

    //=========================================================================
    // MISC
    //=========================================================================

    public static void Shuffle<T>(this IList<T> list, int seed)
    {
        System.Random rng = new System.Random(seed);
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }


}