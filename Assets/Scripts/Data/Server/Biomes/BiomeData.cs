using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeData : MonoBehaviour
{
    public string UID;
    public string Name;
}

public class BiomeInfo
{
    public Dictionary<string, double> weights = new Dictionary<string, double>()
    {
        { "open_world", 0.0 },
        { "forest", 0.0 },
        { "village", 0.0 },
        { "town", 0.0 },
        { "city", 0.0 },
        { "crypt", 0.0 }        
    };
}
