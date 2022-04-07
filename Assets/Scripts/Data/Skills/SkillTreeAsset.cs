using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillTreeEntryRequirement : IServerSerializable
{
    public string Talent;
    public string Stat;
    public int PlayerLevel = -1;

    public void Deserialize(Dictionary<string, object> serialized)
    {
        Talent = serialized["Talent"].ToString();
        Stat = serialized["Stat"].ToString();
        PlayerLevel = int.Parse(serialized["PlayerLevel"].ToString());
    }

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("Talent", Talent);
        data.Add("Stat", Stat);
        data.Add("PlayerLevel", PlayerLevel);

        return data;
    }
}

[System.Serializable]
public class SkillTreeEntry : IServerSerializable
{
    public string ID, Name, Description;
    public int SlotIndex;
    public Sprite sprite;
    public SkillTreeEntryRequirement Requirements;
    public int MaxPoints = 1;

    public void Deserialize(Dictionary<string, object> serialized)
    {
        ID = serialized["ID"].ToString();
        Name = serialized["Name"].ToString();
        Description = serialized["Description"].ToString();
        SlotIndex = int.Parse(serialized["SlotIndex"].ToString());
        Requirements = new SkillTreeEntryRequirement();
        Requirements.Deserialize((Dictionary<string, object>)serialized["Requirements"]);
        MaxPoints = int.Parse(serialized["MaxPoints"].ToString());
    }

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ID", ID);
        data.Add("Name", Name);
        data.Add("Description", Description);
        data.Add("SlotIndex", SlotIndex);
        data.Add("Requirements", Requirements.Serialize());
        data.Add("MaxPoints", MaxPoints);
        return data;

    }
}

[System.Serializable]
public class SkillTreeLevel : IServerSerializable
{
    public string ID;
    public List<SkillTreeEntry> Skills = new List<SkillTreeEntry>();

    public SkillTreeEntry GetForSlot(int slot)
    {
        foreach(SkillTreeEntry e in Skills)
        {
            if (e.SlotIndex == slot)
                return e;
        }
        return null;
    }

    public void Deserialize(Dictionary<string, object> serialized)
    {
        ID = serialized["ID"].ToString();

        Skills = new List<SkillTreeEntry>();

        List<object> skillsData = (List<object>)serialized["Skills"];
        foreach(object o in skillsData)
        {
            Dictionary<string, object> skillData = (Dictionary<string, object>)o;
            SkillTreeEntry skill = new SkillTreeEntry();
            skill.Deserialize(skillData);
            Skills.Add(skill);
        }
    }

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("ID", ID);
        List<Dictionary<string, object>> skillsData = new List<Dictionary<string, object>>();

        foreach(SkillTreeEntry skill in Skills)
        {
            skillsData.Add(skill.Serialize());
        }

        data.Add("Skills", skillsData);

        return data;
    }
}

public class SkillTreeAsset : ScriptableObject, IServerSerializable
{
    public string ID, Name;

    public List<SkillTreeLevel> Levels;

    public void Deserialize(Dictionary<string, object> serialized)
    {
        Name = serialized["Name"].ToString();
        Levels = new List<SkillTreeLevel>();

        List<object> levelsData = (List<object>)serialized["Levels"];
        foreach(object o in levelsData)
        {
            Dictionary<string, object> levelData = (Dictionary<string, object>)o;
            SkillTreeLevel level = new SkillTreeLevel();
            level.Deserialize(levelData);
            Levels.Add(level);
        }
    }

    public Dictionary<string, object> Serialize()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        data.Add("id", ID);
        data.Add("Name", Name);

        List<Dictionary<string, object>> levelsData = new List<Dictionary<string, object>>();

        foreach(SkillTreeLevel level in Levels)
        {
            levelsData.Add(level.Serialize());
        }

        data.Add("Levels", levelsData);

        return data;
    }

    [ContextMenu("Download SkillTree")]
    public void Download()
    {
        Bridge.GET(Bridge.url + "Skills/DownloadSkillTree?id=" + ID, (r) =>
            {
                Debug.Log("[DownloadSkillTree Response] " + r);
                ServerResponse resp = new ServerResponse(r);
                if (resp.status != ServerResponse.ResultType.Error)
                {
                    Dictionary<string, object> data = resp.GetIncomingDictionary();
                    this.Deserialize(data);
                    Debug.Log("Downloaded Skill");
                }
            });
    }

    [ContextMenu("Upload Skilltree")]
    public void Upload()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        data.Add("id", ID);
        data.Add("data", MiniJSON.Json.Serialize(this.Serialize()));
        Bridge.POST(Bridge.url + "Skills/UploadSkillTree", data, (r) =>
        {
            ServerResponse resp = new ServerResponse(r);
            Debug.Log("[SkillTreeUpload Response] " + r);
            if (resp.status != ServerResponse.ResultType.Error)
            {
                Debug.Log("SkillTree Uploaded");
            }
        });
    }
}
