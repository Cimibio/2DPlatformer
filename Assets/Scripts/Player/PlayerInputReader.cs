using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    private const string Horizontal = "Horizontal";
    private const string Jump = "Jump";
    private const string Attack = "Fire1";

    public float HorizontalInput { get; private set; }
    public bool IsJumpPressed { get; private set; }
    public bool IsAttackPressed { get; private set; }

    private void Update()
    {
        HorizontalInput = Input.GetAxisRaw(Horizontal);
        IsJumpPressed = Input.GetButtonDown(Jump);
        IsAttackPressed = Input.GetButtonDown(Attack);
    }
}
