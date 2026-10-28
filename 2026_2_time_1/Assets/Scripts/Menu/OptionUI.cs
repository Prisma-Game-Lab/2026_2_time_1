using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class OptionUi : MonoBehaviour
{
    [SerializeField] private string OptionScene = "Options";

    public void OptionButton()
    {
        SceneManager.LoadScene(OptionScene);
    }

}

