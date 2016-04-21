using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("LudumDareResources/Effects/Numeric Value Text")]
[RequireComponent(typeof(Text))]
public class NumericText : MonoBehaviour {

	public int LeadingZeros = 0;

	Text _t;

	// Set this in another class that has a reference to this NumericText
	// Accepts value of the message sent, returns the value that should be displayed
	public Func<int, int> NumberUpdater;

	void Start () {
		_t = GetComponent<Text>();
	}

	static Dictionary<int, string> leadingZeroText = new Dictionary<int, string>();

	int lz;
	string GenerateLeadingZeros(string txt)
	{
		lz = LeadingZeros - txt.Length;
		if (lz <= 0)
			return txt;

		if (!leadingZeroText.ContainsKey(lz))
			leadingZeroText[lz] = GenerateLeadingZeroFormat(lz);

		return string.Format(leadingZeroText[lz], txt);
	}

	const string ZERO = "0";
	const string FORMAT = "{0}";
	string GenerateLeadingZeroFormat(int zeros)
	{
		string s = "";
		for (int i = 0; i < zeros; i++)
			s += ZERO;
		s += FORMAT;
		return s;
	}

	int lastNumber = 0;
	public void SetNumber(int number)
	{
		if (lastNumber != number)
		{
			_t.text = GenerateLeadingZeros(number.ToString());
			lastNumber = number;
		}
	}

	public void UpdateNumber(int number)
	{
		if (NumberUpdater != null)
			SetNumber(NumberUpdater(number));
		else
			SetNumber(number);
	}
}
