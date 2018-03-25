using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBackground : MonoBehaviour {

	[SerializeField] GameObject _bgTop;
	[SerializeField] GameObject _bgMiddle;
	[SerializeField] GameObject _bgBottom;
	[SerializeField] GameObject _bgTopGradation;
	[SerializeField] GameObject _bgMiddleGradation;
	[SerializeField] GameObject _bgBottomGradation;
	[SerializeField] GameObject _bgSoup;
	[SerializeField] int _speed;

	public static GameBackground instance;

	void Start () {
		if (instance == null) {
			instance = this;
		}
		ResetGameBackground ();
	}
	
	void Update () {
		float gameTimeElapsed = GameManager.instance.GetTime () - GameManager.instance.GetStartCookingTime ();
		float gameDuration = GameManager.instance.GetMaxTime ();
		if (GameManager.instance.HasGameStarted ()) {
			_bgTop.transform.Translate(Vector3.up * Time.deltaTime * _speed, Space.World);
			if (gameTimeElapsed / gameDuration >= 0.33f) {
				_bgMiddle.transform.Translate(Vector3.up * Time.deltaTime * _speed, Space.World);
			}
			if (gameTimeElapsed / gameDuration >= 0.66f) {
				_bgBottom.transform.Translate(Vector3.up * Time.deltaTime * _speed, Space.World);
			}
			if (gameTimeElapsed / gameDuration >= 0.9f) {
				_bgSoup.transform.Translate(Vector3.up * Time.deltaTime * 5, Space.World);
			}
		}
	}

	public void ResetGameBackground() {
		_bgTop.transform.position = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 1));
		_bgMiddle.transform.position = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0));
		_bgBottom.transform.position = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0));
		_bgSoup.transform.position = Camera.main.ViewportToWorldPoint (new Vector2 (0.5f, 0));
		_bgTop.transform.position = new Vector3 (_bgTop.transform.position.x, _bgTop.transform.position.y, 0);
		_bgMiddle.transform.position = new Vector3 (_bgMiddle.transform.position.x, _bgMiddle.transform.position.y, 0);
		_bgBottom.transform.position = new Vector3 (_bgBottom.transform.position.x, _bgBottom.transform.position.y, 0);
		_bgSoup.transform.position = new Vector3 (_bgSoup.transform.position.x, _bgSoup.transform.position.y, 0);
	}
}
