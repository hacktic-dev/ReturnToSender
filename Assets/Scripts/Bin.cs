using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EBinType
{
	First,
	Second,
	Discard,
	None
}


public class Bin : MonoBehaviour
{
	public EBinType type;

	void OnMouseEnter()
	{
		LetterManager.instance.SetHoveredBin(type);
	}
	void OnMouseExit()
	{
		LetterManager.instance.RemoveHoveredBin(type);
	}
}
