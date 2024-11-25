using UnityEngine;

internal interface IUniversalInteractions
{
    public void Grabbed();

    public void Move(Vector3 touch_position, float speed);

    public void Selected();

    public void InteractionEnded();
}
