using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class FeatureTable : ScriptableObject {
	public List<Feature> features;
	public Feature Get(string featureName)
	{
		return features.Where<Feature> ((f) => f.key == featureName).FirstOrDefault<Feature> ();
	}
    public bool isPresent(string featureName)
    {
        return Get(featureName) != null;
    }    
    public void Replace(string key, FeatureAsset root)
    {
        for (int i = 0; i < features.Count; i++)
        {
            if (features[i].key == key)
            {
                features[i].feature = root;
                break;
            }
        }
    }
	public FeatureAsset this [string type] { get {

            foreach (Feature f in features)
            {
                if (f.key == type)
                {
                    return f.feature;
                }
            }
            return null;
    } }
}
[System.Serializable]
public class Feature
{
	public string key;
	public FeatureAsset feature;
}
