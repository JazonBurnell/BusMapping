using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppController : MonoBehaviour {
	public MapIndicatorsController mapIndicatorController;
	public BusRouteDataController busRouteDataController;

	public LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
//		this.busRouteDataController.BeginDownloadingDataForType(BusDataType.Stops, this.LoadCompletedForDataType);
//		this.busRouteDataController.BeginDownloadingDataForType(BusDataType.RouteStops, this.LoadCompletedForDataType);

		this.busRouteDataController.gtfsDataController.LoadStopsData(delegate() {
//			foreach (BusGTFSDataController.StopInfo stopInfo in this.busRouteDataController.gtfsDataController.stopInfos) {
//				this.mapIndicatorController.AddIndicatorAtLatLong(stopInfo.latlong, 1);
//			}
		});

		this.busRouteDataController.gtfsDataController.LoadShapesData(delegate() {
			this.StartCoroutine(this.co_LoadCompletedForGTFSDataShapes(false));
		});

		this.busRouteDataController.gtfsDataController.LoadTripStopPointsData(delegate() {
			Debug.Log("Trip stop points loaded!");

			this.busRouteDataController.gtfsDataController.LoadTripInfoData(delegate() {
				Debug.Log("Trip infos loaded too!");

//				string routeStringId = "3C_OB_CNT";
				string routeStringId = "3";

				float currentSecondsIntoDay = BusGTFSDataController.SecondsIntoDayForTimeString(System.DateTime.Now.ToString("HH:mm:ss"));

				foreach (KeyValuePair<string, List<BusGTFSDataController.TripInfo>> routeTripInfosPair in this.busRouteDataController.gtfsDataController.tripInfosByRouteId) {

					if (routeTripInfosPair.Key == routeStringId) {

						foreach (BusGTFSDataController.TripInfo tripInfo in routeTripInfosPair.Value) {

							string tripId = tripInfo.tripId;

							if (this.busRouteDataController.gtfsDataController.stopPointInfosByTripId.ContainsKey(tripId)) {								
								List<BusGTFSDataController.StopPointInfo> routeStopInfos = this.busRouteDataController.gtfsDataController.stopPointInfosByTripId[tripId];

								for (int i = 0; i < routeStopInfos.Count - 1; i++) {
									BusGTFSDataController.StopPointInfo stopInfoA = routeStopInfos[i];
									BusGTFSDataController.StopPointInfo stopInfoB = routeStopInfos[i+1];

									if (currentSecondsIntoDay >= stopInfoA.arrivalSecondsIntoTheDay && currentSecondsIntoDay < stopInfoB.arrivalSecondsIntoTheDay) {

										float percentageBetweenStops = Mathf.InverseLerp(stopInfoA.arrivalSecondsIntoTheDay, stopInfoB.arrivalSecondsIntoTheDay, currentSecondsIntoDay);

										Debug.Log("Hit active stop for tripId: " + tripId + " sequence: " + i);

										LatitudeLongitude stopALongLat = this.busRouteDataController.gtfsDataController.stopInfos[stopInfoA.stopId].latlong;
										LatitudeLongitude stopBLongLat = this.busRouteDataController.gtfsDataController.stopInfos[stopInfoB.stopId].latlong;

										LatitudeLongitude percentageLatLong = LatitudeLongitude.Lerp(stopALongLat, stopBLongLat, (double) percentageBetweenStops);

//										this.mapIndicatorController.AddIndicatorAtLatLong(stopALongLat, 4);
//										this.mapIndicatorController.AddIndicatorAtLatLong(stopBLongLat, 4);

										this.mapIndicatorController.AddIndicatorAtLatLong(percentageLatLong, 4);

									}
								}
							}
							else {
//								if (tripId.Trim().Equals("3-1636-I-1a"))							
								Debug.LogError("Found unknown tripId: <" + tripId + ">" + "nl: " + tripId.Contains("\n"));
							}
						}

					}

				}
			});
		});			
	}

	private IEnumerator co_LoadCompletedForGTFSDataShapes(bool overTime) {
		string routeStringId = "3C_OB_CNT";// "102_IB";// "13_IB";//"3C_OB_CNT";//	this.busRouteDataController.gtfsDataController.shapesLatLongsByRouteStringId.Keys[0];

		int latLongCount = this.busRouteDataController.gtfsDataController.shapesLatLongsByRouteStringId[routeStringId].Count;

		List<Vector3> linePathPoints = new List<Vector3>(latLongCount);

		foreach (LatitudeLongitude latLong in this.busRouteDataController.gtfsDataController.shapesLatLongsByRouteStringId[routeStringId]) {
			Vector3 addedPos = this.mapIndicatorController.AddIndicatorAtLatLong(latLong, 0);

			linePathPoints.Add(addedPos);

			this.lineRenderer.numPositions = linePathPoints.Count;
			this.lineRenderer.SetPositions(linePathPoints.ToArray());

			if (overTime)
				yield return null;
		}

		this.lineRenderer.numPositions = linePathPoints.Count;
		this.lineRenderer.SetPositions(linePathPoints.ToArray());

		yield return null;
	}

//	private void SetFloatData(float floatVal, System.Action<float> setter) {
//		setter(floatVal);
//	}

	private void LoadCompletedForDataType(BusDataType dataType) {
		this.StartCoroutine(this.co_LoadCompletedForDataType(dataType));
	}

	private IEnumerator co_LoadCompletedForDataType(BusDataType dataType) {
		if (dataType == BusDataType.Stops) {
			foreach (BusDataStop busStop in this.busRouteDataController.busStops) {
				// Show all bus stops
//				this.mapIndicatorController.AddIndicatorAtLatLong(busStop.latitudeLongitude);
			}
		}
		else if (dataType == BusDataType.RouteStops) {
			Debug.Log("-------- this.busRouteDataController.busRouteStops.Count: " + this.busRouteDataController.busRouteStops.Count);

//			foreach (BusRouteStopItemData routeStop in this.busRouteDataController.busRouteStops) {
//				if (routeStop.routeNumber == 1) {
//					BusDataStop busStop = this.busRouteDataController.BusStopForStopId(routeStop.stopId);
//
//					this.mapIndicatorController.AddIndicatorAtLatLong(busStop.latitudeLongitude);
//
//					yield return null;
//				}
//			}

			int routeId = 3;

			for (int i = 0; i < BusRouteStopItemData.NumberOfRouteStopItemsForRoute(routeId); i++) {
				BusRouteStopItemData routeStopItem = BusRouteStopItemData.RouteStopItemForRouteAtIndex(routeId, i);

				BusDataStop busStop = this.busRouteDataController.BusStopForStopId(routeStopItem.stopId);

				this.mapIndicatorController.AddIndicatorAtLatLong(busStop.latitudeLongitude, 1);

				Debug.Log("Adding routeStopItem sort id: " + routeStopItem.sortOrder + " stopIds are equal: " + (routeStopItem.stopId == busStop.id).ToString());

//				yield return new WaitForSeconds(0.25f);
			}
		}

		yield return null;
	}
}
