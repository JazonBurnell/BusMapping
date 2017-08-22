using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineBarUIController : MonoBehaviour {
	public Text currentTimeMainLabel;
	public Text currentTimeShadowLabel;

	public ScrollRect timelineScrollRect;
	public Image timeBarTickMarkTemplate;
	private List<Image> timeBarTickMarks = new List<Image>();

	void Start () {		
		this.timelineScrollRect.vertical = false;

		for (int i = 0; i < 100; i++) {
			Image newTickMark = Instantiate<Image>(this.timeBarTickMarkTemplate);

			newTickMark.transform.SetParent(this.timeBarTickMarkTemplate.transform.parent, false);

			newTickMark.rectTransform.anchoredPosition = new Vector2(
				this.timeBarTickMarkTemplate.rectTransform.anchoredPosition.x + (i * 400),
				this.timeBarTickMarkTemplate.rectTransform.anchoredPosition.y
			);
		}

		this.timeBarTickMarkTemplate.gameObject.SetActive(false);
	}

	void Update () {
//		Debug.Log("scroll x pos: " + this.timelineScrollRect.normalizedPosition.x);

//		if (this.timelineScrollRect.normalizedPosition.x > 0.2f) {
//			this.timelineScrollRect.normalizedPosition = new Vector2(0.1f, this.timelineScrollRect.normalizedPosition.y);
//		}
	}

	public void SetCurrentTime(System.DateTime dateTime) {
		string dateTimeString = dateTime.ToString("F");

		this.currentTimeMainLabel.text = this.currentTimeShadowLabel.text = dateTimeString;

		this.currentTimeMainLabel.transform.parent.SetAsLastSibling();
	}
}
