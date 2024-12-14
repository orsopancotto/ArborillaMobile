using UnityEngine;
using UnityEngine.UI;

public class CameraAnimationsPanelScript : MonoBehaviour
{
    [SerializeField] private Slider speed_slider, distance_from_plant_slider, lowering_amount_slider, FOV_speed_slider;
    [SerializeField] private Button close_btn;
    private Text speed_txt, distance_from_plant_txt, lowering_amount_txt, FOV_speed_txt;
    private string default_speed_str, default_distance_from_plant_str, default_lowering_amount_str, default_FOV_speed_str;
    private CameraAnimations camera_anim_script;

    private void Start()
    {
        InitializeVariables();
        InitializeEvents();

    }

    private void InitializeVariables()
    {
        camera_anim_script = Camera.main.GetComponent<CameraAnimations>();

        speed_slider.value = camera_anim_script.speed;
        speed_txt = speed_slider.GetComponentInChildren<Text>();
        default_speed_str = speed_txt.text;
        speed_txt.text = $"{default_speed_str} {camera_anim_script.speed}";

        distance_from_plant_slider.value = camera_anim_script.distance_from_plant;
        distance_from_plant_txt = distance_from_plant_slider.GetComponentInChildren<Text>();
        default_distance_from_plant_str = distance_from_plant_txt.text;
        distance_from_plant_txt.text = $"{default_distance_from_plant_str} {camera_anim_script.distance_from_plant}";

        lowering_amount_slider.value = camera_anim_script.lowering_amount;
        lowering_amount_txt = lowering_amount_slider.GetComponentInChildren<Text>();
        default_lowering_amount_str = lowering_amount_txt.text;
        lowering_amount_txt.text = $"{default_lowering_amount_str} {camera_anim_script.lowering_amount}";

        FOV_speed_slider.value = camera_anim_script.FOV_speed;
        FOV_speed_txt = FOV_speed_slider.GetComponentInChildren<Text>();
        default_FOV_speed_str = FOV_speed_txt.text;
        FOV_speed_txt.text = $"{default_FOV_speed_str} {camera_anim_script.FOV_speed}";
    }

    private void InitializeEvents()
    {
        speed_slider.onValueChanged.AddListener(delegate { 
            UpdateValue_OnValueChanged(ref speed_slider, ref camera_anim_script.speed, ref speed_txt, default_speed_str); 
        });

        distance_from_plant_slider.onValueChanged.AddListener(delegate { 
            UpdateValue_OnValueChanged(ref distance_from_plant_slider, ref camera_anim_script.distance_from_plant, ref distance_from_plant_txt, default_distance_from_plant_str); 
        });
        
        lowering_amount_slider.onValueChanged.AddListener(delegate { 
            UpdateValue_OnValueChanged(ref lowering_amount_slider, ref camera_anim_script.lowering_amount, ref lowering_amount_txt, default_lowering_amount_str); 
        });

        FOV_speed_slider.onValueChanged.AddListener(delegate
        {
            UpdateValue_OnValueChanged(ref FOV_speed_slider, ref camera_anim_script.FOV_speed, ref FOV_speed_txt, default_FOV_speed_str);
        });

        close_btn.onClick.AddListener(delegate {
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
        speed_slider.onValueChanged.RemoveAllListeners();
        distance_from_plant_slider.onValueChanged.RemoveAllListeners();
        lowering_amount_slider.onValueChanged.RemoveAllListeners();
        close_btn.onClick.RemoveAllListeners();

        DevOptionsManagerScript.Instance.gameObject.SetActive(true);

        Destroy(gameObject);
    }
}
