using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class BusRouteDataController : MonoBehaviour {
	public List<BusDataStop> busStops = new List<BusDataStop>();

	// Use this for initialization
	void Start () {
		this.BeginDownloadingDataForStops(this.LoadStopsData);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void BeginDownloadingDataForStops(System.Action<XMLQuickParser> dataReadyCallback) {
		this.StartCoroutine(this.co_DownloadData("http://bustracker.muni.org/InfoPoint/XML/stops.xml", dataReadyCallback));
	}

	private string[] busTrackerDataUrls = new string[] {
		"http://bustracker.muni.org/InfoPoint/XML/stopdepartures.xml",
		"http://bustracker.muni.org/InfoPoint/XML/publicmessages.xml",
		"http://bustracker.muni.org/InfoPoint/XML/stops.xml",
		"http://bustracker.muni.org/InfoPoint/XML/routestops.xml",
		"http://bustracker.muni.org/InfoPoint/XML/routes.xml",
		"http://bustracker.muni.org/InfoPoint/XML/vehiclelocation.xml",
	};

	private IEnumerator co_DownloadData (string dataURL, System.Action<XMLQuickParser> dataReadyCallback) {
//		while (true) 
		{
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

				XMLQuickParser xmlParsing = new XMLQuickParser(dataURL, dataString);

				dataReadyCallback(xmlParsing);
			}
		}
	}

	private void LoadStopsData(XMLQuickParser xmlData) {
		int dataLength = 0;

		foreach (XmlNode node in xmlData.xmlDoc) {
			Debug.Log("node is: " + node.Name);

			if (node.Name == "stops") {
				foreach (XmlNode stopNode in node) {
					dataLength++;

//					if (dataLength < 99999) 
					{
						BusDataStop newStop = new BusDataStop();

						foreach (XmlNode stopNodeElement in stopNode) {
							if (stopNodeElement.Name == "name") {
								newStop.name = stopNodeElement.InnerText;
							}
							else if (stopNodeElement.Name == "latitude") {
								newStop.latitudeLongitude.latitude = double.Parse(stopNodeElement.InnerText);
							}
							else if (stopNodeElement.Name == "longitude") {
								newStop.latitudeLongitude.longitude = double.Parse(stopNodeElement.InnerText);
							}
						}

						this.busStops.Add(newStop);

//						this.AddIndicatorAtLatLong(new LatitudeLongitude(latitude, longitude));
					}
//					else {
//						break;
//					}
				}
			}


		}

		//		Debug.Log("dataLength: " + dataLength);

		//		for (int i = 0; i < xmlData.NumberOfNodesInFirstChildAtDepth(1); i++) {
		//
		//		}
	}
}
