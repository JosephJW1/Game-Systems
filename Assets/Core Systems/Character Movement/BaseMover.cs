using UnityEngine;

public abstract class BaseMover : MonoBehaviour
{
    public abstract void Move(Vector3 direction);
    public abstract void SetLookDirection(Vector3 direction);
    public abstract void SetJumpInput(bool held);
    public abstract void SetCrouchInput(bool crouching);
}