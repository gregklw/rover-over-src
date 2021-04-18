using UnityEngine;
using UnityEditor;

namespace PadInput
{
    internal static class PadUtility
    {
        public static readonly string Version = "1.0.0";

        [MenuItem("Assets/Pad/Setup Inputs")]
        private static void CreateInputAxes()
        {
            for (int i = 1; i <= 4; i++)
            {
                for (int j = 1; j < 20; j++)
                {
                    AddAxis(new InputAxis() { name = string.Format("Joystick {0} Axis {1}", i, j), dead = 0.19f, sensitivity = 1f, type = AxisType.JoystickAxis, axis = j, joyNum = i });
                }
            }

            // Mouse
            AddAxis(new InputAxis() { name = "Mouse X", sensitivity = 0.1f, type = AxisType.MouseMovement, axis = 1 });
            AddAxis(new InputAxis() { name = "Mouse Y", sensitivity = 0.1f, type = AxisType.MouseMovement, axis = 2 });
            AddAxis(new InputAxis() { name = "Mouse ScrollWheel", sensitivity = 0.11f, type = AxisType.MouseMovement, axis = 3 });

            AddAxis(new InputAxis() { name = "Submit", positiveButton = "return", altPositiveButton = "joystick button 0", gravity = 1000.0f, dead = 0.001f, sensitivity = 1000.0f, axis = 1 });
            AddAxis(new InputAxis() { name = "Cancel", positiveButton = "escape", altPositiveButton = "joystick button 1", gravity = 1000.0f, dead = 0.001f, sensitivity = 1000.0f, axis = 1 });
            AddAxis(new InputAxis() { name = "Horizontal", positiveButton = "left arrow", altPositiveButton = "right arrow", gravity = 1000.0f, dead = 0.001f, sensitivity = 1000.0f, axis = 1 });
            AddAxis(new InputAxis() { name = "Vertical", positiveButton = "up arrow", altPositiveButton = "down arrow", gravity = 1000.0f, dead = 0.001f, sensitivity = 1000.0f, axis = 2 });
            
        }

        [MenuItem("Assets/Pad/Clear Input Manager")]
        private static void ClearInputAxes()
        {
            var serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            var axesProperty = serializedObject.FindProperty("m_Axes");
            axesProperty.ClearArray();
            serializedObject.ApplyModifiedProperties();
        }

        [MenuItem("GameObject/Pad/New Pad Manager")]
        private static void CreateNewPadManager()
        {
            GameObject manager = new GameObject("Input Manger");
            manager.AddComponent<PadManager>();
        }

        private static SerializedProperty GetChildProperty(SerializedProperty parent, string name)
        {
            var child = parent.Copy();
            child.Next(true);
            do
            {
                if (child.name == name) return child;
            }
            while (child.Next(false));
            return null;
        }

        private static bool isAxisDefined(string axisName)
        {
            var serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            var axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.Next(true);
            axesProperty.Next(true);
            while (axesProperty.Next(false))
            {
                SerializedProperty axis = axesProperty.Copy();
                axis.Next(true);
                if (axis.stringValue == axisName) return true;
            }
            return false;
        }

        private enum AxisType
        {
            KeyOrMouseButton = 0,
            MouseMovement = 1,
            JoystickAxis = 2
        }

        private class InputAxis
        {
            public string name;
            public string descriptiveName = "";
            public string descriptiveNegativeName = "";
            public string negativeButton = "";
            public string positiveButton = "";
            public string altNegativeButton = "";
            public string altPositiveButton = "";

            public float gravity;
            public float dead;
            public float sensitivity;

            public bool snap = false;
            public bool invert = false;

            public AxisType type;

            public int axis;
            public int joyNum;
        }

        private static void AddAxis(InputAxis axis)
        {
            if (isAxisDefined(axis.name)) return;

            SerializedObject serializedObject = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0]);
            SerializedProperty axesProperty = serializedObject.FindProperty("m_Axes");

            axesProperty.arraySize++;
            serializedObject.ApplyModifiedProperties();

            SerializedProperty axisProperty = axesProperty.GetArrayElementAtIndex(axesProperty.arraySize - 1);

            GetChildProperty(axisProperty, "m_Name").stringValue = axis.name;
            GetChildProperty(axisProperty, "descriptiveName").stringValue = axis.descriptiveName;
            GetChildProperty(axisProperty, "descriptiveNegativeName").stringValue = axis.descriptiveNegativeName;
            GetChildProperty(axisProperty, "negativeButton").stringValue = axis.negativeButton;
            GetChildProperty(axisProperty, "positiveButton").stringValue = axis.positiveButton;
            GetChildProperty(axisProperty, "altNegativeButton").stringValue = axis.altNegativeButton;
            GetChildProperty(axisProperty, "altPositiveButton").stringValue = axis.altPositiveButton;
            GetChildProperty(axisProperty, "gravity").floatValue = axis.gravity;
            GetChildProperty(axisProperty, "dead").floatValue = axis.dead;
            GetChildProperty(axisProperty, "sensitivity").floatValue = axis.sensitivity;
            GetChildProperty(axisProperty, "snap").boolValue = axis.snap;
            GetChildProperty(axisProperty, "invert").boolValue = axis.invert;
            GetChildProperty(axisProperty, "type").intValue = (int)axis.type;
            GetChildProperty(axisProperty, "axis").intValue = axis.axis - 1;
            GetChildProperty(axisProperty, "joyNum").intValue = axis.joyNum;

            serializedObject.ApplyModifiedProperties();
        }
    }
}
