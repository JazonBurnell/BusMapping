using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayUIController : MonoBehaviour {
	public BusMapUIController busMapUIController;

	public TimelineBarUIController timelineBarUIController;

	public Camera mapCamera;

	public ScrollRect mapMainScrollView;

	void LateUpdate () {
		// camera x: 0.255
		// mainscrollview.content.x: -278.2393

		this.mapCamera.transform.localPosition = new Vector3(this.mapMainScrollView.content.anchoredPosition.x / -1091.13451f, this.mapMainScrollView.content.anchoredPosition.y / -1091.13451f, this.mapCamera.transform.localPosition.z);
	}
}
