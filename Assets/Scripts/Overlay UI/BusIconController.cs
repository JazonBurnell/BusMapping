using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BusIconController : MonoBehaviour {
	public RectTransform rectTransform;

	public Image backgroundImage;

	public Text mainLabel;
	public Text shadowLabel;

	public int lastFrameUpdated { get; set; }

	// Intended only for debug / Unity visualizing
	public string idString;

	public void SetLatLongPosition(LatitudeLongitude latLong) {
		Vector2 unitScalarPos = MapIndicatorsController.instance.MapUnitScalarPositionForLatLong(latLong);

		this.rectTransform.anchoredPosition = new Vector2(unitScalarPos.x * 1080, unitScalarPos.y * 1080);
	}

	public void SetLabelString(string stringIn) {
		this.mainLabel.text = this.shadowLabel.text = stringIn;
	}
}
