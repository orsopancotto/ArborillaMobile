using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerParamsScript : MonoBehaviour
{
    [SerializeField] private Slider zoom_factor_slider, snappiness_slider, moving_speed_slider, min_zoom_slider, max_zoom_slider;
    [SerializeField] private Button close_btn;
    private string default_zoom_factor_str, default_snappiness_str, default_moving_speed_str, default_min_zoom_str, default_max_zoom_str;
    private Text zoom_factor_txt, snappiness_txt, moving_speed_txt, min_zoom_txt, max_zoom_txt;
    private UserControllerTD user_controller_script;

    private void Start()
    {
        InitializeVariables();
        InitializeEvents();
    }

    private void InitializeVariables()
    {
        user_controller_script = Camera.main.GetComponent<UserControllerTD>();

        zoom_factor_slider.value = user_controller_script.zoom_factor;
        zoom_factor_txt = zoom_factor_slider.GetComponentInChildren<Text>();
        default_zoom_factor_str = zoom_factor_txt.text;
        zoom_factor_txt.text = $"{default_zoom_factor_str} {user_controller_script.zoom_factor}";

        snappiness_slider.value = user_controller_script.snappiness;
        snappiness_txt = snappiness_slider.GetComponentInChildren<Text>();
        default_snappiness_str = snappiness_txt.text;
        snappiness_txt.text = $"{default_snappiness_str} {user_controller_script.snappiness}";

        moving_speed_slider.value = user_controller_script.moving_around_speed;
        moving_speed_txt = moving_speed_slider.GetComponentInChildren<Text>();
        default_moving_speed_str = moving_speed_txt.text;
        moving_speed_txt.text = $"{default_moving_speed_str} {user_controller_script.moving_around_speed}";

        min_zoom_slider.value = user_controller_script.min_FOV;
        min_zoom_txt = min_zoom_slider.GetComponentInChildren<Text>();
        default_min_zoom_str = min_zoom_txt.text;
        min_zoom_txt.text = $"{default_min_zoom_str} {user_controller_script.min_FOV}";

        max_zoom_slider.value = user_controller_script.max_FOV;
        max_zoom_txt = max_zoom_slider.GetComponentInChildren<Text>();
        default_max_zoom_str = max_zoom_txt.text;
        max_zoom_txt.text = $"{default_max_zoom_str} {user_controller_script.max_FOV}";


    }

    private void InitializeEvents()
    {
        zoom_factor_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref zoom_factor_slider, ref user_controller_script.zoom_factor, ref zoom_factor_txt, default_zoom_factor_str);
        });

        snappiness_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref snappiness_slider, ref user_controller_script.snappiness, ref snappiness_txt, default_snappiness_str);
        });

        moving_speed_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref moving_speed_slider, ref user_controller_script.moving_around_speed, ref moving_speed_txt, default_moving_speed_str);
        });

        min_zoom_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref min_zoom_slider, ref user_controller_script.min_FOV, ref min_zoom_txt, default_min_zoom_str);
        });

        max_zoom_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref max_zoom_slider, ref user_controller_script.max_FOV, ref max_zoom_txt, default_max_zoom_str);
        });


        close_btn.onClick.AddListener(delegate
        {
            ClosePanel_OnClick();
        });
    }

    private void UpdateValue_OnValueChanged(ref Slider sender, ref float value, ref Text text, string default_string)
    {
        value = sender.value;
        text.text = $"{default_string} {value}";
    }

    private void ClosePanel_OnClick()
    {
        Slider[] sliders = { zoom_factor_slider, snappiness_slider, moving_speed_slider, min_zoom_slider, max_zoom_slider};

        foreach (Slider slider in sliders) slider.onValueChanged.RemoveAllListeners();

        DevOptionsManagerScript.Instance.gameObject.SetActive(true);

        Destroy(gameObject);
    }
}
