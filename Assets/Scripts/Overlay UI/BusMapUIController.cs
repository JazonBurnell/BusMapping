using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BusMapUIController : MonoBehaviour {
	public BusIconController busIndicatorIconTemplate;

	private Dictionary<string, BusIconController> busIconControllersByIdString = new Dictionary<string, BusIconController>();

	void Start () {
		this.busIndicatorIconTemplate.gameObject.SetActive(false);
	}

	public BusIconController BusIndicatorIconForId(string idString, BusGTFSDataController.RouteInfo routeInfo) {
		if (this.busIconControllersByIdString.ContainsKey(idString)) {
			BusIconController busIcon = this.busIconControllersByIdString[idString];

			busIcon.gameObject.SetActive(true);

			return busIcon;
		}
		else {
//			Debug.Log("Creating bus icon for id: " + idString);

			BusIconController newBusIcon = this.NewBusIndicatorIcon(idString, routeInfo);

			this.busIconControllersByIdString.Add(idString, newBusIcon);

			return newBusIcon;
		}
	}

	private BusIconController NewBusIndicatorIcon(string idString, BusGTFSDataController.RouteInfo routeInfo) {
		BusIconController newBusIndicator = Instantiate<BusIconController>(this.busIndicatorIconTemplate);

		newBusIndicator.transform.SetParent(this.busIndicatorIconTemplate.transform.parent, false);

		newBusIndicator.SetLabelString(routeInfo.routeId);
		newBusIndicator.idString = idString;
		newBusIndicator.backgroundImage.color = Color.Lerp(routeInfo.routeColor, Color.black, 0.0f);
		newBusIndicator.shadowLabel.color = Color.white;// routeInfo.routeTextColor;

		newBusIndicator.name += " - " + idString;

		newBusIndicator.gameObject.SetActive(true);

		return newBusIndicator;
	}

	public void DisableAllIndicatorsNotEqualToFrame(int frameCount) {
		foreach (KeyValuePair<string, BusIconController> pair in this.busIconControllersByIdString) {
			if (pair.Value.lastFrameUpdated != frameCount) {
				pair.Value.gameObject.SetActive(false);
			}
		}
	}
}
