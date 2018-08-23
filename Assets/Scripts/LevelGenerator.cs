using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelGenerator : MonoBehaviour  {

	private UIController ui;
	private WinLoseManager winlose;
	public GameObject powerSource;
	public GameObject powerSourceLightning;
	public GameObject display;
	public GameObject[] wires = new GameObject[6];
	public GameObject b_voltage;
	public Transform canvas;
	public Transform elementsRoot;

	private int buttonOffset = 0;
	private GameObject[] powersources;
	public static ElementBehaviour[,] elements = new ElementBehaviour[8,8];
	public static int elementsCnt = 0;		//overall useful generated elements

	void Start()  {
		DataCenter.LoadData();

		ui = GameObject.FindObjectOfType<UIController>();
		winlose = GameObject.FindObjectOfType<WinLoseManager>();

		ElementBehaviour.elementsUsed = 0;
		PowerElement.enabledSources = 0;
		elementsCnt = 0;

		CreateInitialPowersource();						
		CreateInitialPowersource();
		CreateInitialPowersource();

		for(int i = 0; i < 8; i++)
			for(int j = 0; j < 8; j++)  {
				if(elements[i,j] != null)  {
					elementsCnt++;
					elements[i,j].transform.Rotate(0f, 0f, 90f * Random.Range(0, 4), Space.Self);		//randomly rotate all elements
				}
				else  {
					//generate junk
					GenerateElementAtPosition(i, j);
					elements[i,j].transform.Rotate(0f, 0f, 90f * Random.Range(0, 4), Space.Self);		//randomly rotate all elements
				}
			}
		
		for(int i = 0; i < 8; i++)
			for(int j = 0; j < 8; j++)
				if(elements[i,j] != null)  {
					elements[i,j].FindConnectionPoints();
					elements[i,j].winlose = winlose;
					elements[i,j].ui = ui;
				}

		ElementBehaviour.FindElements();
		ElementBehaviour.FindPowerElements();

		if(ui.isClassicMode)
			DataCenter.classic.lvlnumber++;
		else
			DataCenter.timeattack.lvlnumber++;
		DataCenter.SaveData();
		ui.LevelNumber();
		if(!ui.isClassicMode)  {
			ui.TimeToPlay();
			ui.timerIsGoing = true;
		}
	}

	void GenerateVoltageButton()  {
		GameObject b = (GameObject)Instantiate(b_voltage, Vector3.zero, Quaternion.identity);
		b.transform.position = new Vector3(-3.2f + buttonOffset * 1.5f, -6.5f);
		b.GetComponent<PowerElement>().ui = ui;
		b.name = b.name.Remove(b.name.Length - 7);
		buttonOffset++;
	}

	void CreateInitialPowersource()  {
		bool isPlaced = false;
		while(!isPlaced)  {
			int x = Random.Range(-3, 5);
			int y = Random.Range(-3, 5);
			if(elements[x+3,y+3] == null)  {
				GameObject go = (GameObject)Instantiate(powerSource, new Vector3(x, y), Quaternion.identity);		//instantiate and move to position
				ElementBehaviour elt = go.GetComponent<ElementBehaviour>();
				elements[x+3,y+3] = elt;																			//add to the matrix
				go.transform.Rotate(0f, 0f, 90f * Random.Range(0, 4), Space.Self);
				for(int i = 0; i < 4; i++)  {
					go.transform.Rotate(0f, 0f, 90f, Space.Self);
					elt.FindConnectionPoints();
					if(elt.connectionsPts[0].x > -3.5f && elt.connectionsPts[0].x < 4.5f && elt.connectionsPts[0].y > -3.5f && elt.connectionsPts[0].y < 4.5f)  {	//if we're not out of bounds
						//calculate position for next link
						Vector3 offset = Vector3.zero;
						if(UniMath.ApproximatelyEqual(elt.connectionsPts[0].x, elt.transform.position.x))  {
							if(elt.connectionsPts[0].y > elt.transform.position.y)  
								offset = Vector3.up;		//above the center of current link
							else offset = Vector3.down;		//below the center
						}
						else if(UniMath.ApproximatelyEqual(elt.connectionsPts[0].y, elt.transform.position.y))  {
							if(elt.connectionsPts[0].x > elt.transform.position.x)
								offset = Vector3.right;		//to the right to the center
							else offset = Vector3.left;		//to the left to the center
						}
						if(elements[x+3+(int)offset.x, y+3+(int)offset.y] == null)  {
							isPlaced = true;
							break;
						}
					}
				}
				if(!isPlaced)  {			//if we fail to place our new powersource at given (x, y) coords
					Destroy(go);
					continue;
				}																			
				go.name = go.name.Remove(go.name.Length - 7);														//edit name
				go.transform.SetParent(elementsRoot);																//edit hierachy
				GenerateVoltageButton();																			//make voltage button

				GameObject lightning = (GameObject)Instantiate(powerSourceLightning, go.transform.position, Quaternion.identity);
				elt.lightningSprite = lightning.GetComponent<SpriteRenderer>();

				CreateChainLink(null, elt, 0, 0);
			}
		}
	}

	void CreateChainLink(ElementBehaviour lastLink, ElementBehaviour curLink, int lastIndex, int index)  {
		int eltIndex = 0, newIndex = 0;
		bool isPlaced = false;

		if(!(curLink.connectionsPts[index].x > -3.5f && curLink.connectionsPts[index].x < 4.5f && curLink.connectionsPts[index].y > -3.5f && curLink.connectionsPts[index].y < 4.5f))  {		//if we're out of bounds
			if(UniMath.ApproximatelyEqual((Vector2)curLink.transform.position, new Vector2(-3f, -3f)) || 
			UniMath.ApproximatelyEqual((Vector2)curLink.transform.position, new Vector2(4f, -3f)) ||
			UniMath.ApproximatelyEqual((Vector2)curLink.transform.position, new Vector2(-3f, 4f)) ||
			UniMath.ApproximatelyEqual((Vector2)curLink.transform.position, new Vector2(4f, 4f)))  {		//corner?
				//here we need to REPLACE curLink with indicator
				Vector3 curPos = curLink.transform.position;
				Destroy(curLink.gameObject);
				CreateDisplay(curPos);		
				return;
			}
			else  {
				//here we need to REPLACE curLink with a turn
				Vector3 curPos = curLink.transform.position;
				Destroy(curLink.gameObject);
				curLink = CreateCurrentLinkManually(curPos, 1, lastLink.connectionsPts, lastIndex, out index);
			}
		}		//if we're not out of bounds, just ignore and go further
			
		//Calculate position for new link
		Vector3 offset = Vector3.zero;
		if(UniMath.ApproximatelyEqual(curLink.connectionsPts[index].x, curLink.transform.position.x))  {
			if(curLink.connectionsPts[index].y > curLink.transform.position.y)  
				offset = Vector3.up;		//above the center of current link
			else offset = Vector3.down;		//below the center
		}
		else if(UniMath.ApproximatelyEqual(curLink.connectionsPts[index].y, curLink.transform.position.y))  {
			if(curLink.connectionsPts[index].x > curLink.transform.position.x)
				offset = Vector3.right;		//to the right to the center
			else offset = Vector3.left;		//to the left to the center
		}

		int ix = (int)((curLink.transform.position + offset).x) + 3;
		int iy = (int)((curLink.transform.position + offset).y) + 3;
		if(elements[ix,iy] != null)  {		//if there's already an element
			if(elements[ix,iy].isPower)  {		//is it a powersource?
				//here we need to REPLACE curLink with indicator
				Vector3 curPos = curLink.transform.position;
				Destroy(curLink.gameObject);
				CreateDisplay(curPos);		
				return;
			}
			else  {
				int iix = (int)((curLink.transform.position + offset * 2).x);		//индекс элемента через элемент в том же направлении
				int iiy = (int)((curLink.transform.position + offset * 2).y);		// --||--
				if(iix >= -3 && iix <= 4 && iiy >= -3 && iiy <= 4)  {			//if we're still in bounds
					//here we need to REPLACE curLink with indicator
					Vector3 curPos = curLink.transform.position;
					Destroy(curLink.gameObject);
					CreateDisplay(curPos);		
					return;
				}
				else  {
					//here we need to REPLACE curLink with indicator
					Vector3 curPos = curLink.transform.position;
					Destroy(curLink.gameObject);
					CreateDisplay(curPos);		
					return;
				}
			}
		}
		else  {		//else if the place is free
			//new element will be turn or straight
			eltIndex = Random.Range(0, 2);
		}

		//create new element
		GameObject link = (GameObject)Instantiate(wires[eltIndex], curLink.transform.position + offset, Quaternion.identity);
		ElementBehaviour elt = link.GetComponent<ElementBehaviour>();
		elements[ix,iy] = elt;
		isPlaced = false;
		for(int k = 0; k < 4; k++)  {
			link.transform.Rotate(0f, 0f, 90f, Space.Self);
			elt.FindConnectionPoints();
			for(int i = 0; i < elt.connectionsPts.Length; i++)  {
				if(UniMath.ApproximatelyEqual(elt.connectionsPts[i], curLink.connectionsPts[index]))  {
					isPlaced = true;
					newIndex = 1 - i;
					break;
				}
			}
			if(isPlaced)
				break;
		}
		link.name = link.name.Remove(link.name.Length - 7);														
		link.transform.SetParent(elementsRoot);
		CreateChainLink(curLink, elt, index, newIndex);
	}

	ElementBehaviour CreateCurrentLinkManually(Vector3 curPos, int index, Vector3[] connectPts, int lastIndex, out int newIndex)  {
		newIndex = 0; 		//reminder: it will change in ANY way, just to get rid of error
		GameObject link = (GameObject)Instantiate(wires[1], curPos, Quaternion.identity);
		ElementBehaviour elt = link.GetComponent<ElementBehaviour>();
		elements[(int)(curPos.x) + 3, (int)(curPos.y) + 3] = elt;
		bool isPlaced = false;
		for(int k = 0; k < 4; k++)  {
			link.transform.Rotate(0f, 0f, 90f, Space.Self);
			elt.FindConnectionPoints();
			for(int i = 0; i < elt.connectionsPts.Length; i++)  {
				if(UniMath.ApproximatelyEqual(elt.connectionsPts[i], connectPts[lastIndex]))  {
					if(elt.connectionsPts[1-i].x > -3.5f && elt.connectionsPts[1-i].x < 4.5f && elt.connectionsPts[1-i].y > -3.5f && elt.connectionsPts[1-i].y < 4.5f)  {	//if we're not out of bounds
						isPlaced = true;
						newIndex = 1 - i;
						break;
					}
				}
			}
			if(isPlaced)
				break;
		}
		link.name = link.name.Remove(link.name.Length - 7);														
		link.transform.SetParent(elementsRoot);
		return elt;
	}

	void CreateDisplay(Vector3 curPos)  {
		GameObject disp = (GameObject)Instantiate(display, curPos, Quaternion.identity);
		ElementBehaviour elt = disp.GetComponent<ElementBehaviour>();
		elements[(int)(curPos.x) + 3, (int)(curPos.y) + 3] = elt;
		disp.name = disp.name.Remove(disp.name.Length - 7);														
		disp.transform.SetParent(elementsRoot);
		winlose.levelDisplays++;
	}

	void GenerateElementAtPosition(int i, int j)  {
		GameObject go = (GameObject)Instantiate(wires[Random.Range(0, 4)], new Vector3(i - 3, j - 3), Quaternion.identity);
		ElementBehaviour elt = go.GetComponent<ElementBehaviour>();
		elements[i,j] = elt;
		go.name = go.name.Remove(go.name.Length - 7);
		go.transform.SetParent(elementsRoot);
	}

}