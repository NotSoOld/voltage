using UnityEngine;
using System.Collections;
using System;

public class WinLoseManager : MonoBehaviour  {

	private bool isClassicMode;
	public int levelDisplays;
	public int enabledDisplays = 0;
	public UIController ui;
	public int score;

	private bool won = false;
	private bool lost = false;

	private Coroutine electronTimerCor;
	public static float nowtime;
	public bool noMoreElectrons;

	void Start()  {
		isClassicMode = ui.isClassicMode;
	}

	void Update()  {
		if(!won && enabledDisplays == levelDisplays)  {
			won = true;
			if(!isClassicMode)  {
				DataCenter.timeattack.wins++;
				score = (int)(ui.timer * 200 + ui.savedCntdown * 200 + (3 - PowerElement.enabledSources) * 400);
				DataCenter.timeattack.score += score;
				DataCenter.overallscore += score;
				if(score > DataCenter.timeattack.record)  {
					DataCenter.timeattack.record = score;
					ui.ShowShareButtons();
					ShareController.SetScoreToShare(score, "Connect (classic)");
				}
			}
			else  {
				DataCenter.classic.wins++;
				score = (int)(ElementBehaviour.elementsUsed * 200 + (3 - PowerElement.enabledSources) * 400);
				DataCenter.classic.score += score;
				DataCenter.overallscore += score;
				if(score > DataCenter.classic.record)  {
					DataCenter.classic.record = score;
					ui.ShowShareButtons();
					ShareController.SetScoreToShare(score, "Connect (time attack)");
				}
			}
			ui.Win();
			DataCenter.SaveData();
		}

		if(nowtime > 0f)  {
			nowtime -= Time.deltaTime;
			noMoreElectrons = false;
		}
		else if(nowtime <= 0f)  {
			noMoreElectrons = true;
		}

		if(PowerElement.enabledSources == 3 && enabledDisplays != levelDisplays && noMoreElectrons && !lost)  {
			LoseCounter();
			lost = true;
		}
	}

	public void EnableDisplay()  {
		enabledDisplays++;
	}

	public void LoseCounter()  {
		if(!isClassicMode)
			DataCenter.timeattack.loses++;
		else
			DataCenter.classic.loses++;
		score = 0;
		ui.Lose("unconnected indicators remained: "+(levelDisplays - enabledDisplays));
		DataCenter.SaveData();
	}

	void OnApplicationQuit()  {
		if(ui.isClassicMode)
			DataCenter.classic.lvlnumber--;
		else
			DataCenter.timeattack.lvlnumber--;
		DataCenter.SaveData();
	}

}