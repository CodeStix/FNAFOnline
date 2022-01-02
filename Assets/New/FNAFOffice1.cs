using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFOffice1 : MonoBehaviour
{
    public AudioSource fanSound;
    public FNAFAnimatedSprite fanRenderer;
    [Space]
    public FNAFBetweenAnimation rightDoor;
    public SpriteRenderer officeRenderer;
    public Sprite normalOfficeSprite;
    public Sprite darkOfficeSprite;
    public SpriteRenderer rightOfficeRenderer;
    public Sprite rightOfficeNormalSprite;
    public Sprite rightOfficeMonsterSprite;
    public Sprite rightOfficeLightSprite;
    public SpriteRenderer rightLightButtonRenderer;
    public Sprite rightLightButtonOnSprite;
    public Sprite rightLightButtonOffSprite;
    public SpriteRenderer rightDoorButtonRenderer;
    public Sprite rightDoorButtonOnSprite;
    public Sprite rightDoorButtonOffSprite;
    public FNAFBetweenAnimation leftDoor;
    public SpriteRenderer leftOfficeRenderer;
    public Sprite leftOfficeNormalSprite;
    public Sprite leftOfficeMonsterSprite;
    public Sprite leftOfficeLightSprite;
    public SpriteRenderer leftLightButtonRenderer;
    public Sprite leftLightButtonOnSprite;
    public Sprite leftLightButtonOffSprite;
    public SpriteRenderer leftDoorButtonRenderer;
    public Sprite leftDoorButtonOnSprite;
    public Sprite leftDoorButtonOffSprite;
    public AudioSource lightSound;
    public AudioSource doorSound;
    [Range(0f, 1f)]
    public float lightRandomThreshold = 0.85f;
    [Space]
    public FNAFBetweenAnimation monitor;
    public AudioSource monitorSound;
    public GameObject monitorEnableObject;
    public AudioSource cameraSwitchSound;
    [Tooltip("1A, 1B, 1C, 2A, 2B, 3, 4A, 4B, 5, 6, 7")]
    public FNAFOffice1Camera[] cameras;
    public FNAFAnimatedSprite cameraSwitchEffect;

    private bool canToggleMonitor = true;
    private bool monitorOpen = false;
    private int fanFrame = 0;
    private bool enableFan = true;
    private bool leftLight = false;
    private bool leftDoorDown = false;
    private bool rightLight = false;
    private bool rightDoorDown = false;
    private int currentCamera = 0;

    private void Start()
    {
        UpdateCameras();
    }

    private void Update()
    {
        if (enableFan)
        {
            fanRenderer.gameObject.SetActive(true);
            if (!fanSound.isPlaying)
                fanSound.Play();
        }
        else
        {
            fanRenderer.gameObject.SetActive(false);
            if (fanSound.isPlaying)
                fanSound.Stop();
        }

        if (leftLight)
        {
            leftOfficeRenderer.sprite = Random.value > lightRandomThreshold ? leftOfficeLightSprite : leftOfficeNormalSprite;
            leftLightButtonRenderer.sprite = leftLightButtonOnSprite;
        }
        else
        {
            leftOfficeRenderer.sprite = leftOfficeNormalSprite;
            leftLightButtonRenderer.sprite = leftLightButtonOffSprite;
        }

        if (rightLight)
        {
            rightOfficeRenderer.sprite = Random.value > lightRandomThreshold ? rightOfficeLightSprite : rightOfficeNormalSprite;
            rightLightButtonRenderer.sprite = rightLightButtonOnSprite;
        }
        else
        {
            rightOfficeRenderer.sprite = rightOfficeNormalSprite;
            rightLightButtonRenderer.sprite = rightLightButtonOffSprite;
        }

        if (leftLight || rightLight)
        {
            if (!lightSound.isPlaying)
                lightSound.Play();
        }
        else
        {
            if (lightSound.isPlaying)
                lightSound.Stop();
        }
    }

    public void ToggleMonitor()
    {
        if (!canToggleMonitor) return;
        canToggleMonitor = false;
        monitorOpen = !monitorOpen;
        if (monitorOpen)
        {
            monitorSound.Play();
            monitor.End();
            Invoke(nameof(EnableMonitor), 0.25f);
        }
        else
        {
            monitorSound.Stop();
            monitor.Start();
            monitorEnableObject.SetActive(false);
            Invoke(nameof(DisableMonitor), 0.25f);
        }
    }

    private void DisableMonitor()
    {
        canToggleMonitor = true;
    }

    private void EnableMonitor()
    {
        monitorEnableObject.SetActive(true);
        cameraSwitchSound.Play();
        cameraSwitchEffect.Play();
        canToggleMonitor = true;
    }

    public void LeftLightToggle()
    {
        if (monitorOpen) return;
        leftLight = !leftLight;
    }

    public void LeftDoorToggle()
    {
        if (monitorOpen) return;
        doorSound.Play();
        leftDoorDown = !leftDoorDown;
        if (leftDoorDown)
        {
            leftDoorButtonRenderer.sprite = leftDoorButtonOnSprite;
            leftDoor.End();
        }
        else
        {
            leftDoorButtonRenderer.sprite = leftDoorButtonOffSprite;
            leftDoor.Start();
        }
    }

    public void RightLightToggle()
    {
        if (monitorOpen) return;
        rightLight = !rightLight;
    }

    public void RightDoorToggle()
    {
        if (monitorOpen) return;
        doorSound.Play();
        rightDoorDown = !rightDoorDown;
        if (rightDoorDown)
        {
            rightDoorButtonRenderer.sprite = rightDoorButtonOnSprite;
            rightDoor.End();
        }
        else
        {
            rightDoorButtonRenderer.sprite = rightDoorButtonOffSprite;
            rightDoor.Start();
        }
    }

    public void SwitchCamera(int index)
    {
        if (currentCamera == index) return;

        cameraSwitchSound.Play();
        cameraSwitchEffect.Play();
        currentCamera = index;

        UpdateCameras();
    }

    private void UpdateCameras()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == currentCamera)
                cameras[i].gameObject.SetActive(true);
            else
                cameras[i].gameObject.SetActive(false);
        }
    }
}
