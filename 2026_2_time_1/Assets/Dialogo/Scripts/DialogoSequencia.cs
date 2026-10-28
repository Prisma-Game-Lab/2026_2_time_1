
using UnityEngine;

//NÃO MODIFICAR
//DEFINE UMA SEQUÊNCIA DE DIÁLOGO

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

