using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ReturnUI : MonoBehaviour
{
    [SerializeField] private string MenuScene = "Menu";

    public void ReturnButton()
    {
        SceneManager.LoadScene(MenuScene);
    }

}

