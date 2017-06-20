using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class AppController : MonoBehaviour {
	public MapIndicatorsController mapIndicatorController;
	public BusRouteDataController busRouteDataController;

	// Use this for initialization
	void Start () {
		this.busRouteDataController.BeginDownloadingDataForStops(this.LoadStopsData);	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void LoadStopsData(XMLQuickParser xmlData) {
		int dataLength = 0;

		foreach (XmlNode node in xmlData.xmlDoc) {
			Debug.Log("node is: " + node.Name);

			if (node.Name == "stops") {
				foreach (XmlNode stopNode in node) {
					dataLength++;

					if (dataLength < 99999) {
						double latitude = 0;
						double longitude = 0;

						foreach (XmlNode stopNodeElement in stopNode) {
							if (stopNodeElement.Name == "name") {
//								Debug.Log("Stop Node Value: " + stopNodeElement.InnerText);
							}
							else if (stopNodeElement.Name == "latitude") {
								latitude = double.Parse(stopNodeElement.InnerText);
							}
							else if (stopNodeElement.Name == "longitude") {
								longitude = double.Parse(stopNodeElement.InnerText);
							}
						}

						this.mapIndicatorController.AddIndicatorAtLatLong(new LatitudeLongitude(latitude, longitude));
					}
					else {
						break;
					}
				}
			}


		}

		Debug.Log("dataLength: " + dataLength);

//		for (int i = 0; i < xmlData.NumberOfNodesInFirstChildAtDepth(1); i++) {
//
//		}
	}
}
