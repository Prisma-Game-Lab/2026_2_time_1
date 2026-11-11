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

    [Header("Input")]
    [SerializeField] private InputActionAsset _inputActions;

    [Header("Teste (só para o protótipo)")]
    [SerializeField] private DialogoSequencia _dialogoTeste;

    private DialogoSequencia _dialogoAtual;
    private int _linhaIndice;
    private InputAction _clickAction;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DialogoIniciar(_dialogoTeste);
        }

        // Ignora o clique do frame em que o diálogo abriu: ele já foi usado para abrir
        else if (_clickAction.WasPressedThisFrame() && _dialogoAtual != null && !MudouNesteFrame)
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
    }

    private void DialogoTerminar()
    {
        _caixaDoDialogo.SetActive(false);
        _dialogoAtual = null;
        _frameDaUltimaMudanca = Time.frameCount;
    }
}
