using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour {
	public MapIndicatorsController mapIndicatorController;
	public BusRouteDataController busRouteDataController;

	private bool haveAddedStopData = false;

	void Awake () {

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.haveAddedStopData) {
			if (this.busRouteDataController.busStops.Count > 0) {
				this.haveAddedStopData = true;

				foreach (BusDataStop busStop in this.busRouteDataController.busStops) {
					this.mapIndicatorController.AddIndicatorAtLatLong(busStop.latitudeLongitude);
				}
			}
		}
	}
}
