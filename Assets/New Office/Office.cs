using FNAFOnline.Shared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Office : MonoBehaviour
{
    private enum OfficeSideState
    {
        Dark,
        DarkMonsterPresent,
        Light,
        LightMonsterPresent
    }

    // Renderer layout: [[Left renderer] Middle renderer [Right renderer]]
    public SpriteRenderer mainOfficeRenderer;
    public SpriteRenderer leftOfficeRenderer;
    public SpriteRenderer rightOfficeRenderer;
    [Space]
    public SpriteRenderer attackRenderer;
    public SpriteRenderer leftDoorRenderer;
    public SpriteRenderer rightDoorRenderer;
    public SpriteRenderer leftLightButtonRenderer;
    public SpriteRenderer rightLightButtonRenderer;
    public SpriteRenderer leftDoorButtonRenderer;
    public SpriteRenderer rightDoorButtonRenderer;
    [Space]
    public Image powerUsageImage;
    [Header("Whole screen sprites")] // Will disable L and R renderer.
    public Sprite darkerOffice;
    public string lightFlikkerSound;
    public Sprite darkOffice;
    public string goDarkSoundEffect;
    public Sprite monsterDarkOffice;
    [Header("Attack sprites")]
    public string attackSound;
    public Sprite[] freddyAttackFrames;
    public Sprite[] chicaAttackFrames;
    public Sprite[] bonnieAttackFrames;
    public Sprite[] foxyAttackFrames;
    public Sprite goldenFreddyAttack;
    [Header("Middle screen sprites")]
    public string ambientSound;
    public Sprite[] normalOfficeFrames;
    [Header("Left screen sprites")]
    public Sprite lightLeftOffice;
    public Sprite monsterLeftOffice;
    [Header("Right screen sprites")]
    public Sprite lightRightOffice;
    public Sprite monsterRightOffice;
    [Header("Door sprites")]
    public string leftDoorSound;
    public string rightDoorSound;
    public string leftLightSound;
    public string rightLightSound;
    public string windowLeftSound;
    public string windowRightSound;
    public string errorLeftSound;
    public string errorRightSound;
    public Sprite[] leftDoorFrames;
    public Sprite[] rightDoorFrames;
    [Header("Buttons sprites")]
    public Sprite leftDoorButtonOn;
    public Sprite leftDoorButtonOff;
    public Sprite rightDoorButtonOn;
    public Sprite rightDoorButtonOff;
    public Sprite leftLightButtonOn;
    public Sprite leftLightButtonOff;
    public Sprite rightLightButtonOn;
    public Sprite rightLightButtonOff;
    public bool leftButtonsDisabled = false;
    public bool rightButtonsDisabled = false;
    [Header("Power usage sprites")]
    public Sprite[] powerUsageSprites;

    private bool leftDoorClosed = false;
    private bool rightDoorClosed = false;
    private int normalOfficeSpriteIndex = 0;
    private OfficeState state = OfficeState.Normal;
    private OfficeSideState leftState = OfficeSideState.Dark;
    private OfficeSideState rightState = OfficeSideState.Dark;
    private bool sawMonsterLeft = true;
    private bool sawMonsterRight = true;
    private int powerUsage = 0;

    private int leftDoorFrame = 0;
    private int rightDoorFrame = 0;

    void Start()
    {
        SoundEffects.Play(ambientSound);

        attackRenderer.enabled = false;

        //Invoke(nameof(Test), 5f);
        //Invoke(nameof(Test2), 9f);
        //Invoke(nameof(Test3), 13f);
        //Invoke(nameof(Test2), 20f);

        Invoke(nameof(Test), 5f);
    }

    private void Test()
    {
        ShortFlikker();

        SetLeftMonster(true);
        SetRightMonster(false);

        Invoke(nameof(Test2), 5f);
    }

    private void Test2()
    {
        ShortFlikker();

        SetLeftMonster(false);
        SetRightMonster(true);

        Invoke(nameof(Test), 5f);
    }

    void FixedUpdate()
    {
        if (++normalOfficeSpriteIndex >= normalOfficeFrames.Length)
            normalOfficeSpriteIndex = 0;

        // Calculate power usage

        powerUsage = 1
         + ((leftState == OfficeSideState.Light || leftState == OfficeSideState.LightMonsterPresent) ? 1 : 0)
         + ((rightState == OfficeSideState.Light || rightState == OfficeSideState.LightMonsterPresent) ? 1 : 0)
         + (leftDoorClosed ? 1 : 0)
         + (rightDoorClosed ? 1 : 0);

        // Render the main office 

        switch (state)
        {
            case OfficeState.Normal:

                SetSideRenderers(true);

                mainOfficeRenderer.sprite = normalOfficeFrames[normalOfficeSpriteIndex];

                break;

            case OfficeState.LightFlikker:

                if (Random.value > 0.6f)
                {
                    SetSideRenderers(false);

                    mainOfficeRenderer.sprite = darkerOffice;

                    if (Random.value > 0.9f)
                        SoundEffects.Play(lightFlikkerSound);
                }
                else
                {
                    SetSideRenderers(true);

                    mainOfficeRenderer.sprite = normalOfficeFrames[normalOfficeSpriteIndex];
                }

                break;

            case OfficeState.Dark:

                SetSideRenderers(false);

                mainOfficeRenderer.sprite = darkOffice;

                powerUsage = 0;

                break;
        }

        // Render corridors, left and right side of office

        if (leftState == OfficeSideState.DarkMonsterPresent || leftState == OfficeSideState.Dark)
        {
            leftOfficeRenderer.sprite = null;
        }
        else if (leftState == OfficeSideState.Light)
        {
            leftOfficeRenderer.sprite = lightLeftOffice;
        }
        else
        {
            leftOfficeRenderer.sprite = monsterLeftOffice;
        }

        if (rightState == OfficeSideState.DarkMonsterPresent || rightState == OfficeSideState.Dark)
        {
            rightOfficeRenderer.sprite = null;
        }
        else if (rightState == OfficeSideState.Light)
        {
            rightOfficeRenderer.sprite = lightRightOffice;
        }
        else
        {
            rightOfficeRenderer.sprite = monsterRightOffice;
        }

        // Render doors

        if (rightDoorClosed)
        {
            if (rightDoorFrame < rightDoorFrames.Length - 1)
                rightDoorFrame++;
        }
        else
        {
            if (rightDoorFrame > 0)
                rightDoorFrame--;
        }

        if (leftDoorClosed)
        {
            if (leftDoorFrame < leftDoorFrames.Length - 1)
                leftDoorFrame++;
        }
        else
        {
            if (leftDoorFrame > 0)
                leftDoorFrame--;
        }

        rightDoorRenderer.sprite = rightDoorFrames[rightDoorFrame];
        leftDoorRenderer.sprite = leftDoorFrames[leftDoorFrame];

        // Play saw monster sounds

        if (!sawMonsterRight && (rightState == OfficeSideState.Light || rightState == OfficeSideState.LightMonsterPresent))
        {
            sawMonsterRight = true;

            SoundEffects.Play(windowRightSound);
        }

        if (!sawMonsterLeft && (leftState == OfficeSideState.Light || leftState == OfficeSideState.LightMonsterPresent))
        {
            sawMonsterLeft = true;

            SoundEffects.Play(windowLeftSound);
        }

        // Show power usage

        if (powerUsage < 1)
            powerUsageImage.sprite = null;
        else
            powerUsageImage.sprite = powerUsageSprites[powerUsage - 1];
    }

    private void SetSideRenderers(bool enabled)
    {
        leftOfficeRenderer.enabled = enabled;
        rightOfficeRenderer.enabled = enabled;
    }

    public void SetRightMonster(bool monsterPresent)
    {
        sawMonsterRight = !monsterPresent;

        if (monsterPresent && rightState == OfficeSideState.Dark)
            rightState = OfficeSideState.DarkMonsterPresent;
        else if (monsterPresent && rightState == OfficeSideState.Light)
            rightState = OfficeSideState.LightMonsterPresent;
        else if (!monsterPresent && rightState == OfficeSideState.DarkMonsterPresent)
            rightState = OfficeSideState.Dark;
        else if (!monsterPresent && rightState == OfficeSideState.LightMonsterPresent)
            rightState = OfficeSideState.Light;
    }

    public void SetLeftMonster(bool monsterPresent)
    {
        sawMonsterLeft = !monsterPresent;

        if (monsterPresent && leftState == OfficeSideState.Dark)
            leftState = OfficeSideState.DarkMonsterPresent;
        else if (monsterPresent && leftState == OfficeSideState.Light)
            leftState = OfficeSideState.LightMonsterPresent;
        else if (!monsterPresent && leftState == OfficeSideState.DarkMonsterPresent)
            leftState = OfficeSideState.Dark;
        else if (!monsterPresent && leftState == OfficeSideState.LightMonsterPresent)
            leftState = OfficeSideState.Light;
    }

    public void SetRightLight(bool on)
    {
        if (on && rightState == OfficeSideState.Dark)
            rightState = OfficeSideState.Light;
        else if (on && rightState == OfficeSideState.DarkMonsterPresent)
            rightState = OfficeSideState.LightMonsterPresent;
        else if (!on && rightState == OfficeSideState.Light)
            rightState = OfficeSideState.Dark;
        else if (!on && rightState == OfficeSideState.LightMonsterPresent)
            rightState = OfficeSideState.DarkMonsterPresent;

        if (on)
            SoundEffects.Play(rightLightSound);
        else
            SoundEffects.Stop(rightLightSound);

        rightLightButtonRenderer.sprite = on ? rightLightButtonOn : rightLightButtonOff;
    }

    public void SetLeftLight(bool on)
    {
        if (on && leftState == OfficeSideState.Dark)
            leftState = OfficeSideState.Light;
        else if (on && leftState == OfficeSideState.DarkMonsterPresent)
            leftState = OfficeSideState.LightMonsterPresent;
        else if (!on && leftState == OfficeSideState.Light)
            leftState = OfficeSideState.Dark;
        else if (!on && leftState == OfficeSideState.LightMonsterPresent)
            leftState = OfficeSideState.DarkMonsterPresent;

        if (on)
            SoundEffects.Play(leftLightSound);
        else
            SoundEffects.Stop(leftLightSound);

        leftLightButtonRenderer.sprite = on ? leftLightButtonOn : leftLightButtonOff;
    }

    public void SetRightDoor(bool closed)
    {
        if (rightDoorClosed != closed)
        {
            SoundEffects.Play(rightDoorSound);
        }

        rightDoorClosed = closed;

        rightDoorButtonRenderer.sprite = rightDoorClosed ? rightDoorButtonOn : rightDoorButtonOff;
    }

    public void SetLeftDoor(bool closed)
    {
        if (leftDoorClosed != closed)
        {
            SoundEffects.Play(leftDoorSound);
        }

        leftDoorClosed = closed;

        leftDoorButtonRenderer.sprite = leftDoorClosed ? leftDoorButtonOn : leftDoorButtonOff;
    }

    public void SetOfficeState(OfficeState newState)
    {
        state = newState;

        if (newState == OfficeState.Dark)
        {
            SoundEffects.Stop(ambientSound);
            SoundEffects.Play(goDarkSoundEffect);
        }
        else
        {
            SoundEffects.Play(ambientSound);
        }
    }

    public bool TryAttack(MonsterType monster)
    {
        if ((monster == MonsterType.Chica || monster == MonsterType.Freddy) && rightDoorClosed)
            return false;
        if ((monster == MonsterType.Foxy || monster == MonsterType.Bonnie) && leftDoorClosed)
            return false;

        ForceAttack(monster);

        return true;
    }

    public void ForceAttack(MonsterType monster)
    {
        StartCoroutine(IAttack(monster));
    }

    public void PressLeftDoorButton()
    {
        if (leftButtonsDisabled && !leftDoorClosed)
        {
            SoundEffects.Play(errorLeftSound);

            return;
        }

        SetLeftDoor(!leftDoorClosed);
    }

    public void PressRightDoorButton()
    {
        if (rightButtonsDisabled && !rightDoorClosed)
        {
            SoundEffects.Play(errorRightSound);

            return;
        }

        SetRightDoor(!rightDoorClosed);
    }

    public void PressLeftLightButton()
    {
        if (leftButtonsDisabled && (leftState == OfficeSideState.Dark || leftState == OfficeSideState.DarkMonsterPresent))
        {
            SoundEffects.Play(errorLeftSound);

            return;
        }

        bool newLightState = !(leftState == OfficeSideState.Light || leftState == OfficeSideState.LightMonsterPresent);
        SetLeftLight(newLightState);
    }

    public void PressRightLightButton()
    {
        if (rightButtonsDisabled && (rightState == OfficeSideState.Dark || rightState == OfficeSideState.DarkMonsterPresent))
        {
            SoundEffects.Play(errorRightSound);

            return;
        }

        bool newLightState = !(rightState == OfficeSideState.Light || rightState == OfficeSideState.LightMonsterPresent);
        SetRightLight(newLightState);
    }

    public void ShortFlikker()
    {
        SetOfficeState(OfficeState.LightFlikker);

        Invoke(nameof(StopFlikker), 1f);
    }

    public void StopFlikker()
    {
        SetOfficeState(OfficeState.Normal);
    }

    public int GetPowerUsage()
    {
        return powerUsage;
    }

    private IEnumerator IAttack(MonsterType monster)
    {
        SoundEffects.Play(attackSound);

        attackRenderer.enabled = true;

        const float AttackLength = 0.8f;
        const int Loops = 30;

        if (monster == MonsterType.GoldenFreddy)
        {
            mainOfficeRenderer.sprite = goldenFreddyAttack;

            yield return new WaitForSeconds(AttackLength);

            attackRenderer.enabled = false;

            yield break; 
        }

        Sprite[] frames;

        if (monster == MonsterType.Bonnie)
            frames = bonnieAttackFrames;
        else if (monster == MonsterType.Chica)
            frames = chicaAttackFrames;
        else if (monster == MonsterType.Foxy)
            frames = foxyAttackFrames;
        else
            frames = freddyAttackFrames;

        for(int i = 0; i < Loops; i++)
        {
            attackRenderer.sprite = frames[i % frames.Length];

            yield return new WaitForSeconds(AttackLength / Loops);
        }

        attackRenderer.enabled = false;
    }

    public bool GetRightDoor()
    {
        return rightDoorClosed;
    }

    public bool GetLeftDoor()
    {
        return leftDoorClosed;
    }

    public bool GetRightLight()
    {
        return (rightState == OfficeSideState.Light || rightState == OfficeSideState.LightMonsterPresent);
    }

    public bool GetLeftLight()
    {
        return (leftState == OfficeSideState.Light || leftState == OfficeSideState.LightMonsterPresent);
    }
}

public enum OfficeState
{
    Normal,
    LightFlikker,
    Dark
}