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
		}
		else {
			Debug.LogWarning("Unknown elementName: " + elementName);
		}
	}

	public virtual BusDataBaseObject CreateNewObject() {
		return new BusDataStop();
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