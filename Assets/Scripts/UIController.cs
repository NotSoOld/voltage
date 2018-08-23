using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIController : MonoBehaviour  {			

	public bool isClassicMode;
	public WinLoseManager winLoseManager;
	public GameObject winloseGroup;
	public Image i_panel;
	public Text t_winloseheader;
	public Text t_winlosereason;
	public Text t_timerheader;
	public Text t_winlosetimer;
	public Text t_cntdownheader;
	public Text t_winlosecntdown;
	public Text t_scoreheader;
	public Text t_score;
	public Button b_next;
	private Image i_next;
	public Text t_lvlnumber;
	public Button b_restart;
	private Image i_restart;
	public Button b_menu;
	private Image i_menu;

	public Text t_shareheader;
	public Button b_sharevk;
	public Button b_sharefb;
	public Button b_sharetw;

	public Text t_timer;
	public float timer = 0f;
	public bool timerIsGoing = false;

	public Text t_cntdown;
	public float countdown = 5f;
	private bool cntdownIsStarted = false;
	public float savedCntdown = 0f;
	private bool cntdownIsSaved = false;

	public GameObject returnToMainMenuGroup;
	public Graphic[] returnToMainElements;

	void Start()  {
		i_next = b_next.GetComponent<Image>();
		i_menu = b_menu.GetComponent<Image>();
		returnToMainElements = returnToMainMenuGroup.GetComponentsInChildren<Graphic>();
		if(isClassicMode)  {
			i_restart = b_restart.GetComponent<Image>();
			StartCoroutine(EnableRestartButton());
		}
	}

	void Update()  {
		if(!isClassicMode)  {
			if(timerIsGoing)  {
				timer -= Time.deltaTime;
				t_timer.text = string.Format("{0:F1}", timer);
				if(timer <= 0f)  {
					timerIsGoing = false;
					Lose("Timer has run out");
				}
			}

			if(cntdownIsStarted)  {
				if(countdown > 0f)  {
					countdown -= Time.deltaTime;
					t_cntdown.text = string.Format("{0:F1}", countdown);
					if(!cntdownIsSaved && ((winLoseManager.enabledDisplays == winLoseManager.levelDisplays) || (PowerElement.enabledSources == 3)))  {		//winning
						savedCntdown = countdown;
						cntdownIsSaved = true;
						t_winlosecntdown.text = string.Format("{0:F1}", savedCntdown);
						cntdownIsStarted = false;
						StartCoroutine(Fading.FadeUIGraphic<Text>(1f, 0f, 0.07f, t_cntdown));
					}
				}
				else  {
					cntdownIsStarted = false;
					winLoseManager.LoseCounter();
				}
			}
		}
	}

	public void LevelNumber()  {
		if(isClassicMode)
			t_lvlnumber.text = string.Format("{0}", DataCenter.classic.lvlnumber);
		else
			t_lvlnumber.text = string.Format("{0}", DataCenter.timeattack.lvlnumber);
	}

	public void TimeToPlay()  {
		timer = LevelGenerator.elementsCnt * 1.3f;
	}

	public void Countdown()  {
		if(!isClassicMode)  {
			//stop timer
			timerIsGoing = false;
			//and start countdown
			cntdownIsStarted = true;
			t_cntdown.gameObject.SetActive(true);
		}
	}

	public void Win()  {
		ControlWinLoseGroupElements();
		t_winloseheader.text = "YOU WIN!";
		t_winlosereason.text = "all indicators are powered";
		for(int i = 0; i < 8; i++)
			for(int j = 0; j < 8; j++)
				if(LevelGenerator.elements[i,j] != null)
					LevelGenerator.elements[i,j].enabled = false;
	}

	public void Lose(string message)  {
		ControlWinLoseGroupElements();
		t_winloseheader.text = "YOU LOSE!";
		t_winlosereason.text = message;
		if(!isClassicMode)
			t_winlosecntdown.text = "0.0";
		for(int i = 0; i < 8; i++)
			for(int j = 0; j < 8; j++)
				if(LevelGenerator.elements[i,j] != null)
					LevelGenerator.elements[i,j].enabled = false;
	}

	void ControlWinLoseGroupElements()  {
		StartCoroutine(Fading.FadeUIGraphic<Text>(1f, 0f, 0.05f, t_lvlnumber));
		winloseGroup.SetActive(true);
		StartCoroutine(Fading.FadeUIGraphic<Image>(0f, 0.6f, 0.05f, i_panel));
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_winloseheader, 1f));
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_winlosereason, 1f));
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_scoreheader, 1.5f));
		StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_score, 1.5f));
		StartCoroutine(Fading.FadeUIGraphic<Image>(0f, 1f, 0.1f, i_next, 3f));
		b_menu.interactable = false;
		StartCoroutine(Fading.FadeUIGraphic<Image>(1f, 0f, 0.05f, i_menu));
		if(isClassicMode)  {
			b_restart.interactable = false;
			StartCoroutine(Fading.FadeUIGraphic<Image>(1f, 0f, 0.05f, i_restart));
		}
		if(!isClassicMode)  {
			StartCoroutine(Fading.FadeUIGraphic<Text>(1f, 0f, 0.05f, t_timer));
			StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_timerheader, 1.5f));
			StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_winlosetimer, 1.5f));
			StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_cntdownheader, 1.8f));
			StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_winlosecntdown, 1.8f));
			StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_scoreheader, 2.1f));
			StartCoroutine(Fading.FadeUIGraphic<Text>(0f, 1f, 0.1f, t_score, 2.1f));

			t_winlosetimer.text = string.Format("{0:F1}", timer);
		}
		t_score.text = string.Format("{0:D}", winLoseManager.score);
	}

	public void ShowShareButtons()  {
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

	public void FadeButton(SpriteRenderer r)  {
		StartCoroutine(Fading.FadeSprite(1f, 0f, 0.1f, r));
	}

	public void OnClick_MainMenuGroup()  {
		returnToMainMenuGroup.SetActive(true);
		StartCoroutine(Fading.FadeUIGraphic<Graphic>(0f, 0.6f, 0.15f, returnToMainElements[0]));
		for(int i = 1; i < returnToMainElements.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(0f, 1f, 0.1f, returnToMainElements[i]));
		}
	}

	public void OnClick_ReturnToGame()  {
		StartCoroutine(Fading.FadeUIGraphic<Graphic>(0.6f, 0f, 0.15f, returnToMainElements[0]));
		for(int i = 1; i < returnToMainElements.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(1f, 0f, 0.1f, returnToMainElements[i]));
		}
		StartCoroutine(Fading.ChangeActivenessDelayed(returnToMainMenuGroup, false, 0.5f));
	}

	public void OnClick_ToMainMenu()  {
		if(!winloseGroup.activeInHierarchy)  {
			if(isClassicMode)
				DataCenter.classic.lvlnumber--;
			else
				DataCenter.timeattack.lvlnumber--;
			DataCenter.SaveData();
		}
		GameObject.FindObjectOfType<Background>().ChangeColor(Background.NextLevel.MainMenu);
		SceneManager.LoadScene("mainmenu");
	}

	public void OnClick_NextLevel()  {
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	IEnumerator EnableRestartButton()  {
		yield return new WaitForSeconds(5f);
		b_restart.interactable = true;
	}

	public void OnClick_Restart()  {
		if(isClassicMode)
			DataCenter.classic.lvlnumber--;
		else
			DataCenter.timeattack.lvlnumber--;
		DataCenter.SaveData();
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

}