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
			(valueA.latitude * (1.0 - percentage)) + (valueB.latitude * percentage),
			(valueA.longitude * (1.0 - percentage)) + (valueB.longitude * percentage)
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

public abstract class BusDataBaseObject : System.Object {
	public virtual void ParseAndLoadDataElement(string elementName, string elementValue) { }

	public virtual void ParseAndLoadFinishedForObject() { }

	public virtual void ParseAndLoadFinishedForClass() { }
}

public class BusDataStop : BusDataBaseObject {
	public string name;
	public int id;
	public LatitudeLongitude latitudeLongitude;

	public static int _lowestIdValue = -1;
	public static int _highestIdValue = -1;

	private static BusDataStop[] _busStopsById = new BusDataStop[2000]; // shifted 1000, typical range is 1437 to 2820
	private const int kBusStopByIdIndexOffset = 1000;

	private static void StoreBusDataStop(BusDataStop busStop) {
		if (busStop.id > kBusStopByIdIndexOffset && busStop.id < (_busStopsById.Length + kBusStopByIdIndexOffset)) {
			if (_busStopsById[busStop.id - kBusStopByIdIndexOffset] == null) {
				_busStopsById[busStop.id - kBusStopByIdIndexOffset] = busStop;
			}
			else {
				Debug.LogError("BusDataStop collision (duplicate bus stop ids) for id: " + busStop.id);
			}
		}
		else {
			Debug.LogError("Bus stop id out of range 1000 to 3000: " + busStop.id);
		}
	}

	public static BusDataStop BusStopByStopId(int stopId) {
		int stopIdIndex = stopId - kBusStopByIdIndexOffset;

		if (_busStopsById != null && _busStopsById.Length > stopIdIndex) {
			return _busStopsById[stopIdIndex];
		}
		else {
			Debug.LogError("No bus stop populated for stop id: " + stopId);
			return null;
		}
	}

	public override void ParseAndLoadDataElement(string elementName, string elementValue) {
		if (elementName == "name") {
			this.name = elementValue;
		}
		else if (elementName == "latitude") {
			this.latitudeLongitude.latitude = double.Parse(elementValue);
		}
		else if (elementName == "longitude") {
			this.latitudeLongitude.longitude = double.Parse(elementValue);
		}
		else if (elementName == "id") {
			this.id = int.Parse(elementValue);

			if (_lowestIdValue < 0 || this.id < _lowestIdValue)
				_lowestIdValue = this.id;
			if (_highestIdValue < 0 || this.id > _highestIdValue)
				_highestIdValue = this.id;

			StoreBusDataStop(this);
		}
		else {
			Debug.LogWarning("Unknown elementName: " + elementName);
		}
	}
/*
	<stops>
		<stop>
			<name>BAXTER and NORTHERN LIGHTS</name>
			<id>1437</id>
			<latitude>61.195278</latitude>
			<longitude>-149.763412</longitude>
		</stop>
		<stop>
			<name>LAKE OTIS and TUDOR</name>
			<id>1438</id>
			<latitude>61.180838</latitude>
			<longitude>-149.838184</longitude>
		</stop>
		[...]
	</stops>
*/
}



public class BusRouteStopsDataSet : System.Object {
	public List<int> stopIds;

	public BusRouteStopsDataSet() {
		this.stopIds = new List<int>(150); // whatever the highest sort order is
	}
}

public class BusRouteStopItemData : BusDataBaseObject {
	public int routeNumber;
	public int stopId;
	public int sortOrder;

	private static bool _haveSortedBusRouteStops = false;
	private static Dictionary<int, List<BusRouteStopItemData>> _sortedBusRouteStopsByRouteId = new Dictionary<int, List<BusRouteStopItemData>>();

	public override void ParseAndLoadDataElement(string elementName, string elementValue) {
		if (_haveSortedBusRouteStops) {
			Debug.LogError("BusRouteStopItemData.ParseAndLoadDataElement called after _busRouteSortedStopIds already sorted");
		}

		if (elementName == "route_number") {
			this.routeNumber = int.Parse(elementValue);
		}
		else if (elementName == "stop_id") {
			this.stopId = int.Parse(elementValue);
		}
		else if (elementName == "sort_order") {
			this.sortOrder = int.Parse(elementValue);
		}
		else {
			Debug.LogWarning("Unknown elementName: " + elementName);
		}
	}

	//
	// Data Accessors
	//

	public static int NumberOfRouteStopItemsForRoute(int routeId) {
		if (!_haveSortedBusRouteStops) 
			Debug.LogError("_haveSortedBusRouteStops = false !");

		if (_sortedBusRouteStopsByRouteId.ContainsKey(routeId)) {
			return _sortedBusRouteStopsByRouteId[routeId].Count;
		}
		else {
			Debug.LogError("No stop items found for route: " + routeId);

			return -1;
		}
	}

	public static BusRouteStopItemData RouteStopItemForRouteAtIndex(int routeId, int index) {
		if (!_haveSortedBusRouteStops) 
			Debug.LogError("_haveSortedBusRouteStops = false !");

		if (_sortedBusRouteStopsByRouteId.ContainsKey(routeId)) {
			return _sortedBusRouteStopsByRouteId[routeId][index];
		}
		else {
			Debug.LogError("No stop items found for route: " + routeId);

			return null;
		}
	}

	//
	// Overrides
	//

	private const int kRouteStopItemListLength = 200;

	public override void ParseAndLoadFinishedForObject() { 
		if (this.routeNumber > 0 && this.stopId > 0 && this.sortOrder > 0) {
			if (!_sortedBusRouteStopsByRouteId.ContainsKey(this.routeNumber)) {
				_sortedBusRouteStopsByRouteId.Add(this.routeNumber, new List<BusRouteStopItemData>(kRouteStopItemListLength));
			}
			else {
				if (_sortedBusRouteStopsByRouteId[this.routeNumber].Count > kRouteStopItemListLength) {
					Debug.LogWarning("Should increase capacity for _busRouteSortedStopIds arrays");
				}
			}

			_sortedBusRouteStopsByRouteId[this.routeNumber].Add(this);
		}
		else {
			Debug.LogError("ParseAndLoadFinishedForObject finished without all data");
		}
	}

	public override void ParseAndLoadFinishedForClass() {
		foreach (KeyValuePair<int, List<BusRouteStopItemData>> routeSetPair in _sortedBusRouteStopsByRouteId) {
			Debug.Log("--------- BusRouteStopItemData.ParseAndLoadFinishedForClass route: " + routeSetPair.Key + " coutn: " + routeSetPair.Value.Count);

			routeSetPair.Value.Sort(delegate(BusRouteStopItemData x, BusRouteStopItemData y) {
				if (x.sortOrder == y.sortOrder)
					return 0;
				else
					return (x.sortOrder > y.sortOrder) ? 1 : -1;	
			});
		}

		_haveSortedBusRouteStops = true;
	}

/*
	<routestops>
		<routestop>
			<route_number>1</route_number>
			<stop_id>1437</stop_id>
			<sort_order>43</sort_order>
		</routestop>
		<routestop>
			<route_number>2</route_number>
			<stop_id>1438</stop_id>
			<sort_order>116</sort_order>
		</routestop>
		[...]
	</routestops>
*/
}