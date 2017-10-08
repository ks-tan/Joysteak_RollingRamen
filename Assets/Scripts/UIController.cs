using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour {

	public static UIController instance;

	[SerializeField] private Text _recipeNameUIText;
	[SerializeField] private RectTransform _playUIScoreBar;
	[SerializeField] private RectTransform _scoreUIScoreBar;
	[SerializeField] private GameObject _scoreUI;
	[SerializeField] private GameObject _playUIIngredientsPanel;
	[SerializeField] private GameObject _collectibleUIPrefab;
	[SerializeField] private GameObject _customerPanel;
	[SerializeField] private GameObject _scoreUIStarsPanel;
	[SerializeField] private GameObject _scoreUIIngredientsPanel;
	[SerializeField] private GameObject _scoreUIRamneNamePanel;
	[SerializeField] private GameObject _explosionParticleSystem;
	[SerializeField] private GameObject _playUICountdown;
	[SerializeField] private Sprite _happyCustomerImage;
	[SerializeField] private Sprite _angryCustomerImage;

	private float _originalPlayUIScoreBarWidth;
	private float _originalPlayUIScoreBarHeight;
	private float _originalScoreUIScoreBarWidth;
	private float _originalScoreUIScoreBarHeight;

	void Start () {
		if (instance == null) {
			instance = this;
		}
		_originalPlayUIScoreBarWidth = _playUIScoreBar.rect.width;
		_originalPlayUIScoreBarHeight = _playUIScoreBar.rect.height;
		_originalScoreUIScoreBarWidth = _scoreUIScoreBar.rect.width;
		_originalScoreUIScoreBarHeight = _scoreUIScoreBar.rect.height;
	}

	public void UpdateCountdownText(string text) {
		_playUICountdown.GetComponent<Text> ().text = text;
	}

	public void ShowCountdown(bool shouldShow){
		if (shouldShow) {
			_playUICountdown.SetActive (true);
		} else {
			_playUICountdown.SetActive (false);
		}
	}

	public void UpdateScore(int score) {
		float newScoreBarWidth = (score / 100f) * _originalPlayUIScoreBarWidth;
		_playUIScoreBar.sizeDelta = new Vector2 (newScoreBarWidth, _originalPlayUIScoreBarHeight);
	}

	public void UpdateRecipeName(string recipeName) {
		_recipeNameUIText.text = recipeName;
	}

	public void UpdateIngredientsUI(string ingredientName) {
		bool isCollectedBefore = false;
		foreach (Transform collectibleTransform in _playUIIngredientsPanel.transform) {
			CollectibleUI collectibleUI = collectibleTransform.GetComponent<CollectibleUI> ();
			if (collectibleUI.collectibleName == ingredientName) {
				isCollectedBefore = true;
				collectibleUI.collectibleCount++;
				collectibleUI.UpdateCollectibleCountText ();
				break;
			}
		}
		if (!isCollectedBefore) {
			GameObject collectible = GameObject.Instantiate (_collectibleUIPrefab);
			collectible.transform.SetParent (_playUIIngredientsPanel.transform, false);
			CollectibleUI collectibleUI = collectible.GetComponent<CollectibleUI> ();
			collectibleUI.collectibleCount = 1;
			collectibleUI.collectibleName = ingredientName;
			collectibleUI.UpdateCollectibleCountText ();
		}
	}


	public void ShowScorePanel(int resultScore) {
		StartCoroutine (ExecuteScoreSequence (resultScore));
	}

	IEnumerator ExecuteScoreSequence(int resultScore){
		_scoreUI.transform.DOMove (new Vector3 (0, 0, 0), 1f, true);
		yield return new WaitForSeconds (1.5f);
		if (resultScore < 60) {
			_customerPanel.GetComponent<Image> ().sprite = _angryCustomerImage;
		} else {
			_customerPanel.GetComponent<Image> ().sprite = _happyCustomerImage;
		}
		yield return new WaitForSeconds (1.5f);
		// Show stars
		_customerPanel.GetComponent<Image> ().DOColor (new Color (107/255f, 107/255f, 107/255f), 0.2f);
		float newScoreBarWidth = (GameManager.instance.GetScore () / 100f) * _originalScoreUIScoreBarWidth;
		_scoreUIScoreBar.sizeDelta = new Vector2 (newScoreBarWidth, _originalScoreUIScoreBarHeight);
		_scoreUIStarsPanel.SetActive (true);
		// Show ingredients collected
		foreach (KeyValuePair<string, int> ingredient in GameManager.instance.GetCollectedIngredients()) {
			string ingredientName = ingredient.Key;
			int ingredientAmount = ingredient.Value;
			for (int i = 0; i < ingredientAmount; i++) {
				GameObject collectibleUI = GameObject.Instantiate (_collectibleUIPrefab);
				collectibleUI.transform.SetParent (_scoreUIIngredientsPanel.transform, false);
				collectibleUI.transform.Find ("Text").gameObject.SetActive (false);
				collectibleUI.name = ingredientName;
				GameObject explosionParticleSystem = GameObject.Instantiate (_explosionParticleSystem);
				explosionParticleSystem.transform.SetParent (collectibleUI.transform, false);
				explosionParticleSystem.transform.localPosition = new Vector2 (0, 0);
				yield return new WaitForSeconds (0.2f);
			}
		}
		// Show name of the ramen you made
		_scoreUIRamneNamePanel.SetActive(true);
	}

}
