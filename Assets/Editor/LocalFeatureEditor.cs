using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LocalFeatureEditor : EditorWindow
{

    public Feature currentSelected;

    public Camera tempCamera;
    public GameObject rootInstance;
    RenderTexture renderTexture;

    [MenuItem("BattleFox/Local Feature editor")]
    public static void open()
    {
        EditorWindow.GetWindow<LocalFeatureEditor>().Show();
    }

    void OnGUI()
    {
        /*
        if (currentSelected == null)
        {
            EditorGUILayout.LabelField("No feature selected");
            return;
        }
        BasicFeature selectedFeature = null;
        if (currentSelected.feature != null)
            selectedFeature = currentSelected.feature.OnMapFeature;
            
        selectedFeature = (BasicFeature)EditorGUILayout.ObjectField(selectedFeature, typeof(BasicFeature));
        if (selectedFeature != null)
            currentSelected.feature = selectedFeature.gameObject;
        if (currentSelected.feature != null)
        {
            if (tempCamera == null)
            {
                tempCamera = new GameObject("tempCam").AddComponent<Camera>();
                tempCamera.gameObject.hideFlags = HideFlags.DontSave;
                tempCamera.transform.position = Vector3.zero;
                renderTexture = new RenderTexture(800, 600, 15);            
                tempCamera.targetTexture = renderTexture;
                tempCamera.enabled = false;
                tempCamera.renderingPath = RenderingPath.DeferredShading;
                tempCamera.clearFlags = CameraClearFlags.Color;            
                rootInstance = GameObject.Instantiate<GameObject>(currentSelected.root);
                rootInstance.transform.position = tempCamera.transform.position + (tempCamera.transform.forward * 2.7f) - Vector3.up;
            }
        

            tempCamera.Render();

            EditorGUI.DrawTextureTransparent(new Rect(10, 40, 100, 100), renderTexture);

            GUILayout.Space(45);
            if (Registry.assets.features.Get(currentSelected.key).root != currentSelected.root)
                Registry.assets.features.Replace(currentSelected.key, currentSelected.root);
        }
        else
        {

        }*/

    }

    void OnDestroy()
    {
        Debug.Log("CLOSED!");
        if (tempCamera != null)
        {
            DestroyImmediate(tempCamera.gameObject);
            DestroyImmediate(rootInstance);
        }
    }
}