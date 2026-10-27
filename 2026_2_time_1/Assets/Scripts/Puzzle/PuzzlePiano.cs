using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PuzzlePiano : MonoBehaviour
{
    [SerializeField] private int[] _sequenciaCorreta;
    [SerializeField] private GameObject _objetoParaAparecer;
    private int _indiceAtual;
    public void RegistrarTecla(int idTecla)
    {
        if (idTecla == _sequenciaCorreta[_indiceAtual])
        {
            _indiceAtual++;

            if (_indiceAtual == _sequenciaCorreta.Length)
            {
                _objetoParaAparecer.SetActive(true);
                _indiceAtual = 0;
            }
        }
        else
        {
            _indiceAtual = (idTecla == _sequenciaCorreta[0]) ? 1 : 0;
        }
    }
}