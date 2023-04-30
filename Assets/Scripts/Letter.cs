using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDeliveryType 
{
	FirstClass,
	SecondClass,
	Missing,
	FakeFirstClass,
	FakeSecondClass,
}

public class Letter : Dragabble
{
	string address;
	public bool isValid { get; private set; }
	public EDeliveryType deliveryType { get; private set; }
	public SpriteRenderer letterSprite;
	public TextMesh text;
	public SpriteRenderer stamp;

	bool held = false;

	bool isShrinking = false;
	bool isGrowing = false;


	public void Initialise(string _address, bool _isAddressValid, EDeliveryType _type, float z)
	{
	  //letterSprite = GetComponentInChildren<SpriteRenderer>();
		//text = GetComponentInChildren<TextMesh>();

		float rotation = (Random.Range(0f, 1f) > .5f ? 1 : -1) * Random.Range(0f, 20f);
		float stampRotation = (Random.Range(0f, 1f) > .5f ? 1 : -1) * Random.Range(0f, 8f) + rotation;

		transform.rotation = Quaternion.Euler(0, 0, rotation);
		transform.position = new Vector3(Random.Range(-4.5f, 4.5f), Random.Range(-2.5f, 2.5f), -z - .5f);
		text.transform.position = new Vector3(text.transform.position.x, text.transform.position.y, -z - .5f);
		//stamp.transform.position = new Vector3(stamp.transform.position.x, stamp.transform.position.y, -z - .05f);
		stamp.transform.rotation = Quaternion.Euler(0, 0, stampRotation);

		address = _address;
		isValid = _isAddressValid;
		deliveryType = _type;

		Texture2D stampTex;
		string stampTexFile;

		switch(deliveryType)
		{
			case EDeliveryType.FirstClass:
				stampTexFile = "1st";
				break;
			case EDeliveryType.SecondClass:
				stampTexFile = "2nd";
				break;

			case EDeliveryType.FakeFirstClass:
				stampTexFile = "fake1st";
				break;
			case EDeliveryType.FakeSecondClass:
				stampTexFile = "fake2nd";
				break;
			case EDeliveryType.Missing:
				stampTexFile = "";
				break;
			default:
				stampTexFile = "";
				break;
		}

		//missing stamps
		if (stampTexFile == "")
		{
			Destroy(stamp.gameObject);
		}
		else
		{
			stampTex = Resources.Load(stampTexFile, typeof(Texture2D)) as Texture2D;
			stamp.sprite = Sprite.Create(stampTex, new Rect(0.0f, 0.0f, stampTex.width, stampTex.height), new Vector2(0.5f, 0.5f)); ;
		}

		text.text = address;

		//choose letter texture
		int index = Mathf.RoundToInt(Random.Range(0.5f, 4.49f));

		int specialLetter = Mathf.RoundToInt(Random.Range(0.5f, 9.49f));

		Texture2D tex;

		if (specialLetter == 1)
		{
			index = Mathf.RoundToInt(Random.Range(4.5f, 6.49f));
			tex = Resources.Load("letter" + index.ToString(), typeof(Texture2D)) as Texture2D;
		}
		else
		{
			tex = Resources.Load("letter" + index.ToString(), typeof(Texture2D)) as Texture2D;
		}

		//adjust collision for thinner letters
		if(index == 1 || index == 3 || index == 4)
		{
			GetComponent<BoxCollider2D>().size = new Vector2(GetComponent<BoxCollider2D>().size.x, 3.5f);
		}

		letterSprite.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
	}

	void OnMouseDown()
	{
		base.OnMouseDown();
		LetterManager.instance.SetHeldLetter(this);
	}

	void OnMouseUp()
	{
		LetterManager.instance.ReleaseHeldLetter(this);
		base.OnMouseUp();
	}

	public void Shrink()
	{
		offset = new Vector3(0, 0, offset.z);

		if (!isShrinking && !isGrowing)
			isShrinking = true;
	}

	public void Grow()
	{
		if (!isShrinking && !isGrowing)
			isGrowing = true;
	}

	void Update()
	{
		base.Update();

		if(isShrinking && transform.localScale.x > .2f)
		{
			transform.localScale -= new Vector3(5f, 5f, 0) * Time.deltaTime;
		}
		else if(isShrinking)
		{
			isShrinking = false;
		}

		if (isGrowing && transform.localScale.x < 1f)
		{
			transform.localScale += new Vector3(5f, 5f, 0) * Time.deltaTime;
		}
		else if (isGrowing)
		{
			isGrowing = false;
		}
	}
}
