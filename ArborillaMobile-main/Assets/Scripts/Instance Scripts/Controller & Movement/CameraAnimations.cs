using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[DisallowMultipleComponent]
public class CameraAnimations : MonoBehaviour
{
#if !UNITY_EDITOR

    internal float speed = .2f;
    internal float FOV_speed = .15f;

#else

    [Range(.01f, .1f)]
    [SerializeField] internal float speed = .04f;       //da mettere private  

    [Range(.01f, .5f)]
    [SerializeField] internal float FOV_speed = .1f;      //da mettere private

#endif


    [Range(1, 10)]
    [SerializeField] internal float distance_from_plant = 6;     //da mettere private

    [Range(1, 5)]
    [SerializeField] internal float lowering_amount = 2;     //da mettere private

    [SerializeField] private UserControllerTD controller;

    internal event Action<bool> OnCameraCloseUp;    //ho scoperto adesso che non so usare Action perchè ho dei problemi a disiscrivere dall'evento un delegato
    internal event Action<bool> OnCameraMoveAway;

    private Vector3 default_position;
    private const float default_camera_height = 8, default_FOV = 70;      //impostato su unity arbitrariamente

    private bool DoCoordinatesMatch(Vector3 target_pos, float starting_distance)
    {
        return Vector3.Distance(transform.position, target_pos) / starting_distance * 100 <= 1 && this != null;
    }

    private bool DoValuesMatch(float target_value, float starting_delta, float current_value)
    {
        return MathF.Abs(current_value - target_value) / starting_delta * 100 <= 1 && this != null;
    }


    //#region METODO COROUTINE INCOMPLETO

    //internal void MoveToPlant(Vector2 plant_coordinates)        //chiamato in fase di raccolta frutti
    //{
    //    default_position = transform.position;

    //    OnCameraCloseUp?.Invoke();

    //    StopAllCoroutines();

    //    StartCoroutine(MoveToCoordinatesAnimation(new Vector3(plant_coordinates.x, default_camera_height, plant_coordinates.y), distance_from_plant, lowering_amount));
    //}

    //internal void MoveToPlant(Vector2 plant_coordinates, GameObject impollination_canvas)       //overload chiamato in fase di impollinazione
    //{
    //    default_position = transform.position;

    //    impollination_canvas.SetActive(true);

    //    OnCameraCloseUp?.Invoke();

    //    StopAllCoroutines();

    //    StartCoroutine(MoveToCoordinatesAnimation(new Vector3(plant_coordinates.x, default_camera_height, plant_coordinates.y), distance_from_plant, lowering_amount));
    //}

    //internal void ResetPosition()       //chiamato in uscita da raccolta frutti
    //{
    //    OnCameraMoveAway?.Invoke();

    //    StopAllCoroutines();

    //    StartCoroutine(MoveToCoordinatesAnimation(default_position, 0, 0));
    //}

    //internal void ResetPosition(GameObject impollination_canvas)        //overload chiamato in uscita da impolliazione
    //{
    //    impollination_canvas.SetActive(false);

    //    OnCameraMoveAway?.Invoke();

    //    StopAllCoroutines();

    //    StartCoroutine(MoveToCoordinatesAnimation(default_position, 0, 0));
    //}

    //private IEnumerator MoveToCoordinatesAnimation(Vector3 target_coordinates, float z_offset, float y_offset)
    //{
    //    controller.enabled = false;

    //    if (Camera.main.fieldOfView != default_FOV) StartCoroutine(ResetFOV());

    //    float starting_distance = Vector3.Distance(transform.position, target_coordinates);

    //    target_coordinates += new Vector3(0, -y_offset, -z_offset);

    //    while (!DoCoordinatesMatch(target_coordinates, starting_distance))
    //    {
    //        transform.position = Vector3.Lerp(transform.position, target_coordinates, speed);

    //        yield return null;
    //    }

    //    controller.enabled = true;
    //}

    //private IEnumerator ResetFOV()
    //{
    //    float starting_delta = MathF.Abs(Camera.main.fieldOfView - default_FOV);

    //    while (!DoValuesMatch(default_FOV, starting_delta))
    //    {
    //        Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, default_FOV, FOV_speed);

    //        yield return null;
    //    }

    //    Camera.main.fieldOfView = default_FOV;

    //}

    //#endregion


    #region METODO ASYNC

    internal void MoveToPlant(Vector2 plant_coordinates)        //chiamato in fase di raccolta frutti
    {
        default_position = transform.position;

        OnCameraCloseUp?.Invoke(false);

        MoveToCoordinatesAnimation(new Vector3(plant_coordinates.x, default_camera_height, plant_coordinates.y), distance_from_plant, lowering_amount);
    }

    internal void MoveToPlant(Vector2 plant_coordinates, GameObject impollination_canvas)       //overload chiamato in fase di impollinazione
    {
        default_position = transform.position;

        impollination_canvas.SetActive(true);

        OnCameraCloseUp?.Invoke(false);

        MoveToCoordinatesAnimation(new Vector3(plant_coordinates.x, default_camera_height, plant_coordinates.y), distance_from_plant, lowering_amount);
    }

    internal void ResetPosition()       //chiamato in uscita da raccolta frutti
    {
        OnCameraMoveAway?.Invoke(true);

        MoveToCoordinatesAnimation(default_position, 0, 0);
    }

    internal void ResetPosition(GameObject impollination_canvas)        //overload chiamato in uscita da impolliazione
    {
        impollination_canvas.SetActive(false);

        OnCameraMoveAway?.Invoke(true);

        MoveToCoordinatesAnimation(default_position, 0, 0);
    }

    private async void MoveToCoordinatesAnimation(Vector3 target_coordinates, float z_offset, float y_offset)
    {
        if(Camera.main.fieldOfView != default_FOV) await LateralMovement(target_coordinates.x);

        controller.enabled = false;

        float starting_distance = Vector3.Distance(transform.position, target_coordinates);

        target_coordinates += new Vector3(0, -y_offset, -z_offset);

        while (!DoCoordinatesMatch(target_coordinates, starting_distance))
        {
            if (!this) return;

            transform.position = Vector3.Lerp(transform.position, target_coordinates, speed);

            await Task.Yield();
        }

        controller.enabled = true;
    }

    private async Task LateralMovement(float target_xPos)
    {
        float starting_delta = Mathf.Abs(transform.position.x - target_xPos);

        ResetFOV();

        while(!DoValuesMatch(target_xPos, starting_delta, transform.position.x))
        {
            if (!this) return;

            transform.position = Vector3.Lerp(transform.position, new Vector3(target_xPos, transform.position.y, transform.position.z), speed);

            await Task.Yield();
        }
    }

    private async void ResetFOV()
    {
        float starting_delta = MathF.Abs(Camera.main.fieldOfView - default_FOV);

        while (!DoValuesMatch(default_FOV, starting_delta, Camera.main.fieldOfView))
        {
            if (!this) return;

            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, default_FOV, FOV_speed);

            await Task.Yield();
        }

        Camera.main.fieldOfView = default_FOV;
    }

    #endregion
}
