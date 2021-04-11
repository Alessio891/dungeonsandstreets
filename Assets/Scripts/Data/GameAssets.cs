using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : ScriptableObject {
	public FeatureTable features;
	public ItemsTable items;
	public AppInfoAsset VersionInfo;
    public AppInfoAsset ServerConfig;
	public ShopsTable shops;
    public MonstersTable monsters;
    public POITable pois;
    public QuestTable quests;
    public DialogueTable dialogues;
    public ProfessionsTable professions;
    public RecipesTable recipes;
}
