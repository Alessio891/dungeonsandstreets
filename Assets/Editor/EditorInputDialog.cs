using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorInputDialog : EditorWindow
{

    public static EditorInputDialog instance;
    System.Action<string> OnInputSent;
    System.Action OnCancel;
    string value = "";
    private void OnGUI()
    {        
        value = EditorGUILayout.TextField(value);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Ok", GUILayout.Width(100)))
        {
            if (!string.IsNullOrEmpty(value))
            {
                OnInputSent(value);
                this.Close();
                this.Destroy();
            }
        }
        if (GUILayout.Button("Cancel", GUILayout.Width(100)))
        {
            if (OnCancel != null)
                OnCancel();
            
            this.Close();
            this.Destroy();
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    public static void Show(System.Action<string> onSent, string defaultValue = "", System.Action onCancel = null)
    {
        if (instance == null)
        {
            float x = (Screen.currentResolution.width - 400) / 2;
            float y = (Screen.currentResolution.height - 50) / 2;
            instance = EditorWindow.GetWindowWithRect<EditorInputDialog>(new Rect( x,y, 400, 50));
        }
        instance.value = defaultValue;
        instance.OnInputSent = onSent;
        instance.OnCancel = onCancel;
        instance.Show();
    }
}
