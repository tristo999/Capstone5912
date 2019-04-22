using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer mixer;
	public AudioMixer music;
	public Toggle fullscreen;
	public Dropdown resolutionDropdown;
    public TMPro.TMP_InputField fpsField;
	Resolution[] resolutions;
	List<string> resolutionOptions = new List<string>();

	void Start(){
		int currentResIndex = 0;
		fullscreen.isOn = Screen.fullScreen;
        //get resolutions, clear placeholders
		resolutions = Screen.resolutions;
		//set resolution to saved value if it exists
		if(PlayerPrefs.HasKey("Resolution")){
			SetResolution(PlayerPrefs.GetInt("Resolution"));
		}
		//set quality to saved value if it exists
		if(PlayerPrefs.HasKey("Quality")){
			SetQuality(PlayerPrefs.GetInt("Quality"));
		}

        if (PlayerPrefs.HasKey("Fullscreen")) {
            SetFullscreen(PlayerPrefs.GetInt("Fullscreen") == 1);
        }

        if (PlayerPrefs.HasKey("Vsync")) {
            SetVsync(PlayerPrefs.GetInt("Vsync") == 1);
        }

        if (PlayerPrefs.HasKey("Fps")) {
            SetFpsTarget(PlayerPrefs.GetInt("Fps"));
            fpsField.text = PlayerPrefs.GetInt("Fps").ToString();
        }
		resolutionDropdown.ClearOptions();
        //add resolutions to options list, find current resolution
		for(int i = 0; i < resolutions.Length; i++){
			resolutionOptions.Add(resolutions[i].width + " x " + resolutions[i].height);
			if(resolutions[i].width == Screen.currentResolution.width && 
				resolutions[i].height == Screen.currentResolution.height){
				currentResIndex = i;
			}
		}
        //add options to dropdown
		resolutionDropdown.AddOptions(resolutionOptions);
		resolutionDropdown.value = currentResIndex;
		resolutionDropdown.RefreshShownValue();
	}

	public void SetMasterVolume(float volume){
		mixer.SetFloat("masterVolume", volume);
		music.SetFloat("masterVolume", volume);
	}

    public void SetMusicVolume(float volume){
		mixer.SetFloat("musicVolume", volume);
		music.SetFloat("masterVolume", volume);
	}

    public void SetSFXVolume(float volume){
		mixer.SetFloat("sfxVolume", volume);
	}

    public void SetUIVolume(float volume){
		mixer.SetFloat("uiVolume", volume);
	}

	public void SetFullscreen(bool yn){
		Screen.fullScreen = yn;
        if (yn)
            PlayerPrefs.SetInt("Fullscreen", 1);
        else
            PlayerPrefs.SetInt("Fullscreen", 0);
	}

    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
		PlayerPrefs.SetInt("Quality", qualityIndex);
        if (PlayerPrefs.HasKey("Vsync")) {
            QualitySettings.vSyncCount = PlayerPrefs.GetInt("Vsync");
            fpsField.interactable = PlayerPrefs.GetInt("Vsync") == 0;
        } 
		PlayerPrefs.Save();
    }

	public void SetResolution(int resIndex){
		Resolution resolution = resolutions[resIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
		PlayerPrefs.SetInt("Resolution", resIndex);
		PlayerPrefs.Save();
	}
    
    public void SetVsync(bool vsync) {
        if (vsync) {
            PlayerPrefs.SetInt("Vsync", 1);
            QualitySettings.vSyncCount = 1;
            fpsField.interactable = false;
        } else {
            PlayerPrefs.SetInt("Vsync", 0);
            QualitySettings.vSyncCount = 0;
            fpsField.interactable = true;
        }
    }

    public void SetFpsTarget() {
        SetFpsTarget(int.Parse(fpsField.text));
    }

    public void SetFpsTarget(int target) {
        PlayerPrefs.SetInt("Fps", target);
        Application.targetFrameRate = target;
    }

}
