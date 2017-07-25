using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusGTFSDataController : MonoBehaviour {
	public TextAsset shapesTextData;

	public Dictionary<string, List<LatitudeLongitude>> shapesLatLongsByRouteStringId = new Dictionary<string, List<LatitudeLongitude>>();

	public void LoadShapesData(System.Action dataLoadedCallback) {
		string shapesDataText = this.shapesTextData.text;

		string[] shapesDataTextLines = shapesDataText.Split(new string[]{"\n"}, System.StringSplitOptions.None);

		for (int i = 1; i < shapesDataTextLines.Length; i++) {
			string[] lineComponents = shapesDataTextLines[i].Split(new string[]{","}, System.StringSplitOptions.None);

			if (lineComponents.Length == 4) {
				string routeStringId = lineComponents[0];

				if (!this.shapesLatLongsByRouteStringId.ContainsKey(routeStringId)) {
					this.shapesLatLongsByRouteStringId.Add(routeStringId, new List<LatitudeLongitude>(200));
				}

				this.shapesLatLongsByRouteStringId[routeStringId].Add(new LatitudeLongitude(double.Parse(lineComponents[1]), double.Parse(lineComponents[2])));
			}
			else {
				if (i != shapesDataTextLines.Length - 1)
					Debug.LogError("Line: " + i + " doesn't have 3 components: " + shapesDataTextLines[i]);
			}
		}

		dataLoadedCallback();
	}
}
