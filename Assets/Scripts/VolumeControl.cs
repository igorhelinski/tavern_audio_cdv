using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VolumeControl : MonoBehaviour
{
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;

    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;

    FMOD.Studio.VCA sfxVCA;
    FMOD.Studio.VCA musicVCA;

    void Awake()
    {
        sfxVCA = FMODUnity.RuntimeManager.GetVCA("vca:/SFX");
        musicVCA = FMODUnity.RuntimeManager.GetVCA("vca:/Music");

        sfxVolumeSlider.onValueChanged.AddListener(OnUpdateSFXSlider);
        sfxVolumeSlider.minValue = -100;
        sfxVolumeSlider.maxValue = 0;
        sfxVolumeSlider.value = 0;

        musicVolumeSlider.onValueChanged.AddListener(OnUpdateMusicSlider);
        musicVolumeSlider.minValue = -100;
        musicVolumeSlider.maxValue = 0;
        musicVolumeSlider.value = 0;
    }

    void Start()
    {
        controlPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            controlPanel.SetActive(!controlPanel.activeSelf);

            if (controlPanel.activeSelf) 
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void OnUpdateSFXSlider(float volume)
    {
        float linear = DecibelToLinear(volume);
        sfxVCA.setVolume(linear);

        float percentValue = Mathf.RoundToInt(100 - Mathf.Abs(volume));
        sfxVolumeText.text = percentValue + "%";
    }

    void OnUpdateMusicSlider(float volume)
    {
        float linear = DecibelToLinear(volume);
        musicVCA.setVolume(linear);

        float percentValue = Mathf.RoundToInt(100 - Mathf.Abs(volume));
        musicVolumeText.text = percentValue + "%";
    }

    float DecibelToLinear(float dB)
    {
        return Mathf.Pow(10.0f, dB / 20f);
    }

    //float DecibelToPercent(float dB)
    //{
    //    float percentValue = Mathf.RoundToInt(100 - Mathf.Abs(dB));
    //    return dB
    //}
}
