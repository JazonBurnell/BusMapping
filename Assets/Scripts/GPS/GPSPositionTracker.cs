using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GPSPositionTracker : MonoBehaviour {

	public bool enableTracking = false;

	public Text outputText;

	private float lastLocationCheck = 0;
	private float kLocationCheckInterval = 1f;

	private int locationCheckCount = 0;

	private List<LocationInfo> storedLocationInfos = new List<LocationInfo>();

	// Use this for initialization
	void Start () {
		if (this.enableTracking) {
			Debug.LogWarning("GPS Positioning is being stored");

			this.outputText.transform.parent.gameObject.SetActive(true);

			Input.location.Start();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (this.enableTracking) {
			if (Time.time - this.lastLocationCheck > kLocationCheckInterval) {
				this.lastLocationCheck = Time.time;

				this.locationCheckCount++;

				LocationServiceStatus status = Input.location.status;

				#if UNITY_EDITOR
				if (this.locationCheckCount > 2) {
					status = LocationServiceStatus.Running;
				}
				#endif

				string textLabelText = "Status: " + status.ToString() + " c:" + this.locationCheckCount;

				if (status == LocationServiceStatus.Stopped) {
					Input.location.Start(1, 1);				
				}
				else if (status == LocationServiceStatus.Running) {
					LocationInfo locInfo = Input.location.lastData;

					textLabelText += "\nAccuracy: " + locInfo.horizontalAccuracy.ToString("F1") + " x " + locInfo.verticalAccuracy.ToString("F1");

					this.storedLocationInfos.Add(locInfo);
				}

				for (int i = 0; i < 5; i++) {
					int index = this.storedLocationInfos.Count - i - 1;

					if (index > 0) {
						LocationInfo locInfo = this.storedLocationInfos[i];

						textLabelText += "\ni: " + index + " {" + locInfo.latitude.ToString("F3") + "," + locInfo.longitude.ToString("F3") + "} acc: " + locInfo.horizontalAccuracy.ToString("F1") + " x " + locInfo.verticalAccuracy.ToString("F1");
					}

				}

				this.outputText.text = textLabelText;
			}
		}
	}

	public void UserDidPressCloseButton(Button button) {
		button.transform.parent.gameObject.SetActive(false);
	}

	public void UserDidPressOutputButton(Button button) {
		string email = "jazon.burnell@gmail.com";

		string subject = "GPS Data";

		string outputString = "altitude, horizontalAccuracy, latitude, longitude, timestamp, verticalAccuracy\n";

		foreach (LocationInfo locInfo in this.storedLocationInfos) {
//			outputString += locInfo.altitude + "," + 
			outputString += locInfo.altitude + "," + locInfo.horizontalAccuracy + "," + locInfo.latitude + "," + locInfo.longitude + "," + locInfo.timestamp + "," + locInfo.verticalAccuracy + "\n";
		}

//		altitude	Geographical device location altitude.
//		horizontalAccuracy	Horizontal accuracy of the location.
//		latitude	Geographical device location latitude.
//		longitude	Geographical device location latitude.
//		timestamp	Timestamp (in seconds since 1970) when location was last time updated.
//		verticalAccuracy	Vertical accuracy of the location.

		string body = outputString;

		Application.OpenURL ("mailto:" + email + "?subject=" + EscapeURL(subject) + "&body=" + EscapeURL(body));
	}

	private string EscapeURL (string url) {
		return WWW.EscapeURL(url).Replace("+","%20");
	}
}
