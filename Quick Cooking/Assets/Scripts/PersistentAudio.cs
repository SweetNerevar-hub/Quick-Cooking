using UnityEngine;

public class PersistentAudio : MonoBehaviour {

    public static PersistentAudio Instance;

    AudioSource audioSource;

    [SerializeField] AudioClip eatFood;

    private void Awake() {
        if(Instance != null && Instance != this) {
            Destroy(this);
        }

        else {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void EatFoodAudio() {
        audioSource.PlayOneShot(eatFood);
    }
}
