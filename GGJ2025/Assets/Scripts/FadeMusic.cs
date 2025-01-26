using UnityEngine;

public class FadeMusic : MonoBehaviour
{
    /*
    NOTE: Both Audio Sources should have these settings:
    - Audio Resource: Select the variations for each source.
    - Play on Awake: False
    - Loop: True
    */
    public AudioSource sourceSlow;
    public AudioSource sourceFast;
    public float fadeInSeconds = 1f;
    private bool fading = false;
    private bool speedingUp = false;
    private float fadeTimeElapsed = 0f;

    void Start()
    {
        fading = false;
        fadeTimeElapsed = 0f;
        sourceSlow.volume = 0.2f;
        sourceFast.volume = 0;
        sourceSlow.Play(0);
        sourceFast.Play(0);
    }

    void Update() {
        if(fading) {
            float lerpValue = fadeTimeElapsed / fadeInSeconds;
            if(speedingUp) {
                sourceSlow.volume = Mathf.Lerp(0.2f, 0f, lerpValue);
                sourceFast.volume = Mathf.Lerp(0f, 0.2f, lerpValue);
                if(sourceSlow.volume <= 0f) {
                    fading = false;
                }
            } else {
                sourceSlow.volume = Mathf.Lerp(0f, 0.2f, lerpValue);
                sourceFast.volume = Mathf.Lerp(0.2f, 0f, lerpValue);
                if(sourceSlow.volume >= 1f) {
                    fading = false;
                }
            }
            fadeTimeElapsed += Time.deltaTime;
        }
    }

    public void StartFade(bool speedUp) {
        // Just double-checking the duration value is somewhat acceptable
        if(fadeInSeconds <= 0f) {
            fadeInSeconds = 0.5f;
        }
        fadeTimeElapsed = 0;
        fading = true;
        speedingUp = speedUp;
    }

}
