using UnityEngine;
using System.Collections;

public class TrailRendererCorrection : MonoBehaviour {

	public TrailRenderer trail;

	void Start () {
		trail.sortingLayerName = "default";
		trail.sortingOrder = 3;
	}

}