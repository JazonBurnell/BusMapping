using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimelineBarUIController : MonoBehaviour {
	public Canvas canvas;

	public Text currentTimeMainLabel;
	public Text currentTimeShadowLabel;

	public ScrollRect timelineScrollRect;
	public Image timeBarTickMarkTemplate;
	private List<Image> timeBarTickMarks = new List<Image>();

	void Start () {		
		this.timelineScrollRect.vertical = false;

//		this.timelineScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
//		this.timelineScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

		float tickSpacing = 40;



		for (int i = 0; i < 100; i++) {
			Image newTickMark = Instantiate<Image>(this.timeBarTickMarkTemplate);

			newTickMark.transform.SetParent(this.timeBarTickMarkTemplate.transform.parent, false);

			newTickMark.rectTransform.anchoredPosition = new Vector2(
				-this.timelineScrollRect.content.rect.width/2 + (i * tickSpacing),
				this.timeBarTickMarkTemplate.rectTransform.anchoredPosition.y
			);

			this.timeBarTickMarks.Add(newTickMark);
		}

		this.timeBarTickMarkTemplate.gameObject.SetActive(false);
	}

	void Update () {
		if (Time.frameCount % 30 == 0) {
			if (this.timeBarTickMarks.Count > 0)
//				Debug.Log("First image pos: " + (this.timeBarTickMarks[0].GetComponent<RectTransform>().anchoredPosition.x + this.timelineScrollRect.content.rect.width/2 + this.timelineScrollRect.content.anchoredPosition.x + (Screen.width / this.canvas.GetComponent<CanvasScaler>().scaleFactor)/2));
				Debug.Log("First image pos: " + (this.timeBarTickMarks[0].GetComponent<RectTransform>().position.x));
				Debug.Log("Last image pos: " + (this.timeBarTickMarks[this.timeBarTickMarks.Count - 1].GetComponent<RectTransform>().position.x));
		}

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
