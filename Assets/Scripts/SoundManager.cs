using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESound
{
	New,
	Correct,
	Fail,
	Clock
}


public class SoundManager : MonoBehaviour
{
	public AudioClip clock;
	public AudioClip paper;
	public AudioClip correct;
	public AudioClip fail;

	public void PlaySound(ESound sound)
	{
		AudioClip clip = GetClip(sound);
		GetComponent<AudioSource>().pitch = Random.Range(.92f, 1.08f);
		GetComponent<AudioSource>().PlayOneShot(clip);
	}

	public void StartClock()
	{
		if(!transform.Find("ClockAudio").GetComponent<AudioSource>().isPlaying)
			transform.Find("ClockAudio").GetComponent<AudioSource>().Play();
	}

	public void StopClock()
	{
		if (transform.Find("ClockAudio").GetComponent<AudioSource>().isPlaying)
			transform.Find("ClockAudio").GetComponent<AudioSource>().Stop();
	}

	AudioClip GetClip(ESound sound)
	{
		switch (sound)
		{
			case ESound.New:
				return paper;
			case ESound.Correct:
				return correct;
			case ESound.Fail:
				return fail;
			default:
				return paper;
		}
	}
}


