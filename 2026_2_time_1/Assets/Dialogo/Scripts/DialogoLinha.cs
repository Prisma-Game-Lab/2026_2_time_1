using System;
using UnityEngine;

// NÃO MODIFICAR
// DEFINE UMA LINHA DE DIÁLOGO
[Serializable] public class DialogoLinha 
{
    [SerializeField] private string _personagem;

    [SerializeField, TextArea(2,5)] private string _texto;

    public string Personagem
    {
        get
        {
            return _personagem;
        }
    }
    public string Texto
    {
        get
        {
            return _texto;
        }
    }
}
