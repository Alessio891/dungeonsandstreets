using Mapbox.Unity.Utilities;
using Mapbox.Utils;

namespace Mapbox.Examples.LocationProvider
{
    using UnityEngine;
    using Mapbox.Unity.MeshGeneration;
    using Mapbox.Unity.Location;
    using Mapbox.Unity.Map;

    /// <summary>
    /// Override the map center (latitude, longitude) for a MapController, based on the DefaultLocationProvider.
    /// This will enable you to generate a map for your current location, for example.
    /// </summary>
    public class BuildMapAtLocation : MonoBehaviour
    {
        [SerializeField]
        AbstractMap _mapController;

		Vector2 currentTile;

		public int viewRange = 2;

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

        void Start()
        {
            LocationProvider.OnLocationUpdated += LocationProvider_OnLocationUpdated;
            Map.UnwrappedTileId v = Conversions.LatitudeLongitudeToTileId(LocationProvider.CurrentLocation.LatitudeLongitude.x, LocationProvider.CurrentLocation.LatitudeLongitude.y, _mapController.AbsoluteZoom);
			Vector2 lastTile = currentTile;
			currentTile = new Vector2 ((float)v.X, (float)v.Y);
			Debug.Log ("current tile: " + currentTile.x + "," + currentTile.y);
			if (lastTile.x != currentTile.x || lastTile.y != currentTile.y) {
				Debug.Log ("Tile changed!");
				for (int i = -viewRange; i <= viewRange; i++) {
					for (int j = -viewRange; j <= viewRange; j++) {
                        Vector2 vf = currentTile + new Vector2(i, j);
                        Vector2d vd = new Vector2d(vf.x, vf.y);
                        _mapController.UpdateMap(vd, _mapController.AbsoluteZoom);
					}
				}
			}
        }

        void LocationProvider_OnLocationUpdated(Location loc)
        {
            //_mapController.LatLng = string.Format("{0}, {1}", e.Location.x, e.Location.y);
            //_mapController.enabled = true;
            Map.UnwrappedTileId v = Conversions.LatitudeLongitudeToTileId ((float)loc.LatitudeLongitude.x,(float)loc.LatitudeLongitude.y, _mapController.AbsoluteZoom);
			Vector2 lastTile = currentTile;
			currentTile = new Vector2 ((float)v.X, (float)v.Y);
//			Debug.Log ("current tile: " + currentTile.x + "," + currentTile.y);
			if (lastTile.x != currentTile.x || lastTile.y != currentTile.y) {
//				Debug.Log ("Tile changed!");
				for (int i = -viewRange; i <= viewRange; i++) {
					for (int j = -viewRange; j <= viewRange; j++) {
                        Vector2 vf = currentTile + new Vector2(i, j);
                        Vector2d vd = new Vector2d(vf.x, vf.y);

                        _mapController.UpdateMap(vd, _mapController.AbsoluteZoom);
					}
				}
			}
         //			LocationProvider.OnLocationUpdated -= LocationProvider_OnLocationUpdated;
        }
    }
}