using TMPro;
using UnityEngine;

public class CombinationLockView : InspectView
{
    [SerializeField] private string code = "1234";
    [SerializeField] private TMP_Text[] digitLabels;

    private int[] digits;

    private void Reset()
    {
        completeOn = CompleteMode.WhenSolved;
    }

    protected override void OnOpened()
    {
        if (digitLabels.Length != code.Length)
            Debug.LogWarning($"[CombinationLockView] '{name}' has {digitLabels.Length} digit labels, but its code has {code.Length} digits.");

        digits = new int[digitLabels.Length];
        for (int i = 0; i < digits.Length; i++)
        {
            RefreshLabel(i);

            TMP_Text label = digitLabels[i];
            if (label == null)
                continue;

            label.raycastTarget = true;
            if (!label.TryGetComponent(out CombinationWheel wheel))
                wheel = label.gameObject.AddComponent<CombinationWheel>();
            wheel.Init(this, i);
        }
    }

    public void Increase(int wheel)
    {
        if (IsCompleted || digits == null || wheel < 0 || wheel >= digits.Length)
            return;

        digits[wheel] = (digits[wheel] + 1) % 10;
        RefreshLabel(wheel);

        if (IsSolved())
            Complete();
    }

    private bool IsSolved()
    {
        if (digits.Length != code.Length)
            return false;

        for (int i = 0; i < digits.Length; i++)
        {
            if (code[i] - '0' != digits[i])
                return false;
        }
        return true;
    }

    private void RefreshLabel(int wheel)
    {
        if (digitLabels[wheel] != null)
            digitLabels[wheel].text = digits[wheel].ToString();
    }
}
