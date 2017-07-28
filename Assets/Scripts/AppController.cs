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

		this.busRouteDataController.gtfsDataController.LoadShapesData(delegate() {
			this.StartCoroutine(this.co_LoadCompletedForGTFSDataShapes());
		});
	}

	private IEnumerator co_LoadCompletedForGTFSDataShapes() {
		string routeStringId = "3C_OB_CNT";// "102_IB";// "13_IB";//"3C_OB_CNT";//	this.busRouteDataController.gtfsDataController.shapesLatLongsByRouteStringId.Keys[0];

		int latLongCount = this.busRouteDataController.gtfsDataController.shapesLatLongsByRouteStringId[routeStringId].Count;

		List<Vector3> linePathPoints = new List<Vector3>(latLongCount);

		foreach (LatitudeLongitude latLong in this.busRouteDataController.gtfsDataController.shapesLatLongsByRouteStringId[routeStringId]) {
			Vector3 addedPos = this.mapIndicatorController.AddIndicatorAtLatLong(latLong, 0);

			linePathPoints.Add(addedPos);

			this.lineRenderer.numPositions = linePathPoints.Count;
			this.lineRenderer.SetPositions(linePathPoints.ToArray());

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
