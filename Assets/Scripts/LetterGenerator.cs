using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterGenerator : MonoBehaviour
{
	List<string> firstNames = new List<string>{ "John", "Jenny", "Barbara", "Tom", "Daphne",
																						  "Jimmy", "Sarah", "Brad", "Dale", "Jeff", "Anna", "Lily",
																							"Mieke", "Hannah", "Laurence", "Oliver", "Sofia", "Louis",
																							"Muhammad", "Steffan", "Harry", "Rebecca", "Sebastian", "Simone",
																							"Mike", "Angelina", "Marcus", "Alan", "Richard", "Helen", "Daniel",
																							"Fern", "Joaquin", "Lorna"};

	List<string> surnames = new List<string>{ "Peterson", "Bacon", "Liebermann", "Brown", "Woodson", "Rodriguez", "Jones",
																						"Wilson", "Aarons", "Brocklehurst", "Broadbent", "Potter", "Cooper", "Carr",
																						"Daniels", "Williams", "Schultz", "Santos", "Monroe", "Watson", "Neals", "MacDonald",
																						"Cain", "Archer", "Richards", "Bishop", "Butcher", "Stark", "Mueller", "Fischer", "Schneider",
																						"Wainwright", "Baker", "Smith", "Bauer", "Jaeger", "Ziegler" };

	List<string> streetLastWords = new List<string> { "Street", "Road", "Street", "Road", "Street", "Road", "Street", "Road",
																										"Lane", "Avenue", "Boulevard", "Alley", "Close", "Way", "Court", "Crescent",
																										"Mews", "Drive", "Hill", "Lane", "Row", "Square", "Grove"};

	List<string> streetFirstWords = new List<string> { "High", "Church", "Park", "Main", "Station", "Green", "Manor", "Queens", "New", "Grange",
																										"Kings", "Highfield", "Mill", "Alexander", "Springfield", "Victoria", "George", "West", "North", "East",
																										"South", "The", "Stanley", "School", "Blue Jay", "Crow", "Orion", "Aquarius", "Apollo", "Birch", "Oak",
																										"Ash", "Yew", "Dove", "Pigeon", "Peacock", "Partridge", "Nightingale", "Mallard", "Magpie", "Falcon",
																										"Montague", "Ophelia", "Yorick", "Titania", "Puck", "Hamlet", "Picasso", "Raphael", "Leonardo", "Van Gogh",
																										"Fortran", "Pascal", "Cobol"};

	List<string> cities = new List<string> { "Manchester", "London", "Liverpool", "Birmingham", "Edinburgh", "Glasgow", "Coventry", "Chester", "York", "Bristol",
																					 "Oxford", "Aberdeen", "Bangor", "Aberystwyth", "Cardiff", "Chichester", "Dundee", "Dunfermline", "Inverness", "Hull", "St. Andrews",
																					 "Milton Keynes", "Newcastle", "Perth", "Sheffield", "Slough", "Truro", "Penistone", "Swindon", "Bath", "Ipswitch",
																						"Southend-on-Sea", "Stratford-upon-Avon",
																					 "Westward Ho!", "Plwmp", "Upton Snodsbury", "Blubberhouses", "Dull", "Braintree", "Barton in the Beans"};

	List<string> fakeCities = new List<string> { "Norington", "Marslea", "Durrich", "Langington", "Wretched Bottom", "Soulminster", "Thallborough", "Bursley", "Sheadmond Upon Ntehetmere",
																								"Wrighton", "St. Rynes", "North Keynes", "St. Looes", "Locktrout", "Sit", "Dandus", "Hestherholme",
																								"Percoeba-on-Sea", "Maifield", "Shoremill-upon-Thames"};


	int difficulty = 0;
	public float z = 0;

	const int baseFakeValue = 6;

	public void SetDifficulty(int newDifficulty)
	{
		difficulty = newDifficulty;
	}

	public void Generate(Letter letterComponent)
	{
		string firstName = PickRandomFromList(firstNames) + " ";
		string surname = PickRandomFromList(surnames);
		string houseNumber = Mathf.RoundToInt(Random.Range(1, 420)).ToString() + " ";
		string streetFirstWord = PickRandomFromList(streetFirstWords) + " ";
		string streetLastWord = PickRandomFromList(streetLastWords);
		string city = PickRandomFromList(cities);

		EDeliveryType type = Random.Range(0f, 1f) >= .65f ? EDeliveryType.FirstClass : EDeliveryType.SecondClass;

		bool isLetterValid = true;

		int value = Random.Range(1, baseFakeValue + 1 + difficulty);

		//first couple letters guaranteed to be right
		if (LetterManager.instance.scorePennies < 3 && LetterManager.instance.scorePounds < 1)
			value = 0;

		//10 percent chance of an evil letter if the letter is incorrect, overall 1/100 letters are evil
		int evilLetter = Mathf.RoundToInt(Random.Range(1, 10));

		//generate incorrect letter
		if (value >= baseFakeValue)
		{
			isLetterValid = false;

			//if (evilLetter == 1)
			//{
				//letter = MakeEvilLetter();
				//return;
			//}

			switch (value)
			{
				case baseFakeValue:
					//no stamp
					type = EDeliveryType.Missing;
					break;
				case baseFakeValue+1:
					type = Random.Range(0f, 1f) >= .5f ? EDeliveryType.FakeFirstClass : EDeliveryType.FakeSecondClass;
					//todo
					break;
				case baseFakeValue+2:
					//wrong name
					{
						int roll = Random.Range(0, 2);
						if(roll == 0)
							firstName = "";
						else
							surname = "";
						break;
					}
				case baseFakeValue+3:
					//wrong address
					{
						int roll = Random.Range(0, 3);
						if (roll == 0)
							houseNumber = "";
						else if (roll == 1)
							streetFirstWord = "";
						else
							streetLastWord = "";
						break;
					}
				case baseFakeValue+4:
					//fake city
					city = PickRandomFromList(fakeCities);
					break;
			}

		}

		string address = firstName + surname + "\n" + houseNumber + streetFirstWord + streetLastWord + "\n" + city;
		letterComponent.Initialise(address, isLetterValid, type, z);
		z+= .2f;
		if (z > 8.5)
			z = 0;
	}

	//Letter MakeEvilLetter()
	//{
	//	return new Letter();
	//}

	string PickRandomFromList(List<string> list)
	{
		int index = Mathf.RoundToInt(Random.Range(0, list.Count));

		return list[index];
	}
}
