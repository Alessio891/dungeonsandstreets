using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkillTreeAsset))]
[CanEditMultipleObjects]
public class SkillAssetEditor : Editor
{
    static string[] StatsName = new string[] { "None", "Strength", "Dexterity", "Intelligence", "Toughness", "Luck" };

    SkillTreeAsset skillAsset;
    float CellSize = 50;
    SkillTreeEntry SelectedEntry = null;
    GUISkin customSkin;
    Texture MoveLeft, MoveRight;

    int currentSelLevel = 0;
    int currentSelTalent = 0;

    bool PickingRequirement = false;

    bool requirementFold = false;

    bool hideEmpty = false;

    List<List<Rect>> Rects = new List<List<Rect>>();

    private void OnEnable()
    {
        skillAsset = (SkillTreeAsset)target;
    }

    public override void OnInspectorGUI()
    {
        Rects.Clear();
        if (customSkin == null)
        {
            customSkin = EditorGUIUtility.Load("SkillEditor.guiskin") as GUISkin;
            MoveLeft = EditorGUIUtility.Load("MoveLeft.png") as Texture;
            MoveRight = EditorGUIUtility.Load("MoveRight.png") as Texture;
        }
        //serializedObject.Update();
        skillAsset.ID = EditorGUILayout.TextField("ID:", skillAsset.ID);
        skillAsset.Name = EditorGUILayout.TextField("Name:", skillAsset.Name);
        GUILayout.Space(10);
        EditorGUILayout.BeginVertical(customSkin.box);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Hide Empty", GUILayout.Width(70));
        hideEmpty = EditorGUILayout.Toggle(hideEmpty, GUILayout.Width(40));
        EditorGUILayout.EndHorizontal();

        for (int i1 = 0; i1 < skillAsset.Levels.Count; i1++)
        {
            Rects.Add(new List<Rect>());
            SkillTreeLevel level = skillAsset.Levels[i1];
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            Color oldBg = GUI.backgroundColor;
            if (PickingRequirement)
            {
                GUI.backgroundColor = Color.green;
            }
            for(int i = 0; i < 3; i++)
            {
                bool present = false;
                for (int j = 0; j < level.Skills.Count; j++)
                {
                    SkillTreeEntry entry = level.Skills[j];
                    if (entry.SlotIndex == i)
                    {
                        if (GUILayout.Button(new GUIContent(entry.Name, entry.sprite.texture), customSkin.button, GUILayout.Width(CellSize), GUILayout.Height(CellSize)))
                        {
                            if (!PickingRequirement)
                            {
                                SelectedEntry = entry;
                                currentSelLevel = i1;
                                currentSelTalent = j;                                
                            } else
                            {
                                skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Requirements.Talent = level.ID + ":" + entry.ID;
                                PickingRequirement = false;
                            }
                        }
                        present = true;
                        Rect r = GUILayoutUtility.GetLastRect();
                        Rects[i1].Add(r);
                        break;
                    }
                }
                if (!present)
                {
                    if (PickingRequirement)
                    {
                        GUI.backgroundColor = Color.red;
                    }
                    if (!hideEmpty)
                    {
                        if (GUILayout.Button("Add", customSkin.button, GUILayout.Width(CellSize), GUILayout.Height(CellSize)))
                        {
                            if (!PickingRequirement)
                            {
                                SkillTreeEntry newEntry = new SkillTreeEntry();
                                newEntry.Name = "new";
                                newEntry.SlotIndex = i;
                                newEntry.Requirements = new SkillTreeEntryRequirement();
                                newEntry.Requirements.Stat = "None";
                                newEntry.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Resources/DefaultSkillSprite.png");
                                currentSelLevel = i1;
                                currentSelTalent = level.Skills.Count;
                                SelectedEntry = newEntry;
                                level.Skills.Add(newEntry);
                            }
                        }
                    } else
                    {
                        GUILayout.Space(CellSize+3);
                    }
                    GUI.backgroundColor = Color.white;
                }
                
                GUILayout.Space(10);

            }
            
           

            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace();           
            EditorGUILayout.EndHorizontal();

           


            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(CellSize), GUILayout.Height(15)))
            {
                skillAsset.Levels.RemoveAt(i1);
                break;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
        for (int i = 0; i < skillAsset.Levels.Count; i++)
        {
            for (int j = 0; j < skillAsset.Levels[i].Skills.Count; j++)
            {
                string req = skillAsset.Levels[i].Skills[j].Requirements.Talent;
                if (!string.IsNullOrEmpty(req))
                {
                    string[] splitReq = req.Split(':');
                    for (int k = 0; k < skillAsset.Levels.Count; k++)
                    {
                        if (skillAsset.Levels[k].ID == splitReq[0])
                        {
                            for (int w = 0; w < skillAsset.Levels[k].Skills.Count; w++)
                            {
                                if (skillAsset.Levels[k].Skills[w].ID == splitReq[1])
                                {
                                    Vector2 pos1 = new Vector2(Rects[i][j].position.x + Rects[i][j].width / 2, Rects[i][j].yMin);
                                    Vector2 pos2 = new Vector2(Rects[k][w].position.x + Rects[i][j].width / 2, Rects[k][w].yMax);
                                    Handles.color = Color.black;
                                    
                                    Handles.DrawLine(pos1, pos2);
                                    Handles.DrawLine(pos1+new Vector2(1,0), pos2+new Vector2(1,0));
                                }
                            }
                        }
                    }
                }
            }
        }
        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Add Level"))
        {
            SkillTreeLevel l = new SkillTreeLevel();
            l.ID = "Level_" + skillAsset.Levels.Count.ToString();
            skillAsset.Levels.Add(l);
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.Space(15);
        if (SelectedEntry != null)
        {
            EditorGUILayout.BeginVertical(customSkin.box);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Name = EditorGUILayout.TextField(skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Name, customSkin.textField);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Edit", skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].sprite.texture), customSkin.button, GUILayout.Width(50), GUILayout.Height(50)))
            {
                EditorGUIUtility.ShowObjectPicker<Sprite>(SelectedEntry.sprite, false, "", 0);
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);


            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent(MoveLeft), customSkin.button,GUILayout.Width(30), GUILayout.Height(30)))
            {
                if (SelectedEntry.SlotIndex > 0)
                {

                    int toSwap = -1;
                    SkillTreeEntry current = skillAsset.Levels[currentSelLevel].Skills[currentSelTalent];
                    for (int i = 0; i < skillAsset.Levels[currentSelLevel].Skills.Count; i++)
                    {
                        SkillTreeEntry e = skillAsset.Levels[currentSelLevel].Skills[i];
                        if (e.SlotIndex == SelectedEntry.SlotIndex-1)
                        {
                            toSwap = i;
                            break;
                        }
                    }

                    skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].SlotIndex -= 1;
                    if (toSwap != -1)
                    {
                        skillAsset.Levels[currentSelLevel].Skills[toSwap].SlotIndex += 1;
                    }
                                                            
                }
            }
            GUILayout.Space(30);
            if (GUILayout.Button(new GUIContent(MoveRight), customSkin.button, GUILayout.Width(30), GUILayout.Height(30)))
            {
                if (SelectedEntry.SlotIndex < 2)
                {

                    int toSwap = -1;
                    SkillTreeEntry current = skillAsset.Levels[currentSelLevel].Skills[currentSelTalent];
                    for (int i = 0; i < skillAsset.Levels[currentSelLevel].Skills.Count; i++)
                    {
                        SkillTreeEntry e = skillAsset.Levels[currentSelLevel].Skills[i];
                        if (e.SlotIndex == SelectedEntry.SlotIndex + 1)
                        {
                            toSwap = i;
                            break;
                        }
                    }
                     
                    skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].SlotIndex += 1;
                    if (toSwap != -1)
                    {
                        skillAsset.Levels[currentSelLevel].Skills[toSwap].SlotIndex -= 1;
                    }

                }
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("ID:", GUILayout.Width(80));
            skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].ID = EditorGUILayout.TextField(skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].ID, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("Max Points:", GUILayout.Width(80));
            skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].MaxPoints = EditorGUILayout.IntField(skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].MaxPoints, GUILayout.Width(150));
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("Description:");
           
            skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Description = EditorGUILayout.TextArea(skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Description, customSkin.textArea, GUILayout.Height(100));


            EditorGUILayout.BeginVertical(customSkin.box);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(20);
            requirementFold = EditorGUILayout.Foldout(requirementFold, "- Requirements -");
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            if (requirementFold)
            {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Level:", customSkin.label, GUILayout.Width(50));
                skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Requirements.PlayerLevel = EditorGUILayout.IntField(skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Requirements.PlayerLevel, GUILayout.Width(80));
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Stat:", customSkin.label, GUILayout.Width(50));
                int selected = 0;
                string[] split = skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Requirements.Stat.Split(':');
                if (split[0] != "None")
                {
                    for (int i = 0; i < StatsName.Length; i++)
                    {
                        if (split[0] == StatsName[i])
                        {
                            selected = i;
                            break;
                        }
                    }
                }
                selected = EditorGUILayout.Popup(selected, StatsName, GUILayout.Width(150));
                int amount = 0;
                if (split.Length > 1)
                {
                    amount = int.Parse(split[1]);
                }
                amount = EditorGUILayout.IntField(amount, GUILayout.Width(50));
                skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Requirements.Stat = StatsName[selected] + ":" + amount.ToString();

                GUILayout.FlexibleSpace();

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Talent Requirement:", customSkin.label);
                string talent = skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Requirements.Talent;

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                if (string.IsNullOrEmpty(talent))
                {
                    if (GUILayout.Button(new GUIContent("None", EditorGUIUtility.Load("TalentNotSelected.png") as Texture), customSkin.button, GUILayout.Width(40), GUILayout.Height(40)))
                    {
                        PickingRequirement = true;
                    }
                }
                else
                {
                    string[] splitTalent = talent.Split(':');
                    bool found = false;
                    foreach (SkillTreeLevel l in skillAsset.Levels)
                    {
                        if (l.ID == splitTalent[0])
                        {
                            foreach (SkillTreeEntry e in l.Skills)
                            {
                                if (e.ID == splitTalent[1])
                                {
                                    EditorGUILayout.BeginVertical();
                                    if (GUILayout.Button(new GUIContent("Edit", e.sprite.texture), customSkin.button, GUILayout.Width(40), GUILayout.Height(40)))
                                    {
                                        PickingRequirement = true;
                                    }
                                    if (GUILayout.Button("Reset", customSkin.button, GUILayout.Width(45), GUILayout.Height(20)))
                                    {
                                        skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Requirements.Talent = "";
                                    }
                                    found = true;
                                    EditorGUILayout.EndVertical();
                                }
                            }
                        }
                    }
                    if (!found)
                    {
                        skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].Requirements.Talent = "";
                    }
                }
                
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

            }
                EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove", GUILayout.Width(100)))
            {
                skillAsset.Levels[currentSelLevel].Skills.RemoveAt(currentSelTalent);
                SelectedEntry = null;
                currentSelLevel = -1;
                currentSelTalent = -1;                
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            if (Event.current.commandName == "ObjectSelectorUpdated")
            {
                skillAsset.Levels[currentSelLevel].Skills[currentSelTalent].sprite = EditorGUIUtility.GetObjectPickerObject() as Sprite;
            }

        }
        
        EditorUtility.SetDirty(skillAsset);
        
    }
}
