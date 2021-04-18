using UnityEngine;
using System.Collections;

namespace PadInput
{
    public enum ControllerCode : int
    {
        None,

		// Known
        LeftStickUp, //1
        LeftStickDown,
        LeftStickLeft,
        LeftStickRight,
        RightStickUp,
        RightStickDown,
        RightStickLeft,
        RightStickRight,
        ActionUp, //9
        ActionDown,
        ActionLeft,
        ActionRight,
        DpadUp,
        DpadDown,
        DpadLeft,
        DpadRight,
        LeftBumper,
        RightBumper,
        LeftTrigger,
        RightTrigger,
        RightStick,
        LeftStick,
        Select,
        Start,//24

		// Unknown
		Button0,//25
		Button1,
		Button2,
		Button3,
		Button4,
		Button5,
		Button6,
		Button7,
		Button8,
		Button9,
		Button10,
		Button11,
		Button12,
		Button13,
		Button14,
		Button15,
		Button16,
		Button17,
		Button18,
		Button19,//44
		Axis1P,
		Axis1N,
		Axis2P,
		Axis2N,
		Axis3P,
		Axis3N,
		Axis4P,
		Axis4N,
		Axis5P,
		Axis5N,
		Axis6P,
		Axis6N,
		Axis7P,
		Axis7N,
		Axis8P,
		Axis8N,
		Axis9P,
		Axis9N,
		Axis10P,
		Axis10N,
		Axis11P,
		Axis11N,
		Axis12P,
		Axis12N,
		Axis13P,
		Axis13N,
		Axis14P,
		Axis14N,
		Axis15P,
		Axis15N,
		Axis16P,
		Axis16N,
		Axis17P,
		Axis17N,
		Axis18P,
		Axis18N,
		Axis19P,
		Axis19N //82
    }

    public enum ControllerIndex : int
    {
        Any,
        One,
        Two,
        Three,
        Four,
		Five,
		Six,
		Seven,
		Eight,
        None
    }

    public enum ControllerSource : int
    {
        None,
        PlayStationMac,
        PlayStationWindows,
        XboxMac,
        XboxWindows,
		Unknown
    }
}