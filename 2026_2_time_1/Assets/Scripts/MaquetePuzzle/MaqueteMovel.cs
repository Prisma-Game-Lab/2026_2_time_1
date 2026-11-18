using UnityEngine;
using UnityEngine.EventSystems;

public class MaqueteMovel : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    public MaqueteView Maquete { get; private set; }
    public int Index { get; private set; }
    private RectTransform rect;
    private Vector3 offset;
    private bool arrastando;

    public void Init(MaqueteView maquete, int index)
    {
        Maquete = maquete;
        Index = index;
        rect = (RectTransform)transform;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Maquete == null || !Maquete.PodeMexer || eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!PontoNoMundo(eventData, out Vector3 ponto))
            return;

        arrastando = true;
        GameCursor.Holding = true;
        offset = rect.position - ponto;
        rect.SetAsLastSibling();
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (arrastando && PontoNoMundo(eventData, out Vector3 ponto))
            rect.position = ponto + offset;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (!arrastando)
            return;

        arrastando = false;
        GameCursor.Holding = false;
        Maquete.Soltar(Index);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Maquete == null || eventData.button != PointerEventData.InputButton.Left)
            return;
        if (arrastando || eventData.dragging)
            return;

        Maquete.Girar(Index);
    }
    private bool PontoNoMundo(PointerEventData eventData, out Vector3 ponto)
    {
        RectTransform pai = (RectTransform)rect.parent;
        return RectTransformUtility.ScreenPointToWorldPointInRectangle(pai, eventData.position, eventData.pressEventCamera, out ponto);
    }
}