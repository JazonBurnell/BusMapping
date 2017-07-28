using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusGTFSDataController : MonoBehaviour {
	
	//
	// Shapes data for routes
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