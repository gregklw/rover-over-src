using UnityEngine;
using System.Collections;

namespace PadInput
{
    public enum MouseCode : int
    {
        None,
        MouseXPositive,
        MouseXNegative,
        MouseYPositive,
        MouseYNegative,
        MouseWheelPositive,
        MouseWheelNegative,
        MiddleMouseClick,
        LeftMouseClick,
        RightMouseClick
    }
}
