using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

//CONTROLA O DIÁLOGO
//COLOQUE EM UM OBJETO PARA CONFIGURAR O DIALOGO

public class DialogoPlayer : MonoBehaviour
{
    // O DialogoPlayer ativo na cena. Qualquer script lê com DialogoPlayer.Current; só esta classe altera
    public static DialogoPlayer Current { get; private set; }

    // Existe um diálogo aberto agora?
    public static bool IsOpen => Current != null && Current._dialogoAtual != null;

    // O clique deste frame pertence ao diálogo? Verdadeiro enquanto o diálogo está aberto
    // E também no frame exato em que ele abriu ou fechou: aquele clique já foi usado
    public static bool BloqueiaCliques => IsOpen || (Current != null && Current.MudouNesteFrame);

    [Header("UI")]
    [SerializeField] private GameObject _caixaDoDialogo;
    [SerializeField] private TMP_Text _personagem;
    [SerializeField] private TMP_Text _formatoDoTexto;
    // Seta animada que avisa "clique para continuar". Só aparece quando a linha terminou de ser escrita
    [SerializeField] private GameObject _indicador;

    [Header("Efeito de máquina de escrever")]
    [Tooltip("Quantos caracteres aparecem por segundo")]
    [SerializeField, Min(1f)] private float _caracteresPorSegundo = 40f;

    [Header("Input")]
    [SerializeField] private InputActionAsset _inputActions;

    private DialogoSequencia _dialogoAtual;
    private int _linhaIndice;
    private InputAction _clickAction;

    // A corrotina que está escrevendo a linha atual. null = a linha já está completa
    private Coroutine _digitacao;

    private bool EstaDigitando => _digitacao != null;

    // Número do frame (Time.frameCount) em que o diálogo abriu ou fechou pela última vez
    private int _frameDaUltimaMudanca = -1;

    private bool MudouNesteFrame => _frameDaUltimaMudanca == Time.frameCount;

    private void Awake()
    {
        // Esconde a caixa antes de qualquer verificação, para ela nunca ficar visível por engano
        _caixaDoDialogo.SetActive(false);

        if (_inputActions == null)
        {
            Debug.LogError($"[DialogoPlayer] O campo 'Input Actions' de '{name}' está vazio. Arraste o asset PlayerInput no Inspector.", this);
            // Desliga o componente: OnEnable e Update não rodam, então nenhuma NullReferenceException esconde este erro
            enabled = false;
            return;
        }

        _clickAction = _inputActions.FindAction("Player/Click", throwIfNotFound: true);
    }

    private void OnEnable()
    {
        Current = this;

        // Só LIGA a ação. Nunca a desligamos em OnDisable: o mapa "Player" é compartilhado com
        // PointAndClick, PlayerController, Inventory e InspectionManager
        _clickAction.Enable();
    }

    private void OnDisable()
    {
        // Só limpa a referência se ela ainda aponta para este objeto
        if (Current == this)
        {
            Current = null;
        }

        // Unity para as corrotinas quando o objeto é desativado; esquecemos a referência para não ficar "digitando" para sempre
        PararDigitacao();
    }

    private void Update()
    {
        // Ignora o clique do frame em que o diálogo abriu: ele já foi usado para abrir
        if (!_clickAction.WasPressedThisFrame() || _dialogoAtual == null || MudouNesteFrame)
        {
            return;
        }

        // Jogo pausado (Time.timeScale = 0, ver PauseAudio): o clique é do menu de pausa, não do diálogo
        if (Time.timeScale == 0f)
        {
            return;
        }

        // Primeiro clique: completa a linha que ainda está sendo escrita. Próximo clique: avança
        if (EstaDigitando)
        {
            CompletarLinha();
        }
        else
        {
            ProximaLinha();
        }
    }

    public void DialogoIniciar (DialogoSequencia dialogo)
    {
        if(dialogo == null || dialogo.LinhasTotal == 0)
        {
            return;
        }

        _dialogoAtual = dialogo;
        _linhaIndice = 0;
        _frameDaUltimaMudanca = Time.frameCount;

        _caixaDoDialogo.SetActive(true);
        MostrarLinhaAtual();
    }

    private void ProximaLinha()
    {
        _linhaIndice++;

        if(_linhaIndice >= _dialogoAtual.LinhasTotal)
        {
            DialogoTerminar();
            return;
        }

        MostrarLinhaAtual();
    }

    private void MostrarLinhaAtual()
    {
        DialogoLinha linha = _dialogoAtual.RetornaLinha(_linhaIndice);

        _personagem.text = linha.Personagem;
        _formatoDoTexto.text = linha.Texto;

        // Começa com o texto inteiro invisível e deixa a corrotina revelá-lo aos poucos
        PararDigitacao();
        _formatoDoTexto.maxVisibleCharacters = 0;
        MostrarIndicador(false);
        _digitacao = StartCoroutine(Digitar());
    }

    // Corrotina: roda um pouco a cada frame, continuando de onde parou depois de cada "yield return null"
    private IEnumerator Digitar()
    {
        // Gera a malha agora para saber quantos caracteres VISÍVEIS a linha tem (tags como <b> não contam)
        _formatoDoTexto.ForceMeshUpdate();
        int total = _formatoDoTexto.textInfo.characterCount;

        float visiveis = 0f;
        while (visiveis < total)
        {
            // Time.deltaTime: a escrita para junto com o jogo quando ele é pausado
            visiveis += _caracteresPorSegundo * Time.deltaTime;
            _formatoDoTexto.maxVisibleCharacters = Mathf.Min((int)visiveis, total);
            yield return null;
        }

        _digitacao = null;
        CompletarLinha();
    }

    private void CompletarLinha()
    {
        PararDigitacao();
        // Valor alto o bastante para mostrar qualquer linha inteira
        _formatoDoTexto.maxVisibleCharacters = int.MaxValue;
        MostrarIndicador(true);
    }

    private void PararDigitacao()
    {
        if (_digitacao != null)
        {
            StopCoroutine(_digitacao);
            _digitacao = null;
        }
    }

    private void MostrarIndicador(bool mostrar)
    {
        // O indicador é opcional: se o campo estiver vazio, o diálogo funciona sem ele
        if (_indicador != null)
        {
            _indicador.SetActive(mostrar);
        }
    }

    private void DialogoTerminar()
    {
        PararDigitacao();
        MostrarIndicador(false);
        _caixaDoDialogo.SetActive(false);
        _dialogoAtual = null;
        _frameDaUltimaMudanca = Time.frameCount;
    }
}
