using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SceneLoaderScript : MonoBehaviour
{
    [SerializeField] private Image loading_panel;
    [SerializeField] private GameObject canvas;
#if !UNITY_EDITOR
    private float fade_speed = .3f;
#else 
    private float fade_speed = 0.05f;
#endif
    private byte current_scene;
    private Button change_scene_btn;
    internal static SceneLoaderScript Instance;
    internal Action OnSceneLoadingProcedure;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        StartSceneChangeProcedures();
    }

    private void InitializeButton()     //Assegno riferimento e evento OnClick al bottone di cambio scena della nuova scena
    {
        change_scene_btn = GameObject.Find("Change Scene Btn").GetComponent<Button>();

        change_scene_btn.onClick.AddListener(delegate
        {
            StartSceneChangeProcedures();
        });
    }

    private async void StartSceneChangeProcedures()
    {
        // 0:StartingScene  1:PersonalOasisScene  2:TransitionScene  3:MainScene

        current_scene = (byte)SceneManager.GetActiveScene().buildIndex;

        switch (current_scene)
        {
            case 0:
                await SwapScenes(1, 0);     //StartingScene --> PersonalOasis
                await FadeAnimation(0);     
                canvas.SetActive(false);
                InitializeButton();
                break;

            case 1:
                canvas.SetActive(true);
                await FadeAnimation(1);
                await SwapScenes(2, 1);     //PersonalOasisScene --> TransitionScene
                await SwapScenes(3, 2);     //TransitionScene --> MainScene
                await FadeAnimation(0);
                canvas.SetActive(false);
                InitializeButton();
                break;

            case 3:
                canvas.SetActive(true);
                await FadeAnimation(1);
                await SwapScenes(2, 3);     //MainScene --> TransitionScene
                await SwapScenes(1, 2);     //TransitionScene --> PersonalOasisScene
                await FadeAnimation(0);
                canvas.SetActive(false);
                InitializeButton();
                break;

        }
    }

    private async Task FadeAnimation(float target_alpha)
    {
        float starting_delta = Mathf.Abs(loading_panel.color.a - target_alpha);

        while (!HasReachedTargetValue(starting_delta, target_alpha, loading_panel.color.a))
        {
            loading_panel.color = new Color(
                loading_panel.color.r,
                loading_panel.color.g,
                loading_panel.color.b,
                Mathf.Lerp(loading_panel.color.a, target_alpha, fade_speed)
                );

            await Task.Yield();
        }
    }

    private async Task SwapScenes(int new_scene_index, int old_scene_index)     //loading della nuova scena con seguente unloading della precedente
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync(new_scene_index, LoadSceneMode.Additive);

        while (!loading.isDone)
        {
            await Task.Yield();
        }

        SetUpScenes(new_scene_index);

        SceneManager.UnloadSceneAsync(old_scene_index);
    }

    private void SetUpScenes(int new_scene_index)       //set up di nuova e vecchia scena
    {
        OnSceneLoadingProcedure?.Invoke();      //dico a DataPersistanceManager della scena precedente di salvare i dati (se esiste)

        if(change_scene_btn != null) change_scene_btn.onClick.RemoveAllListeners();     //rimuovo il listener dal bottone della scena precedente

        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex(new_scene_index));     //sposto questo oggetto alla nuova scena
    }

    private bool HasReachedTargetValue(float starting_delta, float target_value, float current_value)
    {
        return Mathf.Abs(current_value - target_value) / starting_delta * 100 <= 1;
    }
}
