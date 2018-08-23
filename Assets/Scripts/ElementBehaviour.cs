using UnityEngine;
using System.Collections;

public class ElementBehaviour : MonoBehaviour  {

	public bool isPower = false;
	public bool isDisplay = false;
	public Sprite powerOn;
	public Sprite displayOn;
	public SpriteRenderer lightningSprite;

	private Vector3 screenTouchPos;
	private Vector3 touchPos;
	private bool isRotating = false;

	public Vector3[] connectionsPts;

	public static ElementBehaviour[] elements;
	private static bool[] elementCanBeUsed;
	public static SpriteRenderer[] voltagebuttons;

	public GameObject electron;
	public WinLoseManager winlose;
	public UIController ui;

	public static int elementsUsed;

	void Update()  {
		#if !UNITY_EDITOR
		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !ui.returnToMainMenuGroup.activeInHierarchy)  {			//response to a touch
			touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
			screenTouchPos = new Vector3(touchPos.x, touchPos.y);
			bool isFreeFromHovering = true;
			for(int i = 0; i < 3; i++)  {
				if(voltagebuttons[i].bounds.Contains(screenTouchPos))  {
					isFreeFromHovering = false;
					break;
				}
			}
			if(isFreeFromHovering && this.GetComponent<SpriteRenderer>().bounds.Contains(screenTouchPos) && !isRotating)  {	//just use pixel-to-world coordinates and rotate element if there's any at this point
				StartCoroutine(Rotating());
				isRotating = true;
			}
		}
		#else
		if(Input.GetMouseButtonDown(0) && !ui.returnToMainMenuGroup.activeInHierarchy)  {													//response to a click
			Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			Vector3 screenclickpos = new Vector3(clickPos.x, clickPos.y);
			bool isFreeFromHovering = true;
			for(int i = 0; i < 3; i++)  {
				if(voltagebuttons[i].bounds.Contains(screenclickpos))  {
					isFreeFromHovering = false;
					break;
				}
			}
			if(isFreeFromHovering && this.GetComponent<SpriteRenderer>().bounds.Contains(screenclickpos) && !isRotating)  {	//just use pixel-to-world coordinates and rotate element if there's any at this point
				StartCoroutine(Rotating());
				isRotating = true;
			}
		}
		#endif
	}

	IEnumerator Rotating()  {
		int i = 0;
		while(i < 9)  {
			this.transform.Rotate(0f, 0f, 10f, Space.Self);
			i++;
			yield return new WaitForFixedUpdate();
		}
		isRotating = false;
		FindConnectionPoints();
	}

	public void FindConnectionPoints()  {
		Transform[] connectiontransforms = this.GetComponentsInChildren<Transform>();
		connectionsPts = new Vector3[connectiontransforms.Length - 1];
		for(int i = 1; i < connectiontransforms.Length; i++)  {
			connectionsPts[i-1] = connectiontransforms[i].position;
		}
	}

	public static void FindElements()  {
		elements = GameObject.FindObjectsOfType<ElementBehaviour>();
		elementCanBeUsed = new bool[elements.Length];
		for(int i = 0; i < elementCanBeUsed.Length; i++)  {
			elementCanBeUsed[i] = true;
		}
	}

	public static void FindPowerElements()  {
		PowerElement[] bs = GameObject.FindObjectsOfType<PowerElement>();
		voltagebuttons = new SpriteRenderer[3];
		for(int i = 0; i < 3; i++)  {
			voltagebuttons[i] = bs[i].GetComponent<SpriteRenderer>();
		}
	}

	public void CheckForCurcuit(Vector3 lastPt)  {
		bool pathWasFound = false;
		for(int i = 0; i < elements.Length; i++)  {			//for every element in level
			if(elements[i].GetInstanceID() != this.GetInstanceID() && elementCanBeUsed[i])  {		//if this element wasn't used and it is not the same element that has this script
				for(int j = 0; j < elements[i].connectionsPts.Length; j++)  {			//we need to check all connection points of that element
					for(int k = 0; k < connectionsPts.Length; k++)  {					//and compare with points of element that has this script
						if(UniMath.ApproximatelyEqual(elements[i].connectionsPts[j], connectionsPts[k]))  {		//float equality
							if(!UniMath.ApproximatelyEqual(lastPt, connectionsPts[k]))  {	//check if we're not "going backwards" to the same point that we found before
								if(elements[i].isDisplay)  {		//if it is a display, turn it on
									elements[i].GetComponent<SpriteRenderer>().sprite = displayOn;
									winlose.EnableDisplay();
									StartCoroutine(elements[i].MoveElectron(lastPt, elements[i].connectionsPts[j], null, this.transform.position));
									this.enabled = false;
									pathWasFound = true;
								}
								else if(elements[i].isPower)  {
									elements[i].GetComponent<SpriteRenderer>().sprite = powerOn;
									StartCoroutine(elements[i].MoveElectron(lastPt, elements[i].connectionsPts[j], null, this.transform.position));
									this.enabled = false;
									pathWasFound = true;
								}
								else  {								//continue search
									elementCanBeUsed[i] = false;	//we can't use founded element no more (to prevent short curcuits (and stack overflows :D))
									StartCoroutine(elements[i].MoveElectron(lastPt, elements[i].connectionsPts[j], elements[i], this.transform.position));
									this.enabled = false;
									pathWasFound = true;
								}
							}
						}
					}
				}
			}
		}
		if(!pathWasFound)  {
			if(UniMath.ApproximatelyEqual(lastPt, connectionsPts[0]))
				StartCoroutine(MoveElectron(lastPt, connectionsPts[1], null, this.transform.position));
			else
				StartCoroutine(MoveElectron(lastPt, connectionsPts[0], null, this.transform.position));
		}
	}

	IEnumerator MoveElectron(Vector3 start, Vector3 end, ElementBehaviour element, Vector3 center)  {
		GameObject newElectron = (GameObject)Instantiate(electron, start, Quaternion.identity);
		WinLoseManager.nowtime = 0.4f;
		elementsUsed++;
		int i = 1;
		if(!UniMath.ApproximatelyEqual(start.x, end.x) && !UniMath.ApproximatelyEqual(start.y, end.y))  {
			while(newElectron.transform.position != center)  {
				newElectron.transform.position = Vector3.Lerp(start, center, 0.2f*i);
				i++;
				yield return new WaitForFixedUpdate();
			}
			i = 1;
			while(newElectron.transform.position != end)  {
				newElectron.transform.position = Vector3.Lerp(center, end, 0.2f*i);
				i++;
				yield return new WaitForFixedUpdate();
			}
		}
		else  {
			while(newElectron.transform.position != end)  {
				newElectron.transform.position = Vector3.Lerp(start, end, 0.1f*i);
				i++;
				yield return new WaitForFixedUpdate();
			}
		}
		if(element != null)		//if we need to check further
			element.CheckForCurcuit(end);

		while(true)  {
			i = 1;
			newElectron.transform.position = start;
			if(!UniMath.ApproximatelyEqual(start.x, end.x) && !UniMath.ApproximatelyEqual(start.y, end.y))  {
				while(newElectron.transform.position != center)  {
					newElectron.transform.position = Vector3.Lerp(start, center, 0.2f*i);
					i++;
					yield return new WaitForFixedUpdate();
				}
				i = 1;
				while(newElectron.transform.position != end)  {
					newElectron.transform.position = Vector3.Lerp(center, end, 0.2f*i);
					i++;
					yield return new WaitForFixedUpdate();
				}
			}
			else  {
				while(newElectron.transform.position != end)  {
					newElectron.transform.position = Vector3.Lerp(start, end, 0.1f*i);
					i++;
					yield return new WaitForFixedUpdate();
				}
			}
		}
	}

}