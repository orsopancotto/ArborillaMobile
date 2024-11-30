using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class DevOptionsManagerScript : MonoBehaviour
{
    [SerializeField] private Button close_btn, open_btn, camera_anim_btn, controller_params_btn, shovel_params_btn;
    [SerializeField] private Transform canvas;
    internal static DevOptionsManagerScript Instance;
    private readonly string main_directory = "Prefabs/UI/Dev Options Panels";

    private void OnEnable()
    {
        Instance = this;
    }

    private void Start()
    {
        InitializeButtons();
        gameObject.SetActive(false);
    }

    private void InitializeButtons()
    {
        open_btn.onClick.AddListener(delegate { OpenMenu_OnClick(); });
        close_btn.onClick.AddListener(delegate { CloseMenu_OnClick(); });
        camera_anim_btn.onClick.AddListener(delegate { OpenPanelInResources("/Camera Animations Panel"); });
        controller_params_btn.onClick.AddListener(delegate { OpenPanelInResources("/Controller Params Panel"); });
        shovel_params_btn.onClick.AddListener(delegate { OpenPanelInResources("/Shovel Params Panel"); });
    }

    private void OpenMenu_OnClick()
    {
        gameObject.SetActive(true);
        open_btn.enabled = false;
        Camera.main.GetComponent<UserControllerTD>().enabled = false;
    }

    private void CloseMenu_OnClick()
    {
        gameObject.SetActive(false);
        open_btn.enabled = true;
        Camera.main.GetComponent<UserControllerTD>().enabled = true;
    }

    private void OpenPanelInResources(string specific_directory)
    {
        Instantiate(Resources.Load($"{main_directory}{specific_directory}") as GameObject, canvas);
        gameObject.SetActive(false);
    }

    //private void OpenCameraAnimPanel()
    //{
    //    Instantiate(Resources.Load($"{main_directory}/Camera Animations Panel") as GameObject, canvas);
    //    gameObject.SetActive(false);
    //}

    //private void OpenControllerParamsPanel()
    //{
    //    Instantiate(Resources.Load($"{main_directory}/Controller Params Panel") as GameObject, canvas);
    //    gameObject.SetActive(false);
    //}
}
