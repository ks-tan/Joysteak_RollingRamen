using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	[SerializeField] private TextAsset _ingredientsJson;
	[SerializeField] private TextAsset _recipeJson;
	[SerializeField] private GameObject _collectiblePrefab;
	[SerializeField] private float _maxTime;
	[SerializeField] private float _spawnBufferDuration;

	private Dictionary<string, int> _collectedIngredients = new Dictionary<string, int>();
	private List<string> _ingredientList = new List<string> ();
	private string _recipeName;
	private float _time;
	private float _startCookingTime;
	private float _lastCollectibleSpawnTime;
	private int _resultScore;
	private bool _scoreCalculated;
	private bool _hasGameEnded;
	private bool _hasGameStarted;

	void Start() {
		if (instance == null) {
			instance = this;
		}
		_time = 0f;
		_lastCollectibleSpawnTime = 0f;
		_ingredientList = LoadIngredientsJSON ();
		_recipeName = ChooseRecipeFromJSON ();
		UIController.instance.UpdateRecipeName (_recipeName);
		UIController.instance.UpdateScore (0);
	}

	IEnumerator StartGameTimer() {
		_hasGameStarted = false;
		yield return new WaitForSeconds (1f);
		UIController.instance.ShowCountdown (true);
		for (int i = 3; i >= 0; i--) {
			string timerText = i > 0 ? i.ToString () : "ROLL!";
			UIController.instance.UpdateCountdownText (timerText);
			yield return new WaitForSeconds (1f);
		}
		UIController.instance.ShowCountdown (false);
		_startCookingTime = _time;
		_hasGameStarted = true;
	}

	string ChooseRecipeFromJSON() {
		JSONNode N = JSON.Parse (_recipeJson.text);
		int recipeCount = N.Count;
		var recipeData = N [Random.Range (0, recipeCount)];
		string recipeName = recipeData [Constants.RECIPE_NAME_DATAFIELD];
		return recipeName;
	}

	List<string> LoadIngredientsJSON() {
		List<string> ingredientsListData = new List<string> ();
		JSONNode N = JSON.Parse (_ingredientsJson.text);
		for (int i = 0; i < N.Count; i++) {
			ingredientsListData.Add (N [i]);
		}
		return ingredientsListData;
	}

	void Update() {
		_time += Time.deltaTime;
		_hasGameEnded = _hasGameStarted && _time > _startCookingTime + _maxTime;
		bool shouldShowScorePanel = _hasGameEnded && !_scoreCalculated;
		bool shouldSpawnCollectible =  _hasGameStarted && _time - _lastCollectibleSpawnTime > _spawnBufferDuration && !_hasGameEnded;
		if (shouldShowScorePanel) {
			_resultScore = CalculateScore ();
			_scoreCalculated = true;
			UIController.instance.ShowScorePanel (_resultScore);
		} else if (shouldSpawnCollectible) {
			_lastCollectibleSpawnTime = _time;
			Vector2 viewportPos = new Vector2 (Random.Range (0f, 1f), 0);
			Vector2 spawnPos = Camera.main.ViewportToWorldPoint (viewportPos);
			GameObject collectible = GameObject.Instantiate (_collectiblePrefab);
			collectible.transform.position = spawnPos;
		}
	}

	int CalculateScore() {
		JSONNode chosenRecipe = GetRecipeFromName (_recipeName);
		JSONNode recipeIngredients = chosenRecipe [Constants.RECIPE_INGREDIENTS_DATAFIELD];
		int totalThingsCollected = 0;
		int resultScore = 0;
		foreach (string key in _collectedIngredients.Keys) {
			totalThingsCollected += _collectedIngredients [key];
		}
		for (int i = 0; i < recipeIngredients.Count; i++) {
			JSONNode recipeIngredientData = recipeIngredients [i];
			string ingredientName = recipeIngredientData [Constants.RECIPE_INGREDIENT_NAME_DATAFIELD];
			int ingredientWeightage = recipeIngredientData[Constants.RECIPE_INGREDIENT_WEIGHTAGE_DATAFIELD];
			int amountCollected = 0;
			_collectedIngredients.TryGetValue(ingredientName, out amountCollected);
			float collectedPercentage = (float)amountCollected / totalThingsCollected * 100f;
			float scorePercentage = collectedPercentage / ingredientWeightage * 100f;
			if (scorePercentage > ingredientWeightage) {
				scorePercentage = ingredientWeightage;
			}
			resultScore += (int) scorePercentage;
		}
		return resultScore;
	}

	JSONNode GetRecipeFromName(string recipeName) {
		JSONNode N = JSON.Parse (_recipeJson.text);
		JSONNode chosenRecipe = null;
		for (int i = 0; i < N.Count; i++) {
			var thisRecipe = N [i];
			var thisRecipeName = thisRecipe [Constants.RECIPE_NAME_DATAFIELD];
			if (thisRecipeName.Equals(recipeName)) {
				chosenRecipe = thisRecipe;
				break;
			}
		}
		return chosenRecipe;
	}

	public void CookButtonPressed() {
		UIController.instance.SlideInScoreUI (false);
		UIController.instance.ShowCookButton (false);
		StartCoroutine (StartGameTimer ());
	}

	public void AgainButtonPressed() {
		UIController.instance.ShowScoreUICustomerRequest();
	}

	public void AddIngredient(string ingredientName) {
		int currentCount;
		_collectedIngredients.TryGetValue(ingredientName, out currentCount); 
		_collectedIngredients[ingredientName] = currentCount + 1;
		_resultScore = CalculateScore ();
		UIController.instance.UpdateScore (_resultScore);
		UIController.instance.UpdateIngredientsUI (ingredientName);
	}

	public List<string> GetIngredientList() {
		return _ingredientList;
	}

	public Dictionary<string, int> GetCollectedIngredients() {
		return _collectedIngredients;
	}

	public int GetScore() {
		return _resultScore;
	}

}
