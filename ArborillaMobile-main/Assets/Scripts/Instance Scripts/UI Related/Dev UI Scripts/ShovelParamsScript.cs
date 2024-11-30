using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShovelParamsScript : MonoBehaviour
{
    [SerializeField] private Slider rise_speed_slider, return_speed_slider, rotation_speed_slider, target_rotation_slider, target_height_slider;
    [SerializeField] private Button close_btn;
    private Text rise_speed_txt, return_speed_txt, rotation_speed_txt, target_rotation_txt, target_height_txt;
    private string default_rise_speed_str, default_return_speed_str, default_rotation_speed_str, default_target_rotation_str, default_target_height_str;
    private TrashbinScript trashbin_script;

    private void Start()
    {
        InitializeVariables();
        InitializeEvents();
    }

    private void InitializeVariables()
    {
        trashbin_script = GameObject.Find("PALA").GetComponent<TrashbinScript>();

        rise_speed_slider.value = trashbin_script.rise_speed;
        rise_speed_txt = rise_speed_slider.GetComponentInChildren<Text>();
        default_rise_speed_str = rise_speed_txt.text;
        rise_speed_txt.text = $"{default_rise_speed_str} {trashbin_script.rise_speed}";

        return_speed_slider.value = trashbin_script.return_speed;
        return_speed_txt = return_speed_slider.GetComponentInChildren<Text>();
        default_return_speed_str = return_speed_txt.text;
        return_speed_txt.text = $"{default_return_speed_str} {trashbin_script.return_speed}";

        rotation_speed_slider.value = trashbin_script.rotation_speed;
        rotation_speed_txt = rotation_speed_slider.GetComponentInChildren<Text>();
        default_rotation_speed_str = rotation_speed_txt.text;
        rotation_speed_txt.text = $"{default_rotation_speed_str} {trashbin_script.rotation_speed}";

        target_rotation_slider.value = trashbin_script.target_xEulerRotation;
        target_rotation_txt = target_rotation_slider.GetComponentInChildren<Text>();
        default_target_rotation_str = target_rotation_txt.text;
        target_rotation_txt.text = $"{default_target_rotation_str} {trashbin_script.target_xEulerRotation}";

        target_height_slider.value = trashbin_script.target_yPosition;
        target_height_txt = target_height_slider.GetComponentInChildren<Text>();
        default_target_height_str = target_height_txt.text;
        target_height_txt.text = $"{default_target_height_str} {trashbin_script.target_yPosition}";

        close_btn.onClick.AddListener(delegate
        {
            ClosePanel_OnClick();
        });
    }

    private void InitializeEvents()
    {
        rise_speed_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref rise_speed_slider, ref trashbin_script.rise_speed, ref rise_speed_txt, default_rise_speed_str);
        });

        return_speed_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref return_speed_slider, ref trashbin_script.return_speed, ref return_speed_txt, default_return_speed_str);
        });

        rotation_speed_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref rotation_speed_slider, ref trashbin_script.rotation_speed, ref rotation_speed_txt, default_rotation_speed_str);
        });

        target_rotation_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref target_rotation_slider, ref trashbin_script.target_xEulerRotation, ref target_rotation_txt, default_target_rotation_str);
        });

        target_height_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref target_height_slider, ref trashbin_script.target_yPosition, ref target_height_txt, default_target_height_str);
        });
    }

    private void UpdateValue_OnValueChanged(ref Slider sender, ref float value, ref Text text, string default_string)
    {
        value = sender.value;
        text.text = $"{default_string} {value}";
    }

    private void ClosePanel_OnClick()
    {
        Slider[] sliders = { rise_speed_slider, rotation_speed_slider, target_rotation_slider, target_height_slider };

        foreach (Slider slider in sliders) slider.onValueChanged.RemoveAllListeners();

        DevOptionsManagerScript.Instance.gameObject.SetActive(true);

        Destroy(gameObject);
    }

}
