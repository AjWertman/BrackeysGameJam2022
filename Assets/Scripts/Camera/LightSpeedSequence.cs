using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightSpeedSequence : MonoBehaviour
{
    [SerializeField] AudioClip lightSpeedSoundFX = null;
    [SerializeField] GameObject directionalLight = null;
    [SerializeField] GameObject lavaToShutoff = null;
    [SerializeField] float timeToLightSpeed = .4f;

    [SerializeField] GameObject reticle = null;

    Camera mainCam = null;
    Volume volume = null;

    WhaleManager whaleManager = null;
    Fader fader;
    MusicPlayer musicPlayer = null;
    SoundFXManager soundFXManager = null;

    bool sequenceStarted = false;

    LensDistortion lensDistortion = null;

    private void Awake()
    {
        mainCam = Camera.main;
        whaleManager = FindObjectOfType<WhaleManager>();
        fader = FindObjectOfType<Fader>();
        musicPlayer = FindObjectOfType<MusicPlayer>();
        soundFXManager = FindObjectOfType<SoundFXManager>();

        volume = mainCam.GetComponent<Volume>();

        LensDistortion tempLD;

        if (volume.profile.TryGet<LensDistortion>(out tempLD))
        {
            lensDistortion = tempLD;
        }

        lensDistortion.active = false;
    }

    public IEnumerator ActivateLightSpeedSequence(PlayerController playerController)
    {
        if (!sequenceStarted)
        {
            sequenceStarted = true;

            musicPlayer.Pause();
            soundFXManager.CreateSoundFX(lightSpeedSoundFX, transform, .5f);

            yield return LightSpeed(true);
            lavaToShutoff.gameObject.SetActive(false);

            yield return new WaitForSeconds(1f);

            directionalLight.SetActive(false);
            playerController.SetPlayerPhase(PlayerPhase.Three);
            whaleManager.TurnOffWhalePhase();

            yield return new WaitForSeconds(1f);
            StartCoroutine(LightSpeed(false));
            reticle.SetActive(true);
        }
    }

    public IEnumerator LightSpeed(bool isActivating)
    {
        float intensityValue = lensDistortion.intensity.value;
        float scaleValue = lensDistortion.scale.value;

        if (isActivating)
        {
            lensDistortion.active = true;
            while (intensityValue > -1)
            {
                intensityValue -= Time.deltaTime / timeToLightSpeed;
                lensDistortion.intensity.value = intensityValue;
                yield return null;
            }

            while (scaleValue > .01)
            {
                scaleValue -= Time.deltaTime / .2f;
                lensDistortion.scale.value = scaleValue;
                yield return null;
            }
        }
        else
        {
            while (scaleValue < 1)
            {
                scaleValue += Time.deltaTime / .2f;
                lensDistortion.scale.value = scaleValue;
                yield return null;
            }

            while (intensityValue < 0)
            {
                intensityValue += Time.deltaTime / timeToLightSpeed;
                lensDistortion.intensity.value = intensityValue;
                yield return null;
            }

            lensDistortion.active = false;
        }

        yield return null;
    }
}
