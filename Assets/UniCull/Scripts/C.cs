using UnityEngine;
using System.Collections;

public class C : MonoBehaviour {
    public Vector2 screenPoint;
    public Camera cam;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        screenPoint = new Vector2(screenPoint.x + 50, screenPoint.y);
        if (screenPoint.x > Screen.width)
            screenPoint = new Vector2(0, screenPoint.y + 50);
        if (screenPoint.y > Screen.height)
            screenPoint = Vector2.zero;

        Ray r = cam.ScreenPointToRay(screenPoint);
        Debug.DrawRay(r.origin, r.direction * 100, Color.red, Time.deltaTime);
	}
}
