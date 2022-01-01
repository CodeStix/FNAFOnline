using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FNAFOffice1 : MonoBehaviour
{
    public AudioSource fanSound;
    public Sprite[] fanTextures;
    public SpriteRenderer fanRenderer;
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

    private bool monitorOpen = false;
    private int fanFrame = 0;
    private bool enableFan = true;
    private bool leftLight = false;
    private bool leftDoorDown = false;
    private bool rightLight = false;
    private bool rightDoorDown = false;


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
        monitorOpen = !monitorOpen;
        if (monitorOpen)
        {
            monitor.Start();
        }
        else
        {
            monitor.End();
        }
        monitorSound.Play();
    }

    public void LeftLightToggle()
    {
        leftLight = !leftLight;
    }

    public void LeftDoorToggle()
    {
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
        rightLight = !rightLight;
    }

    public void RightDoorToggle()
    {
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
}
