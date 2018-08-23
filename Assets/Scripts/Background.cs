using UnityEngine;
using System.Collections;

public class Background : MonoBehaviour {

	public Gradient bgGrad = new Gradient();
	private SpriteRenderer bg;
	public Color mainMenuCol;
	public Color connect3classicCol;
	public Color connect3timeattackCol;
	public Color gridCol;

	public enum NextLevel  {
		MainMenu,
		Connect3_Classic,
		Connect3_TimeAttack,
		Grid
	}

	void Awake()  {
		DontDestroyOnLoad(this.gameObject);
		if(GameObject.FindGameObjectsWithTag("background").Length > 1)  {
			Destroy(this.gameObject);
		}
		bg = this.GetComponent<SpriteRenderer>();
	}

	void SetupGradient(NextLevel nextlvl)  {
		float a = 1f;
		GradientColorKey[] colkeys = new GradientColorKey[2];
		colkeys[0] = new GradientColorKey(bg.color, 0f);
		switch(nextlvl)  {
			case NextLevel.MainMenu:
				colkeys[1] = new GradientColorKey(mainMenuCol, 1f);
				break;
			case NextLevel.Connect3_Classic:
				colkeys[1] = new GradientColorKey(connect3classicCol, 1f);
				break;
			case NextLevel.Connect3_TimeAttack:
				colkeys[1] = new GradientColorKey(connect3timeattackCol, 1f);
				break;
			case NextLevel.Grid:
				colkeys[1] = new GradientColorKey(gridCol, 1f);
				break;
		}
		GradientAlphaKey[] alphakeys = new GradientAlphaKey[2];
		alphakeys[0] = new GradientAlphaKey(a, 0f);
		alphakeys[1] = new GradientAlphaKey(a, 1f);
		bgGrad.SetKeys(colkeys, alphakeys);
	}

	public void ChangeColor(NextLevel nextLvl)  {
		SetupGradient(nextLvl);
		StartCoroutine(ChangeCol());
	}

	IEnumerator ChangeCol()  {
		for(int i = 0; i <= 20; i++)  {
			bg.color = bgGrad.Evaluate(0.05f * i);
			yield return new WaitForFixedUpdate();
		}
	}

}