using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NURotator : MonoBehaviour {
	public Vector3 SpeedVector;
	
	RectTransform rt;
	
	// Use this for initialization
	void Start () {
		rt = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
		rt.Rotate(SpeedVector * Time.deltaTime);
	}
}
