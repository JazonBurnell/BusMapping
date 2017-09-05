using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public enum BusDataType {
	StopDepartures,
	PublicMessages,
	Stops,
	RouteStops,
	Routes,
	VehicleLocation
};

[System.Serializable]
public class BusRoutePredownloadDataSet : System.Object {
	public List<TextAsset> stopDepartures = new List<TextAsset>();
	public List<TextAsset> publicMessages = new List<TextAsset>();
	public List<TextAsset> stops = new List<TextAsset>();
	public List<TextAsset> routeStops = new List<TextAsset>();
	public List<TextAsset> routes = new List<TextAsset>();
	public List<TextAsset> vehicleLocation = new List<TextAsset>();

	public List<List<TextAsset>> AllDataArrayByType() {
		return new List<List<TextAsset>>(new List<TextAsset>[] {
			this.stopDepartures,
			this.publicMessages,
			this.stops,
			this.routeStops,
			this.routes,
			this.vehicleLocation
		});
	}
}

class BusRouteItemInfoSet : System.Object {
	public string dataUrl;
	public System.Action<XMLQuickParser> dataParsedCallback;

	public BusRouteItemInfoSet(string dataUrl, System.Action<XMLQuickParser> dataParsedCallback) {
		this.dataUrl = dataUrl;
		this.dataParsedCallback = dataParsedCallback;
	}
}

public class BusRouteDataController : MonoBehaviour {
	// GTFS Data
	public BusGTFSDataController gtfsDataController;

	// Raw Data
	public List<BusDataStop> busStops = new List<BusDataStop>();
	public List<BusRouteStopItemData> busRouteStops = new List<BusRouteStopItemData>();

	// Processed data
//	public Dictionary<int, BusRouteStopsDataSet> busRouteSortedStopIds = new Dictionary<int, BusRouteStopsDataSet>();

	// Using pre-downloaded data
	public bool usePredownloadedFiles = true;
	public BusRoutePredownloadDataSet predownloadedDataSet = new BusRoutePredownloadDataSet();
	
	public void BeginDownloadingDataForType(BusDataType dataType, System.Action<BusDataType> dataReadyCallback) {
		int dataIndex = (int) dataType;

		if (dataIndex < this.busTrackerItemDataInfo.Length) {
			if (!this.usePredownloadedFiles) {
				this.StartCoroutine(this.co_DownloadData(this.busTrackerItemDataInfo[dataIndex].dataUrl, delegate(string dataString) {
					this.CreateParserForData(dataType, this.busTrackerItemDataInfo[dataIndex].dataUrl, dataString, dataReadyCallback);
				}));
			}
			else {
				if (dataIndex < this.predownloadedDataSet.AllDataArrayByType().Count && this.predownloadedDataSet.AllDataArrayByType()[dataIndex].Count 
				     > 0) {
					string dataText = this.predownloadedDataSet.AllDataArrayByType()[dataIndex][0].text;
					string dataInfoString = predownloadedDataSet.AllDataArrayByType()[dataIndex][0].name;

					this.CreateParserForData(dataType, dataInfoString, dataText, dataReadyCallback);
				}
				else {
					Debug.LogError("Couldn't load predownloaded data, data not populated for type: " + dataType);
				}
			}
		}
	}

	// 
	// Data Retreival
	//

	public BusDataStop BusStopForStopId(int stopId) {
		return BusDataStop.BusStopByStopId(stopId);
	}

	//
	// Data parsers and processors
	//

	private BusRouteItemInfoSet[] busTrackerItemDataInfo = new BusRouteItemInfoSet[] {
		new BusRouteItemInfoSet("http://bustracker.muni.org/InfoPoint/XML/stopdepartures.xml", null),
		new BusRouteItemInfoSet("http://bustracker.muni.org/InfoPoint/XML/publicmessages.xml", null),
		new BusRouteItemInfoSet("http://bustracker.muni.org/InfoPoint/XML/stops.xml", null),
		new BusRouteItemInfoSet("http://bustracker.muni.org/InfoPoint/XML/routestops.xml", null),
		new BusRouteItemInfoSet("http://bustracker.muni.org/InfoPoint/XML/routes.xml", null),
		new BusRouteItemInfoSet("http://bustracker.muni.org/InfoPoint/XML/vehiclelocation.xml", null)
	};

	private IEnumerator co_DownloadData (string dataURL, System.Action<string> dataDownloadedCallback) {
		WWW webData = new WWW(dataURL);

		while (!webData.isDone) {
			yield return null;
		}

		if (webData.error != null) {
			Debug.LogError("Received error: " + webData.error);
		}
		else {
			Debug.Log("Downloaded data at: " + dataURL + " bytes: " + webData.bytesDownloaded);

			string dataString = webData.text;

			dataDownloadedCallback(dataString);
		}
	}

	private void CreateParserForData(BusDataType dataType, string infoString, string dataString, System.Action<BusDataType> dataReadyCallback) {
		XMLQuickParser xmlParsing = new XMLQuickParser(infoString, dataString);

		if (dataType == BusDataType.Stops) {
			this.LoadDataIntoObjects<BusDataStop>(dataType, xmlParsing, "stops", this.busStops, dataReadyCallback);

			Debug.Log("Stops, lowest id: " + BusDataStop._lowestIdValue + " highest id: " + BusDataStop._highestIdValue);
		}
		else if (dataType == BusDataType.RouteStops) {
			this.LoadDataIntoObjects<BusRouteStopItemData>(dataType, xmlParsing, "routestops", this.busRouteStops, dataReadyCallback);
		}
		else {
			Debug.LogError("No loading algorithm specified for dataType: " + dataType);
		}
	}

	private void LoadDataIntoObjects<T>(BusDataType busDataType, XMLQuickParser xmlData, string rootNodeName, List<T> dataArray, System.Action<BusDataType> dataReadyCallback) where T : BusDataBaseObject {
		int dataLength = 0;

		try {
			BusDataBaseObject dataObj = null;

			foreach (XmlNode node in xmlData.xmlDoc) {
				if (node.Name == rootNodeName) {
					foreach (XmlNode stopNode in node) {
						dataLength++;

						// Hmm... need to look up this error
//						dataObj = new T(); // Cannot create an instance of the variable type `T' because it does not have the new() constraint

						if (typeof(T) == typeof(BusDataStop)) {
							dataObj = new BusDataStop();
						}
						else if (typeof(T) == typeof(BusRouteStopItemData)) {
							dataObj = new BusRouteStopItemData();
						}
						else {
							Debug.LogWarning("No class defined for T: " + typeof(T).ToString());
							dataObj = null;
						}

						foreach (XmlNode stopNodeElement in stopNode) {
							dataObj.ParseAndLoadDataElement(stopNodeElement.Name, stopNodeElement.InnerText);
						}

						dataObj.ParseAndLoadFinishedForObject();

						dataArray.Add((T)dataObj);
					}
				}
			}

			if (dataObj != null)
				dataObj.ParseAndLoadFinishedForClass();
			else
				Debug.LogError("dataObj null on ParseAndLoadFinishedForClass");

			if (dataReadyCallback != null) {
				dataReadyCallback(busDataType);
			}
		}
		catch (System.Exception e) {
			Debug.LogError("Failed to parse data for type: " + BusDataType.Stops + " error: " + e.ToString());
		}

		Debug.Log("Loaded data count: " + dataLength + " for type: " + typeof(T).ToString());
	}
}
