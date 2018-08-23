using UnityEngine;
using System.Collections;

public class Fading  {

	public static IEnumerator FadeUIGraphic<T>(float start, float end, float speed, T obj, float initdelay = 0f) where T : UnityEngine.UI.Graphic  {
		yield return new WaitForSeconds(initdelay);
		if(start < end)  {		//decrease transparency
			while(obj.color.a < end)  {
				obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, obj.color.a + Mathf.Lerp(start, end, speed));
				yield return new WaitForFixedUpdate();
			}
		}
		else  {					//increase transparency
			while(obj.color.a > end)  {
				obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, obj.color.a - Mathf.Lerp(end, start, speed));
				yield return new WaitForFixedUpdate();
			}
		}
	}

	public static IEnumerator ChangeActivenessDelayed(GameObject obj, bool newState, float delay)  {
		yield return new WaitForSeconds(delay);
		obj.SetActive(newState);
	}

	public static IEnumerator ChangeInteractivityDelayed(UnityEngine.UI.Button[] b, bool newState, float delay)  {
		yield return new WaitForSeconds(delay);
		for(int i = 0; i < b.Length; i++)  {
			b[i].interactable = newState;
		}
	}

	public static IEnumerator FadeSprite(float start, float end, float speed, SpriteRenderer obj, float initdelay = 0f, bool destroyAfter = false)  {
		yield return new WaitForSeconds(initdelay);
		if(start < end)  {		//decrease transparency
			while(obj.color.a < end)  {
				obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, obj.color.a + Mathf.Lerp(start, end, speed));
				yield return new WaitForFixedUpdate();
			}
		}
		else  {					//increase transparency
			while(obj.color.a > end)  {
				obj.color = new Color(obj.color.r, obj.color.g, obj.color.b, obj.color.a - Mathf.Lerp(end, start, speed));
				yield return new WaitForFixedUpdate();
			}
		}
		yield return new WaitForSeconds(1f);
		if(destroyAfter)
			MonoBehaviour.Destroy(obj.gameObject);
	}

	public static IEnumerator TimeCounter(float starttime, float endtime, System.Action<float> passNowTime, System.Action<bool> passIfFinished)  {
		float nowtime;
		bool isFinished = false;
		nowtime = starttime;
		if(starttime < endtime)  {		//count ascending
			while(nowtime <= endtime)  {
				nowtime += Time.deltaTime;
				passNowTime(nowtime);
				yield return new WaitForEndOfFrame();
			}
		}
		else  {							//count descending
			while(nowtime >= endtime)  {
				nowtime -= Time.deltaTime;
				passNowTime(nowtime);
				yield return new WaitForEndOfFrame();
			}
		}
		isFinished = true;
		passIfFinished(isFinished);
	}

	public static IEnumerator TimeCounter(float starttime, float endtime, System.Action<float> passNowTime, System.Action<bool> passIfStarted, System.Action<bool> passIfFinished)  {
		float nowtime;
		bool isStarted = true;
		bool isFinished = false;
		passIfStarted(isStarted);
		nowtime = starttime;
		if(starttime < endtime)  {		//count ascending
			while(nowtime <= endtime)  {
				nowtime += Time.deltaTime;
				passNowTime(nowtime);
				yield return new WaitForEndOfFrame();
			}
		}
		else  {							//count descending
			while(nowtime >= endtime)  {
				nowtime -= Time.deltaTime;
				passNowTime(nowtime);
				yield return new WaitForEndOfFrame();
			}
		}
		isFinished = true;
		passIfFinished(isFinished);
	}

}