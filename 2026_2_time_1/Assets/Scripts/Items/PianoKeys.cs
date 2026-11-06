using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PianoKey : MonoBehaviour
{
    [SerializeField, Range(60, 72)]
    private int note = 60;

    public int Note => note;
}
