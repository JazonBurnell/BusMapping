using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LatitudeLongitude {
	public double latitude;// = 61.180912;
	public double longitude;// = -149.974061;

	public LatitudeLongitude(double latIn, double longIn) {
		this.latitude = latIn;
		this.longitude = longIn;
	}

	public static LatitudeLongitude Lerp(LatitudeLongitude valueA, LatitudeLongitude valueB, double percentage) {
		return new LatitudeLongitude(
			(valueA.latitude * (1.0 - percentage)) + (valueA.latitude * percentage),
			(valueB.longitude * (1.0 - percentage)) + (valueB.longitude * percentage)
		);
	}

	public static LatitudeLongitude operator +(LatitudeLongitude left, LatitudeLongitude right)
	{
		left.latitude += right.latitude;
		left.longitude += right.longitude;
		return left;
	}

	public static LatitudeLongitude operator -(LatitudeLongitude left, LatitudeLongitude right)
	{
		left.latitude -= right.latitude;
		left.longitude -= right.longitude;
		return left;
	}
}

public class MapIndicatorsController : MonoBehaviour {
	public MeshRenderer referenceCube;

	public LatitudeLongitude latLong;

//	public double latitude = 61.180912;
//	public double longitude = -149.974061;

	private LatitudeLongitude centerPosition = new LatitudeLongitude(61.180943, -149.886106);

	public double scalar1 = 5.69;
	public double scalar2 = 11.79;

	// Use this for initialization
	void Start () {

		this.latLong = new LatitudeLongitude(61.223283, -149.974061);

//		LatitudeLongitude relativeLatLongDelta = this.latLong - this.centerPosition;
//
//		Vector3 localPos = new Vector3((float)(relativeLatLongDelta.latitude / 0.000131), 0, 0);
//
//		this.referenceCube.transform.localPosition = localPos;

		this.AddIndicatorAtLatLong(new LatitudeLongitude(61.223283, -149.974061)); // upper left
	}
	
	// Update is called once per frame
	void Update () {
		LatitudeLongitude relativeLatLongDelta = this.latLong - this.centerPosition;

//		long centerLatInt = (long) (this.centerPosition.latitude * 10000000);
//		long centerLongInt = (long) (this.centerPosition.longitude * 10000000);
//
//		long targetLatInt = (long) (this.latLong.latitude * 10000000);
//		long targetLongInt = (long) (this.latLong.longitude * 10000000);
//
//		long deltaLatInt = targetLatInt - centerLatInt;
//		long deltaLongInt = targetLongInt - centerLongInt;

//		Debug.Log("relativeLatLongDelta is: " + deltaLatInt + ", " + deltaLongInt);

		Vector3 localPos = new Vector3((float)(relativeLatLongDelta.longitude * this.scalar1), ((float)(relativeLatLongDelta.latitude * this.scalar2)), 0);

//		Vector3 localPos = new Vector3((float)(deltaLatInt * this.scalar1), ((float)(deltaLongInt * this.scalar2)), 0);

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
}


/*
 * 


Center:
61.180943, -149.886106


Left side (mid lake)
61.180912, -149.974061


Right side:
61.181074, -149.797944


Upper center: 
61.223283, -149.886003




y, x


xDif: 0.087955

yDif: 


*/