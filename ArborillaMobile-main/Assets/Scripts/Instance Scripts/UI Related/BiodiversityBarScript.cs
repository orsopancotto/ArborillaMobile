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

    
    void Start()
    {
        BiodiversityManager.Singleton.OnBiodivProgressUpdated += OnBiodivProgressUpdated_UpdateUI;
        BiodiversityManager.Singleton.OnBiodivLevelUp += OnBiodivLevelUp_UpdateUI;

        if (SystemInfo.deviceType != DeviceType.Desktop) speed = .3f;

        else speed = .15f;
    }

    internal void InitializeUI(byte loaded_level, float loaded_progress, float max_lvl_progress)
    {
        float f;

        //gestisce l'errore f/0;
        try
        {
            f = loaded_progress / max_lvl_progress;
        }

        catch
        {
            f = 0;
        }

        level_txt.SetText(loaded_level.ToString());

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
        if(mask.fillAmount < new_progress)
        {
            while (mask.fillAmount < new_progress)
            {
                mask.fillAmount = Mathf.Lerp(mask.fillAmount, new_progress + .05f, speed);

                yield return null;
            }
        }

        else
        {
            while (mask.fillAmount > new_progress)
            {
                mask.fillAmount = Mathf.Lerp(mask.fillAmount, new_progress - .05f, speed);

                yield return null;
            }
        }

        progress_txt.SetText($"PROGRESS: {(byte)(new_progress * 100)}%");
    }
}
