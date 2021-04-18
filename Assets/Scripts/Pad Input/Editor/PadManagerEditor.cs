using UnityEngine;
using UnityEditor;

namespace PadInput
{
    [CustomEditor(typeof(PadManager))]
    public class PadManagerEditor : Editor
    {
        PadManager manager;
        
        void OnEnable()
        {

            manager = (PadManager)target;
        }

        void OnDisable()
        {
            
        }

        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();

			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            manager.ControllerIndex = (ControllerIndex)EditorGUILayout.EnumPopup("Controller Index", manager.ControllerIndex);

            EditorGUILayout.Space();

            EditAxes();

            EditorGUILayout.EndVertical();
        }

        void EditAxes ()
        {
            EditorGUILayout.BeginVertical();
            
            for (int i = 0; i < manager.Axes.Count; i++)
            {
				EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                if (manager.Axes[i].Inputs.Count > 0)
					manager.Axes[i].fold = EditorGUILayout.Toggle(manager.Axes[i].fold, EditorStyles.foldout, GUILayout.MaxWidth(10));
				else
					manager.Axes[i].fold = false;
                
				EditPadAxis(manager.Axes[i], manager.Axes[i].fold);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("+", GUILayout.MaxWidth(100)))
            {
                manager.Axes.Add(new PadAxis());
            }

            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        void EditPadAxis (PadAxis axis, bool showInputs)
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();

            axis.Name = EditorGUILayout.TextField(axis.Name);

            if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.MaxWidth(50)))
            {
                axis.AddNewInput(new PadCode(), 1.00f, false);
            }

            if (GUILayout.Button("x", EditorStyles.miniButtonRight, GUILayout.MaxWidth(50)))
            {
                manager.Axes.Remove(axis);
            }

            EditorGUILayout.EndHorizontal();

            if (!showInputs)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            axis.Invert = EditorGUILayout.Toggle("Invert", axis.Invert, EditorStyles.radioButton);
            
            for (int i = 0; i < axis.Inputs.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

				axis.Inputs[i].fold = EditorGUILayout.Toggle(axis.Inputs[i].fold, EditorStyles.foldout, GUILayout.MaxWidth(10));

                EditorGUILayout.LabelField(Pad.GetPadCodeName(axis.Inputs[i].Button));

                if (GUILayout.Button("x", EditorStyles.miniButtonRight, GUILayout.MaxWidth(50)))
                {
                    axis.RemoveInput(axis.Inputs[i]);

                    EditorGUILayout.EndHorizontal();

                    return;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                EditorGUILayout.Space();

				EditPadAxisInput(axis.Inputs[i], axis.Inputs[i].fold);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        void EditPadAxisInput (PadAxis.Input input, bool show)
        {
            if (!show)
                return;

            EditorGUILayout.BeginVertical();

            input.Button.Source = (InputSource)EditorGUILayout.EnumPopup(input.Button.Source);

            switch (input.Button.Source)
            {
                case InputSource.Keyboard:
                    input.Button.Keyboard = (KeyboardCode)EditorGUILayout.EnumPopup(input.Button.Keyboard);
                    break;
                case InputSource.Mouse:
                    input.Button.Mouse = (MouseCode)EditorGUILayout.EnumPopup(input.Button.Mouse);
                    break;
                case InputSource.Controller:
                    input.Button.Controller = (ControllerCode)EditorGUILayout.EnumPopup(input.Button.Controller);
                    break;
            }
            
            input.Scale = EditorGUILayout.Slider("Scale", input.Scale, -1.0f, 1.0f);
            input.Snap = EditorGUILayout.Toggle("Snap", input.Snap, EditorStyles.radioButton);
            
            EditorGUILayout.EndVertical();
        }
    }
}
