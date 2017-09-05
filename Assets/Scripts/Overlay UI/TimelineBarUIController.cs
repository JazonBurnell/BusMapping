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

	public RectTransform fastForwardUIPanel;
//	private List<Button> fastForwardSubButtons = new List<Button>();

	public float playSpeedScalar = 1f;

	void Start () {		
		this.timelineScrollRect.vertical = false;

//		this.timelineScrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;
//		this.timelineScrollRect.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

		this.timeBarTickMarkTemplate.gameObject.SetActive(false);

		this.fastForwardUIPanel.gameObject.SetActive(false);

		this.UpdateTickMarks();

//		this.fastForwardSubButtons = new List<Button>(this.fastForwardUIPanel.GetComponentsInChildren<Button>());
	}

	private void UpdateTickMarks() {
		int tickSpacing = 40;

		float uiWidth = this.canvas.GetComponent<CanvasScaler>().referenceResolution.x;

		float sideBuffer = uiWidth * 0;

		float tickMarkSectionWidth = uiWidth + (sideBuffer * 2);

		float scrollLeftEdgePos = (this.timelineScrollRect.content.anchoredPosition.x + (uiWidth / 2));

		float timeBasedOffset = Time.time;

		float scrollLeftEdgeTickPos = scrollLeftEdgePos - (((int)(scrollLeftEdgePos)) % tickSpacing);

		this.tickMarkBaseXPos = -this.timelineScrollRect.content.rect.width/2 - sideBuffer - scrollLeftEdgeTickPos + tickSpacing + timeBasedOffset;

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
		
	private static float EaseInOutSine(float start, float end, float value){
		end -= start;
		return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
	}

	void LateUpdate () {
		this.UpdateTickMarks();
	}

	void Update () {		
		// Close fast forward panel if tapped somewhere outside of it
		if (this.fastForwardUIPanel.gameObject.activeSelf) {
			bool inputDown = (Input.touchCount > 0) ? (Input.GetTouch(0).phase == TouchPhase.Began) : Input.GetMouseButtonDown(0);
			Vector2 inputPos = (Input.touchCount > 0) ? (Input.GetTouch(0).position) : new Vector2(Input.mousePosition.x, Input.mousePosition.y);

			if (inputDown && !RectTransformUtility.RectangleContainsScreenPoint(this.fastForwardUIPanel, inputPos)) {
				this.fastForwardUIPanel.gameObject.SetActive(false);
			}
		}
	}

	public void SetCurrentTime(System.DateTime dateTime) {
//		string dateTimeString = dateTime.ToString("F"); // Sunday, 03 September 2017 14:25:15

		string dateTimeString = dateTime.ToString("dddd, MMMM ") + this.AddOrdinal(dateTime.Day) + dateTime.ToString("  h:mm:ss ") + dateTime.ToString("tt").ToLower();

		dateTimeString = dateTimeString.Insert(dateTimeString.Length - 6, "<size=" + (this.currentTimeMainLabel.fontSize - 6) + ">") + "</size>";
		dateTimeString = dateTimeString.Insert(dateTimeString.Length - (3 + "</size>".Length), "</size><size=" + (this.currentTimeMainLabel.fontSize - 6) + ">");

		this.currentTimeMainLabel.text = this.currentTimeShadowLabel.text = dateTimeString;

		this.currentTimeMainLabel.transform.parent.SetAsLastSibling();
	}
		
	public string AddOrdinal(int num) { // https://stackoverflow.com/questions/20156/is-there-an-easy-way-to-create-ordinals-in-c/20166#20166
		if( num <= 0 ) return num.ToString();

		switch(num % 100) {
			case 11:
			case 12:
			case 13:
			return num + "th";
		}

		switch(num % 10) {
			case 1:
				return num + "st";
			case 2:
				return num + "nd";
			case 3:
				return num + "rd";
			default:
				return num + "th";
		}
	}
		
	//
	// UI stuff
	//

	private const float kDelayToCloseFastForwardMenu = 0.05f;

	private Button activeFastForwardButton;

	public void PressedFastFowardButton(Button button) {
		this.fastForwardUIPanel.gameObject.SetActive(!this.fastForwardUIPanel.gameObject.activeSelf);
	}

	public void PressedFastForwardMenuSubButton(Button button) {
		if (this.activeFastForwardButton == button) {
			this.playSpeedScalar = 1;
			this.activeFastForwardButton = null;

			this.ShowButtonAsActivated(button, false);

//			this.StartCoroutine(this.co_HideFastForwardMenu(kDelayToCloseFastForwardMenu));

			return;
		}

		Text buttonLabel = button.GetComponentInChildren<Text>();

		if (buttonLabel == null) {
			Debug.LogWarning("No text label found in button");
			return;
		}

		if (buttonLabel.text.Substring(0,buttonLabel.text.Length - 2).Contains("Fast Forward ")) {
			int parsedValue;
			if (int.TryParse(buttonLabel.text.Replace("Fast Forward ", "").Replace("x", ""), out parsedValue)) {
				this.playSpeedScalar = parsedValue;
			}
		}
		else {
			Debug.LogWarning("Was expecting button text to contain string Fast Forward");
			return;
		}

		if (this.activeFastForwardButton != null) {
			this.ShowButtonAsActivated(this.activeFastForwardButton, false);
		}

		this.activeFastForwardButton = button;

		this.ShowButtonAsActivated(button, true);

//		this.StartCoroutine(this.co_HideFastForwardMenu(kDelayToCloseFastForwardMenu));
	}

	private void ShowButtonAsActivated(Button button, bool activated) {
		Text buttonLabel = button.GetComponentInChildren<Text>();

		if (buttonLabel == null) {
			Debug.LogWarning("No text label found in button");
			return;
		}

		button.GetComponent<Image>().color = (activated) ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1f);

		buttonLabel.fontStyle = (activated) ? FontStyle.Bold : FontStyle.Normal;
		buttonLabel.fontSize = (activated) ? 23 : 22;
	}

	private IEnumerator co_HideFastForwardMenu(float delay) {
		yield return new WaitForSeconds(delay);

		this.fastForwardUIPanel.gameObject.SetActive(false);
	}
}
