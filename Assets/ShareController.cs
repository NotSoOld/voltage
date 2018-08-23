using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShareController : MonoBehaviour {

	public static int scoreToShare;
	public static string modeToShare;

	public void ShareIt (string app) {
		if (app != "ok")
			Sharing.ShareVia (app, "Check out Voltage - new game from NotSoOld Games! Available in Google Play: ");
		else
			Sharing.ShareVia (app, "Hi from Unity!", string.Format("{0};{1}", App.OdnoklassnikiAppId, App.OdnoklassnikiSecretId));
	}

	public void ShareScore(string app)  {
		Sharing.ShareVia(app, string.Format("I scored a new record - {0} pts - in Voltage '{1}' mode. Download and beat me! ", scoreToShare, modeToShare));
	}

	public static void SetScoreToShare(int score, string mode)  {
		scoreToShare = score;
		modeToShare = mode;
	}

}