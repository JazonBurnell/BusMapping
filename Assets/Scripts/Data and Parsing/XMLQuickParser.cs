using System;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class XMLQuickParser : System.Object {
	public XmlDocument xmlDoc { get; private set; }

	public XMLQuickParser(string metaString, string xmlData) {
		this.xmlDoc = new XmlDocument();

		try {
			this.xmlDoc.LoadXml(xmlData); // Load the XML document from the specified file
		}
		catch (Exception e) {
			Debug.LogError("XML parse error: " + e.ToString());
		}
//			foreach (XmlNode node in xmlDoc.FirstChild) {
//				if (node.Name == "TextItem") {
//					string xmlKeyValue = null; // Text
//					string xmlTextValue = null; // Replacement
//					string xmlDescriptionValue = null;
//
//					foreach (XmlNode subNode in node) {
//						if (subNode.Name == "Text")

	}

//	public int NumberOfNodesAtDepthFromRoots(int depth = 0) {
//		while (
//	}
}
