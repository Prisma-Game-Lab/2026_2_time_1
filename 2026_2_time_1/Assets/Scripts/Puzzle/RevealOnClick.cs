using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
public class RevealOnClick : MonoBehaviour
{
    [SerializeField] private string stateId = "Cena_FundoMudou";

    [Header("Background")]
    [SerializeField] private SpriteRenderer background;
    [SerializeField] private Sprite revealedBackground;

    [Header("Objects")]
    [SerializeField] private GameObject[] objectsToHide;
    [SerializeField] private GameObject[] objectsToShow;

    [Header("Audio")]
    [SerializeField] private EventReference revealSound;

    [Header("Debug")]
    [SerializeField] private bool logState = true;
    private void OnEnable()
    {
        bool revealed = WorldState.Has(stateId);
        if (logState)
            Debug.Log($"[RevealOnClick] '{name}' checked '{stateId}': {(revealed ? "revealed" : "not set yet")}", this);
        ApplyState(revealed);
    }
    private void Start()
    {
        ApplyState(WorldState.Has(stateId));
    }
    public void Reveal()
    {
        if (WorldState.Has(stateId))
            return;

        WorldState.Set(stateId);

        if (!revealSound.IsNull)
            RuntimeManager.PlayOneShot(revealSound, transform.position);

        ApplyState(true);
    }
    private void ApplyState(bool revealed)
    {
        if (revealed && background != null && revealedBackground != null)
            background.sprite = revealedBackground;

        SetActive(objectsToHide, !revealed);
        SetActive(objectsToShow, revealed);
    }
    private static void SetActive(GameObject[] objects, bool active)
    {
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(active);
        }
    }
}