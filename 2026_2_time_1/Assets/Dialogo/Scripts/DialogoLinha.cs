using System;
using UnityEngine;


// DEFINE UMA LINHA DE DIÁLOGO
// ATENÇÃO: os campos com [SerializeField] são salvos nos assets pelo NOME.
// Renomear um deles faz os diálogos já escritos sumirem do Inspector.
// Se precisar renomear, use [FormerlySerializedAs("nomeAntigo")] (using UnityEngine.Serialization).
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
