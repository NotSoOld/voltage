using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class UI_MainMenu : MonoBehaviour  {

	public GameObject mainMenuGroup;
	public GameObject infoGroup;
	public GameObject howToPlayGroup;

	public GameObject info_connect3classicGroup;
	public GameObject info_connect3timeattackGroup;
	public GameObject info_gridGroup;
	public int curInfoState;

	public GameObject help_connect3classicGroup;
	public GameObject help_connect3timeattackGroup;
	public GameObject help_gridGroup;
	public int curHelpState;

	[System.Serializable]
	public struct InfoTexts  {
		public Text t_lvls;
		public Text t_won;
		public Text t_lost;
		public Text t_score;
		public Text t_record;
	}

	public InfoTexts classic;
	public InfoTexts timeattack;
	public InfoTexts grid;
	public Text t_overallscore;


	public void OnClick_Connect3_TimeAttack()  {
		GameObject.FindObjectOfType<Background>().ChangeColor(Background.NextLevel.Connect3_TimeAttack);
		SceneManager.LoadScene("connect3_timeattack");
	}

	public void OnClick_Connect3_Classic()  {
		GameObject.FindObjectOfType<Background>().ChangeColor(Background.NextLevel.Connect3_Classic);
		SceneManager.LoadScene("connect3_classic");
	}

	public void OnClick_Grid()  {
		GameObject.FindObjectOfType<Background>().ChangeColor(Background.NextLevel.Grid);
		SceneManager.LoadScene("grid");
	} 

	public void OnClick_InfoGroup()  {
		curInfoState = 0;
		ChooseInfoGroup();
		Graphic[] mainMenuItems = mainMenuGroup.GetComponentsInChildren<Graphic>(true);
		for(int i = 0; i < mainMenuItems.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(1f, 0f, 0.15f, mainMenuItems[i], 0.05f * i));		//fade main menu items
		}
		StartCoroutine(Fading.ChangeActivenessDelayed(mainMenuGroup, false, 1.5f));		//disable main menu group after it has been faded
		Button[] mainMenuButtons = mainMenuGroup.GetComponentsInChildren<Button>(true);
		for(int i = 0; i < mainMenuButtons.Length; i++)  {
			mainMenuButtons[i].interactable = false;		//disable these buttons to prevent bugs
		}

		infoGroup.SetActive(true);
		Graphic[] infoItems = infoGroup.GetComponentsInChildren<Graphic>(true);
		for(int i = 0; i < infoItems.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(0f, 1f, 0.1f, infoItems[i], 1.5f));
		}
		Button[] infoButtons = infoGroup.GetComponentsInChildren<Button>(true);
		StartCoroutine(Fading.ChangeInteractivityDelayed(infoButtons, true, 1f));		//enable these buttons delayed to prevent bugs

		DataCenter.LoadData();

		classic.t_lvls.text = string.Format("{0}", DataCenter.classic.lvlnumber);
		classic.t_won.text = string.Format("{0}", DataCenter.classic.wins);
		classic.t_lost.text = string.Format("{0}", DataCenter.classic.loses);
		classic.t_score.text = string.Format("{0}", DataCenter.classic.score);
		classic.t_record.text = string.Format("{0}", DataCenter.classic.record);

		timeattack.t_lvls.text = string.Format("{0}", DataCenter.timeattack.lvlnumber);
		timeattack.t_won.text = string.Format("{0}", DataCenter.timeattack.wins);
		timeattack.t_lost.text = string.Format("{0}", DataCenter.timeattack.loses);
		timeattack.t_score.text = string.Format("{0}", DataCenter.timeattack.score);
		timeattack.t_record.text = string.Format("{0}", DataCenter.timeattack.record);

		grid.t_lvls.text = string.Format("{0}", DataCenter.grid.complexity);
		grid.t_won.text = string.Format("{0}", DataCenter.grid.successfulAttempts);
		grid.t_lost.text = string.Format("{0}", DataCenter.grid.failedAttempts);
		grid.t_score.text = string.Format("{0}", DataCenter.grid.score);
		grid.t_record.text = string.Format("{0}", DataCenter.grid.record);

		t_overallscore.text = string.Format("{0}", DataCenter.overallscore);
	}

	public void OnClick_HowToPlayGroup()  {
		curHelpState = 0;
		ChooseHelpGroup();
		Graphic[] mainMenuItems = mainMenuGroup.GetComponentsInChildren<Graphic>(true);
		for(int i = 0; i < mainMenuItems.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(1f, 0f, 0.15f, mainMenuItems[i], 0.05f * i));
		}
		StartCoroutine(Fading.ChangeActivenessDelayed(mainMenuGroup, false, 1.5f));
		Button[] mainMenuButtons = mainMenuGroup.GetComponentsInChildren<Button>(true);
		for(int i = 0; i < mainMenuButtons.Length; i++)  {
			mainMenuButtons[i].interactable = false;		//disable these buttons to prevent bugs
		}

		howToPlayGroup.SetActive(true);
		Graphic[] howToPlayItems = howToPlayGroup.GetComponentsInChildren<Graphic>(true);
		for(int i = 0; i < howToPlayItems.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(0f, 1f, 0.1f, howToPlayItems[i], 1f + 0.05f * i));
		}
		Button[] howToPlayButtons = howToPlayGroup.GetComponentsInChildren<Button>(true);
		StartCoroutine(Fading.ChangeInteractivityDelayed(howToPlayButtons, true, 1f + 0.05f * howToPlayItems.Length));	//enable these buttons delayed to prevent bugs
	}

	public void OnClick_BackToMainMenu(string group)  {
		if(group.Equals("info"))  {
			Graphic[] infoItems = infoGroup.GetComponentsInChildren<Graphic>(true);
			for(int i = 0; i < infoItems.Length; i++)  {
				StartCoroutine(Fading.FadeUIGraphic<Graphic>(1f, 0f, 0.1f, infoItems[i]));
			}
			StartCoroutine(Fading.ChangeActivenessDelayed(infoGroup, false, 1f));
			Button[] infoButtons = infoGroup.GetComponentsInChildren<Button>(true);
			for(int i = 0; i < infoButtons.Length; i++)  {
				infoButtons[i].interactable = false;		//disable these buttons to prevent bugs
			}
		}
		else if(group.Equals("howtoplay"))  {
			Graphic[] howToPlayItems = howToPlayGroup.GetComponentsInChildren<Graphic>(true);
			for(int i = 0; i < howToPlayItems.Length; i++)  {
				StartCoroutine(Fading.FadeUIGraphic<Graphic>(1f, 0f, 0.1f, howToPlayItems[i], 0.05f * i));
			}
			StartCoroutine(Fading.ChangeActivenessDelayed(howToPlayGroup, false, 1f));
			Button[] howToPlayButtons = howToPlayGroup.GetComponentsInChildren<Button>(true);
			for(int i = 0; i < howToPlayButtons.Length; i++)  {
				howToPlayButtons[i].interactable = false;		//disable these buttons to prevent bugs
			}
		}

		mainMenuGroup.SetActive(true);
		Graphic[] mainMenuItems = mainMenuGroup.GetComponentsInChildren<Graphic>(true);
		float delay = 1f;
		if(group.Equals("info"))
			delay = 0f;
		for(int i = 0; i < mainMenuItems.Length; i++)  {
			StartCoroutine(Fading.FadeUIGraphic<Graphic>(0f, 1f, 0.15f, mainMenuItems[i], delay + 0.05f * i));
		}
		Button[] mainMenuButtons = mainMenuGroup.GetComponentsInChildren<Button>(true);
		StartCoroutine(Fading.ChangeInteractivityDelayed(mainMenuButtons, true, delay + 0.05f * mainMenuItems.Length));		//enable these buttons delayed to prevent bugs
	}

	public void OnClick_NextInfo(int a)  {
		curInfoState += a;
		if(curInfoState > 2)
			curInfoState = 0;
		else if(curInfoState < 0)
			curInfoState = 2;
		ChooseInfoGroup();
	}

	void ChooseInfoGroup()  {
		info_connect3classicGroup.SetActive(false);
		info_connect3timeattackGroup.SetActive(false);
		info_gridGroup.SetActive(false);
		switch(curInfoState)  {
		case 0:
			info_connect3classicGroup.SetActive(true);
			break;
		case 1:
			info_connect3timeattackGroup.SetActive(true);
			break;
		case 2:
			info_gridGroup.SetActive(true);
			break;
		}
	}

	public void OnClick_NextHelp(int a)  {
		curHelpState += a;
		if(curHelpState > 2)
			curHelpState = 0;
		else if(curHelpState < 0)
			curHelpState = 2;
		ChooseHelpGroup();
	}

	void ChooseHelpGroup()  {
		help_connect3classicGroup.SetActive(false);
		help_connect3timeattackGroup.SetActive(false);
		help_gridGroup.SetActive(false);
		switch(curHelpState)  {
			case 0:
				help_connect3classicGroup.SetActive(true);
				break;
			case 1:
				help_connect3timeattackGroup.SetActive(true);
				break;
			case 2:
				help_gridGroup.SetActive(true);
				break;
		}
	}

}