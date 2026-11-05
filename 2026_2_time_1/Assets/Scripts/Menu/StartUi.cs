using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUi : MonoBehaviour
{

    [SerializeField] private string StartScene = "RECEBA";

     public void StartButton()
    {
        SceneManager.LoadScene(StartScene);
    }



}
