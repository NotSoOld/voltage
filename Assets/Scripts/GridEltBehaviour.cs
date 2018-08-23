using UnityEngine;
using System.Collections;

public class GridEltBehaviour : MonoBehaviour  {

	public bool isPower = false;
	public bool isDisplay = false;
	private Vector3 screenTouchPos;
	private Vector3 touchPos;
	private bool isRotating = false;
	private bool canGivePower = true;

	public Vector3[] connectionsPts;
	public bool needToFindConnectionPts = true;

	public static GridEltBehaviour[] elements = new GridEltBehaviour[90];
	private static bool[] elementCanBeUsed = new bool[90];

	public GameObject electron;
	public Sprite powerOn;
	public Sprite displayOn;
	public Sprite lightningOn;
	public Sprite powerOff;
	public Sprite displayOff;
	public Sprite lightningOff;
	public SpriteRenderer lightningSprite;

	public GridUIController ui;

	void Update()  {
		#if !UNITY_EDITOR
		if(!ui.endgame && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && !ui.returnToMainMenuGroup.activeInHierarchy)  {			//response to a touch
			touchPos = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
			screenTouchPos = new Vector3(touchPos.x, touchPos.y);
			if(this.GetComponent<SpriteRenderer>().bounds.Contains(screenTouchPos) && !isRotating)  {	//just use pixel-to-world coordinates and rotate element if there's any at this point
				if(!isPower && !isDisplay)  {
					StartCoroutine(Rotating());
					isRotating = true;
				}
				else if(isPower && canGivePower)  {
					GetComponent<SpriteRenderer>().sprite = powerOn;
					canGivePower = false;
					StartCoroutine(PowersourcesCooldown());
					lightningSprite.sprite = lightningOn;
					CheckForCurcuit(this.transform.position);
				}
			}
		}
		#else
		if(!ui.endgame && Input.GetMouseButtonDown(0) && !ui.returnToMainMenuGroup.activeInHierarchy)  {		//response to a click
			Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); 
			Vector3 screenclickpos = new Vector3(clickPos.x, clickPos.y);
			if(this.GetComponent<SpriteRenderer>().bounds.Contains(screenclickpos) && !isRotating)  {	//just use pixel-to-world coordinates and rotate element if there's any at this point
				if(!isPower && !isDisplay)  {
					StartCoroutine(Rotating());
					isRotating = true;
				}
				else if(isPower && canGivePower)  {
					GetComponent<SpriteRenderer>().sprite = powerOn;
					lightningSprite.sprite = lightningOn;
					for(int i = 0; i < 10; i++)  {
						GridGenerator.powers[i].canGivePower = false;
					}
					StartCoroutine(PowersourcesCooldown());
					CheckForCurcuit(this.transform.position);
				}
			}
		}
		#endif

		if(needToFindConnectionPts)  {
			FindConnectionPoints();
			needToFindConnectionPts = false;
		}
	}

	IEnumerator Rotating()  {
		int i = 0;
		while(i < 9)  {
			this.transform.Rotate(0f, 0f, 10f, Space.Self);
			i++;
			yield return new WaitForFixedUpdate();
		}
		isRotating = false;
		needToFindConnectionPts = true;
	}

	public void FindConnectionPoints()  {
		Transform[] connectiontransforms = this.GetComponentsInChildren<Transform>();
		connectionsPts = new Vector3[connectiontransforms.Length - 1];
		for(int i = 1; i < connectiontransforms.Length; i++)  {
			connectionsPts[i-1] = connectiontransforms[i].position;
		}
	}

	public static void FindElements()  {
		for(int i = 0; i < 10; i++)  {
			elements[i] = GridGenerator.powers[i];
			elements[i+10] = GridGenerator.displays[i];
		}
		for(int i = 0; i < 7; i++)  {
			for(int j = 0; j < 10; j++)  {
				elements[(i+2)*10 + j] = GridGenerator.grid[i,j];
			}
		}
		for(int i = 0; i < 90; i++)  {
			elementCanBeUsed[i] = true;
		}
	}

	public void CheckForCurcuit(Vector3 lastPt)  {
		bool pathWasFound = false;
		for(int i = 0; i < elements.Length; i++)  {			//for every element in level
			if(elements[i].GetInstanceID() != this.GetInstanceID() && elementCanBeUsed[i])  {		//if this element wasn't used and it is not the same element that has this script
				for(int j = 0; j < elements[i].connectionsPts.Length; j++)  {			//we need to check all connection points of that element
					for(int k = 0; k < connectionsPts.Length; k++)  {					//and compare with points of element that has this script
						if(UniMath.ApproximatelyEqual(elements[i].connectionsPts[j], connectionsPts[k], 0.1f))  {		//float equality
							if(!UniMath.ApproximatelyEqual(lastPt, connectionsPts[k], 0.1f))  {	//check if we're not "going backwards" to the same point that we found before
								if(elements[i].isDisplay)  {		//if it is a display, turn it on
									elements[i].GetComponent<SpriteRenderer>().sprite = displayOn;
									ui.displaysRemain--;
									ui.AddComboDisplay();
									StartCoroutine(DisplayReplace(elements[i].transform));
									StartCoroutine(elements[i].MoveElectron(lastPt, elements[i].connectionsPts[j], null, this.transform.position));
									pathWasFound = true;
								}
								else if(elements[i].isPower)  {
									elements[i].GetComponent<SpriteRenderer>().sprite = powerOn;
									StartCoroutine(elements[i].PowersourcesCooldown());
									StartCoroutine(elements[i].MoveElectron(lastPt, elements[i].connectionsPts[j], null, this.transform.position));
									pathWasFound = true;
								}
								else  {								//continue search
									elementCanBeUsed[i] = false;	//we can't use founded element no more (to prevent short curcuits (and stack overflows :D))
									StartCoroutine(elements[i].MoveElectron(lastPt, elements[i].connectionsPts[j], elements[i], this.transform.position));
									if(!elements[i].isPower)  {
										StartCoroutine(Fading.FadeSprite(1f, 0f, 0.15f, elements[i].GetComponent<SpriteRenderer>(), 0.5f, false));
										GridGenerator.grid[(int)Mathf.Round(elements[i].transform.position.x + 2.5f), (int)Mathf.Round(elements[i].transform.position.y + 4f)] = null;
									}
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

	IEnumerator MoveElectron(Vector3 start, Vector3 end, GridEltBehaviour element, Vector3 center)  {
		GameObject newElectron = (GameObject)Instantiate(electron, start, Quaternion.identity);
		GridGenerator.checkGrid = true;
		//elementsUsed++;
		int i = 1;
		if(!UniMath.ApproximatelyEqual(start.x, end.x) && !UniMath.ApproximatelyEqual(start.y, end.y))  {
			while(newElectron.transform.position != center)  {
				newElectron.transform.position = Vector3.Lerp(start, center, 0.4f*i);
				i++;
				yield return new WaitForFixedUpdate();
			}
			i = 1;
			while(newElectron.transform.position != end)  {
				newElectron.transform.position = Vector3.Lerp(center, end, 0.4f*i);
				i++;
				yield return new WaitForFixedUpdate();
			}
		}
		else  {
			while(newElectron.transform.position != end)  {
				newElectron.transform.position = Vector3.Lerp(start, end, 0.2f*i);
				i++;
				yield return new WaitForFixedUpdate();
			}
		}
		if(element != null)
			element.CheckForCurcuit(end);

		Destroy(newElectron);
	}

	IEnumerator PowersourcesCooldown()  {
		yield return new WaitForSeconds(0.1f);		//wait a little after enabling a powersource (because 'checkGrid' will still be "false" at this moment)
		yield return new WaitUntil(() => GridGenerator.checkGrid == false);		//so, this will execute only when all electrons will have died
		for(int i = 0; i < 10; i++)  {
			GridGenerator.powers[i].canGivePower = true;
		}
		GetComponent<SpriteRenderer>().sprite = powerOff;
		lightningSprite.sprite = lightningOff;
	}

	IEnumerator DisplayReplace(Transform disp)  {
		Vector3 oldPos = disp.position;
		yield return new WaitForSeconds(0.3f);
		StartCoroutine(Fading.FadeSprite(1f, 0f, 0.15f, disp.GetComponent<SpriteRenderer>()));
		while(!UniMath.ApproximatelyEqual(disp.position.x, 5.5f, 0.1f))  {
			disp.Translate(Vector3.right * 0.1f, Space.World);
			yield return new WaitForFixedUpdate();
		}
		yield return new WaitUntil(() => GridGenerator.checkGrid == false);		//this will execute only when all electrons will have died

		disp.GetComponent<SpriteRenderer>().color = Color.white;
		disp.GetComponent<SpriteRenderer>().sprite = displayOff;
		float curvel = 0f;
		while(!UniMath.ApproximatelyEqual(disp.position.x, 4.5f, 0.001f))  {
			disp.position = new Vector3(Mathf.SmoothDamp(disp.position.x, oldPos.x, ref curvel, 0.2f), disp.position.y);
			yield return new WaitForFixedUpdate();
		}
	}

}