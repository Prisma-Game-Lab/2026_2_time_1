
using UnityEngine;

//DEFINE UMA SEQUÊNCIA DE DIÁLOGO
// ATENÇÃO: os campos com [SerializeField] são salvos nos assets pelo NOME.
// Renomear um deles faz os diálogos já escritos sumirem do Inspector.
// Se precisar renomear, use [FormerlySerializedAs("nomeAntigo")] (using UnityEngine.Serialization).
[CreateAssetMenu(
    fileName = "Dialogo_Novo",
    menuName = "Dialogo/Sequência de diálogo"
)]

public class DialogoSequencia : ScriptableObject
{
    [SerializeField] private DialogoLinha[] _linhas = new DialogoLinha[0];

    public int LinhasTotal
    {
        get
        {
            return _linhas.Length;
        }
    }

    public DialogoLinha RetornaLinha(int i)
    {
        return _linhas[i];
    }
}

