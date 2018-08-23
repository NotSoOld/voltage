using UnityEngine;
using System.Collections;

public class GridGenerator : MonoBehaviour  {

	public GameObject[] wires = new GameObject[4];
	public static GridEltBehaviour[,] grid = new GridEltBehaviour[7,10];
	public static GridEltBehaviour[] powers = new GridEltBehaviour[10];
	public static GridEltBehaviour[] displays = new GridEltBehaviour[10];
	public static bool checkGrid = false;

	public GridUIController ui;

	void Start()  {
		GridEltBehaviour[] powersAndDisplays = GameObject.FindObjectsOfType<GridEltBehaviour>();
		int powerscnt = 0;
		int displayscnt = 0;
		for(int i = 0; i < 20; i++)  {
			if(powersAndDisplays[i].isPower)  {
				powers[powerscnt] = powersAndDisplays[i];
				powers[powerscnt].ui = ui;
				powerscnt++;
			}
			if(powersAndDisplays[i].isDisplay)  {
				displays[displayscnt] = powersAndDisplays[i];
				displays[displayscnt].ui = ui;
				displayscnt++;
			}
		}
		for(int i = 0; i < 7; i++)  {
			for(int j = 0; j < 10; j++)  {
				GameObject elt = (GameObject)Instantiate(wires[Random.Range(0, 4)], new Vector3(i-2.5f, j-2), Quaternion.identity);
				elt.transform.Rotate(0f, 0f, 90f * Random.Range(0, 4), Space.Self);
				GridEltBehaviour eltscript = elt.GetComponent<GridEltBehaviour>();
				grid[i,j] = eltscript;
				eltscript.ui = ui;
				StartCoroutine(MoveToPosition(j-4, elt.transform));
			}
		}
		checkGrid = false;
		GridEltBehaviour.FindElements();
	}

	void Update()  {
		if(checkGrid)  {
			if(GameObject.FindGameObjectsWithTag("electron").Length == 0)  {
				CreateNewGridElts();
				checkGrid = false;
				GridEltBehaviour.FindElements();
				if(ui.comboDisplays > 1)
					ui.CalculateTimeBonus();
				ui.AddScore();
				ui.ClearComboDisplay();
			}
		}
	}

	public void CreateNewGridElts()  {
		GridEltBehaviour[,] newgrid = new GridEltBehaviour[7,10];
		for(int i = 0; i < 7; i++)  {		//for each column we check, where are empty spaces and how many new wires we need to add
			int deletedEltsCnt = 0;
			for(int j = 0; j < 10; j++)  {
				if(grid[i,j] == null)  {		//count deleted wires
					deletedEltsCnt++;
				}
				else  {					//else if element is present
					int deletedUnderCnt = 0;
					for(int k = j; k >= 0; k--)  {		//we watch down from it and count deleted
						if(grid[i,k] == null)  {
							deletedUnderCnt++;
						}
					}
					StartCoroutine(MoveToPosition(grid[i,j].transform.position.y - deletedUnderCnt, grid[i,j].transform));		//move it
					newgrid[i,j-deletedUnderCnt] = grid[i,j];		//and remember to reassign it
				}
			}
			for(int j = 0; j < 10; j++)  {		//reassign all present wires in this column
				grid[i,j] = newgrid[i,j];
			}
			for(int j = 0; j < deletedEltsCnt; j++)  {		//and create missing wires
				GameObject elt = (GameObject)Instantiate(wires[Random.Range(0, 4)], new Vector3(i-2.5f, j+7), Quaternion.identity);
				elt.transform.Rotate(0f, 0f, 90f * Random.Range(0, 4), Space.Self);
				GridEltBehaviour eltscript = elt.GetComponent<GridEltBehaviour>();
				grid[i,j+10-deletedEltsCnt] = eltscript;
				eltscript.ui = ui;
				StartCoroutine(MoveToPosition(j+10-deletedEltsCnt-4, elt.transform));
			}
		}
	}

	IEnumerator MoveToPosition(float y, Transform elt)  {
		float curvel = 0f;
		while(!UniMath.ApproximatelyEqual(elt.position.y, y, 0.001f))  {
			elt.position = new Vector3(elt.position.x, Mathf.SmoothDamp(elt.position.y, y, ref curvel, 0.2f));
			yield return new WaitForFixedUpdate();
		}
		elt.GetComponent<GridEltBehaviour>().needToFindConnectionPts = true;
	}

}