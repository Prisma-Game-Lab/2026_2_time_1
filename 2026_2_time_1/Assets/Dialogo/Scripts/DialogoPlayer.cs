using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;




//CONTROLA O DIÁLOGO
//COLOQUE EM UM OBJETO PARA CONFIGURAR O DIALOGO

public class DialogoPlayer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _caixaDoDialogo;
    [SerializeField] private TMP_Text _personagem;
    [SerializeField] private TMP_Text _formatoDoTexto;
    
    [Header("Input")]
    [SerializeField] private InputActionAsset _inputActions;
    
    [Header("Teste")]
    [SerializeField] private DialogoSequencia _dialogoTeste;
    
    private DialogoSequencia _dialogoAtual;
    private int _linhaIndice;
    private InputAction _clickAction;
    
    

    private void Awake()
    {
        _caixaDoDialogo.SetActive(false);
        
        if (_inputActions == null)
        {
            Debug.LogError("Input actions field missing",this);
            enabled = false;
            return;
        }
        
        _clickAction = _inputActions.FindAction("Player/Click", throwIfNotFound: true);
    }

     private void OnEnable()
    {
        _clickAction.Enable();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            DialogoIniciar(_dialogoTeste);
        }

        else if (_clickAction.WasPressedThisFrame() && _dialogoAtual != null)
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
    }


}
