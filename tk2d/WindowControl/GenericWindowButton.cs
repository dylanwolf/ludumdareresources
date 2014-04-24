using UnityEngine;
using System.Collections;

public class GenericWindowToggle : MonoBehaviour {

	public GenericWindow Window;
	public bool ToggleToState;

	void OnMouseUpAsButton()
	{
		if (Window != null)
		{
			if (ToggleToState)
			{
				Window.Show ();
			}
			else
			{
				Window.Hide ();
			}
		}
	}
}
