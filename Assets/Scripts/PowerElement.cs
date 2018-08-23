using UnityEngine;
using System.Collections;

public class PowerElement : MonoBehaviour {			//is on "voltage" buttons

	private bool isClassicMode;
	public static ElementBehaviour[] powerSources;
	public static bool[] powerSourceIsEnabled;
	public UIController ui;
	public Sprite powerOn;
	public Sprite lightningOn;
	public static bool countStarted = false;
	public static int enabledSources;
	private Vector3 screenTouchPos;
	private Vector3 touchPos;

	void Start()  {
		isClassicMode = ui.isClassicMode;
		countStarted = false;
		GameObject[] powerSourceGOs = GameObject.FindGameObjectsWithTag("element/power");
		powerSources = new ElementBehaviour[powerSourceGOs.Length];
		powerSourceIsEnabled = new bool[powerSources.Length];
		for(int i = 0; i < powerSources.Length; i++)  {
			powerSources[i] = powerSourceGOs[i].GetComponent<ElementBehaviour>();
			powerSourceIsEnabled[i] = false;
		}
	}

	void OnMouseDrag()  {
		#if !UNITY_EDITOR
		touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
		screenTouchPos = new Vector3(touchPos.x, touchPos.y);
		transform.position = screenTouchPos;
		#else
		Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
		Vector3 screenclickpos = new Vector3(clickPos.x, clickPos.y);
		transform.position = screenclickpos;
		#endif
	}

	public void OnMouseUpAsButton()  {
		if(!ui.winloseGroup.activeInHierarchy)  {
			for(int j = 0; j < powerSources.Length; j++)  {
				if(UniMath.ApproximatelyEqual(transform.position, powerSources[j].transform.position, 0.5f))  {
					if(!powerSourceIsEnabled[j])  {
						powerSourceIsEnabled[j] = true;
						ui.FadeButton(this.GetComponent<SpriteRenderer>());
						EnablePower(j);
						break;
					}
				}
			}
		}
	}

	void EnablePower(int a)  {
		powerSources[a].GetComponent<SpriteRenderer>().sprite = powerOn;
		powerSources[a].GetComponent<ElementBehaviour>().lightningSprite.sprite = lightningOn;
		enabledSources++;

		if(!isClassicMode && !countStarted)  {
			countStarted = true;
			ui.Countdown();
		}
		powerSources[a].CheckForCurcuit(powerSources[a].transform.position);
	}

}