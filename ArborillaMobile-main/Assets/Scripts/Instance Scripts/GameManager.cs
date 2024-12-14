using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

[DisallowMultipleComponent]
public class GameManager : MonoBehaviour
{
    public static GameManager Singleton { get; private set; }

    [SerializeField] private Image loading_panel;
    [SerializeField] private GameObject canvas;
    [SerializeField] private string data_file_name;

    private Button change_scene_btn;
    private DataPersistance data_persistance_manager;
    private int current_scene_index = 0;

    internal Action<int> OnSceneLoaded;
    internal Action OnGameStarted;

#if !UNITY_EDITOR
    private float fade_speed = .3f;
#else
    private float fade_speed = 0.05f;
#endif

    private void Awake()
    {
        Singleton = this;
        data_persistance_manager = new DataPersistance(data_file_name, FindDataPersistanceObjects());
        data_persistance_manager.LoadGame();
        Debug.Log(GameData.currentSessionData.ToString());
        OnGameStarted?.Invoke();
    }

    private void Start()
    {
        PushData();
        StartSceneChangeProcedures();
    }

    private async void StartSceneChangeProcedures()     //bella porcata
    {
        // 0:StartingScene  1:PersonalOasisScene  2:TransitionScene  3:MainScene

        switch (current_scene_index)
        {
            case 0:
                await SwapScenes(1, 0);     //StartingScene --> PersonalOasis
                OnSceneLoaded?.Invoke(current_scene_index);
                await FadeAnimation(0);
                canvas.SetActive(false);
                InitializeButton();
                break;

            case 1:
                canvas.SetActive(true);
                await FadeAnimation(1);
                data_persistance_manager.SaveGame();
                await SwapScenes(2, 1);     //PersonalOasisScene --> TransitionScene
                await SwapScenes(3, 2);     //TransitionScene --> MainScene
                OnSceneLoaded?.Invoke(current_scene_index);
                await FadeAnimation(0);
                canvas.SetActive(false);
                InitializeButton();
                break;

            case 3:
                canvas.SetActive(true);
                await FadeAnimation(1);
                data_persistance_manager.SaveGame();
                await SwapScenes(2, 3);     //MainScene --> TransitionScene
                await SwapScenes(1, 2);     //TransitionScene --> PersonalOasisScene
                OnSceneLoaded?.Invoke(current_scene_index);
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

        if (change_scene_btn != null) change_scene_btn.onClick.RemoveAllListeners();     //rimuovo il listener dal bottone della scena precedente

        SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetSceneByBuildIndex(new_scene_index));     //sposto questo oggetto alla nuova scena

        SceneManager.UnloadSceneAsync(old_scene_index);

        current_scene_index = SceneManager.GetActiveScene().buildIndex;
    }

    private void InitializeButton()     //Assegno riferimento e evento OnClick al bottone di cambio scena della nuova scena
    {
        change_scene_btn = GameObject.Find("Change Scene Btn").GetComponent<Button>();

        change_scene_btn.onClick.AddListener(delegate
        {
            StartSceneChangeProcedures();
        });
    }

    private bool HasReachedTargetValue(float starting_delta, float target_value, float current_value)
    {
        return Mathf.Abs(current_value - target_value) / starting_delta <= .01f;
    }

    private IEnumerable<IDataPersistance> FindDataPersistanceObjects()
    {
        return Resources.LoadAll<ScriptableObject>("Managers").OfType<IDataPersistance>();
    }

    private void PushData()
    {
        foreach(var obj in data_persistance_manager.dataPersistanceObjects)
        {
            obj.LoadData();
        }
    }

#if UNITY_EDITOR        //OnApplicationQuit non funziona su android, usiamo OnApplicationPause e OnApplicationFocus (metodo non definitivo)
    private void OnApplicationQuit()
    {
        data_persistance_manager.SaveGame();
    }
#else
    private void OnApplicationPause()
    {
        data_persistance_manager.SaveGame();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            data_persistance_manager.SaveGame();
        }
    }
#endif

}
