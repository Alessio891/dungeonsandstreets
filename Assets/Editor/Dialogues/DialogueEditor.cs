using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditor : EditorWindow
{
    public DialogueAsset currentDialogue;
    public DialogueAsset lastDialogue = null;

    bool responseFoldout = false;
    List<bool> actionFoldout = new List<bool>();

    List<bool> responseFoldouts = new List<bool>();
    List<List<bool>> actionFoldouts = new List<List<bool>>();

    [MenuItem("BattleFox/Dialogue Editor")]
    public static void open()
    {
        EditorWindow.GetWindow<DialogueEditor>().Show();
    }

    private void OnGUI()
    {
        if (Selection.activeObject is DialogueAsset)
            currentDialogue = (DialogueAsset)Selection.activeObject;
        else
            currentDialogue = null;

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
        {
            EditorWindow.GetWindow<DialogueEditorPicker>().Show();
        }

        if (currentDialogue == null)
        {
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(false)))
            {
                DialogueAsset dialogue = new DialogueAsset();
                AssetDatabase.CreateAsset(dialogue, "Assets/Scripts/Data/Dialogues/Assets/NewDialogue.asset");
                Selection.activeObject = dialogue;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Deleting", "Are you sure you want to delete " + currentDialogue.DialogueTitle + "?", "I am"))
                {
                    if (Registry.assets.dialogues.IsPresent(currentDialogue.UID))
                    {
                        if (EditorUtility.DisplayDialog("Item Table", "This item was registered in the Registry. Should i remove it?", "Yes", "No"))
                        {
                            Registry.assets.dialogues.Remove(currentDialogue.UID);
                        }
                    }
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(currentDialogue));
                    currentDialogue = null;
                    return;
                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            GUILayout.Space(2);
            EditorGUILayout.BeginHorizontal("box");
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Upload", GUILayout.ExpandWidth(false)))
            {

                currentDialogue.Upload();
            }
            if (GUILayout.Button("Download", GUILayout.ExpandWidth(false)))
            {
                currentDialogue.Download();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (lastDialogue != null)
            {
                if (GUILayout.Button("<-"+lastDialogue.DialogueTitle,GUILayout.ExpandWidth(false)))
                {
                    currentDialogue = lastDialogue;
                    lastDialogue = null;
                }
            }
            if (!Registry.assets.dialogues.IsPresent(currentDialogue.UID))
            {
                EditorGUILayout.HelpBox("This dialogue is not registered in the dialogues table! Would you like to add it now?", MessageType.Warning);
                if (GUILayout.Button("Add It!", GUILayout.ExpandWidth(false)))
                {
                    DialogueTableEntry e = new DialogueTableEntry();
                    e.UID = currentDialogue.UID;
                    e.quest = currentDialogue;
                    Registry.assets.dialogues.entries.Add(e);
                    EditorUtility.SetDirty(Registry.assets.dialogues);
                }
            }
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            currentDialogue.UID = EditorGUILayout.TextField("UID:", currentDialogue.UID);
            GUI.enabled = true;
            if (GUILayout.Button("Generate UID", GUILayout.ExpandWidth(false)))
            {
                currentDialogue.GenerateUID();
            }
            EditorGUILayout.EndHorizontal();

            currentDialogue.DialogueTitle = EditorGUILayout.TextField("Title:", currentDialogue.DialogueTitle);

            EditorGUILayout.LabelField("Dialogue Text:");
            currentDialogue.DialogueText = EditorGUILayout.TextArea(currentDialogue.DialogueText, GUILayout.Height(130));

            responseFoldout = EditorGUILayout.Foldout(responseFoldout, "Responses");
            if (responseFoldout)
            {
                for (int i = 0; i < currentDialogue.Responses.Count; i++)
                {                    
                    if (responseFoldouts.Count - 1 < i)
                    {
                        responseFoldouts.Add(false);
                    }                    
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    responseFoldouts[i] = EditorGUILayout.Foldout(responseFoldouts[i], currentDialogue.Responses[i].Text);
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        currentDialogue.Responses.RemoveAt(i);
                        return;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (responseFoldouts[i])
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.BeginVertical("box");

                        currentDialogue.Responses[i].Text = EditorGUILayout.TextField("Text:", currentDialogue.Responses[i].Text);
                        if (actionFoldouts.Count-1 < i)
                        {
                            actionFoldouts.Add(new List<bool>());
                        }
                        for(int j = 0; j < currentDialogue.Responses[i].Actions.Count; j++)
                        {
                            if (actionFoldouts[i].Count - 1 < j)
                            {
                                actionFoldouts[i].Add(false);
                            }
                            EditorGUILayout.BeginHorizontal();
                            actionFoldouts[i][j] = EditorGUILayout.Foldout(actionFoldouts[i][j], currentDialogue.Responses[i].Actions[j].action.ToString());
                            if (GUILayout.Button("X", GUILayout.Width(30)))
                            {
                                currentDialogue.Responses[i].Actions.RemoveAt(j);
                                return;
                            }
                            EditorGUILayout.EndHorizontal();
                            if (actionFoldouts[i][j])
                            {
                                EditorGUILayout.BeginHorizontal();
                                GUILayout.Space(30);
                                EditorGUILayout.BeginVertical("button");
                                currentDialogue.Responses[i].Actions[j].action = (DialogueActionData.ResponseAction)EditorGUILayout.EnumPopup("Action:", currentDialogue.Responses[i].Actions[j].action);
                                switch(currentDialogue.Responses[i].Actions[j].action)
                                {
                                    case DialogueActionData.ResponseAction.Continue:
                                        DialogueAsset dialogue = Registry.assets.dialogues[currentDialogue.Responses[i].Actions[j].data1];
                                        dialogue = (DialogueAsset)EditorGUILayout.ObjectField("Dialogue:", dialogue, typeof(DialogueAsset));
                                        if (dialogue != null)
                                            currentDialogue.Responses[i].Actions[j].data1 = dialogue.UID;
                                        break;
                                    case DialogueActionData.ResponseAction.Reward:
                                        EditorGUILayout.BeginHorizontal();
                                        currentDialogue.Responses[i].Actions[j].data1 = EditorGUILayout.TextField("Reward Type:", currentDialogue.Responses[i].Actions[j].data1);
                                        DialogueActionData action = currentDialogue.Responses[i].Actions[j];
                                        if (GUILayout.Button("Pick Item", GUILayout.Width(80)))
                                        {
                                            ItemSelectionEditor.onSelect += (item) =>
                                            {
                                                action.data1 = item.ItemID;
                                                this.Focus();
                                            };
                                            ItemSelectionEditor itemPicker = EditorWindow.GetWindow<ItemSelectionEditor>();
                                            itemPicker.Show();
                                        }
                                        if (Registry.assets.items[action.data1] != null)
                                        {
                                            BaseItem it = Resources.Load<BaseItem>(Registry.assets.items[action.data1]);
                                            if (it != null)
                                                EditorGUILayout.LabelField(it.Name, GUILayout.Width(100));
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        currentDialogue.Responses[i].Actions[j].data2 = EditorGUILayout.TextField("Amount:", currentDialogue.Responses[i].Actions[j].data2);
                                        break;
                                }
                                
                                EditorGUILayout.EndVertical();
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Add Action"))
                        {
                            currentDialogue.Responses[i].Actions.Add(new DialogueActionData());
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                }
                GUILayout.Space(15);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add Response"))
                {
                    currentDialogue.Responses.Add(new DialogueResponse());
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }

        }
    }
}
