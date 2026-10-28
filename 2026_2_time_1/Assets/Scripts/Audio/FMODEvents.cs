using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    public static FMODEvents instance { get; private set; }

    [field: Header("SFX")]
    [field: SerializeField]
    public EventReference coinCollected { get; private set; }

    [field: Header("Music")]
    [field: SerializeField]
    public EventReference backgroundMusic { get; private set; }

    private void Awake()
    {
        instance = this;
    }
}