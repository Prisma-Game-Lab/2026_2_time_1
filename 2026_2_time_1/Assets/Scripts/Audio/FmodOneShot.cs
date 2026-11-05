using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class FmodOneShot : MonoBehaviour
{
    [SerializeField] private EventReference eventReference;

    public void Play()
    {
        RuntimeManager.PlayOneShot(eventReference, transform.position);
    }
}