using UnityEngine;

[CreateAssetMenu(fileName = "Objectives", menuName = "Memory Map/Objectives")]
public class MemoryObjectives : ScriptableObject
{
    [SerializeField, TextArea(1, 3)] private string[] objectives = new string[0];

    public string Current => TextAt(MemoryMapData.ObjectiveIndex);

    public string TextAt(int index) => index >= 0 && index < objectives.Length ? objectives[index] : "";

    public void Advance() => MemoryMapData.AdvanceObjective();
}
