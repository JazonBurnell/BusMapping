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

	private float tickMarkBaseXPos;

	public float playSpeedScalar = 10f;

	void Start () {		
		this.timelineScrollRect.vertical = false;

//		this.timelineScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
//		this.timelineScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

		this.timeBarTickMarkTemplate.gameObject.SetActive(false);

		this.UpdateTickMarks();
	}

	private void UpdateTickMarks() {
		int tickSpacing = 40;

//		float screenRatio = (((float)Screen.width) / Screen.height) / (16f/9);

		float uiWidth = 1920f;

//		float sideBuffer = uiWidth * 0.05f;
		float sideBuffer = uiWidth * 0;

		float tickMarkSectionWidth = uiWidth + (sideBuffer * 2);

		float scrollLeftEdgePos = (this.timelineScrollRect.content.anchoredPosition.x + (uiWidth / 2));

		float scrollLeftEdgeTickPos = scrollLeftEdgePos - (((int)(scrollLeftEdgePos)) % tickSpacing);

		this.tickMarkBaseXPos = -this.timelineScrollRect.content.rect.width/2 - sideBuffer - scrollLeftEdgeTickPos + tickSpacing;

		int numberOfTicks = (int)(tickMarkSectionWidth / tickSpacing);

		int ticksModified = 0;
		for (int i = 0; i < numberOfTicks; i++) {
			Image tickMark = null;

			if (this.timeBarTickMarks.Count > i) {
				tickMark = this.timeBarTickMarks[i];
			}
			else {
				tickMark = Instantiate<Image>(this.timeBarTickMarkTemplate);

				tickMark.transform.SetParent(this.timeBarTickMarkTemplate.transform.parent, false);

				this.timeBarTickMarks.Add(tickMark);
			}

			tickMark.gameObject.SetActive(true);

			float newXPos = this.tickMarkBaseXPos + (i * tickSpacing);

			tickMark.rectTransform.anchoredPosition = new Vector2(
				newXPos,
				this.timeBarTickMarkTemplate.rectTransform.anchoredPosition.y
			);

//			float percentagePos = tickMark.transform.position.x / uiWidth;
			float percentagePos = tickMark.transform.position.x / Screen.width;

			if (percentagePos <= 0.5f) {
				percentagePos = percentagePos * 2;
			}
			else {
				percentagePos = ((1 - percentagePos) * 2);
			}

			percentagePos = EaseInOutSine(0, 1, percentagePos);

			tickMark.color = new Color(1, 1, 1, percentagePos);

			tickMark.transform.localScale = new Vector3(1, Mathf.Pow(percentagePos, 1f/4), 1);

			ticksModified++;
		}

		for (int i = ticksModified; i < this.timeBarTickMarks.Count; i++) {
			this.timeBarTickMarks[i].gameObject.SetActive(false);
		}
	}

	private static float EaseInOutQuad(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value + start;
		value--;
		return -end / 2 * (value * (value - 2) - 1) + start;
	}
	private static float EaseInOutCirc(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
		value -= 2;
		return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
	}
	private static float EaseInOutExpo(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
		value--;
		return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
	}
	private static float EaseInOutQuint(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value * value * value + start;
		value -= 2;
		return end / 2 * (value * value * value * value * value + 2) + start;
	}
	private static float EaseInOutCubic(float start, float end, float value){
		value /= .5f;
		end -= start;
		if (value < 1) return end / 2 * value * value * value + start;
		value -= 2;
		return end / 2 * (value * value * value + 2) + start;
	}
	private static float EaseInOutSine(float start, float end, float value){
		end -= start;
		return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
	}

	void LateUpdate () {
		this.UpdateTickMarks();
	}

	void Update () {		
//		if (Time.frameCount % 30 == 0) {
//			if (this.timeBarTickMarks.Count > 0) {
////				Debug.Log("First image pos: " + (this.timeBarTickMarks[0].GetComponent<RectTransform>().anchoredPosition.x + this.timelineScrollRect.content.rect.width/2 + this.timelineScrollRect.content.anchoredPosition.x + (Screen.width / this.canvas.GetComponent<CanvasScaler>().scaleFactor)/2));
////				Debug.Log("First image pos: " + (this.timeBarTickMarks[0].GetComponent<RectTransform>().position.x));
////				Debug.Log("Last image pos: " + (this.timeBarTickMarks[this.timeBarTickMarks.Count - 1].GetComponent<RectTransform>().position.x));
//
//				float screenRatio = (((float)Screen.width) / Screen.height) / (16f/9);
//
//				float uiWidth = (this.canvas.GetComponent<CanvasScaler>().referenceResolution.x * screenRatio);
//
//				float scrollLeftEdgePos = this.timelineScrollRect.content.anchoredPosition.x + (uiWidth / 2);
//				Debug.Log("scrollLeftPos: " + scrollLeftEdgePos);
//			}
//		}

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
