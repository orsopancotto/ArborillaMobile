using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class BiodiversityBarScript : MonoBehaviour
{
    [SerializeField] private Image mask;
    [SerializeField] private TextMeshProUGUI level_txt, progress_txt;
    private float speed;

    
    private void Awake()
    {
        BiodiversityManagerSO.Singleton.OnBiodivProgressUpdated += OnBiodivProgressUpdated_UpdateUI;
        BiodiversityManagerSO.Singleton.OnBiodivLevelUp += OnBiodivLevelUp_UpdateUI;

        if (SystemInfo.deviceType != DeviceType.Desktop) speed = .3f;

        else speed = .15f;
    }

    private void Start()
    {
        InitializeUI();
    }

    private void InitializeUI()
    {
        float f;

        //gestisce l'errore f/0;
        try
        {
            f = (float)GameData.currentSessionData.biodivLvlProgress / (float)GameData.currentSessionData.maxLvlProgress;
        }

        catch
        {
            f = 0;
        }

        level_txt.SetText(GameData.currentSessionData.biodivLvl.ToString());

        progress_txt.SetText($"PROGRESS: {(byte)(f * 100)}");

        mask.fillAmount = f;
    }

    private void OnBiodivProgressUpdated_UpdateUI(float updated_amount, float max_amount)       //aggiornamento UI in seguito ad evento di variazione biodiv
    {
        StopAllCoroutines();

        StartCoroutine(ProgressAnimation(updated_amount / max_amount));     //animazione progress bar
    }

    private void OnBiodivLevelUp_UpdateUI(byte updated_lvl, float updated_amount, float max_amount)     //aggiornamento UI in seguito ad evento di level up
    {
        level_txt.SetText(updated_lvl.ToString());

        mask.fillAmount = 0;

        StartCoroutine(ProgressAnimation(updated_amount / max_amount));
    }

    private IEnumerator ProgressAnimation(float new_progress)
    {
        float starting_delta = Mathf.Abs(new_progress - mask.fillAmount);

        while(!HasReachedTargetValue(starting_delta, new_progress, mask.fillAmount))
        {
            mask.fillAmount = Mathf.Lerp(mask.fillAmount, new_progress, speed);

            yield return null;
        }

        progress_txt.SetText($"PROGRESS: {(byte)(new_progress * 100)}%");
    }

    private bool HasReachedTargetValue(float start, float target, float current)
    {
        return Mathf.Abs(target - current) / start < .05f;
    }

    private void OnDestroy()
    {
        BiodiversityManagerSO.Singleton.OnBiodivProgressUpdated -= OnBiodivProgressUpdated_UpdateUI;
        BiodiversityManagerSO.Singleton.OnBiodivLevelUp -= OnBiodivLevelUp_UpdateUI;
    }
}
