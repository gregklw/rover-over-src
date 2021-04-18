using UnityEngine;
using PadInput;
using System.Collections;
using PII.Utilities;

namespace PII
{
    public static class InputSystem
    {
        public static readonly string PauseAxes = "Pause";
        public static readonly string[] CameraAxes = new string[] { "Look Up", "Look Down", "Look Left", "Look Right" };
        public static readonly string[] DroneAxes = new string[] { "Move Forward", "Move Back", "Move Left", "Move Right", "Boost", "Toggle Power", "Pick Up" };
        public static readonly string[] ShipAxes = new string[] { "New Drone", "Collect Items", "Switch Camera" };

        public static OnButtonChanaged onButtonChanged { get; set; }

        private static readonly PadCode[] PauseButtons = new PadCode[]
        {
            new PadCode(KeyboardCode.Escape), // Look Up
            new PadCode(ControllerCode.Start), // Look Down
        };
        
        private static readonly PadCode[] DefaultKMCameraButtons = new PadCode[]
        {
            new PadCode(MouseCode.MouseYPositive), // Look Up
            new PadCode(MouseCode.MouseYNegative), // Look Down
            new PadCode(MouseCode.MouseXNegative), // Look Left
            new PadCode(MouseCode.MouseXPositive) // Look Right
        };

        private static readonly PadCode[] DefaultKMDroneButtons = new PadCode[]
        {
            new PadCode(KeyboardCode.W), // Move Forward
            new PadCode(KeyboardCode.S), // Move Back
            new PadCode(KeyboardCode.A), // Move Left
            new PadCode(KeyboardCode.D), // Move Right
            new PadCode(KeyboardCode.LeftShift), // Boost
            new PadCode(KeyboardCode.Space), // Toggle Power
            new PadCode(KeyboardCode.E) // Pick Up
        };

        private static readonly PadCode[] DefaultKMShipButtons = new PadCode[]
        {
            new PadCode(KeyboardCode.Tab), // New Drone
            new PadCode(KeyboardCode.E), // Collect Items
            new PadCode(KeyboardCode.Q) // Switch Camera
        };

        private static readonly PadCode[] DefaultCCameraButtons = new PadCode[]
        {
            new PadCode(ControllerCode.RightStickUp), // Look Up
            new PadCode(ControllerCode.RightStickDown), // Look Down
            new PadCode(ControllerCode.RightStickLeft), // Look Left
            new PadCode(ControllerCode.RightStickRight) // Look Right
        };

        private static readonly PadCode[] DefaultCDroneButtons = new PadCode[]
        {
            new PadCode(ControllerCode.RightTrigger), // Move Forward
            new PadCode(ControllerCode.LeftTrigger), // Move Back
            new PadCode(ControllerCode.LeftStickLeft), // Move Left
            new PadCode(ControllerCode.LeftStickRight), // Move Right
            new PadCode(ControllerCode.LeftBumper), // Boost
            new PadCode(ControllerCode.DpadDown), // Toggle Power
            new PadCode(ControllerCode.DpadUp) // Pick Up
        };

        private static readonly PadCode[] DefaultCShipButtons = new PadCode[]
        {
            new PadCode(ControllerCode.DpadDown), // New Drone
            new PadCode(ControllerCode.DpadUp), // Collect Items
            new PadCode(ControllerCode.DpadRight) // Switch Camera
        };

        public static PadCode[] GetKMCameraButtons()
        {
            var codes = new PadCode[CameraAxes.Length];

            for (int i = 0; i < codes.Length; i++)
            {
                var source = PlayerPrefs.GetInt(CameraAxes[i] + "_KM_Source", -1);
                var code = PlayerPrefs.GetInt(CameraAxes[i] + "_KM_Code", -1);
                codes[i] = GetCode(source, code, DefaultKMCameraButtons[i]);
            }

            return codes;
        }

        public static PadCode[] GetKMDroneButtons()
        {
            var codes = new PadCode[DroneAxes.Length];

            for (int i = 0; i < codes.Length; i++)
            {
                var source = PlayerPrefs.GetInt(DroneAxes[i] + "_KM_Source", -1);
                var code = PlayerPrefs.GetInt(DroneAxes[i] + "_KM_Code", -1);
                codes[i] = GetCode(source, code, DefaultKMDroneButtons[i]);
            }

            return codes;
        }

        public static PadCode[] GetKMShipButtons()
        {
            var codes = new PadCode[ShipAxes.Length];

            for (int i = 0; i < codes.Length; i++)
            {
                var source = PlayerPrefs.GetInt(ShipAxes[i] + "_KM_Source", -1);
                var code = PlayerPrefs.GetInt(ShipAxes[i] + "_KM_Code", -1);
                codes[i] = GetCode(source, code, DefaultKMShipButtons[i]);
            }

            return codes;
        }

        public static PadCode[] GetCCameraButtons()
        {
            var codes = new PadCode[CameraAxes.Length];

            for (int i = 0; i < codes.Length; i++)
            {
                var source = PlayerPrefs.GetInt(CameraAxes[i] + "_C_Source", -1);
                var code = PlayerPrefs.GetInt(CameraAxes[i] + "_C_Code", -1);
                codes[i] = GetCode(source, code, DefaultCCameraButtons[i]);
            }

            return codes;
        }

        public static PadCode[] GetCDroneButtons()
        {
            var codes = new PadCode[DroneAxes.Length];

            for (int i = 0; i < codes.Length; i++)
            {
                var source = PlayerPrefs.GetInt(DroneAxes[i] + "_C_Source", -1);
                var code = PlayerPrefs.GetInt(DroneAxes[i] + "_C_Code", -1);
                codes[i] = GetCode(source, code, DefaultCDroneButtons[i]);
            }

            return codes;
        }

        public static PadCode[] GetCShipButtons()
        {
            var codes = new PadCode[ShipAxes.Length];

            for (int i = 0; i < codes.Length; i++)
            {
                var source = PlayerPrefs.GetInt(ShipAxes[i] + "_C_Source", -1);
                var code = PlayerPrefs.GetInt(ShipAxes[i] + "_C_Code", -1);
                codes[i] = GetCode(source, code, DefaultCShipButtons[i]);
            }

            return codes;
        }

        public static PadManager GetInputManager()
        {
            var manager = new GameObject("Input Manager").AddComponent<PadManager>();

            var pauseAxis = new PadAxis();
            pauseAxis.Name = PauseAxes;
            for (int i = 0; i < PauseButtons.Length; i++)
            {
                pauseAxis.AddNewInput(PauseButtons[i], 1, false);
            }
            manager.AddAxis(pauseAxis);

            var kCameraButtons = GetKMCameraButtons();
            var cCameraButtons = GetCCameraButtons();
            for (int i = 0; i < CameraAxes.Length; i++)
            {
                var axis = new PadAxis();
                axis.Name = CameraAxes[i];
                axis.Invert = axis.Name == "Look Down" || axis.Name == "Look Left";
                axis.AddNewInput(kCameraButtons[i], 1, false);
                axis.AddNewInput(cCameraButtons[i], 1, false);
                manager.AddAxis(axis);
            }

            var kDroneButtons = GetKMDroneButtons();
            var cDroneButtons = GetCDroneButtons();
            for (int i = 0; i < DroneAxes.Length; i++)
            {
                var axis = new PadAxis();
                axis.Name = DroneAxes[i];
                axis.Invert = axis.Name == "Move Back" || axis.Name == "Move Left";
                axis.AddNewInput(kDroneButtons[i], 1, false);
                axis.AddNewInput(cDroneButtons[i], 1, false);
                manager.AddAxis(axis);
            }

            var kShipButtons = GetKMShipButtons();
            var cShipButtons = GetCShipButtons();
            for (int i = 0; i < ShipAxes.Length; i++)
            {
                var axis = new PadAxis();
                axis.Name = ShipAxes[i];
                axis.AddNewInput(kShipButtons[i], 1, false);
                axis.AddNewInput(cShipButtons[i], 1, false);
                manager.AddAxis(axis);
            }

            return manager;
        }

        public static void SetAllButtonsToDefaults(bool invokeCallBack = true)
        {
            for (int i = 0; i < CameraAxes.Length; i++)
            {
                ChangeKMCameraButton(i, DefaultKMCameraButtons[i], false);
                ChangeCCameraButton(i, DefaultCCameraButtons[i], false);
            }
            
            for (int i = 0; i < DroneAxes.Length; i++)
            {
                ChangeKMDroneButton(i, DefaultKMDroneButtons[i], false);
                ChangeCDroneButton(i, DefaultCDroneButtons[i], false);
            }

            for (int i = 0; i < ShipAxes.Length; i++)
            {
                ChangeKMShipButton(i, DefaultKMShipButtons[i], false);
                ChangeCShipButton(i, DefaultCShipButtons[i], false);
            }

            if (invokeCallBack)
                onButtonChanged();
        }

        public static void ChangeKMCameraButton(int axis, PadCode code, bool invokeCallBack = true)
        {
            if (axis > CameraAxes.Length - 1)
                return;

            var ints = GetIntEq(code);
            PlayerPrefs.SetInt(CameraAxes[axis] + "_KM_Source", ints[0]);
            PlayerPrefs.SetInt(CameraAxes[axis] + "_KM_Code", ints[1]);

            if (invokeCallBack)
                onButtonChanged();
        }

        public static void ChangeKMDroneButton(int axis, PadCode code, bool invokeCallBack = true)
        {
            if (axis > DroneAxes.Length - 1)
                return;

            var ints = GetIntEq(code);

            PlayerPrefs.SetInt(DroneAxes[axis] + "_KM_Source", ints[0]);
            PlayerPrefs.SetInt(DroneAxes[axis] + "_KM_Code", ints[1]);

            if (invokeCallBack)
                onButtonChanged();
        }

        public static void ChangeKMShipButton(int axis, PadCode code, bool invokeCallBack = true)
        {
            if (axis > ShipAxes.Length - 1)
                return;

            var ints = GetIntEq(code);

            PlayerPrefs.SetInt(ShipAxes[axis] + "_KM_Source", ints[0]);
            PlayerPrefs.SetInt(ShipAxes[axis] + "_KM_Code", ints[1]);

            if (invokeCallBack)
                onButtonChanged();
        }

        public static void ChangeCCameraButton(int axis, PadCode code, bool invokeCallBack = true)
        {
            if (axis > CameraAxes.Length - 1)
                return;

            var ints = GetIntEq(code);

            PlayerPrefs.SetInt(CameraAxes[axis] + "_C_Source", ints[0]);
            PlayerPrefs.SetInt(CameraAxes[axis] + "_C_Code", ints[1]);

            if (invokeCallBack)
                onButtonChanged();
        }

        public static void ChangeCDroneButton(int axis, PadCode code, bool invokeCallBack = true)
        {
            if (axis > DroneAxes.Length - 1)
                return;

            var ints = GetIntEq(code);

            PlayerPrefs.SetInt(DroneAxes[axis] + "_C_Source", ints[0]);
            PlayerPrefs.SetInt(DroneAxes[axis] + "_C_Code", ints[1]);

            if (invokeCallBack)
                onButtonChanged();
        }

        public static void ChangeCShipButton(int axis, PadCode code, bool invokeCallBack = true)
        {
            if (axis > ShipAxes.Length - 1)
                return;

            var ints = GetIntEq(code);

            PlayerPrefs.SetInt(ShipAxes[axis] + "_C_Source", ints[0]);
            PlayerPrefs.SetInt(ShipAxes[axis] + "_C_Code", ints[1]);

            if (invokeCallBack)
                onButtonChanged();
        }

        private static PadCode GetCode(int source, int code, PadCode defaultValue)
        {
            if (source < 0 || code < 0)
            {
                return defaultValue;
            }

            var pSource = (InputSource)source;

            switch (pSource)
            {
                case InputSource.Keyboard: return new PadCode((KeyboardCode)code);
                case InputSource.Mouse: return new PadCode((MouseCode)code);
                case InputSource.Controller: return new PadCode((ControllerCode)code);
            }

            return new PadCode();
        }

        private static int[] GetIntEq(PadCode code)
        {
            var ints = new int[2];

            ints[0] = code.Source == InputSource.None ? -1 : (int)code.Source;

            ints[1] = code.Source == InputSource.Keyboard ? (int)code.Keyboard :
                (code.Source == InputSource.Mouse ? (int)code.Mouse :
                (code.Source == InputSource.Controller ? (int)code.Controller : -1));

            return ints;
        }

        public delegate void OnButtonChanaged();
    }
}