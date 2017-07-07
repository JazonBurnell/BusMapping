using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour {
	public MapIndicatorsController mapIndicatorController;
	public BusRouteDataController busRouteDataController;

	// Use this for initialization
	void Start () {
		this.busRouteDataController.BeginDownloadingDataForType(BusDataType.Stops, this.LoadCompletedForDataType);
	}

	private void SetFloatData(float floatVal, System.Action<float> setter) {
		setter(floatVal);
	}

	private void LoadCompletedForDataType(BusDataType dataType) {
		if (dataType == BusDataType.Stops) {
			foreach (BusDataStop busStop in this.busRouteDataController.busStops) {
				this.mapIndicatorController.AddIndicatorAtLatLong(busStop.latitudeLongitude);
			}
		}
	}
}
