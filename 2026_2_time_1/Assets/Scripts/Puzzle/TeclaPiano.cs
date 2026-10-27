using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TeclaPiano : MonoBehaviour
{
    [SerializeField] private int _idTecla;
    [SerializeField] private PuzzlePiano _puzzle;

    private void OnMouseDown()
    {
        _puzzle.RegistrarTecla(_idTecla);
    }
}