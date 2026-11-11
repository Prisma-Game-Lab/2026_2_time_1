using UnityEngine;

// INICIA UM DIÁLOGO QUANDO O JOGADOR INTERAGE COM UM WORLDITEM
// COMO USAR: adicione este componente a um objeto que já tem WorldItem e arraste uma
// Sequência de Diálogo para o campo "Dialogo". Não é preciso configurar nenhum evento.
//
// O diálogo começa sempre que o WorldItem dispara o OnInteracted:
// - ClickableItem: ao clicar no objeto
// - InteractableItem: ao usar o item certo nele
// - CollectibleItem: ao coletar o item
// - InspectableItem: quando a inspeção é concluída (ao abrir, ao fechar ou ao resolver, conforme a View)
// - MemoryItem: ao entrar na memória (cuidado: a cena muda logo depois e o diálogo some)

[RequireComponent(typeof(WorldItem))]
public class DialogoTrigger : MonoBehaviour
{
    // ATENÇÃO: os campos com [SerializeField] são salvos nos assets pelo NOME.
    // Renomear um deles faz os diálogos já configurados sumirem do Inspector.
    // Se precisar renomear, use [FormerlySerializedAs("nomeAntigo")] (using UnityEngine.Serialization).
    [SerializeField] private DialogoSequencia _dialogo;

    private WorldItem _worldItem;

    private void Awake()
    {
        _worldItem = GetComponent<WorldItem>();

        if (_dialogo == null)
        {
            Debug.LogWarning($"[DialogoTrigger] '{name}' não tem nenhum diálogo no campo 'Dialogo'.", this);
        }
    }

    private void OnEnable()
    {
        // Inscreve-se no evento do WorldItem pelo código: o designer não precisa configurar nada no Inspector
        _worldItem.OnInteracted.AddListener(IniciarDialogo);
    }

    private void OnDisable()
    {
        // Sempre desfazer a inscrição feita em OnEnable, para o evento não chamar um objeto desligado ou destruído
        _worldItem.OnInteracted.RemoveListener(IniciarDialogo);
    }

    private void IniciarDialogo()
    {
        if (DialogoPlayer.Current == null)
        {
            Debug.LogWarning($"[DialogoTrigger] '{name}' tentou iniciar um diálogo, mas não há nenhum DialogoPlayer ativo na cena.", this);
            return;
        }

        DialogoPlayer.Current.DialogoIniciar(_dialogo);
    }
}
