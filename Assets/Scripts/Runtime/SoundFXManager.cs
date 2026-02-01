using UnityEngine;

public class SoundFXManager : MonoBehaviour
{
	#region Private Fields

	private static SoundFXManager _instance;

	[SerializeField]
	private GameObject _soundFXObject;

	#endregion

	#region Properties

	public static SoundFXManager Instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject go = Instantiate(Resources.Load<GameObject>("SoundFXManager"), null);
				DontDestroyOnLoad(go);

				_instance = go.GetComponent<SoundFXManager>();
			}

			return _instance;
		}
	}

	#endregion

	#region Public Methods

	public void DestroyAudioSource()
	{
		Destroy(_soundFXObject);
	}

	public GameObject PlayRandomSoundFXClip(AudioClip[] audioClip, Transform spawnTransform, float volume)
	{
		//assign a random index
		int rand = Random.Range(0, audioClip.Length);

		//spawn in gameObject
		GameObject audioGameObject = Instantiate(_soundFXObject, spawnTransform.position, Quaternion.identity);
		AudioSource audioSource = audioGameObject.GetComponent<AudioSource>();

		//assign the audioClip
		audioSource.clip = audioClip[rand];

		//assign volume
		audioSource.volume = volume;

		//play sound
		audioSource.Play();

		//get length of soundFX clip
		float clipLength = audioSource.clip.length;

		//destroy the clip after it is done playing
		Destroy(audioSource.gameObject, clipLength);

		return audioSource.gameObject;
	}

	public GameObject PlaySoundFXClip(AudioClip audioClip, Transform spawnTransform, float volume)
	{
		//spawn in gameObject
		GameObject audioGameObject = Instantiate(_soundFXObject, spawnTransform.position, Quaternion.identity);
		AudioSource audioSource = audioGameObject.GetComponent<AudioSource>();

		//assign the audioClip
		audioSource.clip = audioClip;

		//assign volume
		audioSource.volume = volume;

		//play sound
		audioSource.Play();

		//get length of soundFX clip
		float clipLength = audioSource.clip.length;

		//destroy the clip after it is done playing
		Destroy(audioSource.gameObject, clipLength);

		return audioSource.gameObject;
	}

	#endregion
}
