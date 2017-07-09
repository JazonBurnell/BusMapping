using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BusDataBaseObject : System.Object {
	public virtual void ParseAndLoadDataElement(string elementName, string elementValue) { }
}

public class BusDataStop : BusDataBaseObject {
	public string name;
	public int id;
	public LatitudeLongitude latitudeLongitude;

	public static int _lowestIdValue = -1;
	public static int _highestIdValue = -1;

	private static BusDataStop[] _busStopsById = new BusDataStop[2000]; // shifted 1000, typical range is 1437 to 2820

	private static void StoreBusDataStop(BusDataStop busStop) {
		if (busStop.id > 1000 && busStop.id < (_busStopsById.Length + 1000)) {
			if (_busStopsById[busStop.id - 1000] == null) {
				_busStopsById[busStop.id - 1000] = busStop;
			}
			else {
				Debug.LogError("BusDataStop collision (duplicate bus stop ids) for id: " + busStop.id);
			}
		}
		else {
			Debug.LogError("Bus stop id out of range 1000 to 3000: " + busStop.id);
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

	public override void ParseAndLoadDataElement(string elementName, string elementValue) {
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