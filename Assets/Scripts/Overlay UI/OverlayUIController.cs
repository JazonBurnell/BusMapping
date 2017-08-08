using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayUIController : MonoBehaviour {
	public BusMapUIController busMapUIController;

	public Text currentTimeMainLabel;
	public Text currentTimeShadowLabel;

	public void SetCurrentTime(System.DateTime dateTime) {
		string dateTimeString = dateTime.ToString("F");

		this.currentTimeMainLabel.text = this.currentTimeShadowLabel.text = dateTimeString;

		this.currentTimeMainLabel.transform.parent.SetAsLastSibling();
	}
}
