using System.Collections;
using UnityEngine;
using CandyCoded.HapticFeedback;

[DisallowMultipleComponent]
public class TrashbinScript : MonoBehaviour, IUniversalInteractions
{
#if !UNITY_EDITOR
    
    internal float rise_speed = .35f;
    internal float return_speed = .3f;
    internal float rotation_speed = .35f;
    internal float target_xEulerRotation = -70;
    internal float target_yPosition = 2;
#else

    [Range(.01f, .25f)]
    [SerializeField] internal float rise_speed = .05f;

    [Range(.1f, .6f)]
    [SerializeField] internal float return_speed = .1f;

    [Range(.01f, 1f)]
    [SerializeField] internal float rotation_speed = .07f;

    [Range(-90, -45)]
    [SerializeField] internal float target_xEulerRotation = -50;

    [Range(2, 5)]
    [SerializeField] internal float target_yPosition = 2;

#endif

    private Vector3 initial_position;

    private void Start()
    {
        initial_position = transform.position;
    }

    public void Grabbed()
    {
        StopAllCoroutines();
        StartCoroutine(RiseAnimation(target_yPosition));
        StartCoroutine(RotationAnimation(target_xEulerRotation));

        GetComponent<Collider>().enabled = false;
    }

    public void InteractionEnded()
    {
        StopAllCoroutines();
        StartCoroutine(RiseAnimation(initial_position.y));
        StartCoroutine(RotationAnimation(0));
        StartCoroutine(BackToInitialPositionAnim());
    }

    public void Move(Vector3 touch_position, float speed)
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(touch_position.x, transform.position.y, touch_position.z), speed);
    }

    public void Selected()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            other.gameObject.GetComponentInParent<PlantScript>().SelfDestroyProcedures();

            HapticFeedback.MediumFeedback();
        }
    }

    private bool HasReachedTargetValue(float starting_delta, float target_value, float current_value)
    {
        return Mathf.Abs(target_value - current_value) / starting_delta * 100 <= 1;
    }

    private bool DoCoordinatesMatch(Vector3 target_pos, float starting_distance)
    {
        return Vector3.Distance(transform.position, target_pos) / starting_distance * 100 <= 1;
    }

    private IEnumerator RiseAnimation(float target_yPos)
    {
        float starting_delta_pos = Mathf.Abs(target_yPos - transform.position.y);

        while(!HasReachedTargetValue(starting_delta_pos, target_yPos, transform.position.y))
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, target_yPos, transform.position.z), rise_speed);

            yield return null;
        }

        transform.position = new Vector3(transform.position.x, target_yPos, transform.position.z);
    }

    private IEnumerator RotationAnimation(float target_xEulerRot)
    {
        float starting_delta_rot = Mathf.Abs(target_xEulerRot - transform.rotation.eulerAngles.x);

        while(!HasReachedTargetValue(starting_delta_rot, target_xEulerRot, transform.rotation.eulerAngles.x))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(target_xEulerRot, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), rotation_speed);

            yield return null;
        }
    }

    private IEnumerator BackToInitialPositionAnim()
    {
        float starting_distance = Vector3.Distance(transform.position, initial_position);

        Collider collider = GetComponent<Collider>();
        collider.enabled = true;

        yield return new WaitForSeconds(.1f);

        collider.enabled = false;

        while (!DoCoordinatesMatch(initial_position, starting_distance))
        {
            transform.position = Vector3.Lerp(transform.position, initial_position, return_speed);

            yield return null;
        }

        GetComponent<Collider>().enabled = true;
    }
}
