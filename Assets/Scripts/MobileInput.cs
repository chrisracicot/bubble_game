using UnityEngine;

public static class MobileInput
{
    // Legacy support for keyboard testing
    public static float Horizontal { get; private set; }

    // New Trajectory Jump properties
    public static bool TrajectoryJumpRequested { get; private set; }
    public static Vector2 TrajectoryInput { get; private set; }

    public static void SetTrajectoryJump(Vector2 input)
    {
        TrajectoryInput = input;
        TrajectoryJumpRequested = true;
    }

    public static void ConsumeTrajectoryJump()
    {
        TrajectoryJumpRequested = false;
        TrajectoryInput = Vector2.zero;
    }

    // Keep these so existing keyboard integrations theoretically work
    public static void SetHorizontal(float value)
    {
        Horizontal = value;
    }

    public static void StopHorizontal()
    {
        Horizontal = 0f;
    }

    // Obsolete but kept to prevent compile errors during transition
    public static bool JumpPressed { get; private set; }
    public static bool JumpForwardPressed { get; private set; }
    public static void PressJump() { JumpPressed = true; }
    public static void PressJumpForward() { JumpForwardPressed = true; }
    public static void ConsumeJump() { JumpPressed = false; JumpForwardPressed = false; }
}