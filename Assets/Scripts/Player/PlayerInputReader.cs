using UnityEngine;

public class PlayerInputReader : MonoBehaviour
{
    public float HorizontalInput { get; private set; }
    public bool IsJumpPressed { get; private set; }

    private void Update()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        IsJumpPressed = Input.GetButtonDown("Jump");
    }
}
