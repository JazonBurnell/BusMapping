using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusRouteDataController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
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
}
