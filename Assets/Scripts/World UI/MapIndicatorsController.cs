using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIndicatorsController : MonoBehaviour {
	public MeshRenderer referenceCube;

	public LatitudeLongitude latLong;

//	public double latitude = 61.180912;
//	public double longitude = -149.974061;

	private LatitudeLongitude centerPosition = new LatitudeLongitude(61.180943, -149.886106);

	public double scalar1 = 5.69;
	public double scalar2 = 11.79;

	public static MapIndicatorsController instance;

	void Awake () {
		instance = this;
	}

	// Use this for initialization
	void Start () {

		this.latLong = new LatitudeLongitude(61.223283, -149.974061);

		this.AddIndicatorAtLatLong(new LatitudeLongitude(61.223283, -149.974061)); // upper left
	}
	
	// Update is called once per frame
	void Update () {
		LatitudeLongitude relativeLatLongDelta = this.latLong - this.centerPosition;

		Vector3 localPos = new Vector3((float)(relativeLatLongDelta.longitude * this.scalar1), ((float)(relativeLatLongDelta.latitude * this.scalar2)), 0);

		this.referenceCube.transform.localPosition = localPos;
	}

	public Vector3 AddIndicatorAtLatLong(LatitudeLongitude latLongIn, float scale = 1) {
		LatitudeLongitude relativeLatLongDelta = latLongIn - this.centerPosition;

		Vector3 localPos = new Vector3((float)(relativeLatLongDelta.longitude * this.scalar1), ((float)(relativeLatLongDelta.latitude * this.scalar2)), 0);

		if (scale != 0) {
			MeshRenderer newIndicator = Instantiate<MeshRenderer>(this.referenceCube);
			newIndicator.transform.parent = this.referenceCube.transform.parent;

			newIndicator.transform.localPosition = localPos;
			newIndicator.transform.localScale = newIndicator.transform.localScale * scale;

			newIndicator.gameObject.SetActive(true);
		}

		return localPos;

//		return newIndicator;
	}

	public Vector2 MapUnitScalarPositionForLatLong(LatitudeLongitude latLong) {
		LatitudeLongitude relativeLatLongDelta = latLong - this.centerPosition;

		return new Vector2((float)(relativeLatLongDelta.longitude * this.scalar1), ((float)(relativeLatLongDelta.latitude * this.scalar2)));
	}
}

