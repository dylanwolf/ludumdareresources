using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("LudumDareResources/Sound/Sound Board")]
[RequireComponent(typeof(AudioSource))]
public class SoundBoard : MonoBehaviour {

	public static SoundBoard Current;

	public static void Play(string clipName)
	{
		if (Current != null && Current.clips.ContainsKey(clipName) && Current.clips[clipName].Length > 0)
        {
			Current.PlayRandomSound(Current.clips[clipName]);
		}
	}

	public AudioClipLibrary[] Clips;
	
	[Serializable]
	public class AudioClipLibrary
	{
		public string Name;
		public AudioClip[] Clips;
	}

	Dictionary<string, AudioClip[]> clips = new Dictionary<string, AudioClip[]>();

	#region Unity Lifecycle
	AudioSource source;

	void Awake()
	{
		Current = this;
		source = GetComponent<AudioSource>();
	}

	void Start()
	{
		SetupClips();
	}
	#endregion

	void SetupClips()
	{
		clips.Clear();
		for (int i = 0; i < Clips.Length; i++)
			clips[Clips[i].Name] = Clips[i].Clips;
	}

	void PlayRandomSound(AudioClip[] clips)
	{
		PlaySound(clips[Mathf.Clamp(UnityEngine.Random.Range(0, clips.Length), 0, clips.Length - 1)]);
	}

	void PlaySound(AudioClip clip)
	{
		source.PlayOneShot(clip);
	}
}
