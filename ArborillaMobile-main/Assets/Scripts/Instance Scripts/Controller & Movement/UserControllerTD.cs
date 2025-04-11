using UnityEngine;
using CandyCoded.HapticFeedback;

[DisallowMultipleComponent]
public class UserControllerTD : MonoBehaviour
{
    #region PARAMETRI

    [Range(.15f, .3f)]
    [SerializeField] private float max_contact_time;

    [Range(.1f, 1)]
    [SerializeField] internal float snappiness;

    [Range(.5f, 2.5f)]
    [SerializeField] internal float zoom_factor;

    [Range(55, 69)]
    [SerializeField] internal float min_FOV = 60;

    [Range(71, 85)]
    [SerializeField] internal float max_FOV = 80;

    [Range(1, 10)]
    [SerializeField] internal float moving_around_speed;

    private static float current_general_contact_time, current_distance_between_touches, cached_distance_between_touches, predicted_zoom;
    private static Vector3 touch_projection;
    private static IUniversalInteractions obj_interacted_with;
    private static RaycastHit hit;
    private static InteractionStates current_state = InteractionStates.None;

    #endregion

    private enum InteractionStates      //praticamente una state machine
    {
        None, Grabbing, Moving, UI, Zooming
    }

    private void Start()
    {
#if !UNITY_EDITOR
        snappiness = .25f;
        moving_around_speed = 2;
        zoom_factor = .85f;
#else
        snappiness = .15f;
        moving_around_speed = 7;
#endif
    }

    private void Update()
    {
        if (Input.touchCount > 1) TwoFingersInputManager(Input.GetTouch(0), Input.GetTouch(1));

        else if (Input.touchCount > 0) OneFingerInputManager(Input.GetTouch(0));
    }

#region TWO FINGERS
    private void TwoFingersInputManager(Touch current_touch_0, Touch current_touch_1)
    {
        /* appena inizia il secondo tocco imposto il valore di partenza della distanza tra i tocchi,
        * senza avviare la funzione di zoom sennò snappa */

        if (current_touch_1.phase == TouchPhase.Began)
        {
            cached_distance_between_touches = DistanceBetweenTouches(current_touch_0.position, current_touch_1.position);

            current_state = InteractionStates.Zooming;

            ResetParameters();

            return;
        }

        //per evitare problemi di interazione quando si rilascia una delle due dita

        if (current_touch_0.phase == TouchPhase.Ended ^ current_touch_1.phase == TouchPhase.Ended)       
        {
            current_state = InteractionStates.Moving;
        }

        //aggiorno la distanza tra i tocchi, la variazione tra la distanza corrente e quella precedentemente registrata (cached)
        //verranno usate per determinare direzione e intensità dello zoom

        current_distance_between_touches = DistanceBetweenTouches(current_touch_0.position, current_touch_1.position);

        Zoom();

        cached_distance_between_touches = current_distance_between_touches;
    }

    private float DistanceBetweenTouches(Vector2 a, Vector2 b)
    {
        return Mathf.Abs(Vector2.Distance(a, b));
    }

    private void Zoom()
    {
        predicted_zoom = CalculateZoom();

        if (predicted_zoom < min_FOV || predicted_zoom > max_FOV || !CameraMovementAuth.Singleton.IsZoomAuthorized()) return;

        Camera.main.fieldOfView = predicted_zoom;
    }

    private float CalculateZoom()
    {
        return Camera.main.fieldOfView - (current_distance_between_touches - cached_distance_between_touches) * zoom_factor * Time.deltaTime;
    }

#endregion

#region ONE FINGER
    private void OneFingerInputManager(Touch current_touch)
    {
        TouchParametersControl(current_touch);

        ActionsManager(current_touch);
    }

    private void TouchParametersControl(Touch current_touch)        //controllo e caching dei parametri del tocco utili
    {
        //da questo raggio ricavo la proiezione del tocco in World Coordinates e su che oggetto sto premendo
        Physics.Raycast(Camera.main.ScreenPointToRay(current_touch.position), out hit); 

        //caching dell'oggetto interagibile su cui clicco nel primo frame (se esiste)
        if (current_touch.phase == TouchPhase.Began && hit.collider.CompareTag("Interactable"))
        {
            obj_interacted_with = hit.collider.gameObject.GetComponentInParent<IUniversalInteractions>();
        }

        else if(current_touch.phase == TouchPhase.Began && IsTouchInsideUIZone(current_touch.position.y))
        {
            current_state = InteractionStates.UI;
        }

        //cronometro il tempo di contatto tra dito e schermo, se inferiore alla soglia massima di tempo
        if (current_general_contact_time <= max_contact_time)
        {
            current_general_contact_time += Time.deltaTime;
        }

        touch_projection = hit.point;
    }

    private void ActionsManager(Touch current_touch)        //gestore delle possibili azioni, ne avvio solamente una alla volta
    {
        //se sono instato di quiete e ho tenuto premuto su un oggetto interagibile per almeno max_contact_time, allora lo afferro
        if (current_state == InteractionStates.None && hit.collider.CompareTag("Interactable") && current_general_contact_time > max_contact_time)
        {
            TransitionToGrabbing();
        }

        //se sono in stato di Grabbing allora muovo l'oggetto afferrato 
        else if (current_state == InteractionStates.Grabbing)
        {
            obj_interacted_with.Move(touch_projection, snappiness);
        }

        //se sono in stato di quiete e muovo il dito allora entro in stato di movimento 
        else if (current_touch.phase == TouchPhase.Moved && current_state == InteractionStates.None)
        {
            TransitionToMoving();
        }

        //se sono in stato di movimento e muovo il dito sposto la telecamera
        else if (current_touch.phase == TouchPhase.Moved && current_state == InteractionStates.Moving)
        {
            MoveCamera(current_touch.deltaPosition);
        }

        /*se termina il contatto tra dito e schermo, allora:
         * se ho interagito con un oggetto interagibile rimanendo in stato di quiete allora ho selezionato l'oggetto
         * resetto i parametri e la "state machine"
         */
        if (current_touch.phase == TouchPhase.Ended || current_touch.phase == TouchPhase.Canceled)
        {
            if (obj_interacted_with != null && current_state == InteractionStates.None)
                obj_interacted_with.Selected();

            ResetParameters();
        }

    }

    private bool IsTouchInsideUIZone(float y_screenPos)     //da rivedere
    {
        return y_screenPos < .2f * Screen.height;
    }

    private void TransitionToGrabbing()       //transizione a stato Grabbing
    {
        current_state = InteractionStates.Grabbing;

        HapticFeedback.LightFeedback();

        obj_interacted_with.Grabbed();
    }

    private void TransitionToMoving()       //transizione a stato Moving
    {
        current_state = InteractionStates.Moving;
    }

    private void MoveCamera(Vector2 delta_position)     //funzione di spostamento della telecamera
    {
        //richiesta autorizzazione dello spostamento calcolato
        transform.position = CameraMovementAuth.Singleton.AuthorizeMovement(CalculatePositionShift(delta_position));
    }

    private Vector3 CalculatePositionShift(Vector2 delta_position)
    {
        return - (moving_around_speed / 10) * Time.deltaTime * new Vector3(delta_position.x, 0, delta_position.y);
    }

    private void ResetParameters()
    {
        //all'oggetto afferrato viene fatta eseguire la funzione di ritorno a "stato di quiete"
        if (current_state == InteractionStates.Grabbing)
        {
            obj_interacted_with.InteractionEnded();
        }

        obj_interacted_with = null;

        current_state = InteractionStates.None;

        current_general_contact_time = 0;
    }

#endregion
}
