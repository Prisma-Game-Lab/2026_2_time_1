using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class MaqueteView : InspectView
{
    public enum Simetria
    {
        Nenhuma = 1, // a rotação tem que ser exata 
        Meia = 2,    // girar 180° dá no mesmo 
        Total = 4,   // qualquer rotação serve 
    }

    [Serializable]
    private class Movel
    {
        public RectTransform peca;
        public int slotInicial;
        [Range(0, 3)] public int rotacaoInicial;
        public int slotCorreto;
        [Range(0, 3)] public int rotacaoCorreta;
        public Simetria simetria = Simetria.Nenhuma;
    }

    [Header("Puzzle")]
    [SerializeField] private RectTransform[] slots;
    [SerializeField] private float distanciaDeEncaixe = 60f;
    // Rotação em passos de 90° no sentido horário: 0 = como está no sprite, 1 = 90°, 2 = 180°, 3 = 270°
    [SerializeField] private Movel[] moveis;

    [Header("Memória")]
    [SerializeField] private string memorySceneName;

    [Header("Compartimento")]
    [SerializeField] private GameObject compartimentoFechado;
    [SerializeField] private GameObject compartimentoAberto;
    [FormerlySerializedAs("botaoChave")]
    [SerializeField] private Button botaoRecompensa;
    [FormerlySerializedAs("chave")]
    [SerializeField] private Item recompensa;

    private MaquetePuzzle estado;

    public bool PodeMexer => estado != null && !estado.Resolvido;

    private void Reset()
    {
        completeOn = CompleteMode.WhenSolved;
    }

    protected override void OnOpened()
    {
        estado = ObterEstado();
        Validar();

        if (!estado.Inicializado || estado.SlotDosMoveis.Length != moveis.Length)
        {
            int[] slotsIniciais = new int[moveis.Length];
            int[] rotacoesIniciais = new int[moveis.Length];
            for (int i = 0; i < moveis.Length; i++)
            {
                slotsIniciais[i] = moveis[i].slotInicial;
                rotacoesIniciais[i] = moveis[i].rotacaoInicial;
            }
            estado.Inicializar(slotsIniciais, rotacoesIniciais);

            if (EstaCorreto())
                Debug.LogWarning($"[MaqueteView] O arranjo inicial de '{name}' já é a resposta.");
        }
        for (int i = 0; i < moveis.Length; i++)
        {
            RectTransform peca = moveis[i].peca;
            if (peca == null)
                continue;

            if (peca.TryGetComponent(out Graphic grafico))
                grafico.raycastTarget = true;
            if (!peca.TryGetComponent(out MaqueteMovel movel))
                movel = peca.gameObject.AddComponent<MaqueteMovel>();
            movel.Init(this, i);

            Posicionar(i);
        }

        if (botaoRecompensa != null)
        {
            botaoRecompensa.onClick.AddListener(PegarRecompensa);
            if (recompensa != null && recompensa.Sprite != null && botaoRecompensa.TryGetComponent(out Image imagem))
                imagem.sprite = recompensa.Sprite;
        }

        AtualizarCompartimento();
    }
    public void EntrarNaMemoria()
    {
        if (MemoryManager.Current == null)
        {
            Debug.LogWarning($"[MaqueteView] Não há MemoryManager na cena, não dá para entrar na memória '{memorySceneName}'.");
            return;
        }

        if (GameManager.Instance != null && GameManager.Instance.IsTransitioning)
            return;

        MemoryManager.Current.EnterMemory(memorySceneName);
    }
    public void Soltar(int movel)
    {
        if (!MovelValido(movel))
            return;

        if (PodeMexer && moveis[movel].peca != null)
        {
            int destino = SlotMaisProximo(moveis[movel].peca.position);
            int origem = estado.SlotDosMoveis[movel];

            if (destino >= 0 && destino != origem)
            {
                int ocupante = MovelNoSlot(destino);
                if (ocupante >= 0)
                {
                    estado.SlotDosMoveis[ocupante] = origem;
                    Posicionar(ocupante);
                }
                estado.SlotDosMoveis[movel] = destino;
            }
        }

        Posicionar(movel);
        Verificar();
    }

    public void Girar(int movel)
    {
        if (!PodeMexer || !MovelValido(movel))
            return;

        estado.RotacaoDosMoveis[movel] = (estado.RotacaoDosMoveis[movel] + 1) % 4;
        Posicionar(movel);
        Verificar();
    }

    private void Verificar()
    {
        if (!PodeMexer || !EstaCorreto())
            return;

        estado.Resolvido = true;
        AtualizarCompartimento();
        Complete();
    }

    private bool EstaCorreto()
    {
        for (int i = 0; i < moveis.Length; i++)
        {
            Movel movel = moveis[i];
            if (estado.SlotDosMoveis[i] != movel.slotCorreto)
                return false;

            int passo = 4 / (int)movel.simetria;
            int diferenca = ((estado.RotacaoDosMoveis[i] - movel.rotacaoCorreta) % 4 + 4) % 4;
            if (diferenca % passo != 0)
                return false;
        }
        return true;
    }
    private void PegarRecompensa()
    {
        if (estado == null || !estado.Resolvido || estado.RecompensaPega || recompensa == null)
            return;

        switch (recompensa)
        {
            case CollectibleItem coletavel:
                if (Inventory.Current == null)
                {
                    Debug.LogWarning($"[MaqueteView] Não há Inventory na cena, não dá para pegar '{coletavel.ItemName}'.");
                    return;
                }
                if (!Inventory.Current.AddItem(coletavel))
                {
                    Debug.Log($"[MaqueteView] Inventário cheio, não deu para pegar '{coletavel.ItemName}'.");
                    return;
                }
                estado.RecompensaPega = true;
                AtualizarCompartimento();
                break;
            case MemoryItem memoria:
                if (MemoryManager.Current == null)
                {
                    Debug.LogWarning($"[MaqueteView] Não há MemoryManager na cena, não dá para entrar em '{memoria.MemorySceneName}'.");
                    return;
                }
                if (GameManager.Instance != null && GameManager.Instance.IsTransitioning)
                    return;
                MemoryManager.Current.EnterMemory(memoria.MemorySceneName);
                break;

            case InspectableItem inspecionavel:
                if (InspectionManager.Current == null || inspecionavel.InspectView == null)
                {
                    Debug.LogWarning($"[MaqueteView] Não dá para abrir o zoom de '{inspecionavel.ItemName}'.");
                    return;
                }
                InspectionManager.Current.Open(inspecionavel.InspectView, null);
                break;

            default:
                Debug.LogWarning($"[MaqueteView] A maquete não sabe entregar um {recompensa.GetType().Name} ('{recompensa.ItemName}').");
                break;
        }
    }
    private void AtualizarCompartimento()
    {
        bool aberto = estado.Resolvido;

        if (compartimentoFechado != null)
            compartimentoFechado.SetActive(!aberto);
        if (compartimentoAberto != null)
            compartimentoAberto.SetActive(aberto);
        if (botaoRecompensa != null)
            botaoRecompensa.gameObject.SetActive(aberto && !estado.RecompensaPega);
    }
    private void Posicionar(int movel)
    {
        RectTransform peca = moveis[movel].peca;
        int slot = estado.SlotDosMoveis[movel];
        if (peca == null || !SlotValido(slot) || slots[slot] == null)
            return;

        peca.position = slots[slot].position;
        peca.localRotation = Quaternion.Euler(0f, 0f, -90f * estado.RotacaoDosMoveis[movel]);
    }
    private int SlotMaisProximo(Vector3 posicaoDaPeca)
    {
        Vector3 peca = transform.InverseTransformPoint(posicaoDaPeca);

        int maisProximo = -1;
        float menorDistancia = distanciaDeEncaixe;
        for (int s = 0; s < slots.Length; s++)
        {
            if (slots[s] == null)
                continue;

            Vector3 slot = transform.InverseTransformPoint(slots[s].position);
            float distancia = Vector2.Distance(peca, slot);
            if (distancia <= menorDistancia)
            {
                maisProximo = s;
                menorDistancia = distancia;
            }
        }
        return maisProximo;
    }
    private int MovelNoSlot(int slot)
    {
        for (int i = 0; i < estado.SlotDosMoveis.Length; i++)
        {
            if (estado.SlotDosMoveis[i] == slot)
                return i;
        }
        return -1;
    }
    private MaquetePuzzle ObterEstado()
    {
        if (Source == null)
        {
            Debug.LogWarning($"[MaqueteView] '{name}' foi aberto sem WorldItem de origem, o progresso não vai ser guardado.");
            return gameObject.AddComponent<MaquetePuzzle>();
        }
        if (!Source.TryGetComponent(out MaquetePuzzle puzzle))
            puzzle = Source.gameObject.AddComponent<MaquetePuzzle>();
        return puzzle;
    }
    private void Validar()
    {
        if (moveis.Length > slots.Length)
            Debug.LogWarning($"[MaqueteView] '{name}' tem {moveis.Length} móveis e só {slots.Length} slots.");

        HashSet<int> iniciais = new HashSet<int>();
        HashSet<int> corretos = new HashSet<int>();
        for (int i = 0; i < moveis.Length; i++)
        {
            Movel movel = moveis[i];
            if (movel.peca == null)
                Debug.LogWarning($"[MaqueteView] O móvel {i} de '{name}' está sem peça.");
            if (!SlotValido(movel.slotInicial) || !SlotValido(movel.slotCorreto))
                Debug.LogWarning($"[MaqueteView] O móvel {i} de '{name}' aponta para um slot que não existe.");
            if (!iniciais.Add(movel.slotInicial))
                Debug.LogWarning($"[MaqueteView] Dois móveis de '{name}' começam no slot {movel.slotInicial}.");
            if (!corretos.Add(movel.slotCorreto))
                Debug.LogWarning($"[MaqueteView] Dois móveis de '{name}' têm o slot {movel.slotCorreto} como resposta.");
        }

        if (recompensa == null)
            Debug.LogWarning($"[MaqueteView] '{name}' está sem recompensa.");
        else if (!(recompensa is CollectibleItem || recompensa is MemoryItem || recompensa is InspectableItem))
            Debug.LogWarning($"[MaqueteView] A recompensa de '{name}' é um {recompensa.GetType().Name}, que a maquete não sabe entregar.");
        if (string.IsNullOrEmpty(memorySceneName))
            Debug.LogWarning($"[MaqueteView] '{name}' está sem o nome da cena de memória.");
    }
    private bool SlotValido(int slot)
    {
        return slot >= 0 && slot < slots.Length;
    }
    private bool MovelValido(int movel)
    {
        return estado != null && movel >= 0 && movel < moveis.Length;
    }
}