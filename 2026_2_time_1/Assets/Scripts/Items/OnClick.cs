using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(WorldItem))]
public class OnClick : MonoBehaviour
{
    [SerializeField] private bool firstClickOnly;
    [SerializeField] private UnityEvent onClick;

    private WorldItem worldItem;
    private bool clicked;

    private void Awake()
    {
        worldItem = GetComponent<WorldItem>();
    }

    private void OnEnable()
    {
        worldItem.Clicked += HandleClick;
    }

    private void OnDisable()
    {
        worldItem.Clicked -= HandleClick;
    }

    private void HandleClick()
    {
        if (firstClickOnly && clicked)
            return;

        clicked = true;
        onClick.Invoke();
    }
}
