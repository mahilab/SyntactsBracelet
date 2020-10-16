// Evan Pezent (epezent@rice.edu)
// 5/19/2020

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR;

//=============================================================================
// BRACELET CONTROL DISPLAY COMPONENT
//=============================================================================

[DisallowMultipleComponent]
public class ControlDisplay : MonoBehaviour
{

    public Vector3 offset;
    public bool showGhost = false;

    [Header("Prefabs")]
    public GameObject indexColliderPrefab;

    [Header("Materials")]
    public Material ghostMaterial;

    [Header("References")]
    [Tooltip("The Bracelet this CD belongs to")]
    public Bracelet bracelet;

    [Header("Info")]
    [ReadOnly]
    [SerializeField]
    private bool initialized;

    [ReadOnly]
    [SerializeField]
    private GameObject handGhost;

    [ReadOnly]
    [SerializeField]
    private GameObject handProxy;

    [ReadOnly]
    [SerializeField]
    private Transform fingerTipTransform;

    [ReadOnly]
    [SerializeField]
    private SkinnedMeshRenderer ghostRend;

    GameObject indexGhost;

    // Start is called before the first frame update
    void Awake()
    {
        initialized = false;
        if (bracelet == null)
            bracelet = GetComponent<Bracelet>();
    }

    // Update is called once per frame
    void Update()
    {
        // if the proxy hand becomes null, reinitialize
        if (!handProxy)
            initialized = false;
        if (!initialized)
        {
            Initialize();
        }
        else
        {
            handProxy.transform.position = handGhost.transform.position + offset;
            if (showGhost)
                ghostRend.enabled = true;
            else
                ghostRend.enabled = false;
        }
    }

    void Initialize()
    {
        if (bracelet.hand == null)
            return;
        string s = "", side = "", Side = "";
        if (bracelet.hand.handType == SteamVR_Input_Sources.LeftHand)
        {
            s = "l";
            side = "left";
            Side = "Left";
        }
        else if (bracelet.hand.handType == SteamVR_Input_Sources.RightHand)
        {
            s = "r";
            side = "right";
            Side = "Right";
        }
        // hide steam control and hints
        bracelet.hand.HideController(true);
        bracelet.hand.otherHand.HideController(true);
        bracelet.hand.SetSkeletonRangeOfMotion(Valve.VR.EVRSkeletalMotionRange.WithoutController);
        ControllerButtonHints.HideAllButtonHints(bracelet.hand);
        ControllerButtonHints.HideAllTextHints(bracelet.hand);
        Transform t = bracelet.hand.transform;
        if (t = t.Find(Side + "RenderModel Slim(Clone)"))
        {
            if (t = t.Find("vr_glove_" + side + "_model_slim(Clone)"))
            {
                // find the default hand, make it the proxy
                t.name = "proxy_" + side;
                handProxy = t.gameObject;
                // create a ghost hand
                if (handGhost)
                    Destroy(handGhost);
                handGhost = Instantiate(handProxy);
                handGhost.name = "ghost_" + side;
                handGhost.transform.parent = handProxy.transform.parent;
                handGhost.transform.localPosition = Vector3.zero;
                handGhost.transform.localEulerAngles = Vector3.zero;
                handGhost.transform.localScale = Vector3.one;
                if (s == "l")
                {
                    var skeleton = handGhost.GetComponent<SteamVR_Behaviour_Skeleton>();
                    skeleton.mirroring = SteamVR_Behaviour_Skeleton.MirrorType.None;
                }
                // set bracelet device transform to be on proxy hand
                Transform ttt;
                if (ttt = handProxy.transform.Find("slim_" + s))
                {
                    if (ttt = ttt.Find("Root"))
                    {
                        if (ttt = ttt.Find("wrist_r"))
                        {
                            bracelet.transform.parent = ttt;
                            bracelet.transform.localPosition = new Vector3(-0.01f, 0, -0.06f);
                            bracelet.transform.localScale = Vector3.one;
                            bracelet.transform.localEulerAngles = new Vector3(0, 0, 270);
                        }
                    }
                }

                if (t = handGhost.transform.Find("slim_" + s))
                {
                    Transform tt;
                    if (tt = t.Find("vr_glove_right_slim"))
                    {
                        ghostRend = tt.gameObject.GetComponent<SkinnedMeshRenderer>();
                        ghostRend.material = ghostMaterial;
                    }
                    else
                    {
                        return;
                    }
                    if (t = t.Find("Root"))
                    {
                        if (t = t.Find("wrist_r"))
                        {
                            if (t = t.Find("finger_index_meta_r"))
                            {
                                if (t = t.Find("finger_index_0_r"))
                                {
                                    if (t = t.Find("finger_index_1_r"))
                                    {
                                        if (t = t.Find("finger_index_2_r"))
                                        {
                                            if (t = t.Find("finger_index_r_end"))
                                            {
                                                fingerTipTransform = t;
                                                indexGhost = Instantiate(indexColliderPrefab);
                                                indexGhost.transform.parent = t;
                                                indexGhost.transform.localEulerAngles = Vector3.zero;
                                                indexGhost.transform.localPosition = new Vector3(-0.01f, 0.00f, 0);
                                                indexGhost.name = "indexGhost";
                                                indexGhost.GetComponent<FingerTip>().bracelet = bracelet;
                                                indexGhost.GetComponent<FingerTip>().controlDisplay = this;
                                                initialized = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                initialized = true;
            }
        }
    }

}


