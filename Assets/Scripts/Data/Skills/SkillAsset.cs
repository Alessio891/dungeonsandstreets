using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class SkillAsset : ScriptableObject {
    public string ID;
    public string Name;

    public Image sprite;

#if UNITY_EDITOR

    [MenuItem("BattleFox/Download Skills")]
    public static void DownloadSkills()
    {
        Bridge.GET(Bridge.url + "Skills/GetSkills?skills=all", (r) => {
            ServerResponse resp = new ServerResponse(r);
            Debug.Log("[GETSKILLS] " + r);
            if (resp.status == ServerResponse.ResultType.Success)
            {
                List<Dictionary<string, object>> data = resp.GetIncomingList();
                foreach (Dictionary<string, object> skill in data)
                {
                    string id = skill["id"].ToString();
                    SkillAsset asset = Resources.Load<SkillAsset>("Data/Skills/" + id);
                    if (asset != null)
                    {
                        asset.Name = skill["name"].ToString();
                    }
                    else
                    {
                        SkillAsset skillAsset = new SkillAsset();
                        skillAsset.ID = id;
                        skillAsset.name = skill["id"].ToString();
                        skillAsset.Name = skill["name"].ToString();
                        AssetDatabase.CreateAsset(skillAsset, "Assets/Resources/Data/Skills/" + skillAsset.name + ".asset");
                    }
                }
                /**/
            }
        });
    }

#endif

}
