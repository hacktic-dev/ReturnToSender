
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EState
{
	Start,
	Gameplay,
	End
}


public class LetterManager : MonoBehaviour
{
	public LetterGenerator generator;
	public Letter heldLetter;
	public EBinType hoveredBin = EBinType.None;
	public Slider slider;
	public Image deathWheel;

	public EState state { get; private set; } = EState.Start;

	float deathCount = 0;
	int failCount = 0;

	public GameObject cross1;
	public GameObject cross2;
	public GameObject cross3;
	public GameObject cross4;
	public GameObject cross5;

	const int failTotal = 5;
	const float deathTotal = 15;

	public Text scoreText;

	public static LetterManager instance = null;

	GameObject letterObject;

	public GameObject endScreen;
	public GameObject startScreen;
	public GameObject gameplayScreen;
	public Text finalScoreText;

	float letterSpawnCountdown = 10;
	bool initialSpawnCountdown = true;

	float letterValueCount = 0;

	bool spawnedFakeStampNote = false;
	bool spawnedWrongAddressNote = false;
	bool spawnedWrongCityNote = false;

	public int scorePennies { get; private set; } = 0;
	public int scorePounds { get; private set; } = 0;


	// Start is called before the first frame update
	void Start()
	{
		if (instance == null)
			instance = this;
		else
			Destroy(this);

		letterObject = (GameObject)Resources.Load("Letter");
	}

	// Update is called once per frame
	void Update()
	{
		if (state == EState.Gameplay)
		{
			letterSpawnCountdown -= Time.deltaTime;

			//guarantee that no letter spawns for first 10 seconds after a note appearing
			//spawn a letter within 2 seconds if no other letters
			if (!initialSpawnCountdown && letterValueCount < .5f && letterSpawnCountdown > 2f)
			{
				letterSpawnCountdown = Random.Range(.25f, 2f);
			}
			else if (letterSpawnCountdown < 0)
			{
				initialSpawnCountdown = false;
				letterSpawnCountdown = Mathf.Max(0.25f, Random.Range(3.2f, 6.5f) - ((float)scorePennies + scorePounds * 100) / 45f);
				AddLetter();
			}

			/*if (Input.GetKeyDown(KeyCode.E))
			{
				Letter letterComponent = AddLetter();
				generator.Generate(letterComponent);
			}*/

			if (heldLetter && hoveredBin != EBinType.None)
			{
				heldLetter.Shrink();
			}
			else if (heldLetter && hoveredBin == EBinType.None)
			{
				heldLetter.Grow();
			}

			if (letterValueCount >= 19)
			{
				GetComponent<SoundManager>().StartClock();
				deathCount += Time.deltaTime;
				deathWheel.fillAmount = deathCount / deathTotal;
			}
			else if (deathCount >= 0)
			{

				deathCount -= Time.deltaTime;
				deathWheel.fillAmount = deathCount / deathTotal;
				GetComponent<SoundManager>().StopClock();
			}

			if (deathCount >= deathTotal || failCount >= failTotal)
			{
				state = EState.End;
				gameplayScreen.SetActive(false);
				Debug.Log("end");
				endScreen.SetActive(true);
				GetComponent<SoundManager>().StopClock();
				finalScoreText.text = "Final Severace: $" + scorePounds.ToString() + "." + (scorePennies < 10 ? "0" : "") + scorePennies.ToString();
			}
		}
	}

	public void SetHeldLetter(Letter letter)
	{
		heldLetter = letter;
	}

	public void ReleaseHeldLetter(Letter letter)
	{
		if (state != EState.Gameplay)
			return;

		if (letter == heldLetter)
		{
			if (hoveredBin == EBinType.None)
				heldLetter = null;
			else
			{
				if (letter.isValid)
				{
					if (letter.deliveryType == EDeliveryType.FirstClass && hoveredBin == EBinType.First)
					{
						Debug.Log("succeed");
						SetScore(2);
						RemoveLetter(letter);
						return;
					}
					else if (letter.deliveryType == EDeliveryType.SecondClass && hoveredBin == EBinType.Second)
					{
						Debug.Log("succeed");
						SetScore(1);
						RemoveLetter(letter);
						return;
					}

					Fail();
					Debug.Log("fail");
					RemoveLetter(letter);
					return;
				}
				else
				{
					if (hoveredBin != EBinType.Discard)
					{
						Fail();
						Debug.Log("fail");
						RemoveLetter(letter);
						return;
					}
					else
					{
						SetScore(1);
						Debug.Log("succeed");
						RemoveLetter(letter);
					}
				}
			}
		}
		else
			Debug.LogWarning("Tried to release letter that isn't held.");
	}

	public void SetHoveredBin(EBinType type)
	{
		hoveredBin = type;
	}

	public void RemoveHoveredBin(EBinType type)
	{
		if (hoveredBin == type)
			hoveredBin = EBinType.None;
		else
			Debug.LogWarning("Tried to remove bin type that isn't hovered.");
	}

	void SetScore(int delta)
	{
		GetComponent<SoundManager>().PlaySound(ESound.Correct);

		scorePennies += delta;
		if (scorePennies >= 100)
		{
			scorePounds++;
			scorePennies -= 100;
		}
		scoreText.text = "Earned: $" + scorePounds.ToString() + "." + (scorePennies < 10 ? "0" : "") + scorePennies.ToString();

		if (scorePennies >= 12 && !spawnedFakeStampNote)
		{
			GetComponent<SoundManager>().PlaySound(ESound.New);
			spawnedFakeStampNote = true;
			GameObject note = (GameObject)Instantiate(Resources.Load("NoteFakeStamp"));
			note.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), Random.Range(-2.5f, 2.5f), -9.1f);
			generator.SetDifficulty(1);
			initialSpawnCountdown = true;
			letterSpawnCountdown = 10f;
		}
		if (scorePennies >= 30 && !spawnedWrongAddressNote)
		{
			GetComponent<SoundManager>().PlaySound(ESound.New);
			spawnedWrongAddressNote = true;
			GameObject note = (GameObject)Instantiate(Resources.Load("NoteWrongAddress"));
			note.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), Random.Range(-2.5f, 2.5f), -9.2f);
			generator.SetDifficulty(3);
			initialSpawnCountdown = true;
			letterSpawnCountdown = 10f;
		}
		if (scorePennies >= 55 && !spawnedWrongCityNote)
		{
			GetComponent<SoundManager>().PlaySound(ESound.New);
			spawnedWrongCityNote = true;
			GameObject note = (GameObject)Instantiate(Resources.Load("NoteWrongCity"));
			note.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), Random.Range(-2.5f, 2.5f), -9.3f);
			generator.SetDifficulty(4);
			initialSpawnCountdown = true;
			letterSpawnCountdown = 10f;
		}
	}

	void AddLetter()
	{
		GetComponent<SoundManager>().PlaySound(ESound.New);
		GameObject instantiated = Instantiate(letterObject);
		generator.Generate(instantiated.GetComponent<Letter>());
		letterValueCount += instantiated.GetComponent<Letter>().deliveryType == EDeliveryType.FirstClass ? 1.5f : 1;
		slider.value = letterValueCount;
	}

	void RemoveLetter(Letter letter)
	{
		letterValueCount -= letter.deliveryType == EDeliveryType.FirstClass ? 1.5f : 1;
		Destroy(letter.gameObject);
		slider.value = letterValueCount;
	}

	void Fail()
	{
		GetComponent<SoundManager>().PlaySound(ESound.Fail);

		failCount++;

		switch (failCount)
		{
			case 1:
				cross1.SetActive(true);
				break;
			case 2:
				cross2.SetActive(true);
				break;
			case 3:
				cross3.SetActive(true);
				break;
			case 4:
				cross4.SetActive(true);
				break;
			case 5:
				cross5.SetActive(true);
				break;
		}

	}

	public void Reset()
	{
		cross1.SetActive(false);
		cross2.SetActive(false);
		cross3.SetActive(false);
		cross4.SetActive(false);
		cross5.SetActive(false);
		scorePennies = 0;
		scorePounds = 0;
		scoreText.text = "Earned: $" + scorePounds.ToString() + "." + (scorePennies < 10 ? "0" : "") + scorePennies.ToString();
		spawnedFakeStampNote = false;
		spawnedWrongAddressNote = false;
		spawnedWrongCityNote = false;
		

		letterValueCount = 0;

		deathCount = 0;
		failCount = 0;

		deathWheel.fillAmount = 0;
		slider.value = 0;

		foreach (Object draggable in FindObjectsOfType<Dragabble>())
		{
			Destroy(((Dragabble)draggable).gameObject);
		}

		GetComponent<SoundManager>().PlaySound(ESound.New);
		GameObject note = (GameObject)Instantiate(Resources.Load("NoteStart"));
		note.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), Random.Range(-2.5f, 2.5f), -9.0f);
		generator.SetDifficulty(0);
		initialSpawnCountdown = true;
		letterSpawnCountdown = 10f;
		state = EState.Gameplay;
		gameplayScreen.SetActive(true);
		startScreen.SetActive(false);
		endScreen.SetActive(false);
	}
}
