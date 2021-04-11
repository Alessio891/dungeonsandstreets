using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Examples.LocationProvider;
using Mapbox.Examples;
using Mapbox.Unity.Location;

public class SpawnerItem : BaseConsumable {

	public string featureName;
    ILocationProvider _locationProvider;
    ILocationProvider LocationProvider
    {
        get
        {
            if (_locationProvider == null)
            {
                _locationProvider = LocationProviderFactory.Instance.DefaultLocationProvider;
            }

            return _locationProvider;
        }
    }

    public override void OnUse ()
	{
		NotificationCenter.instance.AddNotification ("You spawn a " + featureName);
		Dictionary<string, object> values = new Dictionary<string, object> ();
        values.Add("x", LocationProvider.CurrentLocation.LatitudeLongitude.x.ToString());
        values.Add ("y", LocationProvider.CurrentLocation.LatitudeLongitude.x.ToString());
		values.Add ("feature", featureName);

		Bridge.POST (Bridge.url + "SpawnFeature", values, (r) => {
			ServerResponse resp = new ServerResponse(r);
			if (resp.status == ServerResponse.ResultType.Success)
			{
				Debug.Log("Success spawning item!");
			}
		});
	}
}
