using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GridUIController : MonoBehaviour  {

	public GameObject returnToMainMenuGroup;
	public Graphic[] returnToMainElements;
	public GameObject winloseGroup;
	public Image i_panel;
	public Text t_winloseheader;
	public Text t_winlosereason;
	public Text t_timer;
	public Text t_displays;
	public Text t_combo;
	public Text t_timebonus;
	public Button b_menu;
	public Button b_restart;
	public Button b_continue;
	public Text t_score;
	public Text t_scoreheader;
	public Text t_scoreatwin;

	public Text t_shareheader;
	public Button b_sharevk;
	public Button b_sharefb;
	public Button b_sharetw;

	public float timer;
	public bool timeIsOut;
	public int displaysRemain;
	public bool endgame = false;
	public int comboDisplays = 0;
	public float timeBonus;
	private Coroutine timerCor;
	private int allscore;
	private int stagescore;

	void Start()  {
		returnToMainElements = returnToMainMenuGroup.GetComponentsInChildren<Graphic>();

		DataCenter.LoadData();
		int complexity = DataCenter.grid.complexity;
		timerCor = StartCoroutine(Fading.TimeCounter(150f - 5f * complexity, 0f, nowtime => timer = nowtime, isFinished => timeIsOut = isFinished));
		displaysRemain = 10 + complexity * 2;
		t_combo.text = "";
		t_timebonus.text = "";
		allscore = DataCenter.grid.score;
		stagescore = 0;
		t_score.text = string.Format("{0}", allscore);
	}

	void Update()  {
		if(!timeIsOut)
			t_timer.text = string.Format("{0:F1}", timer);
		else
			if(!GridGenerator.checkGrid && !endgame)  {
				Lose();
				endgame = true;
			}

		if(!GridGenerator.checkGrid && displaysRemain <= 0)  {
			if(!endgame)  {
				Win();
				endgame = true;
			}
		}
		else
			t_displays.text = string.Format("Displays to turn on: {0}", Mathf.Clamp(displaysRemain, 0, 1000));
	}

	void Win()  {
		ControlWinLoseGroupElements();
		t_winloseheader.text = "LEVEL PASSED!";
		t_winlosereason.text = "You have enabled enough displays in time, good job!";
		t_scoreatwin.text = string.Format("{0}", allscore);
		b_continue.gameObject.SetActive(true);
		Image i_continue = b_continue.GetComponent<Image>();
		StartCoroutine(Fading.FadeUIGraphic<Image>(0f, 1f, 0.1f, i_continue, 3f));
		DataCenter.grid.complexity++;
		DataCenter.grid.successfulAttempts++;
		DataCenter.grid.score = allscore;
		if(stagescore > DataCenter.grid.record)  {
			DataCenter.grid.record = stagescore;
			ShowShareButtons();
			ShareController.SetScoreToShare(stagescore, "Infinite Grid");
		}
		DataCenter.overallscore += stagescore;
		DataCenter.SaveData();
	}

	void Lose()  {
		ControlWinLoseGroupElements();
		t_winloseheader.text = "LEVEL FAILED!";
		t_winlosereason.text = "Time has run out! Unenabled displays left: "+displaysRemain;
		t_scoreatwin.text = "0";
		b_restart.gameObject.SetActive(true);
		Image i_restart = b_restart.image;
		StartCoroutine(Fading.FadeUIGraphic<Image>(0f, 1f, 0.1f, i_restart, 3f));
		DataCenter.grid.failedAttempts++;
		DataCenter.SaveData();
	}

	void ControlWinLoseGroupElements()  {
		b_menu.interactable = false;
		Image i_menu = b_menu.image;
		StartCoroutine(Fading.FadeUIGraphic<Image>(1f, 0f, 0.05f, i_menu));
		StartCoroutine(Fading.FadeUIGraphic<Text>(1f, 0f, 0.05f, t_timer));
		StartCoroutine(Fading.FadeUIGraphic<Text>(1f, 0f, 0.05f, t_displays));
		StartCoroutine(Fading.FadeUIGraphic<Text>(1f, 0f, 0.05f, t_score));
		winloseGroup.SetActive(true);
		StartCoroutine(Fading.FadeUIGraphic<Image>(0f, 0.6f, 0.05f, i_panel));
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_winloseheader, 1f));
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_winlosereason, 1f));
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_scoreatwin, 1.8f));
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_scoreheader, 1.8f));
	}

	void ShowShareButtons()  {
		StartCoroutine(Fading.ChangeActivenessDelayed(b_sharevk.gameObject, true, 2.4f));
		StartCoroutine(Fading.ChangeActivenessDelayed(b_sharefb.gameObject, true, 2.4f));
		StartCoroutine(Fading.ChangeActivenessDelayed(b_sharetw.gameObject, true, 2.4f));
		Image i_sharevk = b_sharevk.GetComponent<Image>();
		Image i_sharefb = b_sharefb.GetComponent<Image>();
		Image i_sharetw = b_sharetw.GetComponent<Image>();
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_shareheader, 2f));
		StartCoroutine(Fading.FadeUIGraphic<Image>(0f, 1f, 0.1f, i_sharevk, 2.4f));
		StartCoroutine(Fading.FadeUIGraphic<Image>(0f, 1f, 0.1f, i_sharefb, 2.4f));
		StartCoroutine(Fading.FadeUIGraphic<Image>(0f, 1f, 0.1f, i_sharetw, 2.4f));
	}

	public void AddComboDisplay()  {
		comboDisplays++;
		if(comboDisplays > 1)  {
			t_combo.color = new Color(t_combo.color.r, t_combo.color.g, t_combo.color.b, 1f);
			t_combo.text = string.Format("x{0} combo", comboDisplays);
			if(comboDisplays > 5)
				t_combo.text += "!";
		}
	}

	public void ClearComboDisplay()  {
		StartCoroutine(Fading.FadeUIGraphic<Text>(1f, 0f, 0.2f, t_combo, 0.6f));
		comboDisplays = 0;
	}

	public void OnClick_MainMenuGroup()  {
		returnToMainMenuGroup.SetActive(true);
		StartCoroutine(Fading.FadeUIGraphic<Graphic>(0f, 0.6f, 0.15f, returnToMainElements[0]));
		for(int i = 1; i < returnToMainElements.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(0f, 1f, 0.1f, returnToMainElements[i]));
		}
	}

	public void CalculateTimeBonus()  {
		t_timebonus.color = new Color(t_timebonus.color.r, t_timebonus.color.g, t_timebonus.color.b, 1f);
		switch(comboDisplays)  {
		case 2:
			timeBonus = 0.5f;
			break;
		case 3:
			timeBonus = 1.5f;
			break;
		case 4:
			timeBonus = 3f;
			break;
		case 5:
			timeBonus = 6f;
			break;
		}
		if(comboDisplays > 5)
			timeBonus = comboDisplays * 2f;

		StopCoroutine(timerCor);
		timerCor = StartCoroutine(Fading.TimeCounter(timer + timeBonus, 0f, nowtime => timer = nowtime, isFinished => timeIsOut = isFinished));
		t_timebonus.text = string.Format("+{0:F1} bonus", timeBonus);
		if(comboDisplays > 5)
			t_timebonus.text += "!";
		StartCoroutine(Fading.FadeUIGraphic<Text>(1f, 0f, 0.2f, t_timebonus, 1.2f));
	}

	public void AddScore()  {
		int plusscore = (int)(Mathf.Pow(comboDisplays, 1.5f) * 100);
		allscore += plusscore;
		stagescore += plusscore;
		t_score.text = string.Format("{0}", allscore);
	}

	public void OnClick_ReturnToGame()  {
		StartCoroutine(Fading.FadeUIGraphic<Graphic>(0.6f, 0f, 0.15f, returnToMainElements[0]));
		for(int i = 1; i < returnToMainElements.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(1f, 0f, 0.1f, returnToMainElements[i]));
		}
		StartCoroutine(Fading.ChangeActivenessDelayed(returnToMainMenuGroup, false, 0.5f));
	}

	public void OnClick_ToMainMenu()  {
		GameObject.FindObjectOfType<Background>().ChangeColor(Background.NextLevel.MainMenu);
		SceneManager.LoadScene("mainmenu");
	}

	public void OnClick_NextLevel()  {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void OnClick_Restart()  {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

}