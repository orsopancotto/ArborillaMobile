using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ErrorLogScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI log_msg;
    [SerializeField] private Button close_btn;
    [SerializeField] private GameObject panel;
    private UserControllerTD controller;

    private void Start()
    {
        close_btn.onClick.AddListener(delegate { OnClick_CloseWindow(); });

        Application.logMessageReceived += OnLogMessageReceived_DisplayMessage;

        controller = Camera.main.GetComponent<UserControllerTD>();
    }

    private void OnLogMessageReceived_DisplayMessage(string logString, string stackTrace, LogType type)
    {
        if (type != LogType.Error && type != LogType.Exception) return;

        log_msg.SetText($"{type.ToString().ToUpper()}: {logString}\n\nSTACK TRACE: {stackTrace}");

        if (controller.isActiveAndEnabled && controller != null) controller.enabled = false;

        panel.SetActive(true);
    }

    private void OnClick_CloseWindow()
    {
        log_msg.SetText("");

        Camera.main.GetComponent<UserControllerTD>().enabled = true;

        panel.SetActive(false);
    }
}
