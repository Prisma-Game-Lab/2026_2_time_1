using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldStateSetter : MonoBehaviour
{
    public void Set(string stateId)
    {
        WorldState.Set(stateId);
    }
}