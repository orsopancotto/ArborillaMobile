using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlantScript : MonoBehaviour, IUniversalInteractions
{
    [SerializeField] private GameObject info_canvas, impollination_canvas;
    [SerializeField] private ParticleSystem particle_system;
    private ParticleSystem.MainModule ps_main_module;
    private PlantGenetics genetics;
    private short current_stage_time;
    private bool is_interaction_available = false;
    private LifeStage current_life_stage = 0;
    private PlantGenetics.AllelesCouple current_son_chromes;
    private CameraAnimations camera_animations_script;

    public enum LifeStage
    {
        Growing, Blooming, Pending, FruitsBearing
    }

    internal void Initialization(PlantGenetics.AllelesCouple _chromes)      //overload "base" del metodo, indirettamente chiamato dalla UI in fase di semina
    {
        genetics = new PlantGenetics(_chromes);

        BiodiversityManager.Singleton.UpdateBiodivLevelProgress(genetics, true);

        current_life_stage = LifeStage.Growing;

        current_stage_time = genetics.defaultTimeToGrow;

        PlantDataManager.Singleton.OnSaveDataCall += SaveData_OnSaveDataCall;        //iscrivo la funzione all'evento chiamato dal plant data manager in chiusura app

        impollination_canvas.SetActive(false);

        camera_animations_script = Camera.main.GetComponent<CameraAnimations>();

        camera_animations_script.OnCameraCloseUp += delegate { GetComponentInChildren<Collider>().enabled = false; };       //disattivo il collider a tutte le piante altrimenti cliccando su una pianta in secondo piano si rompe tutto

        camera_animations_script.OnCameraMoveAway += delegate { GetComponentInChildren<Collider>().enabled = true; };

        ps_main_module = particle_system.main;

        impollination_canvas.GetComponentInChildren<Button>(true).onClick.AddListener(delegate { camera_animations_script.ResetPosition(impollination_canvas); });      //assegnazione funzione di uscita impollinazione a bottone specifico del canvas di impollinazione

        StartGrowingStage(0);
    }

    #region PROCEDURE DI CARICAMENTO

    internal void Initialization(PlantGenetics.AllelesCouple loaded_chromes, LifeStage loaded_life_stage, PlantGenetics.AllelesCouple loaded_son_chromes, short loaded_stage_time)     //overload specifico del metodo Initialization, indirettamente chiamato da PlantDataManager in fase di cariamento
    {
        genetics = new PlantGenetics(loaded_chromes);

        BiodiversityManager.Singleton.LoadPlantsInScene(loaded_chromes);       //aggiunge la pianta al dizionario di piante presenti in scena, senza aggiornare il progresso di biodiv

        current_son_chromes = loaded_son_chromes;

        PlantDataManager.Singleton.OnSaveDataCall += SaveData_OnSaveDataCall;        //iscrivo la funzione all'evento chiamato dal plant data manager in chiusura app

        impollination_canvas.SetActive(false);

        camera_animations_script = Camera.main.GetComponent<CameraAnimations>();

        camera_animations_script.OnCameraCloseUp += OnCameraMoveInOrOut_DisableOrEnableCollider;       //disattivo il collider a tutte le piante altrimenti cliccando su una pianta in secondo piano si rompe tutto

        camera_animations_script.OnCameraMoveAway += OnCameraMoveInOrOut_DisableOrEnableCollider;

        ps_main_module = particle_system.main;

        impollination_canvas.GetComponentInChildren<Button>(true).onClick.AddListener(delegate { camera_animations_script.ResetPosition(impollination_canvas); });      //assegnazione funzione di uscita impollinazione a bottone specifico del canvas di impollinazione

        DetermineProgress(loaded_stage_time, loaded_life_stage);
    }

    private void DetermineProgress(short loaded_stage_time, LifeStage loaded_life_stage)       //determina da che procedura partire per determinare in quale stato è ora la pianta che avevo salvato
    {
        switch (loaded_life_stage)
        {
            case LifeStage.Growing:
                DetermineProgress_GrowingStage(loaded_stage_time);
                return;

            case LifeStage.Blooming:
                DetermineProgress_BloomingStage(loaded_stage_time);
                return;

            case LifeStage.Pending:
                DetermineProgress_PendingStage();
                return;

            case LifeStage.FruitsBearing:
                DetermineProgress_FruitsBearingStage(loaded_stage_time);
                return;
        }
    }

    private void DetermineProgress_GrowingStage(short loaded_stage_time)        //controllo progressi in fase Growing (0)
    {
        if (loaded_stage_time <= 0)       //la fase è finita; avvio controllo della fase successiva
        {
            /* a questo punto il timer è fuori fase: è < 0 ma ha progredito allo stesso ritmo.
             * Affinchè il controllo successivo venga eseguito correttamente il timer deve essere rimesso in fase, 
             * e per fare ciò mi basterà aggiungere una quantità di tempo t = defaultTimeToBloom (ovvero la quantità di partenza default della fase successiva) al timer
             */
            DetermineProgress_BloomingStage((short)(loaded_stage_time + genetics.defaultTimeToBloom));  
        }

        else    //la fase non è terminata; determino i progressi e riprendo da lì
        {
            current_stage_time = loaded_stage_time;

            current_life_stage = LifeStage.Growing;

            for (int i = 1; i <= 3; i++)        // i modelli che si susseguono sono 3 + 1 (quello default di partenza); determina qual è il modello di partenza corrente, e riprende il timer
            {
                if (current_stage_time > genetics.defaultTimeToGrow - i * (genetics.defaultTimeToGrow / 3) && current_stage_time != 0)
                {
                    StartGrowingStage(i - 1);

                    break;
                }
            }
        }
    }

    private void DetermineProgress_BloomingStage(short loaded_stage_time)       //controllo progressi in fase Booming (1)
    {
        current_life_stage = LifeStage.Blooming;

        Instantiate(genetics.Models[3], transform);       //se mi trovo in questa fase vuol dire che la pianta è al massimo della sua crescita

        if (loaded_stage_time <= 0)       //la fase è terminata: la pianta è in fiore e il suo polline è pronto ad essere raccolto
        {
            current_stage_time = 0;     //questa fase non ha timer

            PollenProduced();
        }

        else    //la fase non è terminata: proseguo col timer di fioritura considerandone i progressi fatti
        {
            current_stage_time = loaded_stage_time;

            StartBloomingStage();
        }

    }

    private void DetermineProgress_PendingStage()       //controllo progressi in fase Pending (2)
    {
        //non ci sono controlli da fare, è una fase stazionaria; semplicemente ne faccio un set up

        Instantiate(genetics.Models[3], transform);

        current_stage_time = 0;

        current_life_stage = LifeStage.Pending;

        is_interaction_available = true;

        info_canvas.SetActive(true);

        StartPendingStage();
    }

    private void DetermineProgress_FruitsBearingStage(short loaded_stage_time)      //controllo progressi in fase FruitBearing (3)
    {
        Instantiate(genetics.Models[3], transform);

        current_life_stage = LifeStage.FruitsBearing;

        if (loaded_stage_time <= 0)       //la fase è terminata: passo alla prossima e ultima fase del ciclo della pianta
        {
            current_stage_time = 0;

            FruitsProduced();
        }

        else    //la fase non è terminata: proseguo col timer di crescita frutti considerandone i progressi fatti
        {
            current_stage_time = loaded_stage_time;

            StartFruitsBearingStage();
        }
    }

    #endregion

    private void StartGrowingStage(int starting_model_index)
    {
        StartCoroutine(GrowingStageTimer(starting_model_index));
    }

    private void StartBloomingStage()
    {
        StartCoroutine(BloomingStageTimer());
    }

    private void StartPendingStage()
    {
        info_canvas.GetComponentInChildren<TextMeshProUGUI>().SetText("Attesa...");
    }

    private void StartFruitsBearingStage()
    {
        StartCoroutine(GrowingFruitsStageTimer());
    }

    private void PollenProduced()
    {
        is_interaction_available = true;

        info_canvas.GetComponentInChildren<TextMeshProUGUI>().SetText("fiori :)");

        info_canvas.SetActive(true);
    }

    private void FruitsProduced()
    {
        is_interaction_available = true;

        info_canvas.GetComponentInChildren<TextMeshProUGUI>().SetText("frutti :)");

        info_canvas.SetActive(true);
    }

    private void StartFruitsHarvest()
    {
        Transform[] anchors = Instantiate(genetics.Models[4], transform).GetComponentsInChildren<Transform>();

        GameObject fruit;

        //****TEMP****
        switch (current_son_chromes)
        {
            case PlantGenetics.AllelesCouple.BB:
                fruit = Resources.Load<PlantsDictionaryScriptableObject>("Plants Dictionary").chromes_fruit[PlantGenetics.AllelesCouple.BB];
                break;

            case PlantGenetics.AllelesCouple.CC:
                fruit = Resources.Load<PlantsDictionaryScriptableObject>("Plants Dictionary").chromes_fruit[PlantGenetics.AllelesCouple.CC];
                break;

            default:
                fruit = Resources.Load<PlantsDictionaryScriptableObject>("Plants Dictionary").chromes_fruit[PlantGenetics.AllelesCouple.AA];
                break;
        }
        //****TEMP****

        byte amount = (byte)UnityEngine.Random.Range(1, genetics.avrgFruitsOutput + 1);

        for (byte i = 1; i <= amount; i++)      //parto da 1 perche anchors[0] è il transform del parent
        {
            Instantiate(fruit, anchors[i]).GetComponent<FruitScript>().Parameters = new FruitScript.Params(current_son_chromes, gameObject);
        }

        camera_animations_script.MoveToPlant(new Vector2(transform.position.x, transform.position.z));
    }

    private IEnumerator GrowingStageTimer(int starting_model_index)        //veramente brutto
    {
        info_canvas.SetActive(true);

        TextMeshProUGUI txt = info_canvas.GetComponentInChildren<TextMeshProUGUI>();
        txt.SetText(current_stage_time.ToString());

        GameObject current_model = Instantiate(genetics.Models[starting_model_index], transform);

        yield return new WaitForSeconds(1);

        current_stage_time -= 1;

        txt.SetText(current_stage_time.ToString());

        while (current_stage_time > 0)
        {
            if (current_stage_time % (genetics.defaultTimeToGrow / 3) == 0)
            {
                Destroy(current_model);

                current_model = Instantiate(genetics.Models[starting_model_index += 1], transform);
            }

            yield return new WaitForSeconds(1);

            current_stage_time -= 1;

            txt.SetText(current_stage_time.ToString());
        }

        Destroy(current_model);

        Instantiate(genetics.Models[3], transform);

        info_canvas.SetActive(false);

        current_stage_time = genetics.defaultTimeToBloom;

        current_life_stage = LifeStage.Blooming;

        StartBloomingStage();
    }      

    private IEnumerator BloomingStageTimer()
    {        
        while(current_stage_time > 0)
        {
            yield return new WaitForSeconds(1);

            current_stage_time -= 1;
        }

        PollenProduced();
    }

    private IEnumerator GrowingFruitsStageTimer()
    {        
        while(current_stage_time > 0)
        {
            yield return new WaitForSeconds(1);

            current_stage_time -= 1;
        }

        FruitsProduced();
    }

    private void TriggerParticleSystem(Color color)
    {
        ps_main_module.startColor = color;
        particle_system.Play();
    }

    internal void FruitsHarvestEnded()
    {
        //camera_animations_script.ResetPosition();     lo faccio eseguire da FruitScript

        current_life_stage = LifeStage.Blooming;

        current_stage_time = genetics.defaultTimeToBloom;

        is_interaction_available = false;

        StartBloomingStage();
    }

    private void SaveData_OnSaveDataCall(object sender, EventArgs e)        //aggiungo i dati relativi a questa pianta al dizionario di plant data manager
    {
        PlantDataManager.Singleton.plants_saved.Add(
            GetComponentsInParent<Transform>()[1].gameObject.name,
            (genetics.chromosomes, current_life_stage, current_son_chromes, current_stage_time, DateTime.Now.ToString())
            );
    }

    internal void ArtificialImpollination_OnClick(PlantGenetics.AllelesCouple chromes)      //procedure di impollinazione artificiale
    {
        is_interaction_available = false;

        current_life_stage = LifeStage.FruitsBearing;

        current_stage_time = genetics.defaultTimeToBearFruits;

        current_son_chromes = CouplingAlgorithm.CalculateHybrid(genetics, new PlantGenetics(chromes));

        InventoryManagerScript.Singleton.UpdatePollenCollection(chromes, -1);

        camera_animations_script.ResetPosition(impollination_canvas);

        StartFruitsBearingStage();
    }

    internal void SelfDestroyProcedures()       //fine al testing del sistema di salvataggio e caricamento dati
    {
        PlantDataManager.Singleton.OnSaveDataCall -= SaveData_OnSaveDataCall;

        camera_animations_script.OnCameraCloseUp -= OnCameraMoveInOrOut_DisableOrEnableCollider;

        camera_animations_script.OnCameraMoveAway -= OnCameraMoveInOrOut_DisableOrEnableCollider;

        BiodiversityManager.Singleton.UpdateBiodivLevelProgress(genetics, false);

        Destroy(gameObject);
    }

    private void OnCameraMoveInOrOut_DisableOrEnableCollider(bool b)      //ho dovuto rimpiazzare la rispettiva funzione anonima con questo metodo, altrimenti disiscriversi all'evento diventa inutilmente complicato
    {
        Collider coll = GetComponentInChildren<Collider>();
        coll.enabled = b;
    }

    #region AZIONI CHIAMATE DA USER INPUT

    public void Grabbed()
    {
    }

    public void InteractionEnded()
    {
    }

    public void Selected()
    {
        if (!is_interaction_available) return;

        switch (current_life_stage)
        {
            case LifeStage.Blooming:
                TriggerParticleSystem(Color.cyan);
                InventoryManagerScript.Singleton.UpdatePollenCollection(genetics.chromosomes, 1);
                current_life_stage = LifeStage.Pending;
                StartPendingStage();
                break;

            case LifeStage.Pending:
                info_canvas.SetActive(false);
                camera_animations_script.MoveToPlant(new Vector2(transform.position.x, transform.position.z), impollination_canvas);
                break;

            case LifeStage.FruitsBearing:
                info_canvas.SetActive(false);
                StartFruitsHarvest();
                break;
        }

    }

    public void Move(Vector3 touch_position, float snappiness)
    {
    }

    #endregion
}
