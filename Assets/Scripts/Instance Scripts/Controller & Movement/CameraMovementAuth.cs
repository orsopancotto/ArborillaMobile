using UnityEngine;

public class CameraMovementAuth : MonoBehaviour
{
    [Range(.1f, 1)]
    [SerializeField] private float clamp_factor = .5f;

    [SerializeField] private Transform camera_transform;

    internal static CameraMovementAuth Instance;
    private Vector3 predicted_position;
    private Bounds camera_bounds;
    private RaycastHit hit;
    private readonly string l_coll = "L_Collider", r_coll = "R_Collider";
    private string name_to_compare;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        camera_bounds = GetComponent<BoxCollider>().bounds;
    }

    internal Vector3 AuthorizeMovement(Vector3 shift)       //restituisce il movimento autorizzato a seconda della richiesta di movimento
    {
        predicted_position = camera_transform.position + shift;

        if (camera_bounds.Contains(predicted_position)) return predicted_position;

        else return ParallelToColliderComponent(shift);
    }

    internal bool IsZoomAuthorized()
    {
        return camera_bounds.Contains(camera_transform.position);
    }

    private Vector3 ParallelToColliderComponent(Vector3 shift)      //restituisce la componente del vettore spostamento parallela al piano adiacente alla telecamera, diminuendone la magnitudine
    {
        Physics.Raycast(camera_transform.position, predicted_position, out hit);

        if (hit.collider == null)       //gestisce NullReferenceException
        {
            return camera_transform.position;
        }

        name_to_compare = hit.collider.gameObject.name;

        //il piano adiacente alla telecamera è dx o sx; lo spostamento è concesso solo lungo l'asse Z
        if(name_to_compare == l_coll || name_to_compare == r_coll)
        {
            //applico solo lo spostamento in Z e ne riduco la magnitudine
            predicted_position = new Vector3(camera_transform.position.x, camera_transform.position.y, camera_transform.position.z + shift.z * clamp_factor);

            return AuthorizeProcessedMovement();
        }

        //il piano adiacente alla telecamera è quello frontale o posteriore; lo spostamento è concesso solo lungo l'asse x
        else
        {
            //applico solo lo spostamento in X e ne riduco la magnitudine
            predicted_position = new Vector3(camera_transform.position.x + shift.x * clamp_factor, camera_transform.position.y, camera_transform.position.z);

            return AuthorizeProcessedMovement();
        }
    }

    private Vector3 AuthorizeProcessedMovement()        //Autorizzo o meno la restituzione della posizione calcolata precedentemente
    {
        if (camera_bounds.Contains(predicted_position)) return predicted_position;

        else return camera_transform.position;
    }
}
