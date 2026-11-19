using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaquetePuzzle : MonoBehaviour
{
    public bool Inicializado { get; private set; }
    public int[] SlotDosMoveis { get; private set; }
    public int[] RotacaoDosMoveis { get; private set; }
    public bool Resolvido { get; set; }
    public bool RecompensaPega { get; set; }

    public void Inicializar(int[] slots, int[] rotacoes)
    {
        SlotDosMoveis = (int[])slots.Clone();
        RotacaoDosMoveis = (int[])rotacoes.Clone();
        Resolvido = false;
        RecompensaPega = false;
        Inicializado = true;
    }
}