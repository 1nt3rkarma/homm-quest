using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
 {
	public AudioMixer audioMixer;
	public Dropdown resolutionDropdown;
	
	public void SetResolution(int resIndex)
	{
		Resolution res = resolutions[resIndex];

		Screen.SetResolution(res.width,res.height,Screen.fullScreen);
	}
		
	Resolution[] resolutions;

	void Start()
	{
		resolutions = Screen.resolutions;

		resolutionDropdown.ClearOptions();

		List<string> options = new List<string>();

		int currentResIndex = 0;

		for (int i = 0; i < resolutions.Length; i++)
		{
			string option = resolutions[i].width + " x " + resolutions[i].height;
			options.Add(option);

			if (resolutions[i].width == Screen.currentResolution.width &&
				resolutions[i].height == Screen.currentResolution.height)
			{
				currentResIndex = i;
			}
		}

		resolutionDropdown.AddOptions(options);
		resolutionDropdown.value = currentResIndex;
		resolutionDropdown.RefreshShownValue();
	}

	public void SetVolume (float volume)
	{
		audioMixer.SetFloat("volume", volume);
	}

	public void SetFullscreen (bool isFullscreen)
	{
		Screen.fullScreen = isFullscreen;
	}
}
