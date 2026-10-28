using System.Collections;
using System.Collections.Generic;
using UnityEngine;  
using UnityEngine.Events;

public class PuzzlePiano : MonoBehaviour
{
    [SerializeField] private int[] _sequenciaCorreta;
    [SerializeField] private GameObject _objetoParaAparecer;
    [SerializeField] private UnityEvent _aoResolver;

    private int _indiceAtual;

    public bool Resolvido { get; private set; }

    public void RegistrarTecla(int idTecla)
    {
        if (Resolvido || _sequenciaCorreta == null || _sequenciaCorreta.Length == 0)
            return;

        if (idTecla == _sequenciaCorreta[_indiceAtual])
        {
            _indiceAtual++;
            if (_indiceAtual == _sequenciaCorreta.Length)
                Resolver();
        }
        else
        {
            _indiceAtual = (idTecla == _sequenciaCorreta[0]) ? 1 : 0;
        }
    }
    private void Resolver()
    {
        Resolvido = true;
        _indiceAtual = 0;

        if (_objetoParaAparecer != null)
            _objetoParaAparecer.SetActive(true);
        else
            Debug.LogWarning($"[PuzzlePiano] '{name}' não tem objeto para aparecer.");
        _aoResolver.Invoke();
    }
}