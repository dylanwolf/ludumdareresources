using UnityEngine;
using System.Collections;

[AddComponentMenu("LudumDareResources/UI/Generic Window")]
public class GenericWindow : MonoBehaviour {

	public static bool IsShown = false;

	void Start()
	{
		if (!IsShown)
			Hide();
		else
			Show();
	}

	public void Show()
	{
		IsShown = true;
		ToggleSelfAndChildren(true);
	}

	public void Hide()
	{
		IsShown = false;
		ToggleSelfAndChildren(false);
	}

	private Renderer[] renderers;
	private Collider[] colliders;
	void ToggleSelfAndChildren(bool state)
	{
		if (renderers == null)
		{
			renderers = GetComponentsInChildren<Renderer>();
		}
		for (int i = 0; i < renderers.Length; i++)
		{
			renderers[i].enabled = state;
		}
		
		if (colliders == null)
		{
			colliders = GetComponentsInChildren<Collider>();
		}
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = state;
		}
	}
}
