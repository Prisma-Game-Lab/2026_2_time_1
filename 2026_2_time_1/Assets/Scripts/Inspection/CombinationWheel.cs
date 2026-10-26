using UnityEngine;
using UnityEngine.EventSystems;

public class CombinationWheel : MonoBehaviour, IPointerClickHandler
{
    public CombinationLockView Lock { get; private set; }
    public int Index { get; private set; }

    public void Init(CombinationLockView combinationLock, int index)
    {
        Lock = combinationLock;
        Index = index;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Lock != null && eventData.button == PointerEventData.InputButton.Left)
            Lock.Increase(Index);
    }
}
