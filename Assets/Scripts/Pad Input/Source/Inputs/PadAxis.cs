using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace PadInput
{
    [System.Serializable]
    public class PadAxis
    {
        public string Name;
        public bool Invert;
        public List<Input> Inputs = new List<Input>();

        public ControllerIndex controllerIndex { get; set; }
        public float value { get { return GetValue(); } }
        public bool pressing { get { return CheckIfPressing(); } }
        public bool pressed { get { return CheckIfPressed(); } }
        public bool released { get { return CheckIfReleased(); } }

		public bool fold;

        public Input GetInputWithCode(PadCode code)
        {
            foreach (var input in Inputs)
            {
                if (input.Button == code)
                    return input;
            }

            return null;
        }

        [System.Serializable]
        public class Input
        {
            public PadCode Button;
            public float Scale = 1.00f;
            public bool Snap = false;

			public bool fold;

            public Input()
            {
                Button = new PadCode();
                Scale = 1.00f;
                Snap = false;
            }

            public Input(PadCode button, float scale, bool snap)
            {
                Button = button;
                Scale = scale;
                Snap = snap;
            }
        }

        float GetValue()
        {
            float value = 0;

            for (int i = 0; i < Inputs.Count; i++)
            {
                value += Value(Inputs[i].Snap, Inputs[i].Button, Inputs[i].Scale);
            }

            if (Invert)
                value *= -1;

            return Mathf.Clamp(value, -1, 1);
        }

        float Value (bool snap, PadCode button, float scale)
        {
            if (snap)
            {
                return (Pad.GetInputValue(button, controllerIndex) == 0 ? 0 : (Pad.GetInputValue(button) > 0 ? 1 : -1)) * scale;
            }
            else
            {
                return Pad.GetInputValue(button, controllerIndex) * scale;
            }
        }

        bool CheckIfReleased()
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                if (Pad.GetInputReleased(Inputs[i].Button, controllerIndex))
                {
                    return true;
                }
            }

            return false;
        }

        bool CheckIfPressing()
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                if (Pad.GetInputPressing(Inputs[i].Button, controllerIndex))
                {
                    return true;
                }
            }

            return false;
        }

        bool CheckIfPressed()
        {
            for (int i = 0; i < Inputs.Count; i++)
            {
                if (Pad.GetInputPressed(Inputs[i].Button, controllerIndex))
                {
                    return true;
                }
            }

            return false;
        }

        public void AddNewInput(Input input)
        {
            Inputs.Add(input);
        }
        
        public void AddNewInput(PadCode button, float scale, bool snap)
        {
            Inputs.Add(new Input(button, scale, snap));
        }

        public void RemoveInput(Input input)
        {
            Inputs = Inputs.Where(Input => Input != input).ToList();
        }

        public void RemoveInput(PadCode button)
        {
            Inputs = Inputs.Where(Input => Input.Button != button).ToList();
        }
    }
}
