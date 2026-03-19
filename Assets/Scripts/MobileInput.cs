using UnityEngine;

public static class MobileInput
{
    public static float Horizontal { get; private set; }
    public static bool JumpPressed { get; private set; }
    public static bool JumpForwardPressed { get; private set; }

    public static void SetHorizontal(float value)
    {
        Horizontal = value;
    }

    public static void StopHorizontal()
    {
        Horizontal = 0f;
    }

    public static void PressJump()
    {
        JumpPressed = true;
    }

    public static void PressJumpForward()
    {
        JumpForwardPressed = true;
    }

    public static void ResetJump()
    {
        JumpPressed = false;
        JumpForwardPressed = false;
    }

    public static void ConsumeJump()
    {
        JumpPressed = false;
        JumpForwardPressed = false;
    }
}