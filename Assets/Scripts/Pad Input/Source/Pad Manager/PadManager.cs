using UnityEngine;
using System.Collections.Generic;

namespace PadInput
{
    [AddComponentMenu("Pad Input/Manager")]
    public class PadManager : MonoBehaviour
    {
        public List<PadAxis> Axes = new List<PadAxis>();
        public ControllerIndex ControllerIndex { get; set; }
        
        public void AddAxis(PadAxis axis)
        {
            Axes.Add(axis);
        }

        public void AddAxes(PadAxis[] axes)
        {
            Axes.AddRange(axes);
        }

        public bool GetAxisPressing(string axis)
        {
            for (int i = 0; i < Axes.Count; i++)
            {
                if (Axes[i].Name == axis)
                {
                    return Axes[i].pressing;
                }
            }

            Debug.LogError(axis + " not defined in " + name);
            return false;
        }

        public bool GetAxisPressed(string axis)
        {
            for (int i = 0; i < Axes.Count; i++)
            {
                if (Axes[i].Name == axis)
                {
                    return Axes[i].pressed;
                }
            }


            Debug.LogError(axis + " not defined in " + name);
            return false;
        }

        public bool GetAxisReleased(string axis)
        {
            for (int i = 0; i < Axes.Count; i++)
            {
                if (Axes[i].Name == axis)
                {
                    return Axes[i].released;
                }
            }

            Debug.LogError(axis + " not defined in " + name);
            return false;
        }

        public float GetAxisValue(string axis)
        {
            for (int i = 0; i < Axes.Count; i++)
            {
                if (Axes[i].Name == axis)
                {
                    return Axes[i].value;
                }
            }

            Debug.LogError(axis + " not defined in " + name);
            return 0;
        }

        public float GetCombinedAxesValue(string axisA, string axisB)
        {
            return GetAxisValue(axisA) + GetAxisValue(axisB);
        }

        public PadAxis GetPadAxis(string axis)
        {
            for (int i = 0; i < Axes.Count; i++)
            {
                if (Axes[i].Name == axis)
                {
                    return Axes[i];
                }
            }

            Debug.LogError(axis + " not defined in " + name);
            return null;
        }

        public string GetPadAxisButtonName(string axis)
        {
            var name = "";
            var axe = GetPadAxis(axis);

            if (axe != null)
            {
                for (int i = 0; i < axe.Inputs.Count; i++)
                {
                    name += i == 0 ? "" : " Or ";
                    name += Pad.GetCodeName(axe.Inputs[i].Button);
                }
            }

            return name;
        }

        public void AddAxisInput(string axis, PadCode button, float scale, bool snap)
        {
            for (int i = 0; i < Axes.Count; i++)
            {
                if (Axes[i].Name == axis)
                {
                    Axes[i].AddNewInput(button, scale, snap);
                }
            }
        }

        public void RemoveAxisInput(string axis, PadCode button)
        {
            for (int i = Axes.Count - 1; i >= 0; i--)
            {
                if (Axes[i].Name == axis)
                {
                    Axes[i].RemoveInput(button);
                }
            }
        }
        
        private void Update()
        {
            foreach (var axis in Axes)
            {
                axis.controllerIndex = ControllerIndex;
            }
        }
    }
}