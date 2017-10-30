using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

public class BusMapperEditorScripts : MonoBehaviour {

	[MenuItem("BusMapper/Assign GTFS from Folder")]
	public static void AssignGTFSFromFolder() {
		if (Selection.activeObject == null || AssetDatabase.GetAssetPath(Selection.activeObject).Contains("."))	{
			Debug.LogWarning("Select folder of GTFS data");
		}
		else {
			string folderPath = AssetDatabase.GetAssetPath(Selection.activeObject);

			Debug.Log("Folder path is: " + folderPath);

			BusGTFSDataController gtfsController = GameObject.FindObjectOfType<BusGTFSDataController>();

			if (gtfsController == null) {
				Debug.LogWarning("No BusGTFSDataController found");
			}
			else {
				gtfsController.shapesTextData = AssetDatabase.LoadAssetAtPath<TextAsset>(folderPath + "/shapes.txt");
				gtfsController.stopsTextData = AssetDatabase.LoadAssetAtPath<TextAsset>(folderPath + "/stops.txt");
				gtfsController.stopTimesTextData = AssetDatabase.LoadAssetAtPath<TextAsset>(folderPath + "/stop_times.txt");
				gtfsController.tripsTextData = AssetDatabase.LoadAssetAtPath<TextAsset>(folderPath + "/trips.txt");
				gtfsController.routeTextData = AssetDatabase.LoadAssetAtPath<TextAsset>(folderPath + "/routes.txt");
				gtfsController.calendarTextData = AssetDatabase.LoadAssetAtPath<TextAsset>(folderPath + "/calendar.txt");

				Debug.Log("Done");

				Selection.activeGameObject = gtfsController.gameObject;
			}
		}
	}

}
