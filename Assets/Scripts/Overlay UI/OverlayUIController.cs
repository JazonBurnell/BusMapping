using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayUIController : MonoBehaviour {
	public BusMapUIController busMapUIController;

	public TimelineBarUIController timelineBarUIController;

	public Camera mapCamera;

	public ScrollRect mapMainScrollView;

	public CanvasGroup[] introTutorialNotes;

	void Start () {
		this.AnimateTutorialNotes();
	}

	void LateUpdate () {
		// camera x: 0.255
		// mainscrollview.content.x: -278.2393

		this.mapCamera.transform.localPosition = new Vector3(this.mapMainScrollView.content.anchoredPosition.x / -1091.13451f, this.mapMainScrollView.content.anchoredPosition.y / -1091.13451f, this.mapCamera.transform.localPosition.z);
	}

	//
	// Tutorial Note related
	//

	private void AnimateTutorialNotes() {
		foreach (CanvasGroup canvasGroup in this.introTutorialNotes) {
			canvasGroup.alpha = 0;
			canvasGroup.gameObject.SetActive(true);
		}

		this.StartCoroutine(this.co_AnimateTutorialNotes());
	}

	private IEnumerator co_AnimateTutorialNotes() {
		yield return new WaitForSeconds(1f);

		float percentage = 0;

		while (percentage < 1) {
			percentage = Mathf.Clamp01(percentage + Time.deltaTime * 4f);

			foreach (CanvasGroup canvasGroup in this.introTutorialNotes) {
				canvasGroup.transform.localScale = Vector3.one * Mathf.Lerp(0.75f, 1f, percentage);
				canvasGroup.alpha = percentage;
			}

			yield return null;
		}

		float timeSinceFinishedPresent = 0f;
		while (timeSinceFinishedPresent < 10f) {
			timeSinceFinishedPresent += Time.deltaTime;

			if ((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) || Input.GetMouseButtonDown(0)) {
				timeSinceFinishedPresent += 6f;
			}

			yield return null;
		}

		while (percentage > 0) {
			percentage = Mathf.Clamp01(percentage - Time.deltaTime * 4f);

			foreach (CanvasGroup canvasGroup in this.introTutorialNotes) {
				canvasGroup.transform.localScale = Vector3.one * Mathf.Lerp(0.75f, 1f, percentage);
				canvasGroup.alpha = percentage;

				if (percentage == 0) {
					canvasGroup.gameObject.SetActive(false);
				}
			}

			yield return null;
		}
	}
}
