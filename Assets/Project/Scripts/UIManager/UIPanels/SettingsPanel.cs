using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

[System.Serializable]
public class DataSetSettingsPanel
{
    public AudioMixer MasterAudioMixer;
    public Slider Sound;
    public Slider Music;
    public GameObject SettingsPanelObject;
    public Button BtnBackToMenu;
    public Button BtnGoToSettings;
    public Button BtnAutoAim;
    public Sprite[] SpriteAutoAim;
    public Image ImgAutoAim;
    public Text[] textAutoAim;
}

public class SettingsPanel : Panel
{
    private static float vSound;
    private static float vMusic;

    private DataSetSettingsPanel settingsPanel;

    public SettingsPanel(DataSetSettingsPanel settingsPanel) : base(settingsPanel.SettingsPanelObject)
    {

        this.settingsPanel = settingsPanel;

        GetAutoAim();
        GetVolumeMusic();
        GetVolumeSound();
        settingsPanel.Sound.onValueChanged.AddListener(SetVolumeSound);
        settingsPanel.Music.onValueChanged.AddListener(SetVolumeMusic);
        settingsPanel.BtnBackToMenu.onClick.AddListener(HidePanel);
        settingsPanel.BtnGoToSettings.onClick.AddListener(ShowPanel);
        settingsPanel.BtnAutoAim.onClick.AddListener(SetAutoAim);
    }

    protected override void ShowPanel()
    {
        panelObject.SetActive(true);
    }

    protected override void HidePanel()
    {
        base.HidePanel();
    }

    private void GetVolumeSound()
    {
        vSound = PlayerPrefs.GetFloat("VSound", -20);
        settingsPanel.MasterAudioMixer.SetFloat("Sound", vSound);
        settingsPanel.Sound.value = vSound;
    }

    private void GetVolumeMusic()
    {
        vMusic = PlayerPrefs.GetFloat("VMusic", -20);
        settingsPanel.MasterAudioMixer.SetFloat("Music", vMusic);
        settingsPanel.Music.value = vMusic;
    }

    private void SetVolumeSound(float volume)
    {
        settingsPanel.MasterAudioMixer.SetFloat("Sound", volume);
        PlayerPrefs.SetFloat("VSound", volume);
    }

    private void SetVolumeMusic(float volume)
    {
        settingsPanel.MasterAudioMixer.SetFloat("Music", volume);
        PlayerPrefs.SetFloat("VMusic", volume);
    }

    private void GetAutoAim()
    {
        LevelSettings.IsAutoAIM = PlayerPrefs.GetInt("IsAutoAim");
        SetParamAutoAim();
    }

    private void SetAutoAim()
    {
        LevelSettings.IsAutoAIM = LevelSettings.IsAutoAIM == 0 ? 1 : 0;
        SetParamAutoAim();
        PlayerPrefs.SetInt("IsAutoAim", LevelSettings.IsAutoAIM);
    }

    private void SetParamAutoAim()
    {
        settingsPanel.ImgAutoAim.sprite = settingsPanel.SpriteAutoAim[LevelSettings.IsAutoAIM];
        for (int i = 0; i < settingsPanel.textAutoAim.Length; i++)
        {
            if (i == LevelSettings.IsAutoAIM)
            {
                settingsPanel.textAutoAim[i].color = new Color32(255, 255, 255, 255);
            }
            else
            {
                settingsPanel.textAutoAim[i].color = new Color32(255, 255, 255, 100);
            }

        }
    }

}
