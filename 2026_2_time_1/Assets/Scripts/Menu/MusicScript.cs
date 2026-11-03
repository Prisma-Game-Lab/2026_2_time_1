using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicScript : MonoBehaviour
{
    public Slider VolumeSlider;
    void Start(){
        if (PlayerPrefs.HasKey("soundVolume"))
            loadVolume();
        else{
            PlayerPrefs.SetFloat("soundVolume",1);
            loadVolume();
        }
    }

    public void setVolume(){
        AudioListener.volume=VolumeSlider.value;
        saveVolume();
    }

    public void saveVolume(){
        PlayerPrefs.SetFloat("soundVolume",VolumeSlider.value);
    }

    public void loadVolume(){
        VolumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
    }
}

