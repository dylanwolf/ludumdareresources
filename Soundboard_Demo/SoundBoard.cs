using UnityEngine;
using System.Collections;

public class SoundBoard : MonoBehaviour {

	public static SoundBoard Current;

	void Start()
	{
		Current = this;
	}

	public static void PlayCoin()
	{
		if (Current != null && Current.CoinSounds != null)
		{
			Current.PlayRandom(Current.CoinSounds);
		}
	}

	public static void PlayEnemy()
	{
		if (Current != null && Current.EnemySounds != null)
		{
			Current.PlayRandom(Current.EnemySounds);
		}
	}

	public static void PlayCard()
	{
		if (Current != null && Current.CardSounds != null)
		{
			Current.PlayRandom(Current.CardSounds);
		}
	}

	void PlayRandom(AudioSource[] source)
	{
		source[Random.Range(0, source.Length)].Play ();
	}

	public static void PlayFinish()
	{
		if (Current != null && Current.FinishSound != null)
		{
			Current.FinishSound.Play();
		}
	}

	public static void StopMusic()
	{
		if (Current != null && Current.Music != null)
		{
			Current.Music.Stop ();
		}
	}

	public static void StartMusic()
	{
		if (Current != null && Current.Music != null)
		{
			Current.Music.Play ();
		}
	}

	public AudioSource[] CoinSounds;
	public AudioSource[] EnemySounds;
	public AudioSource[] CardSounds;
	public AudioSource FinishSound;
	public AudioSource Music;
}
