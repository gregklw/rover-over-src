using UnityEngine;
using System.Collections;

namespace PadInput
{
    [System.Serializable]
    public class PadCode
    {
        public InputSource Source;
        public KeyboardCode Keyboard;
        public MouseCode Mouse;
        public ControllerCode Controller;

        public PadCode ()
        {
            Source = InputSource.None;
            Keyboard = KeyboardCode.None;
            Mouse = MouseCode.None;
            Controller = ControllerCode.None;
        }

        public PadCode (KeyboardCode keyboard)
        {
			Source = InputSource.Keyboard;
            Keyboard = keyboard;

            Mouse = MouseCode.None;
            Controller = ControllerCode.None;
        }

        public PadCode (MouseCode mouse)
        {
			Source = InputSource.Mouse;
            Mouse = mouse;

            Keyboard = KeyboardCode.None;
            Controller = ControllerCode.None;
        }

        public PadCode (ControllerCode controller)
        {
			Source = InputSource.Controller;
            Controller = controller;

            Mouse = MouseCode.None;
            Keyboard = KeyboardCode.None;
        }
    }
}