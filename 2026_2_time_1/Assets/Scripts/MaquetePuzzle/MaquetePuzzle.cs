using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaquetePuzzle : MonoBehaviour
{
    public bool Inicializado { get; private set; }
    // Índice do slot em que cada móvel está
    public int[] SlotDosMoveis { get; private set; }
    // Rotação de cada móvel, em passos de 90° no sentido horário
    public int[] RotacaoDosMoveis { get; private set; }
    public bool Resolvido { get; set; }
    public bool ChavePega { get; set; }
    public void Inicializar(int[] slots, int[] rotacoes)
    {
        SlotDosMoveis = (int[])slots.Clone();
        RotacaoDosMoveis = (int[])rotacoes.Clone();
        Resolvido = false;
        ChavePega = false;
        Inicializado = true;
    }
}