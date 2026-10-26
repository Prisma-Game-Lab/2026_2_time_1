using UnityEngine;
using UnityEngine.Serialization;

public class InspectView : MonoBehaviour
{
    public enum CompleteMode
    {
        WhenSolved,
        OnOpen,
        OnClose,
    }

    [FormerlySerializedAs("completeOnOpen")]
    [SerializeField] protected CompleteMode completeOn = CompleteMode.OnOpen;
    [SerializeField] private bool closeOnComplete = true;

    public WorldItem Source { get; private set; }
    public bool IsCompleted { get; private set; }

    public void Init(WorldItem source)
    {
        Source = source;
        OnOpened();

        if (completeOn == CompleteMode.OnOpen)
            MarkCompleted();
    }

    public void NotifyClosed()
    {
        OnClosed();

        if (completeOn == CompleteMode.OnClose)
            MarkCompleted();
    }

    protected virtual void OnOpened()
    {
    }

    protected virtual void OnClosed()
    {
    }

    protected void Complete()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;
        if (closeOnComplete)
            Close();
        InvokeSource();
    }

    public virtual bool OnItemDropped(CollectibleItem item)
    {
        return false;
    }

    public void Close()
    {
        if (InspectionManager.Current != null && InspectionManager.Current.CurrentView == this)
            InspectionManager.Current.Close();
    }

    private void MarkCompleted()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;
        InvokeSource();
    }

    private void InvokeSource()
    {
        if (Source != null)
            Source.OnInteracted.Invoke();
    }
}
