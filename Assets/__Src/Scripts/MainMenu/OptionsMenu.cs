using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer mixer;
	public Toggle fullscreen;
	public Dropdown resolutionDropdown;
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
	}

    public void SetMusicVolume(float volume){
		mixer.SetFloat("musicVolume", volume);
	}

    public void SetSFXVolume(float volume){
		mixer.SetFloat("sfxVolume", volume);
	}

    public void SetUIVolume(float volume){
		mixer.SetFloat("uiVolume", volume);
	}

	public void SetFullscreen(bool yn){
		Screen.fullScreen = yn;
	}

    public void SetQuality(int qualityIndex){
        QualitySettings.SetQualityLevel(qualityIndex);
		PlayerPrefs.SetInt("Quality", qualityIndex);
		PlayerPrefs.Save();
    }

	public void SetResolution(int resIndex){
		Resolution resolution = resolutions[resIndex];
		Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
		PlayerPrefs.SetInt("Resolution", resIndex);
		PlayerPrefs.Save();
	}

}
