// mache es eigentlich nur damit kerem abi es sieht
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PushSound : MonoBehaviour
{
    [Header("Sound Clips")]
    public AudioClip[] pushSounds; // Array 
    public bool loopSoundWhilePushing = false; // Loop fürs schieben

    [Header("Collision Settings")]
    public float minCollisionForce = 0.5f; // Aufprall für Sound
    public string[] allowedTags; // Optional: Beschränke auf Tags

    [Header("Fall Behavior")]
    public bool disableSoundAtSceneStart = true; // Mach es lieber an 
    public float sceneStartDisableDuration = 2.0f; 
    public float minFallHeight = -10f; 

    private AudioSource audioSource;
    private bool isPushing = false;
    private float sceneStartTime;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = loopSoundWhilePushing; 

        sceneStartTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (ShouldPlaySound(collision))
        {
            PlayRandomSound();
        }
    }

    void OnCollisionStay(Collision collision)
    {
        if (loopSoundWhilePushing && collision.relativeVelocity.magnitude > minCollisionForce)
        {
            if (!audioSource.isPlaying && ShouldPlaySound(collision))
            {
                PlayRandomSound();
            }
        }
    }
    // mann kann auch trigger nutzen aber es macht kein sinn
    void OnCollisionExit(Collision collision)
    {
        if (loopSoundWhilePushing && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private bool ShouldPlaySound(Collision collision)
    {
        if (disableSoundAtSceneStart && Time.time - sceneStartTime < sceneStartDisableDuration)
        {
            return false;
        }

        if (transform.position.y < minFallHeight)
        {
            return false;
        }

        return collision.relativeVelocity.magnitude > minCollisionForce && IsTagAllowed(collision.gameObject.tag);
    }

    private void PlayRandomSound()
    {
        if (pushSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, pushSounds.Length);
            audioSource.clip = pushSounds[randomIndex];
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("Keine Sounds im Array 'pushSounds' vorhanden!");
        }
    }

    private bool IsTagAllowed(string tag)
    {
        if (allowedTags.Length == 0) return true; 
        foreach (string allowedTag in allowedTags)
        {
            if (tag == allowedTag) return true;
        }
        return false;
    }
}
