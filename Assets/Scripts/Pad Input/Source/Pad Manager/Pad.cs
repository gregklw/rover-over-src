using UnityEngine;
using System;

namespace PadInput
{
    public static class Pad
    {
        static string[] joysticks = Input.GetJoystickNames();

        #region Public Functions

        public static float GetInputValue(PadCode button)
        {
            switch (button.Source)
            {
                case InputSource.Keyboard: return GetKeyboardValue(button.Keyboard);
                case InputSource.Mouse: return GetMouseValue(button.Mouse);
            }

            return 0;
        }

        public static bool GetInputPressing(PadCode button)
        {
            switch (button.Source)
            {
                case InputSource.Keyboard: return GetKeyboardPressing(button.Keyboard);
                case InputSource.Mouse: return GetMousePressing(button.Mouse);
            }

            return false;
        }

        public static bool GetInputPressed(PadCode button)
        {
            switch (button.Source)
            {
                case InputSource.Keyboard: return GetKeyboardPressed(button.Keyboard);
                case InputSource.Mouse: return GetMousePressed(button.Mouse);
            }

            return false;
        }

        public static bool GetInputReleased(PadCode button)
        {
            switch (button.Source)
            {
                case InputSource.Keyboard: return GetKeyboardReleased(button.Keyboard);
                case InputSource.Mouse: return GetMouseReleased(button.Mouse);
            }

            return false;
        }

        public static float GetInputValue(PadCode button, ControllerIndex index)
        {
            if (index == ControllerIndex.Any)
            {
                if (button.Source != InputSource.Controller)
                    return GetInputValue(button);
            }
            else if (index == ControllerIndex.None)
            {
                return GetInputValue(button);
            }

            return GetControllerValue(button.Controller, index);
        }

        public static bool GetInputPressing(PadCode button, ControllerIndex index)
        {
            if (index == ControllerIndex.Any)
            {
                if (button.Source != InputSource.Controller)
                    return GetInputPressing(button);
            }
            else if (index == ControllerIndex.None)
            {
                return GetInputPressing(button);
            }

            return GetControllerPressing(button.Controller, index);
        }

        public static bool GetInputPressed(PadCode button, ControllerIndex index)
        {
            if (index == ControllerIndex.Any)
            {
                if (button.Source != InputSource.Controller)
                    return GetInputPressed(button);
            }
            else if (index == ControllerIndex.None)
            {
                return GetInputPressed(button);
            }

            return GetControllerPressed(button.Controller, index);
        }

        public static bool GetInputReleased(PadCode button, ControllerIndex index)
        {
            if (index == ControllerIndex.Any)
            {
                if (button.Source != InputSource.Controller)
                    return GetInputReleased(button);
            }
            else if (index == ControllerIndex.None)
            {
                return GetInputReleased(button);
            }

            return GetControllerReleased(button.Controller, index);
        }

        public static bool AnyInput()
        {
            foreach (InputSource source in Enum.GetValues(typeof(InputSource)))
            {
                if (AnyInputFrom(source))
                    return true;
            }

            return false;
        }

        public static bool AnyInputFrom(InputSource source)
        {
            var button = new PadCode();

            switch (source)
            {
                case InputSource.Keyboard:
                    foreach (KeyboardCode KBC in Enum.GetValues(typeof(KeyboardCode)))
                    {
                        button.Source = source;
                        button.Keyboard = KBC;

                        if (GetInputPressing(button))
                            return true;
                    }
                    break;
                case InputSource.Mouse:
                    foreach (MouseCode MOC in Enum.GetValues(typeof(MouseCode)))
                    {
                        button.Source = source;
                        button.Mouse = MOC;

                        if (GetInputPressing(button))
                            return true;
                    }
                    break;
                case InputSource.Controller:
                    foreach (ControllerCode COC in Enum.GetValues(typeof(ControllerCode)))
                    {
                        button.Source = source;
                        button.Controller = COC;

                        if (GetInputPressing(button, ControllerIndex.Any))
                            return true;
                    }
                    break;
            }

            return false;
        }

        public static bool AnyInputFromController(ControllerIndex index)
        {
            var button = new PadCode();

            foreach (ControllerCode COC in Enum.GetValues(typeof(ControllerCode)))
            {
                button.Source = InputSource.Controller;
                button.Controller = COC;

                if (GetInputPressing(button, index))
                    return true;
            }

            return false;
        }

        public static bool ListenForInput(out PadCode button)
        {
            foreach (InputSource source in Enum.GetValues(typeof(InputSource)))
            {
                if (ListenForInputFrom(source, out button))
                    return true;
            }

            button = null;

            return false;
        }

        public static bool ListenForInputFrom(InputSource source, out PadCode button)
        {
            button = new PadCode();

            switch (source)
            {
                case InputSource.Keyboard:
                    foreach (KeyboardCode KBC in Enum.GetValues(typeof(KeyboardCode)))
                    {
                        button.Source = source;
                        button.Keyboard = KBC;

                        if (GetInputPressing(button))
                            return true;
                    }
                    break;
                case InputSource.Mouse:
                    foreach (MouseCode MOC in Enum.GetValues(typeof(MouseCode)))
                    {
                        button.Source = source;
                        button.Mouse = MOC;

                        if (GetInputPressing(button))
                            return true;
                    }
                    break;
                case InputSource.Controller:
                    foreach (ControllerCode COC in Enum.GetValues(typeof(ControllerCode)))
                    {
                        button.Source = source;
                        button.Controller = COC;

                        if (GetInputPressing(button, ControllerIndex.Any))
                            return true;
                    }
                    break;
            }

            button = null;

            return false;
        }

        public static bool ListenForInputFromController(ControllerIndex index, out PadCode button)
        {
            button = new PadCode();

            foreach (ControllerCode COC in Enum.GetValues(typeof(ControllerCode)))
            {
                button.Source = InputSource.Controller;
                button.Controller = COC;

                if (GetInputPressing(button, index))
                    return true;
            }

            button = null;

            return false;
        }

        public static string GetPadCodeName (PadCode padCode)
        {
            string name = "None";

            switch (padCode.Source)
            {
                case InputSource.Keyboard:
                    name = ("Keyboard: " + padCode.Keyboard.ToString());
                    break;
                case InputSource.Mouse:
                    name = ("Mouse: " + padCode.Mouse.ToString());
                    break;
                case InputSource.Controller:
                    name = ("Controller: " + padCode.Controller.ToString());
                    break;
            }

            return name;
        }

        public static string GetCodeName(PadCode padCode)
        {
            string name = "None";

            switch (padCode.Source)
            {
                case InputSource.Keyboard:
                    name = (padCode.Keyboard.ToString());
                    break;
                case InputSource.Mouse:
                    name = (padCode.Mouse.ToString());
                    break;
                case InputSource.Controller:
                    name = (padCode.Controller.ToString());
                    break;
            }

            return name;
        }

        public static ControllerSource GetControllerSource(ControllerIndex index)
        {
            int i = (int)index - 1;

            if (i > joysticks.Length - 1 || i < 0)
                return ControllerSource.None;

            if (joysticks.Length > 0)
            {
                if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
                {
                    if (joysticks[i] == "Sony PLAYSTATION(R)3 Controller" || joysticks[i] == "Sony PLAYSTATION(R)4 Controller")
                    {
                        return ControllerSource.PlayStationMac;
                    }
                    else if (joysticks[i] == "Controller (XBOX ONE For Mac)")
                    {
                        return ControllerSource.XboxMac;
                    }

                }
                else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    if (joysticks[i] == "Sony PLAYSTATION(R)4 Controller")
                    {
                        return ControllerSource.PlayStationWindows;
                    }
                    else if (joysticks[i] == "Controller (XBOX 360 For Windows)" || joysticks[i] == "Controller (XBOX ONE For Windows)")
                    {
                        return ControllerSource.XboxWindows;
                    }
                }
            }

			return ControllerSource.Unknown;
        }

        #region Main Funtions

        public static float GetMouseValue(MouseCode code)
        {
            switch (code)
            {
                case MouseCode.MouseXPositive: return Input.GetAxis("Mouse X") >= 0 ? Input.GetAxis("Mouse X") : 0;
                case MouseCode.MouseXNegative: return (Input.GetAxis("Mouse X") * -1) >= 0 ? (Input.GetAxis("Mouse X") * -1) : 0;
                case MouseCode.MouseYPositive: return Input.GetAxis("Mouse Y") >= 0 ? Input.GetAxis("Mouse Y") : 0;
                case MouseCode.MouseYNegative: return (Input.GetAxis("Mouse Y") * -1) >= 0 ? (Input.GetAxis("Mouse Y") * -1) : 0;
                case MouseCode.MouseWheelPositive: return Input.GetAxis("Mouse ScrollWheel") >= 0 ? Input.GetAxis("Mouse ScrollWheel") : 0;
                case MouseCode.MouseWheelNegative: return (Input.GetAxis("Mouse ScrollWheel") * -1) >= 0 ? (Input.GetAxis("Mouse ScrollWheel") * -1) : 0;
                case MouseCode.LeftMouseClick: return GetMousePressing(code) ? 1 : 0;
                case MouseCode.RightMouseClick: return GetMousePressing(code) ? 1 : 0;
                case MouseCode.MiddleMouseClick: return GetMousePressing(code) ? 1 : 0;
            }

            return 0;
        }

        public static float GetKeyboardValue(KeyboardCode code)
        {
            return GetKeyboardPressing(code) ? 1 : 0;
        }

        public static float GetControllerValue(ControllerCode code, ControllerIndex index)
        {
            if (index == ControllerIndex.Any)
            {
                return GetAnyControllerValue(code);
            }
            else if (index != ControllerIndex.None)
            {
                switch (GetControllerSource(index))
                {
				case ControllerSource.PlayStationMac: return GetPlayStationMacValue(code, index);
				case ControllerSource.PlayStationWindows: return GetPlayStationWindowsValue(code, index);
				case ControllerSource.XboxMac: return GetXboxMacValue(code, index);
				case ControllerSource.XboxWindows: return GetXboxWindowsValue(code, index);
				case ControllerSource.Unknown: return GetUnknownValue(code, index);
                }
            }

            return 0;
        }

        public static bool GetMousePressing(MouseCode code)
        {
            switch (code)
            {
                case MouseCode.LeftMouseClick: return Input.GetMouseButton(0);
                case MouseCode.RightMouseClick: return Input.GetMouseButton(1);
                case MouseCode.MiddleMouseClick: return Input.GetMouseButton(2);
                case MouseCode.MouseXPositive: return GetMouseValue(code) > 0 ? true : false;
                case MouseCode.MouseXNegative: return GetMouseValue(code) > 0 ? true : false;
                case MouseCode.MouseYPositive: return GetMouseValue(code) > 0 ? true : false;
                case MouseCode.MouseYNegative: return GetMouseValue(code) > 0 ? true : false;
                case MouseCode.MouseWheelPositive: return GetMouseValue(code) > 0 ? true : false;
                case MouseCode.MouseWheelNegative: return GetMouseValue(code) > 0 ? true : false;
            }

            return false;
        }

        public static bool GetKeyboardPressing(KeyboardCode code)
        {
            return Input.GetKey(GetKeyCodeEquivalent(code));
        }

        public static bool GetControllerPressing(ControllerCode code, ControllerIndex index)
        {
            if (index == ControllerIndex.Any)
            {
                return GetAnyControllerPressing(code);
            }
            else if (index != ControllerIndex.None)
            {
                switch (GetControllerSource(index))
                {
				case ControllerSource.PlayStationMac: return GetPlayStationMacPressing(code, index);
				case ControllerSource.PlayStationWindows: return GetPlayStationWindowsPressing(code, index);
				case ControllerSource.XboxMac: return GetXboxMacPressing(code, index);
				case ControllerSource.XboxWindows: return GetXboxWindowsPressing(code, index);
				case ControllerSource.Unknown: return GetUnknownPressing(code, index);
                }
            }

            return false;
        }

        public static bool GetMousePressed(MouseCode code)
        {
            switch (code)
            {
                case MouseCode.LeftMouseClick: return Input.GetMouseButtonDown(0);
                case MouseCode.RightMouseClick: return Input.GetMouseButtonDown(1);
                case MouseCode.MiddleMouseClick: return Input.GetMouseButtonDown(2);
            }

            return false;
        }

        public static bool GetKeyboardPressed(KeyboardCode code)
        {
            return Input.GetKeyDown(GetKeyCodeEquivalent(code));
        }

        public static bool GetControllerPressed(ControllerCode code, ControllerIndex index)
        {
            if (index == ControllerIndex.Any)
            {
                return GetAnyControllerPressed(code);
            }
            else if (index != ControllerIndex.None)
            {
                switch (GetControllerSource(index))
                {
				case ControllerSource.PlayStationMac: return GetPlayStationMacPressed(code, index);
				case ControllerSource.PlayStationWindows: return GetPlayStationWindowsPressed(code, index);
				case ControllerSource.XboxMac: return GetXboxMacPressed(code, index);
				case ControllerSource.XboxWindows: return GetXboxWindowsPressed(code, index);
				case ControllerSource.Unknown: return GetUnknownPressed(code, index);
                }
            }

            return false;
        }

        public static bool GetMouseReleased(MouseCode code)
        {
            switch (code)
            {
                case MouseCode.LeftMouseClick: return Input.GetMouseButtonUp(0);
                case MouseCode.RightMouseClick: return Input.GetMouseButtonUp(1);
                case MouseCode.MiddleMouseClick: return Input.GetMouseButtonUp(2);
            }

            return false;
        }

        public static bool GetKeyboardReleased(KeyboardCode code)
        {
            return Input.GetKeyUp(GetKeyCodeEquivalent(code));
        }

        public static bool GetControllerReleased(ControllerCode code, ControllerIndex index)
        {
            if (index == ControllerIndex.Any)
            {
                return GetAnyControllerReleased(code);
            }
            else if (index != ControllerIndex.None)
            {
                switch (GetControllerSource(index))
                {
				case ControllerSource.PlayStationMac: return GetPlayStationMacReleased(code, index);
				case ControllerSource.PlayStationWindows: return GetPlayStationWindowsReleased(code, index);
				case ControllerSource.XboxMac: return GetXboxMacReleased(code, index);
				case ControllerSource.XboxWindows: return GetXboxWindowsReleased(code, index);
				case ControllerSource.Unknown: return GetUnknownReleased(code, index);
                }
            }

            return false;
        }

        #endregion

        #endregion

        #region Private Funtions

		static float GetPlayStationMacValue(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.LeftStickDown: return Input.GetAxis(GetAxisName((int)index, 2)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 2)) : 0;
			case ControllerCode.LeftStickUp: return (Input.GetAxis(GetAxisName((int)index, 2)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 2)) * -1) : 0;
			case ControllerCode.LeftStickLeft: return (Input.GetAxis(GetAxisName((int)index, 1)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 1)) * -1) : 0;
			case ControllerCode.LeftStickRight: return Input.GetAxis(GetAxisName((int)index, 1)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 1)) : 0;
			case ControllerCode.RightStickDown: return Input.GetAxis(GetAxisName((int)index, 4)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 4)) : 0;
			case ControllerCode.RightStickUp: return (Input.GetAxis(GetAxisName((int)index, 4)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 4)) * -1) : 0;
			case ControllerCode.RightStickLeft: return (Input.GetAxis(GetAxisName((int)index, 3)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 3)) * -1) : 0;
			case ControllerCode.RightStickRight: return Input.GetAxis(GetAxisName((int)index, 3)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 3)) : 0;
			case ControllerCode.ActionUp: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionDown: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionLeft: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionRight: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadUp: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadDown: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadLeft: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadRight: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftBumper: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.RightBumper: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftTrigger: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.RightTrigger: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.RightStick: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftStick: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.Select: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			case ControllerCode.Start: return GetPlayStationMacPressing(code, index) ? 1 : 0;
			}

			return 0;
		}

		static float GetPlayStationWindowsValue(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.LeftStickDown: return Input.GetAxis(GetAxisName((int)index, 2)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 2)) : 0;
			case ControllerCode.LeftStickUp: return (Input.GetAxis(GetAxisName((int)index, 2)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 2)) * -1) : 0;
			case ControllerCode.LeftStickLeft: return (Input.GetAxis(GetAxisName((int)index, 1)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 1)) * -1) : 0;
			case ControllerCode.LeftStickRight: return Input.GetAxis(GetAxisName((int)index, 1)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 1)) : 0;
			case ControllerCode.RightStickDown: return Input.GetAxis(GetAxisName((int)index, 6)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 6)) : 0;
			case ControllerCode.RightStickUp: return (Input.GetAxis(GetAxisName((int)index, 6)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 6)) * -1) : 0;
			case ControllerCode.RightStickLeft: return (Input.GetAxis(GetAxisName((int)index, 3)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 3)) * -1) : 0;
			case ControllerCode.RightStickRight: return Input.GetAxis(GetAxisName((int)index, 3)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 3)) : 0;
			case ControllerCode.ActionUp: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionDown: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionLeft: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionRight: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadDown: return Input.GetAxis(GetAxisName((int)index, 8)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 8)) : 0;
			case ControllerCode.DpadUp: return (Input.GetAxis(GetAxisName((int)index, 8)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 8)) * -1) : 0;
			case ControllerCode.DpadLeft: return (Input.GetAxis(GetAxisName((int)index, 7)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 7)) * -1) : 0;
			case ControllerCode.DpadRight: return Input.GetAxis(GetAxisName((int)index, 7)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 7)) : 0;
			case ControllerCode.LeftBumper: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.RightBumper: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftTrigger: return Mathf.Clamp01(Input.GetAxis(GetAxisName((int)index, 4)));
			case ControllerCode.RightTrigger: return Mathf.Clamp01(Input.GetAxis(GetAxisName((int)index, 5)));
			case ControllerCode.RightStick: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftStick: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.Select: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.Start: return GetPlayStationWindowsPressing(code, index) ? 1 : 0;
			}

			return 0;
		}

		static float GetXboxWindowsValue(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.LeftStickDown: return Input.GetAxis(GetAxisName((int)index, 2)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 2)) : 0;
			case ControllerCode.LeftStickUp: return (Input.GetAxis(GetAxisName((int)index, 2)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 2)) * -1) : 0;
			case ControllerCode.LeftStickLeft: return (Input.GetAxis(GetAxisName((int)index, 1)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 1)) * -1) : 0;
			case ControllerCode.LeftStickRight: return Input.GetAxis(GetAxisName((int)index, 1)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 1)) : 0;
			case ControllerCode.RightStickDown: return Input.GetAxis(GetAxisName((int)index, 5)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 5)) : 0;
			case ControllerCode.RightStickUp: return (Input.GetAxis(GetAxisName((int)index, 5)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 5)) * -1) : 0;
			case ControllerCode.RightStickLeft: return (Input.GetAxis(GetAxisName((int)index, 4)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 4)) * -1) : 0;
			case ControllerCode.RightStickRight: return Input.GetAxis(GetAxisName((int)index, 4)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 4)) : 0;
			case ControllerCode.ActionUp: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionDown: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionLeft: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionRight: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadUp: return Input.GetAxis(GetAxisName((int)index, 7)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 7)) : 0;
			case ControllerCode.DpadDown: return (Input.GetAxis(GetAxisName((int)index, 7)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 7)) * -1) : 0;
			case ControllerCode.DpadLeft: return (Input.GetAxis(GetAxisName((int)index, 6)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 6)) * -1) : 0;
			case ControllerCode.DpadRight: return Input.GetAxis(GetAxisName((int)index, 6)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 6)) : 0;
			case ControllerCode.LeftBumper: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.RightBumper: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftTrigger: return Input.GetAxis(GetAxisName((int)index, 3)) > 0 ? Input.GetAxis(GetAxisName((int)index, 3)) : 0;
			case ControllerCode.RightTrigger: return (Input.GetAxis(GetAxisName((int)index, 3)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 3)) * -1) : 0;
			case ControllerCode.RightStick: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftStick: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.Select: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			case ControllerCode.Start: return GetXboxWindowsPressing(code, index) ? 1 : 0;
			}

			return 0;
		}

		static float GetXboxMacValue(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.LeftStickDown: return Input.GetAxis(GetAxisName((int)index, 4)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 4)) : 0;
			case ControllerCode.LeftStickUp: return (Input.GetAxis(GetAxisName((int)index, 4)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 4)) * -1) : 0;
			case ControllerCode.LeftStickLeft: return (Input.GetAxis(GetAxisName((int)index, 3)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 3)) * -1) : 0;
			case ControllerCode.LeftStickRight: return Input.GetAxis(GetAxisName((int)index, 3)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 3)) : 0;
			case ControllerCode.RightStickDown: return Input.GetAxis(GetAxisName((int)index, 6)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 6)) : 0;
			case ControllerCode.RightStickUp: return (Input.GetAxis(GetAxisName((int)index, 6)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 6)) * -1) : 0;
			case ControllerCode.RightStickLeft: return (Input.GetAxis(GetAxisName((int)index, 5)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 5)) * -1) : 0;
			case ControllerCode.RightStickRight: return Input.GetAxis(GetAxisName((int)index, 5)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 5)) : 0;
			case ControllerCode.ActionUp: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionDown: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionLeft: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.ActionRight: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadUp: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadDown: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadLeft: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.DpadRight: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftBumper: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.RightBumper: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftTrigger: return Mathf.Clamp01(Input.GetAxis(GetAxisName((int)index, 1)));
			case ControllerCode.RightTrigger: return Mathf.Clamp01(Input.GetAxis(GetAxisName((int)index, 2)));
			case ControllerCode.RightStick: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.LeftStick: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.Select: return GetXboxMacPressing(code, index) ? 1 : 0;
			case ControllerCode.Start: return GetXboxMacPressing(code, index) ? 1 : 0;
			}

			return 0;
		}

		static float GetUnknownValue(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.Button0: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button1: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button2: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button3: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button4: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button5: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button6: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button7: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button8: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button9: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button10: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button11: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button12: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button13: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button14: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button15: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button16: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button17: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button18: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Button19: return GetUnknownPressing(code, index) ? 1 : 0;
			case ControllerCode.Axis1P: return Input.GetAxis(GetAxisName((int)index, 1)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 1)) : 0;
			case ControllerCode.Axis1N: return (Input.GetAxis(GetAxisName((int)index, 1)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 1)) * -1) : 0;
			case ControllerCode.Axis2P: return Input.GetAxis(GetAxisName((int)index, 2)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 2)) : 0;
			case ControllerCode.Axis2N: return (Input.GetAxis(GetAxisName((int)index, 2)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 2)) * -1) : 0;
			case ControllerCode.Axis3P: return Input.GetAxis(GetAxisName((int)index, 3)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 3)) : 0;
			case ControllerCode.Axis3N: return (Input.GetAxis(GetAxisName((int)index, 3)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 3)) * -1) : 0;
			case ControllerCode.Axis4P: return Input.GetAxis(GetAxisName((int)index, 4)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 4)) : 0;
			case ControllerCode.Axis4N: return (Input.GetAxis(GetAxisName((int)index, 4)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 4)) * -1) : 0;
			case ControllerCode.Axis5P: return Input.GetAxis(GetAxisName((int)index, 5)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 5)) : 0;
			case ControllerCode.Axis5N: return (Input.GetAxis(GetAxisName((int)index, 5)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 5)) * -1) : 0;
			case ControllerCode.Axis6P: return Input.GetAxis(GetAxisName((int)index, 6)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 6)) : 0;
			case ControllerCode.Axis6N: return (Input.GetAxis(GetAxisName((int)index, 6)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 6)) * -1) : 0;
			case ControllerCode.Axis7P: return Input.GetAxis(GetAxisName((int)index, 7)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 7)) : 0;
			case ControllerCode.Axis7N: return (Input.GetAxis(GetAxisName((int)index, 7)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 7)) * -1) : 0;
			case ControllerCode.Axis8P: return Input.GetAxis(GetAxisName((int)index, 8)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 8)) : 0;
			case ControllerCode.Axis8N: return (Input.GetAxis(GetAxisName((int)index, 8)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 8)) * -1) : 0;
			case ControllerCode.Axis9P: return Input.GetAxis(GetAxisName((int)index, 9)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 9)) : 0;
			case ControllerCode.Axis9N: return (Input.GetAxis(GetAxisName((int)index, 9)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 9)) * -1) : 0;
			case ControllerCode.Axis10P: return Input.GetAxis(GetAxisName((int)index, 10)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 10)) : 0;
			case ControllerCode.Axis10N: return (Input.GetAxis(GetAxisName((int)index, 10)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 10)) * -1) : 0;
			case ControllerCode.Axis11P: return Input.GetAxis(GetAxisName((int)index, 11)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 11)) : 0;
			case ControllerCode.Axis11N: return (Input.GetAxis(GetAxisName((int)index, 11)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 11)) * -1) : 0;
			case ControllerCode.Axis12P: return Input.GetAxis(GetAxisName((int)index, 12)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 12)) : 0;
			case ControllerCode.Axis12N: return (Input.GetAxis(GetAxisName((int)index, 12)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 12)) * -1) : 0;
			case ControllerCode.Axis13P: return Input.GetAxis(GetAxisName((int)index, 13)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 13)) : 0;
			case ControllerCode.Axis13N: return (Input.GetAxis(GetAxisName((int)index, 13)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 13)) * -1) : 0;
			case ControllerCode.Axis14P: return Input.GetAxis(GetAxisName((int)index, 14)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 14)) : 0;
			case ControllerCode.Axis14N: return (Input.GetAxis(GetAxisName((int)index, 14)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 14)) * -1) : 0;
			case ControllerCode.Axis15P: return Input.GetAxis(GetAxisName((int)index, 15)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 15)) : 0;
			case ControllerCode.Axis15N: return (Input.GetAxis(GetAxisName((int)index, 15)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 15)) * -1) : 0;
			case ControllerCode.Axis16P: return Input.GetAxis(GetAxisName((int)index, 16)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 16)) : 0;
			case ControllerCode.Axis16N: return (Input.GetAxis(GetAxisName((int)index, 16)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 16)) * -1) : 0;
			case ControllerCode.Axis17P: return Input.GetAxis(GetAxisName((int)index, 17)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 17)) : 0;
			case ControllerCode.Axis17N: return (Input.GetAxis(GetAxisName((int)index, 17)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 17)) * -1) : 0;
			case ControllerCode.Axis18P: return Input.GetAxis(GetAxisName((int)index, 18)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 18)) : 0;
			case ControllerCode.Axis18N: return (Input.GetAxis(GetAxisName((int)index, 18)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 18)) * -1) : 0;
			case ControllerCode.Axis19P: return Input.GetAxis(GetAxisName((int)index, 19)) >= 0 ? Input.GetAxis(GetAxisName((int)index, 19)) : 0;
			case ControllerCode.Axis19N: return (Input.GetAxis(GetAxisName((int)index, 19)) * -1) >= 0 ? (Input.GetAxis(GetAxisName((int)index, 19)) * -1) : 0;
			}

			return 0;
		}

        static float GetAnyControllerValue (ControllerCode code)
        {
            float val = 0;

			for (int i = 1; i < joysticks.Length + 1; i++)
            {
                val += GetControllerValue(code, (ControllerIndex)i);
            }

            return Mathf.Clamp01(val);
        }

		static bool GetPlayStationMacPressing(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.LeftStickUp: return GetPlayStationMacValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickDown: return GetPlayStationMacValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickLeft: return GetPlayStationMacValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickRight: return GetPlayStationMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickUp: return GetPlayStationMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickDown: return GetPlayStationMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickLeft: return GetPlayStationMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickRight: return GetPlayStationMacValue(code, index) > 0 ? true : false;
			case ControllerCode.ActionUp: return Input.GetKey(GetKeyCodeEquivalent((int)index, 12));
			case ControllerCode.ActionDown: return Input.GetKey(GetKeyCodeEquivalent((int)index, 14));
			case ControllerCode.ActionLeft: return Input.GetKey(GetKeyCodeEquivalent((int)index, 15));
			case ControllerCode.ActionRight: return Input.GetKey(GetKeyCodeEquivalent((int)index, 13));
			case ControllerCode.DpadUp: return Input.GetKey(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.DpadDown: return Input.GetKey(GetKeyCodeEquivalent((int)index, 6));
			case ControllerCode.DpadLeft: return Input.GetKey(GetKeyCodeEquivalent((int)index, 7));
			case ControllerCode.DpadRight: return Input.GetKey(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.LeftBumper: return Input.GetKey(GetKeyCodeEquivalent((int)index, 10));
			case ControllerCode.RightBumper: return Input.GetKey(GetKeyCodeEquivalent((int)index, 11));
			case ControllerCode.LeftTrigger: return Input.GetKey(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.RightTrigger: return Input.GetKey(GetKeyCodeEquivalent((int)index, 9));
			case ControllerCode.RightStick: return Input.GetKey(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.LeftStick: return Input.GetKey(GetKeyCodeEquivalent((int)index, 1));
			case ControllerCode.Select: return Input.GetKey(GetKeyCodeEquivalent((int)index, 0));
			case ControllerCode.Start: return Input.GetKey(GetKeyCodeEquivalent((int)index, 3));
			}

			return false;
		}

		static bool GetPlayStationWindowsPressing(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.LeftStickUp: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickDown: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickLeft: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickRight: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickUp: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickDown: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickLeft: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickRight: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.ActionUp: return Input.GetKey(GetKeyCodeEquivalent((int)index, 3));
			case ControllerCode.ActionDown: return Input.GetKey(GetKeyCodeEquivalent((int)index, 1));
			case ControllerCode.ActionLeft: return Input.GetKey(GetKeyCodeEquivalent((int)index, 0));
			case ControllerCode.ActionRight: return Input.GetKey(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.DpadUp: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.DpadDown: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.DpadLeft: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.DpadRight: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftBumper: return Input.GetKey(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.RightBumper: return Input.GetKey(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.LeftTrigger: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightTrigger: return GetPlayStationWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStick: return Input.GetKey(GetKeyCodeEquivalent((int)index, 11));
			case ControllerCode.LeftStick: return Input.GetKey(GetKeyCodeEquivalent((int)index, 10));
			case ControllerCode.Select: return Input.GetKey(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.Start: return Input.GetKey(GetKeyCodeEquivalent((int)index, 9));
			}

			return false;
		}

		static bool GetXboxWindowsPressing(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.LeftStickUp: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickDown: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickLeft: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickRight: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickUp: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickDown: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickLeft: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickRight: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.ActionUp: return Input.GetKey(GetKeyCodeEquivalent((int)index, 3));
			case ControllerCode.ActionDown: return Input.GetKey(GetKeyCodeEquivalent((int)index, 0));
			case ControllerCode.ActionLeft: return Input.GetKey(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.ActionRight: return Input.GetKey(GetKeyCodeEquivalent((int)index, 1));
			case ControllerCode.DpadUp: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.DpadDown: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.DpadLeft: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.DpadRight: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftBumper: return Input.GetKey(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.RightBumper: return Input.GetKey(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.LeftTrigger: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightTrigger: return GetXboxWindowsValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStick: return Input.GetKey(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.LeftStick: return Input.GetKey(GetKeyCodeEquivalent((int)index, 9));
			case ControllerCode.Select: return Input.GetKey(GetKeyCodeEquivalent((int)index, 6));
			case ControllerCode.Start: return Input.GetKey(GetKeyCodeEquivalent((int)index, 7));
			}

			return false;
		}

		static bool GetXboxMacPressing(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.LeftStickUp: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickDown: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickLeft: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.LeftStickRight: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickUp: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickDown: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickLeft: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStickRight: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.ActionUp: return Input.GetKey(GetKeyCodeEquivalent((int)index, 6));
			case ControllerCode.ActionDown: return Input.GetKey(GetKeyCodeEquivalent((int)index, 3));
			case ControllerCode.ActionLeft: return Input.GetKey(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.ActionRight: return Input.GetKey(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.DpadUp: return Input.GetKey(GetKeyCodeEquivalent((int)index, 7));
			case ControllerCode.DpadDown: return Input.GetKey(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.DpadLeft: return Input.GetKey(GetKeyCodeEquivalent((int)index, 9));
			case ControllerCode.DpadRight: return Input.GetKey(GetKeyCodeEquivalent((int)index, 10));
			case ControllerCode.LeftBumper: return Input.GetKey(GetKeyCodeEquivalent((int)index, 11));
			case ControllerCode.RightBumper: return Input.GetKey(GetKeyCodeEquivalent((int)index, 12));
			case ControllerCode.LeftTrigger: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightTrigger: return GetXboxMacValue(code, index) > 0 ? true : false;
			case ControllerCode.RightStick: return Input.GetKey(GetKeyCodeEquivalent((int)index, 14));
			case ControllerCode.LeftStick: return Input.GetKey(GetKeyCodeEquivalent((int)index, 13));
			case ControllerCode.Select: return Input.GetKey(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.Start: return Input.GetKey(GetKeyCodeEquivalent((int)index, 1));
			}

			return false;
		}

		static bool GetUnknownPressing(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.Button0: return Input.GetKey(GetKeyCodeEquivalent((int)index, 0));
			case ControllerCode.Button1: return Input.GetKey(GetKeyCodeEquivalent((int)index, 1));
			case ControllerCode.Button2: return Input.GetKey(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.Button3: return Input.GetKey(GetKeyCodeEquivalent((int)index, 3));
			case ControllerCode.Button4: return Input.GetKey(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.Button5: return Input.GetKey(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.Button6: return Input.GetKey(GetKeyCodeEquivalent((int)index, 6));
			case ControllerCode.Button7: return Input.GetKey(GetKeyCodeEquivalent((int)index, 7));
			case ControllerCode.Button8: return Input.GetKey(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.Button9: return Input.GetKey(GetKeyCodeEquivalent((int)index, 9));
			case ControllerCode.Button10: return Input.GetKey(GetKeyCodeEquivalent((int)index, 10));
			case ControllerCode.Button11: return Input.GetKey(GetKeyCodeEquivalent((int)index, 11));
			case ControllerCode.Button12: return Input.GetKey(GetKeyCodeEquivalent((int)index, 12));
			case ControllerCode.Button13: return Input.GetKey(GetKeyCodeEquivalent((int)index, 13));
			case ControllerCode.Button14: return Input.GetKey(GetKeyCodeEquivalent((int)index, 14));
			case ControllerCode.Button15: return Input.GetKey(GetKeyCodeEquivalent((int)index, 15));
			case ControllerCode.Button16: return Input.GetKey(GetKeyCodeEquivalent((int)index, 16));
			case ControllerCode.Button17: return Input.GetKey(GetKeyCodeEquivalent((int)index, 17));
			case ControllerCode.Button18: return Input.GetKey(GetKeyCodeEquivalent((int)index, 18));
			case ControllerCode.Button19: return Input.GetKey(GetKeyCodeEquivalent((int)index, 19));
			case ControllerCode.Axis1P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis1N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis2P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis2N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis3P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis3N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis4P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis4N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis5P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis5N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis6P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis6N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis7P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis7N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis8P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis8N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis9P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis9N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis10P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis10N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis11P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis11N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis12P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis12N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis13P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis13N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis14P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis14N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis15P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis15N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis16P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis16N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis17P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis17N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis18P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis18N: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis19P: return GetUnknownValue(code, index) > 0 ? true : false;
			case ControllerCode.Axis19N: return GetUnknownValue(code, index) > 0 ? true : false;
			}

			return false;
		}

        static bool GetAnyControllerPressing(ControllerCode code)
        {
			for (int i = 1; i < joysticks.Length + 1; i++)
            {
                if (GetControllerPressing(code, (ControllerIndex)i))
                    return true;
            }

            return false;
        }

        static bool GetPlayStationMacPressed(ControllerCode code, ControllerIndex index)
        {
            switch (code)
            {
                case ControllerCode.ActionUp: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 12));
                case ControllerCode.ActionDown: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 14));
                case ControllerCode.ActionLeft: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 15));
                case ControllerCode.ActionRight: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 13));
                case ControllerCode.DpadUp: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 4));
                case ControllerCode.DpadDown: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 6));
                case ControllerCode.DpadLeft: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 7));
                case ControllerCode.DpadRight: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 5));
                case ControllerCode.LeftBumper: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 10));
                case ControllerCode.RightBumper: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 11));
                case ControllerCode.LeftTrigger: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 8));
                case ControllerCode.RightTrigger: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 9));
                case ControllerCode.RightStick: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 2));
                case ControllerCode.LeftStick: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 1));
                case ControllerCode.Select: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 0));
                case ControllerCode.Start: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 3));
            }

            return false;
        }

        static bool GetPlayStationWindowsPressed(ControllerCode code, ControllerIndex index)
        {
            switch (code)
            {
                case ControllerCode.ActionUp: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 3));
                case ControllerCode.ActionDown: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 1));
                case ControllerCode.ActionLeft: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 0));
                case ControllerCode.ActionRight: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 2));
                case ControllerCode.LeftBumper: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 4));
                case ControllerCode.RightBumper: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 5));
                case ControllerCode.RightStick: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 11));
                case ControllerCode.LeftStick: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 10));
                case ControllerCode.Select: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 8));
                case ControllerCode.Start: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 9));
            }

            return false;
        }

        static bool GetXboxWindowsPressed(ControllerCode code, ControllerIndex index)
        {
            switch (code)
            {
                case ControllerCode.ActionUp: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 3));
                case ControllerCode.ActionDown: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 0));
                case ControllerCode.ActionLeft: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 2));
                case ControllerCode.ActionRight: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 1));
                case ControllerCode.LeftBumper: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 4));
                case ControllerCode.RightBumper: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 5));
                case ControllerCode.RightStick: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 8));
                case ControllerCode.LeftStick: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 9));
                case ControllerCode.Select: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 6));
                case ControllerCode.Start: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 7));
            }

            return false;
        }

        static bool GetXboxMacPressed(ControllerCode code, ControllerIndex index)
        {
            switch (code)
            {
			case ControllerCode.ActionUp: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 6));
			case ControllerCode.ActionDown: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 3));
			case ControllerCode.ActionLeft: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.ActionRight: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.DpadUp: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 7));
			case ControllerCode.DpadDown: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.DpadLeft: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 9));
			case ControllerCode.DpadRight: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 10));
			case ControllerCode.LeftBumper: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 11));
			case ControllerCode.RightBumper: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 12));
			case ControllerCode.RightStick: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 14));
			case ControllerCode.LeftStick: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 13));
			case ControllerCode.Select: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.Start: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 1));
            }

            return false;
        }

		static bool GetUnknownPressed(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.Button0: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 0));
			case ControllerCode.Button1: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 1));
			case ControllerCode.Button2: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.Button3: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 3));
			case ControllerCode.Button4: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.Button5: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.Button6: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 6));
			case ControllerCode.Button7: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 7));
			case ControllerCode.Button8: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.Button9: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 9));
			case ControllerCode.Button10: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 10));
			case ControllerCode.Button11: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 11));
			case ControllerCode.Button12: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 12));
			case ControllerCode.Button13: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 13));
			case ControllerCode.Button14: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 14));
			case ControllerCode.Button15: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 15));
			case ControllerCode.Button16: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 16));
			case ControllerCode.Button17: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 17));
			case ControllerCode.Button18: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 18));
			case ControllerCode.Button19: return Input.GetKeyDown(GetKeyCodeEquivalent((int)index, 19));
			}

			return false;
		}

        static bool GetAnyControllerPressed(ControllerCode code)
        {
			for (int i = 1; i < joysticks.Length + 1; i++)
            {
                if (GetControllerPressed(code, (ControllerIndex)i))
                    return true;
            }

            return false;
        }

        static bool GetPlayStationMacReleased(ControllerCode code, ControllerIndex index)
        {
            switch (code)
            {
                case ControllerCode.ActionUp: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 12));
                case ControllerCode.ActionDown: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 14));
                case ControllerCode.ActionLeft: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 15));
                case ControllerCode.ActionRight: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 13));
                case ControllerCode.DpadUp: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 4));
                case ControllerCode.DpadDown: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 6));
                case ControllerCode.DpadLeft: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 7));
                case ControllerCode.DpadRight: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 5));
                case ControllerCode.LeftBumper: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 10));
                case ControllerCode.RightBumper: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 11));
                case ControllerCode.LeftTrigger: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 8));
                case ControllerCode.RightTrigger: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 9));
                case ControllerCode.RightStick: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 2));
                case ControllerCode.LeftStick: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 1));
                case ControllerCode.Select: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 0));
                case ControllerCode.Start: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 3));
            }

            return false;
        }

        static bool GetPlayStationWindowsReleased(ControllerCode code, ControllerIndex index)
        {
            switch (code)
            {
                case ControllerCode.ActionUp: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 3));
                case ControllerCode.ActionDown: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 1));
                case ControllerCode.ActionLeft: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 0));
                case ControllerCode.ActionRight: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 2));
                case ControllerCode.LeftBumper: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 4));
                case ControllerCode.RightBumper: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 5));
                case ControllerCode.RightStick: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 11));
                case ControllerCode.LeftStick: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 10));
                case ControllerCode.Select: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 8));
                case ControllerCode.Start: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 9));
            }

            return false;
        }

        static bool GetXboxWindowsReleased(ControllerCode code, ControllerIndex index)
        {
            switch (code)
            {
                case ControllerCode.ActionUp: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 3));
                case ControllerCode.ActionDown: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 0));
                case ControllerCode.ActionLeft: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 2));
                case ControllerCode.ActionRight: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 1));
                case ControllerCode.LeftBumper: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 4));
                case ControllerCode.RightBumper: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 5));
                case ControllerCode.RightStick: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 8));
                case ControllerCode.LeftStick: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 9));
                case ControllerCode.Select: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 6));
                case ControllerCode.Start: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 7));
            }

            return false;
        }

        static bool GetXboxMacReleased(ControllerCode code, ControllerIndex index)
        {
            switch (code)
            {
			case ControllerCode.ActionUp: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 6));
			case ControllerCode.ActionDown: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 3));
			case ControllerCode.ActionLeft: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.ActionRight: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.DpadUp: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 7));
			case ControllerCode.DpadDown: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.DpadLeft: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 9));
			case ControllerCode.DpadRight: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 10));
			case ControllerCode.LeftBumper: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 11));
			case ControllerCode.RightBumper: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 12));
			case ControllerCode.RightStick: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 14));
			case ControllerCode.LeftStick: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 13));
			case ControllerCode.Select: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.Start: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 1));
            }

            return false;
        }

		static bool GetUnknownReleased(ControllerCode code, ControllerIndex index)
		{
			switch (code)
			{
			case ControllerCode.Button0: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 0));
			case ControllerCode.Button1: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 1));
			case ControllerCode.Button2: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 2));
			case ControllerCode.Button3: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 3));
			case ControllerCode.Button4: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 4));
			case ControllerCode.Button5: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 5));
			case ControllerCode.Button6: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 6));
			case ControllerCode.Button7: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 7));
			case ControllerCode.Button8: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 8));
			case ControllerCode.Button9: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 9));
			case ControllerCode.Button10: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 10));
			case ControllerCode.Button11: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 11));
			case ControllerCode.Button12: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 12));
			case ControllerCode.Button13: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 13));
			case ControllerCode.Button14: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 14));
			case ControllerCode.Button15: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 15));
			case ControllerCode.Button16: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 16));
			case ControllerCode.Button17: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 17));
			case ControllerCode.Button18: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 18));
			case ControllerCode.Button19: return Input.GetKeyUp(GetKeyCodeEquivalent((int)index, 19));
			}

			return false;
		}

        static bool GetAnyControllerReleased(ControllerCode code)
        {
			for (int i = 1; i < joysticks.Length + 1; i++)
            {
                if (GetControllerReleased(code, (ControllerIndex)i))
                    return true;
            }

            return false;
        }

        static KeyCode GetKeyCodeEquivalent (int index, int bNum)
        {
			/*
			 * 
            var name = "Joystick" + index.ToString() + "Button" + bNum.ToString();

            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (name == kcode.ToString())
                    return kcode;
            }
			*/

			switch (index) 
			{
			case 1:
				switch (bNum) 
				{
				case 0: return KeyCode.Joystick1Button0;
				case 1: return KeyCode.Joystick1Button1;
				case 2: return KeyCode.Joystick1Button2;
				case 3: return KeyCode.Joystick1Button3;
				case 4: return KeyCode.Joystick1Button4;
				case 5: return KeyCode.Joystick1Button5;
				case 6: return KeyCode.Joystick1Button6;
				case 7: return KeyCode.Joystick1Button7;
				case 8: return KeyCode.Joystick1Button8;
				case 9: return KeyCode.Joystick1Button9;
				case 10: return KeyCode.Joystick1Button10;
				case 11: return KeyCode.Joystick1Button11;
				case 12: return KeyCode.Joystick1Button12;
				case 13: return KeyCode.Joystick1Button13;
				case 14: return KeyCode.Joystick1Button14;
				case 15: return KeyCode.Joystick1Button15;
				case 16: return KeyCode.Joystick1Button16;
				case 17: return KeyCode.Joystick1Button17;
				case 18: return KeyCode.Joystick1Button18;
				case 19: return KeyCode.Joystick1Button19;
				}
				break;
			case 2:
				switch (bNum) 
				{
				case 0: return KeyCode.Joystick2Button0;
				case 1: return KeyCode.Joystick2Button1;
				case 2: return KeyCode.Joystick2Button2;
				case 3: return KeyCode.Joystick2Button3;
				case 4: return KeyCode.Joystick2Button4;
				case 5: return KeyCode.Joystick2Button5;
				case 6: return KeyCode.Joystick2Button6;
				case 7: return KeyCode.Joystick2Button7;
				case 8: return KeyCode.Joystick2Button8;
				case 9: return KeyCode.Joystick2Button9;
				case 10: return KeyCode.Joystick2Button10;
				case 11: return KeyCode.Joystick2Button11;
				case 12: return KeyCode.Joystick2Button12;
				case 13: return KeyCode.Joystick2Button13;
				case 14: return KeyCode.Joystick2Button14;
				case 15: return KeyCode.Joystick2Button15;
				case 16: return KeyCode.Joystick2Button16;
				case 17: return KeyCode.Joystick2Button17;
				case 18: return KeyCode.Joystick2Button18;
				case 19: return KeyCode.Joystick2Button19;
				}
				break;
			case 3:
				switch (bNum) 
				{
				case 0: return KeyCode.Joystick3Button0;
				case 1: return KeyCode.Joystick3Button1;
				case 2: return KeyCode.Joystick3Button2;
				case 3: return KeyCode.Joystick3Button3;
				case 4: return KeyCode.Joystick3Button4;
				case 5: return KeyCode.Joystick3Button5;
				case 6: return KeyCode.Joystick3Button6;
				case 7: return KeyCode.Joystick3Button7;
				case 8: return KeyCode.Joystick3Button8;
				case 9: return KeyCode.Joystick3Button9;
				case 10: return KeyCode.Joystick3Button10;
				case 11: return KeyCode.Joystick3Button11;
				case 12: return KeyCode.Joystick3Button12;
				case 13: return KeyCode.Joystick3Button13;
				case 14: return KeyCode.Joystick3Button14;
				case 15: return KeyCode.Joystick3Button15;
				case 16: return KeyCode.Joystick3Button16;
				case 17: return KeyCode.Joystick3Button17;
				case 18: return KeyCode.Joystick3Button18;
				case 19: return KeyCode.Joystick3Button19;
				}
				break;
			case 4:
				switch (bNum) 
				{
				case 0: return KeyCode.Joystick4Button0;
				case 1: return KeyCode.Joystick4Button1;
				case 2: return KeyCode.Joystick4Button2;
				case 3: return KeyCode.Joystick4Button3;
				case 4: return KeyCode.Joystick4Button4;
				case 5: return KeyCode.Joystick4Button5;
				case 6: return KeyCode.Joystick4Button6;
				case 7: return KeyCode.Joystick4Button7;
				case 8: return KeyCode.Joystick4Button8;
				case 9: return KeyCode.Joystick4Button9;
				case 10: return KeyCode.Joystick4Button10;
				case 11: return KeyCode.Joystick4Button11;
				case 12: return KeyCode.Joystick4Button12;
				case 13: return KeyCode.Joystick4Button13;
				case 14: return KeyCode.Joystick4Button14;
				case 15: return KeyCode.Joystick4Button15;
				case 16: return KeyCode.Joystick4Button16;
				case 17: return KeyCode.Joystick4Button17;
				case 18: return KeyCode.Joystick4Button18;
				case 19: return KeyCode.Joystick4Button19;
				}
				break;
			case 5:
				switch (bNum) 
				{
				case 0: return KeyCode.Joystick5Button0;
				case 1: return KeyCode.Joystick5Button1;
				case 2: return KeyCode.Joystick5Button2;
				case 3: return KeyCode.Joystick5Button3;
				case 4: return KeyCode.Joystick5Button4;
				case 5: return KeyCode.Joystick5Button5;
				case 6: return KeyCode.Joystick5Button6;
				case 7: return KeyCode.Joystick5Button7;
				case 8: return KeyCode.Joystick5Button8;
				case 9: return KeyCode.Joystick5Button9;
				case 10: return KeyCode.Joystick5Button10;
				case 11: return KeyCode.Joystick5Button11;
				case 12: return KeyCode.Joystick5Button12;
				case 13: return KeyCode.Joystick5Button13;
				case 14: return KeyCode.Joystick5Button14;
				case 15: return KeyCode.Joystick5Button15;
				case 16: return KeyCode.Joystick5Button16;
				case 17: return KeyCode.Joystick5Button17;
				case 18: return KeyCode.Joystick5Button18;
				case 19: return KeyCode.Joystick5Button19;
				}
				break;
			case 6:
				switch (bNum) 
				{
				case 0: return KeyCode.Joystick6Button0;
				case 1: return KeyCode.Joystick6Button1;
				case 2: return KeyCode.Joystick6Button2;
				case 3: return KeyCode.Joystick6Button3;
				case 4: return KeyCode.Joystick6Button4;
				case 5: return KeyCode.Joystick6Button5;
				case 6: return KeyCode.Joystick6Button6;
				case 7: return KeyCode.Joystick6Button7;
				case 8: return KeyCode.Joystick6Button8;
				case 9: return KeyCode.Joystick6Button9;
				case 10: return KeyCode.Joystick6Button10;
				case 11: return KeyCode.Joystick6Button11;
				case 12: return KeyCode.Joystick6Button12;
				case 13: return KeyCode.Joystick6Button13;
				case 14: return KeyCode.Joystick6Button14;
				case 15: return KeyCode.Joystick6Button15;
				case 16: return KeyCode.Joystick6Button16;
				case 17: return KeyCode.Joystick6Button17;
				case 18: return KeyCode.Joystick6Button18;
				case 19: return KeyCode.Joystick6Button19;
				}
				break;
			case 7:
				switch (bNum) 
				{
				case 0: return KeyCode.Joystick7Button0;
				case 1: return KeyCode.Joystick7Button1;
				case 2: return KeyCode.Joystick7Button2;
				case 3: return KeyCode.Joystick7Button3;
				case 4: return KeyCode.Joystick7Button4;
				case 5: return KeyCode.Joystick7Button5;
				case 6: return KeyCode.Joystick7Button6;
				case 7: return KeyCode.Joystick7Button7;
				case 8: return KeyCode.Joystick7Button8;
				case 9: return KeyCode.Joystick7Button9;
				case 10: return KeyCode.Joystick7Button10;
				case 11: return KeyCode.Joystick7Button11;
				case 12: return KeyCode.Joystick7Button12;
				case 13: return KeyCode.Joystick7Button13;
				case 14: return KeyCode.Joystick7Button14;
				case 15: return KeyCode.Joystick7Button15;
				case 16: return KeyCode.Joystick7Button16;
				case 17: return KeyCode.Joystick7Button17;
				case 18: return KeyCode.Joystick7Button18;
				case 19: return KeyCode.Joystick7Button19;
				}
				break;
			case 8:
				switch (bNum) 
				{
				case 0: return KeyCode.Joystick8Button0;
				case 1: return KeyCode.Joystick8Button1;
				case 2: return KeyCode.Joystick8Button2;
				case 3: return KeyCode.Joystick8Button3;
				case 4: return KeyCode.Joystick8Button4;
				case 5: return KeyCode.Joystick8Button5;
				case 6: return KeyCode.Joystick8Button6;
				case 7: return KeyCode.Joystick8Button7;
				case 8: return KeyCode.Joystick8Button8;
				case 9: return KeyCode.Joystick8Button9;
				case 10: return KeyCode.Joystick8Button10;
				case 11: return KeyCode.Joystick8Button11;
				case 12: return KeyCode.Joystick8Button12;
				case 13: return KeyCode.Joystick8Button13;
				case 14: return KeyCode.Joystick8Button14;
				case 15: return KeyCode.Joystick8Button15;
				case 16: return KeyCode.Joystick8Button16;
				case 17: return KeyCode.Joystick8Button17;
				case 18: return KeyCode.Joystick8Button18;
				case 19: return KeyCode.Joystick8Button19;
				}
				break;
			}
            
            return KeyCode.None;
        }

        static KeyCode GetKeyCodeEquivalent (string name)
        {
            foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            {
                if (name == kcode.ToString())
                    return kcode;
            }

            return KeyCode.None;
        }

        static KeyCode GetKeyCodeEquivalent(KeyboardCode code)
        {
            switch (code)
            {
                default: return KeyCode.None;
                case KeyboardCode.Backspace: return KeyCode.Backspace;
                case KeyboardCode.Delete: return KeyCode.Delete;
                case KeyboardCode.Tab: return KeyCode.Tab;
                case KeyboardCode.Clear: return KeyCode.Clear;
                case KeyboardCode.Return: return KeyCode.Return;
                case KeyboardCode.Pause: return KeyCode.Pause;
                case KeyboardCode.Escape: return KeyCode.Escape;
                case KeyboardCode.Space: return KeyCode.Space;
                case KeyboardCode.Keypad0: return KeyCode.Keypad0;
                case KeyboardCode.Keypad1: return KeyCode.Keypad1;
                case KeyboardCode.Keypad2: return KeyCode.Keypad2;
                case KeyboardCode.Keypad3: return KeyCode.Keypad3;
                case KeyboardCode.Keypad4: return KeyCode.Keypad4;
                case KeyboardCode.Keypad5: return KeyCode.Keypad5;
                case KeyboardCode.Keypad6: return KeyCode.Keypad6;
                case KeyboardCode.Keypad7: return KeyCode.Keypad7;
                case KeyboardCode.Keypad8: return KeyCode.Keypad8;
                case KeyboardCode.Keypad9: return KeyCode.Keypad9;
                case KeyboardCode.KeypadPeriod: return KeyCode.KeypadPeriod;
                case KeyboardCode.KeypadDivide: return KeyCode.KeypadDivide;
                case KeyboardCode.KeypadMultiply: return KeyCode.KeypadMultiply;
                case KeyboardCode.KeypadMinus: return KeyCode.KeypadMinus;
                case KeyboardCode.KeypadPlus: return KeyCode.KeypadPlus;
                case KeyboardCode.KeypadEnter: return KeyCode.KeypadEnter;
                case KeyboardCode.KeypadEquals: return KeyCode.KeypadEquals;
                case KeyboardCode.UpArrow: return KeyCode.UpArrow;
                case KeyboardCode.DownArrow: return KeyCode.DownArrow;
                case KeyboardCode.RightArrow: return KeyCode.RightArrow;
                case KeyboardCode.LeftArrow: return KeyCode.LeftArrow;
                case KeyboardCode.Insert: return KeyCode.Insert;
                case KeyboardCode.Home: return KeyCode.Home;
                case KeyboardCode.End: return KeyCode.End;
                case KeyboardCode.PageUp: return KeyCode.PageUp;
                case KeyboardCode.PageDown: return KeyCode.PageDown;
                case KeyboardCode.F1: return KeyCode.F1;
                case KeyboardCode.F2: return KeyCode.F2;
                case KeyboardCode.F3: return KeyCode.F3;
                case KeyboardCode.F4: return KeyCode.F4;
                case KeyboardCode.F5: return KeyCode.F5;
                case KeyboardCode.F6: return KeyCode.F6;
                case KeyboardCode.F7: return KeyCode.F7;
                case KeyboardCode.F8: return KeyCode.F8;
                case KeyboardCode.F9: return KeyCode.F9;
                case KeyboardCode.F10: return KeyCode.F10;
                case KeyboardCode.F11: return KeyCode.F11;
                case KeyboardCode.F12: return KeyCode.F12;
                case KeyboardCode.F13: return KeyCode.F13;
                case KeyboardCode.F14: return KeyCode.F14;
                case KeyboardCode.F15: return KeyCode.F15;
                case KeyboardCode.Alpha0: return KeyCode.Alpha0;
                case KeyboardCode.Alpha1: return KeyCode.Alpha1;
                case KeyboardCode.Alpha2: return KeyCode.Alpha2;
                case KeyboardCode.Alpha3: return KeyCode.Alpha3;
                case KeyboardCode.Alpha4: return KeyCode.Alpha4;
                case KeyboardCode.Alpha5: return KeyCode.Alpha5;
                case KeyboardCode.Alpha6: return KeyCode.Alpha6;
                case KeyboardCode.Alpha7: return KeyCode.Alpha7;
                case KeyboardCode.Alpha8: return KeyCode.Alpha8;
                case KeyboardCode.Alpha9: return KeyCode.Alpha9;
                case KeyboardCode.Exclaim: return KeyCode.Exclaim;
                case KeyboardCode.DoubleQuote: return KeyCode.DoubleQuote;
                case KeyboardCode.Hash: return KeyCode.Hash;
                case KeyboardCode.Dollar: return KeyCode.Dollar;
                case KeyboardCode.Ampersand: return KeyCode.Ampersand;
                case KeyboardCode.Quote: return KeyCode.Quote;
                case KeyboardCode.LeftParen: return KeyCode.LeftParen;
                case KeyboardCode.RightParen: return KeyCode.RightParen;
                case KeyboardCode.Asterisk: return KeyCode.Asterisk;
                case KeyboardCode.Plus: return KeyCode.Plus;
                case KeyboardCode.Comma: return KeyCode.Comma;
                case KeyboardCode.Minus: return KeyCode.Minus;
                case KeyboardCode.Period: return KeyCode.Period;
                case KeyboardCode.Slash: return KeyCode.Slash;
                case KeyboardCode.Colon: return KeyCode.Colon;
                case KeyboardCode.Semicolon: return KeyCode.Semicolon;
                case KeyboardCode.Less: return KeyCode.Less;
                case KeyboardCode.Equals: return KeyCode.Equals;
                case KeyboardCode.Greater: return KeyCode.Greater;
                case KeyboardCode.Question: return KeyCode.Question;
                case KeyboardCode.At: return KeyCode.At;
                case KeyboardCode.LeftBracket: return KeyCode.LeftBracket;
                case KeyboardCode.Backslash: return KeyCode.Backslash;
                case KeyboardCode.RightBracket: return KeyCode.RightBracket;
                case KeyboardCode.Caret: return KeyCode.Caret;
                case KeyboardCode.Underscore: return KeyCode.Underscore;
                case KeyboardCode.BackQuote: return KeyCode.BackQuote;
                case KeyboardCode.A: return KeyCode.A;
                case KeyboardCode.B: return KeyCode.B;
                case KeyboardCode.C: return KeyCode.C;
                case KeyboardCode.D: return KeyCode.D;
                case KeyboardCode.E: return KeyCode.E;
                case KeyboardCode.F: return KeyCode.F;
                case KeyboardCode.G: return KeyCode.G;
                case KeyboardCode.H: return KeyCode.H;
                case KeyboardCode.I: return KeyCode.I;
                case KeyboardCode.J: return KeyCode.J;
                case KeyboardCode.K: return KeyCode.K;
                case KeyboardCode.L: return KeyCode.L;
                case KeyboardCode.M: return KeyCode.M;
                case KeyboardCode.N: return KeyCode.N;
                case KeyboardCode.O: return KeyCode.O;
                case KeyboardCode.P: return KeyCode.P;
                case KeyboardCode.Q: return KeyCode.Q;
                case KeyboardCode.R: return KeyCode.R;
                case KeyboardCode.S: return KeyCode.S;
                case KeyboardCode.T: return KeyCode.T;
                case KeyboardCode.U: return KeyCode.U;
                case KeyboardCode.V: return KeyCode.V;
                case KeyboardCode.W: return KeyCode.W;
                case KeyboardCode.X: return KeyCode.X;
                case KeyboardCode.Y: return KeyCode.Y;
                case KeyboardCode.Z: return KeyCode.Z;
                case KeyboardCode.Numlock: return KeyCode.Numlock;
                case KeyboardCode.CapsLock: return KeyCode.CapsLock;
                case KeyboardCode.ScrollLock: return KeyCode.ScrollLock;
                case KeyboardCode.RightShift: return KeyCode.RightShift;
                case KeyboardCode.LeftShift: return KeyCode.LeftShift;
                case KeyboardCode.RightControl: return KeyCode.RightControl;
                case KeyboardCode.LeftControl: return KeyCode.LeftControl;
                case KeyboardCode.RightAlt: return KeyCode.RightAlt;
                case KeyboardCode.LeftAlt: return KeyCode.LeftAlt;
                case KeyboardCode.LeftCommand: return KeyCode.LeftCommand;
                case KeyboardCode.LeftApple: return KeyCode.LeftApple;
                case KeyboardCode.LeftWindows: return KeyCode.LeftWindows;
                case KeyboardCode.RightCommand: return KeyCode.RightCommand;
                case KeyboardCode.RightApple: return KeyCode.RightApple;
                case KeyboardCode.RightWindows: return KeyCode.RightWindows;
                case KeyboardCode.AltGr: return KeyCode.AltGr;
                case KeyboardCode.Help: return KeyCode.Help;
                case KeyboardCode.Print: return KeyCode.Print;
                case KeyboardCode.SysReq: return KeyCode.SysReq;
                case KeyboardCode.Break: return KeyCode.Break;
                case KeyboardCode.Menu: return KeyCode.Menu;
            }
        }

        static string GetAxisName (int index, int axis)
        {
            return "Joystick " + index.ToString() + " Axis " + axis.ToString();
        }

        #endregion
    }
}