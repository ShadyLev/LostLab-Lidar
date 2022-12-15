using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Graphics UI Elements")]
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown qualityDropdown;
    public TMP_Dropdown textureDropdown;
    public TMP_Dropdown aaDropdown;
    public Toggle isFullscreen;
    Resolution[] resolutions;

    [Header("Controls UI Elements")]
    public Slider sensxSlider;
    public TextMeshProUGUI sensxValue;
    public Slider sensySlider;
    public TextMeshProUGUI sensyValue;
    float sensX;
    float sensY;
    public Toggle isInverted;

    [Header("Audio UI Elements")]
    public Slider musicVolume;
    public TextMeshProUGUI musicValue;
    public Slider sfxVolume;
    public TextMeshProUGUI sfxValue;
    float volumeMusic;
    float volumeSfx;

    // Start is called before the first frame update
    void Start()
    {
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " +
                     resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width
                  && resolutions[i].height == Screen.currentResolution.height)
                currentResolutionIndex = i;
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
        LoadSettings(currentResolutionIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetTextureQuality(int textureIndex)
    {
        QualitySettings.masterTextureLimit = textureIndex;
        qualityDropdown.value = 6;
    }
    public void SetAntiAliasing(int aaIndex)
    {
        QualitySettings.antiAliasing = aaIndex;
        qualityDropdown.value = 6;
    }

    public void SetQuality(int qualityIndex)
    {
        if (qualityIndex != 6) // if the user is not using any of the presets
            QualitySettings.SetQualityLevel(qualityIndex);

        switch (qualityIndex)
        {
            case 0: // quality level - very low
                textureDropdown.value = 3;
                aaDropdown.value = 0;
                break;
            case 1: // quality level - low
                textureDropdown.value = 2;
                aaDropdown.value = 0;
                break;
            case 2: // quality level - medium
                textureDropdown.value = 1;
                aaDropdown.value = 0;
                break;
            case 3: // quality level - high
                textureDropdown.value = 0;
                aaDropdown.value = 0;
                break;
            case 4: // quality level - very high
                textureDropdown.value = 0;
                aaDropdown.value = 1;
                break;
            case 5: // quality level - ultra
                textureDropdown.value = 0;
                aaDropdown.value = 2;
                break;
        }

        qualityDropdown.value = qualityIndex;
    }

    public void SetSensX(float sensx)
    {
        sensX = sensx;
        sensxValue.SetText(sensx.ToString("F2"));
    }

    public void SetSensY(float sensy)
    {
        sensY = sensy;
        sensyValue.SetText(sensy.ToString("F2"));
    }

    public void SetInvertedY(bool isInverted)
    {
        // inverted shit
    }

    public void SetMusicVolume(float value)
    {
        volumeMusic = value;
        float tmpVal = value * 100f;
        AudioManager.Instance.volumeMusic = value;
        musicValue.SetText(tmpVal.ToString("F0") + "%");
    }

    public void SetSFXVolume(float value)
    {
        volumeSfx = value;
        float tmpVal = value * 100f;
        AudioManager.Instance.volumeSFX = value;
        sfxValue.SetText(tmpVal.ToString("F0") + "%");
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualitySettingPreference", qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference", resolutionDropdown.value);
        PlayerPrefs.SetInt("TextureQualityPreference", textureDropdown.value);
        PlayerPrefs.SetInt("AntiAliasingPreference", aaDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference", Convert.ToInt32(Screen.fullScreen));
        PlayerPrefs.SetInt("IsInvertedY", Convert.ToInt32(isInverted.isOn));
        PlayerPrefs.SetFloat("SensX", sensX);
        PlayerPrefs.SetFloat("SensY", sensY);
        PlayerPrefs.SetFloat("MusicVolume", volumeMusic);
        PlayerPrefs.SetFloat("SFXVolume", volumeSfx);
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        if (PlayerPrefs.HasKey("QualitySettingPreference"))
            qualityDropdown.value = PlayerPrefs.GetInt("QualitySettingPreference");
        else
            qualityDropdown.value = 3;

        if (PlayerPrefs.HasKey("ResolutionPreference"))
            resolutionDropdown.value = PlayerPrefs.GetInt("ResolutionPreference");
        else
            resolutionDropdown.value = currentResolutionIndex;

        if (PlayerPrefs.HasKey("TextureQualityPreference"))
            textureDropdown.value = PlayerPrefs.GetInt("TextureQualityPreference");
        else
            textureDropdown.value = 0;

        if (PlayerPrefs.HasKey("AntiAliasingPreference"))
            aaDropdown.value = PlayerPrefs.GetInt("AntiAliasingPreference");
        else
            aaDropdown.value = 1;

        if (PlayerPrefs.HasKey("FullscreenPreference"))
        {
            Screen.fullScreen = Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference"));
        }
        else
        {
            Screen.fullScreen = true;
        }

        if (PlayerPrefs.HasKey("SensX"))
            sensxSlider.value = PlayerPrefs.GetFloat("SensX");
        else
            sensxSlider.value = 100;

        if (PlayerPrefs.HasKey("SensY"))
            sensySlider.value = PlayerPrefs.GetFloat("SensY");
        else
            sensySlider.value = 100;

        if (PlayerPrefs.HasKey("IsInvertedY"))
            isInverted.isOn = Convert.ToBoolean(PlayerPrefs.GetInt("IsInvertedY"));
        else
            isInverted.isOn = false;

        if (PlayerPrefs.HasKey("MusicVolume"))
            musicVolume.value = PlayerPrefs.GetFloat("MusicVolume");
        else
            musicVolume.value = 1;

        if (PlayerPrefs.HasKey("SFXVolume"))
            sfxVolume.value = PlayerPrefs.GetFloat("SFXVolume");
        else
            sfxVolume.value = 1;
    }
}
