using UnityEngine;
using UnityEngine.Audio;

public class SoundMixerManager : MonoBehaviour
{
	#region Private Fields

	[SerializeField]
	private AudioMixer audioMixer;

	#endregion

	#region Public Methods

	public void SetMasterVolume(float level)
	{
		audioMixer.SetFloat("masterVolume", level);
	}

	public void SetMusicVolume(float level)
	{
		audioMixer.SetFloat("musicVolume", level);
	}

	public void SetSoundFXVolume(float level)
	{
		audioMixer.SetFloat("soundFXVolume", level);
	}

	#endregion
}
