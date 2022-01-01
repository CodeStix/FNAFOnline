using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFOffice1 : MonoBehaviour
{
    public AudioSource fanSound;
    public Sprite[] fanTextures;
    public SpriteRenderer fanRenderer;
    [Space]
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
    [Space]
    public FNAFOfficeMonitor monitor;

    private bool monitorOpen = false;
    private int fanFrame = 0;
    private bool enableFan = true;
    private bool leftLight = false;
    private bool leftDoor = false;
    private bool rightLight = false;
    private bool rightDoor = false;

    private const float LIGHT_RANDOM_THRESHOLD = 0.8f;

    private void Start()
    {
        
    }

    private void Update()
    {
        if (enableFan)
        {
            fanRenderer.enabled = true;
            if (!fanSound.isPlaying)
                fanSound.Play();
            fanRenderer.sprite = fanTextures[fanFrame];
            if (++fanFrame >= fanTextures.Length)
                fanFrame = 0;
        }
        else
        {
            fanRenderer.enabled = false;
            if (fanSound.isPlaying)
                fanSound.Stop();
        }

        if (leftLight)
        {
            leftOfficeRenderer.sprite = Random.value > LIGHT_RANDOM_THRESHOLD ? leftOfficeLightSprite : leftOfficeNormalSprite;
            leftLightButtonRenderer.sprite = leftLightButtonOnSprite;
        }
        else
        {
            leftOfficeRenderer.sprite = leftOfficeNormalSprite;
            leftLightButtonRenderer.sprite = leftLightButtonOffSprite;
        }

        if (rightLight)
        {
            rightOfficeRenderer.sprite = Random.value > LIGHT_RANDOM_THRESHOLD ? rightOfficeLightSprite : rightOfficeNormalSprite;
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
        monitorOpen = !monitorOpen;
        if (monitorOpen)
            monitor.Up();
        else
            monitor.Down();
    }

    public void LeftLightToggle()
    {
        leftLight = !leftLight;
    }

    public void LeftDoorToggle()
    {
        leftDoor = !leftDoor;
    }

    public void RightLightToggle()
    {
        rightLight = !rightLight;
    }

    public void RightDoorToggle()
    {
        rightDoor = !rightDoor;
    }
}
