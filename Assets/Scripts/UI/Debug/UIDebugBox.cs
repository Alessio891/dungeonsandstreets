using Mapbox.Examples.LocationProvider;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugBox : MonoBehaviour
{
    public Text text;

    private void Update()
    {
        double x = PositionWithLocationProvider.instance.LocationProvider.CurrentLocation.LatitudeLongitude.x;
        double y = PositionWithLocationProvider.instance.LocationProvider.CurrentLocation.LatitudeLongitude.y;
        string t = "PosX: " + x.ToString() + "\n";
        t += "PosY: " + y.ToString() + "\n";
        t += "TileID: " + PositionWithLocationProvider.instance.xy2tile(x.ToString(), y.ToString()) + "\n";
        t += "Biome: " + PlayerServerSync.instance.TEST_BIOME;        

        text.text = t;
    }
}
