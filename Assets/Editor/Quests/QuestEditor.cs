using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class QuestEditor : EditorWindow
{

    public List<string> PossibleEvents = new List<string>()
    {
        "item_used",
        "item_found",
        "item_bought",
        "item_sold",
        "item_received",
        "battle_started",
        "battle_ended",
        "enemy_defeated",
        "enemy_encountered",
        "enemy_drop_item",
        "item_equipped",
        "trade_completed",        
        "poi_visited",
        "entered_dungeon",
        "finished_dungeon",
        "finished_dungeon_unsuccessful",
        "join_boss",
        "boss_defeated",
        "boss_failed",
        "feature_found",
        "feature_used",
        "gold_received",
        "xp_received",
        "level_up"
    };

    public BaseQuest currentQuest;

    bool objectivesFoldout = false;
    bool rewardsFoldout = false;

    List<bool> objFoldoutList = new List<bool>();
    List<bool> rewardsFoldoutList = new List<bool>();

    Vector2 objectiveScroll;
    Vector2 rewardsScroll; 

    [MenuItem("BattleFox/Quest Editor")]
    public static void open()
    {
        EditorWindow.GetWindow<QuestEditor>().Show();
    }

    void OnGUI()
    {

        if (Selection.activeObject is BaseQuest)
            currentQuest = (BaseQuest)Selection.activeObject;
        else
            currentQuest = null;

        EditorGUILayout.BeginHorizontal("box");
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Open", GUILayout.ExpandWidth(false)))
        {
            EditorWindow.GetWindow<QuestEditorPicker>().Show();
        }

        if (currentQuest == null)
        {
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(false)))
            {
                BaseQuest quest = new BaseQuest();
                AssetDatabase.CreateAsset(quest, "Assets/Scripts/Data/Quests/Assets/NewQuest.asset");
                Selection.activeObject = quest;
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            if (GUILayout.Button("Delete", GUILayout.ExpandWidth(false)))
            {
                if (EditorUtility.DisplayDialog("Deleting", "Are you sure you want to delete " + currentQuest.QuestTitle + "?", "I am"))
                {
                    if (Registry.assets.quests.IsPresent(currentQuest.UID))
                    {
                        if (EditorUtility.DisplayDialog("Item Table", "This item was registered in the Registry. Should i remove it?", "Yes", "No"))
                        {
                            Registry.assets.quests.Remove(currentQuest.UID);
                        }
                    }
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(currentQuest));
                    currentQuest = null;
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

                currentQuest.Upload();
            }
            if (GUILayout.Button("Download", GUILayout.ExpandWidth(false)))
            {              
                currentQuest.Download();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = false;
            currentQuest.UID = EditorGUILayout.TextField("UID:", currentQuest.UID);
            GUI.enabled = true;
            if (GUILayout.Button("Generate UID", GUILayout.ExpandWidth(false)))
            {
                currentQuest.GenerateUID();
            }
            EditorGUILayout.EndHorizontal();
            if (!Registry.assets.quests.IsPresent(currentQuest.UID))
            {
                EditorGUILayout.HelpBox("This item is not registered in the items table! Would you like to add it now?", MessageType.Warning);
                if (GUILayout.Button("Add It!", GUILayout.ExpandWidth(false)))
                {
                    QuestTableEntry e = new QuestTableEntry();
                    e.UID = currentQuest.UID;
                    e.quest = currentQuest;
                    Registry.assets.quests.entries.Add(e);
                    EditorUtility.SetDirty(Registry.assets.quests);
                }
            }

            currentQuest.QuestTitle = EditorGUILayout.TextField("Title:", currentQuest.QuestTitle);
            EditorGUILayout.LabelField("Description:");
            currentQuest.QuestDescription = EditorGUILayout.TextArea(currentQuest.QuestDescription, GUILayout.Height(80));
            currentQuest.SpawnChance = EditorGUILayout.Slider("Spawn Chance:", (float)currentQuest.SpawnChance, 0.01f, 1.0f);
            currentQuest.Repeatable = EditorGUILayout.Toggle("Repeatable:", currentQuest.Repeatable);
            objectivesFoldout = EditorGUILayout.Foldout(objectivesFoldout, "Objectives");
            if (objectivesFoldout)
            {
                objectiveScroll = EditorGUILayout.BeginScrollView(objectiveScroll);
                   
                for(int i = 0; i < currentQuest.Objectives.Count; i++)
                {
                    if (objFoldoutList.Count-1 < i)
                    {
                        objFoldoutList.Add(false);
                    }

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    objFoldoutList[i] = EditorGUILayout.Foldout(objFoldoutList[i], currentQuest.Objectives[i].ObjectiveText);
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        currentQuest.Objectives.RemoveAt(i);
                        i--;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (objFoldoutList[i])
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.BeginVertical();
                        currentQuest.Objectives[i].ObjectiveText = EditorGUILayout.TextField("Text:", currentQuest.Objectives[i].ObjectiveText);
                        int ind = PossibleEvents.IndexOf(currentQuest.Objectives[i].ObjectiveTrigger);
                        if (ind < 0)
                            ind = 0;
                        currentQuest.Objectives[i].ObjectiveTrigger = PossibleEvents[EditorGUILayout.Popup("Trigger:", ind, PossibleEvents.ToArray())];
                        
                        switch(currentQuest.Objectives[i].ObjectiveTrigger)
                        {
                            case "item_bought":
                            case "item_sold":
                            case "item_found":
                            case "trade_completed":
                            case "enemy_drop_item":
                            case "item_equipped":
                            case "item_used":
                                string id = currentQuest.Objectives[i].ObjectiveTriggerData;
                                BaseItem item = Resources.Load<BaseItem>(Registry.assets.items[id]);
                                item = (BaseItem)EditorGUILayout.ObjectField("Item:", item, typeof(BaseItem));
                                if (item != null)
                                    currentQuest.Objectives[i].ObjectiveTriggerData = item.UID;
                                else
                                    currentQuest.Objectives[i].ObjectiveTriggerData = "";
                                break;
                            case "battle_started":
                            case "battle_ended":
                                currentQuest.Objectives[i].ObjectiveTriggerData = EditorGUILayout.TextField("(NI)Data:", currentQuest.Objectives[i].ObjectiveTriggerData);
                                break;
                            case "enemy_defeated":
                            case "enemy_encontered":
                                BaseMonster enemy = Registry.assets.monsters[currentQuest.Objectives[i].ObjectiveTriggerData];
                                enemy = (BaseMonster)EditorGUILayout.ObjectField("Enemy:", enemy, typeof(BaseMonster));
                                if (enemy != null)
                                    currentQuest.Objectives[i].ObjectiveTriggerData = enemy.UID;
                                else
                                    currentQuest.Objectives[i].ObjectiveTriggerData = "";
                                break;
                            case "poi_visited":
                                MapPOI poi = Registry.assets.pois[currentQuest.Objectives[i].ObjectiveTriggerData];
                                poi = (MapPOI)EditorGUILayout.ObjectField("POI:", poi, typeof(MapPOI));
                                if (poi != null)
                                    currentQuest.Objectives[i].ObjectiveTriggerData = poi.UID;
                                else
                                    currentQuest.Objectives[i].ObjectiveTriggerData = "";
                                break;
                            case "entered_dungeon":
                            case "finished_dungeon":
                            case "finished_dungeon_unsuccessful":
                                currentQuest.Objectives[i].ObjectiveTriggerData = EditorGUILayout.TextField("(NI)Data:", currentQuest.Objectives[i].ObjectiveTriggerData);
                                break;
                            case "join_boss":
                            case "boss_defeated":
                            case "boss_failed":
                                currentQuest.Objectives[i].ObjectiveTriggerData = EditorGUILayout.TextField("(NI)Data:", currentQuest.Objectives[i].ObjectiveTriggerData);
                                break;
                            case "feature_found":
                            case "feature_used":
                                BasicFeature feature = (BasicFeature)Registry.assets.features[currentQuest.Objectives[i].ObjectiveTriggerData].OnMapFeature;
                                poi = (MapPOI)EditorGUILayout.ObjectField("Feature:", feature, typeof(BaseMonster));
                                if (feature != null)
                                    currentQuest.Objectives[i].ObjectiveTriggerData = feature.type;
                                else
                                    currentQuest.Objectives[i].ObjectiveTriggerData = "";
                                break;
                            case "gold_received":
                            case "xp_received":
                            case "level_up":
                                int amount = 0;
                                int.TryParse(currentQuest.Objectives[i].ObjectiveTriggerData, out amount);
                                amount = EditorGUILayout.IntField("Amount:", amount);
                                currentQuest.Objectives[i].ObjectiveTriggerData = amount.ToString();
                                break;
                        }

                        currentQuest.Objectives[i].ObjectiveProgress = EditorGUILayout.IntField("Required #:", currentQuest.Objectives[i].ObjectiveProgress);
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                    
                }
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add Objective", GUILayout.ExpandWidth(false)))
                {
                    currentQuest.Objectives.Add(new QuestObjective());
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }

            rewardsFoldout = EditorGUILayout.Foldout(rewardsFoldout, "Rewards");
            if (rewardsFoldout)
            {
                rewardsScroll = EditorGUILayout.BeginScrollView(rewardsScroll);
                for (int i = 0; i < currentQuest.Rewards.Count; i++)
                {
                    if (rewardsFoldoutList.Count - 1 < i)
                        rewardsFoldoutList.Add(false);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Space(10);
                    BaseItem item = Resources.Load<BaseItem>(Registry.assets.items[currentQuest.Rewards[i].reward]);
                    string foldOutText = (item == null) ? currentQuest.Rewards[i].reward : "Item: " + item.Name;
                    rewardsFoldoutList[i] = EditorGUILayout.Foldout(rewardsFoldoutList[i], foldOutText);
                    if (GUILayout.Button("X", GUILayout.Width(30)))
                    {
                        currentQuest.Rewards.RemoveAt(i);
                        i--;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (rewardsFoldoutList[i])
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        EditorGUILayout.BeginVertical();
                        currentQuest.Rewards[i].rewardType = (QuestRewardData.RewardType)EditorGUILayout.EnumPopup("Type:", currentQuest.Rewards[i].rewardType);
                        switch(currentQuest.Rewards[i].rewardType)
                        {
                            case QuestRewardData.RewardType.Gold:
                                currentQuest.Rewards[i].reward = "gold";
                                currentQuest.Rewards[i].amount = EditorGUILayout.IntField("Amount:", currentQuest.Rewards[i].amount);
                                break;
                            case QuestRewardData.RewardType.XP:
                                currentQuest.Rewards[i].reward = "xp";
                                currentQuest.Rewards[i].amount = EditorGUILayout.IntField("Amount:", currentQuest.Rewards[i].amount);
                                break;
                            case QuestRewardData.RewardType.Item:
                                
                                item = (BaseItem)EditorGUILayout.ObjectField("Item:", item, typeof(BaseItem));
                                if (item != null)
                                    currentQuest.Rewards[i].reward = item.UID;
                                currentQuest.Rewards[i].amount = EditorGUILayout.IntField("Amount:", currentQuest.Rewards[i].amount);
                                break;
                        }
                        
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.EndHorizontal();
                    }
                }
                GUILayout.Space(10);
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Add Reward", GUILayout.ExpandWidth(false)))
                {
                    currentQuest.Rewards.Add(new QuestRewardData());
                }
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndScrollView();
            }
        }
        if (currentQuest != null)
            EditorUtility.SetDirty(currentQuest);
        EditorUtility.SetDirty(Registry.assets.quests);
    }
}
