using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusGTFSDataController : MonoBehaviour {
	
	//
	// Shapes
	//

	public TextAsset shapesTextData;

	public Dictionary<string, List<LatitudeLongitude>> shapesLatLongsByRouteStringId = new Dictionary<string, List<LatitudeLongitude>>();

	private const int kShapesDataRouteMaxLength = 650; // largest was 615 for route 102_IB

	public void LoadShapesData(System.Action dataLoadedCallback) {
		System.Action<string[]> lineProcessor = delegate(string[] lineComponents) {
			string routeStringId = lineComponents[0];

			if (!this.shapesLatLongsByRouteStringId.ContainsKey(routeStringId)) {
				this.shapesLatLongsByRouteStringId.Add(routeStringId, new List<LatitudeLongitude>(kShapesDataRouteMaxLength));
			}

			this.shapesLatLongsByRouteStringId[routeStringId].Add(new LatitudeLongitude(double.Parse(lineComponents[1]), double.Parse(lineComponents[2])));
		};

		this.ProcessCSVData(this.shapesTextData.text, lineProcessor, 4, dataLoadedCallback);

		#if UNITY_EDITOR
		foreach (KeyValuePair<string, List<LatitudeLongitude>> pair in this.shapesLatLongsByRouteStringId) {
			if (pair.Value.Count > kShapesDataRouteMaxLength) {
				Debug.LogWarning("Shape length for route " + pair.Key + " exceeds length, is count: " + pair.Value.Count);
			}
		}
		#endif
	}

	//
	// Stops
	//

	public TextAsset stopsTextData;

	public struct StopInfo {
		public LatitudeLongitude latlong;
		public int stopId;
//		public int btId; // not sure what bt_id is for... 
		public string stopName;
	}

	public List<StopInfo> stopInfos; // indexed by stopId

	public void LoadStopsData(System.Action dataLoadedCallback) {
		this.stopInfos = new List<StopInfo>(7050); // highest stopId is 7012, but there are tons of gaps within that

		StopInfo nullStop = new StopInfo();

		System.Action<string[]> lineProcessor = delegate(string[] lineComponents) {
			int stopId = int.Parse(lineComponents[2]);

			StopInfo newStopInfo = new StopInfo();
			newStopInfo.latlong = new LatitudeLongitude(double.Parse(lineComponents[0]), double.Parse(lineComponents[1]));
			newStopInfo.stopId = stopId;
			newStopInfo.stopName = lineComponents[4];

			if (this.stopInfos.Count >= stopId) {
				Debug.LogWarning("Out of order stop detected at count: " + this.stopInfos.Count);
			}
			else {
				while (this.stopInfos.Count < stopId)
					this.stopInfos.Add(nullStop);

				this.stopInfos.Add(newStopInfo);
			}
		};

		this.ProcessCSVData(this.stopsTextData.text, lineProcessor, 5, dataLoadedCallback);

		#if UNITY_EDITOR
		for (int i = 0; i < this.stopInfos.Count; i++) {
			StopInfo stopInfo = this.stopInfos[i];

			if (stopInfo.stopName != null && stopInfo.stopId != i) {
				Debug.LogWarning("Stop Id doesn't match index at index: " + i + " stopId: " + stopInfo.stopId);
			}
		}
		#endif
	}

	//
	// Stop Times
	//

	//
	// General Processing
	//

	private void ProcessCSVData(string csvText, System.Action<string[]> processLineCallback, int expectedNumberOfColumns, System.Action dataFinishedProcessingCallback) {
		string[] csvTextLines = csvText.Split(new string[]{"\n"}, System.StringSplitOptions.None);

		for (int i = 1; i < csvTextLines.Length; i++) {
			string[] lineComponents = csvTextLines[i].Split(new string[]{","}, System.StringSplitOptions.None);

			if (lineComponents.Length == expectedNumberOfColumns) {
				processLineCallback(lineComponents);
			}
			else {
				if (i != csvTextLines.Length - 1)
					Debug.LogError("Line: " + i + " doesn't have 3 components: " + csvTextLines[i]);
			}
		}

		dataFinishedProcessingCallback();
	}
}

//
// Data Structures
//

public struct BusGTFSStopData {
	/* 	stop_lat,	stop_lon,		stop_id,	bt_id,	stop_name
		61.216517,	-149.886091,	0002,		2788,	6TH AVENUE & C STREET ESE    */

	public LatitudeLongitude latLong;
	public int stopId;
	public int btId;
	public string stopName;
}