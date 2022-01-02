using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public Text cameraNameText;
    public FNAFMonsterLocation[] freddyLocations;
    public FNAFMonsterLocation[] chicaLocations;
    public FNAFMonsterLocation[] bonnieLocations;
    public FNAFMonsterLocation[] foxyLocations;

    private int freddyLocationIndex = 0;
    private int freddyLocationState = 0;
    private int chicaLocationIndex = 0;
    private int chicaLocationState = 0;
    private int bonnieLocationIndex = 0;
    private int bonnieLocationState = 0;
    private int foxyLocationIndex = 0;
    private int foxyLocationState = 0;
    private bool canToggleMonitor = true;
    private bool monitorOpen = false;
    private bool enableFan = true;
    private bool leftLight = false;
    private bool leftDoorDown = false;
    private bool rightLight = false;
    private bool rightDoorDown = false;
    private int currentCamera = 0;
    private int guardCurrentCamera = 0;

    private void Start()
    {
        // for testing purposes

        Invoke(nameof(TestJoinRoom), 1.0f);

        UpdateCameras();

        freddyLocations[0].SetState(0);
        chicaLocations[0].SetState(0);
        bonnieLocations[0].SetState(0);
        foxyLocations[0].SetState(0);

    }

    private void TestJoinRoom()
    {
        FNAFClient.Instance.JoinRoom(null);
        Invoke(nameof(TestStartGame), 1.0f);
    }

    private void TestStartGame()
    {
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;
        FNAFClient.Instance.StartGameRequest(true);
    }

    private void OnEnable()
    {
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;
    }

    private void OnDisable()
    {
        FNAFClient.Instance.OnRoomChangeEvent -= Instance_OnRoomChangeEvent;
    }

    private void Instance_OnRoomChangeEvent(object sender, FNAFRoomChangeEvent e)
    {
        // Sync office to received room
        FNAF1Game game = e.room.game;

        if (e.eventType == "move")
        {
            if (freddyLocationIndex != game.freddyLocation || freddyLocationState != game.freddyLocationState)
            {
                freddyLocations[freddyLocationIndex].SetState(-1);
                freddyLocationIndex = game.freddyLocation;
                freddyLocationState = game.freddyLocationState;
                freddyLocations[freddyLocationIndex].SetState(freddyLocationState);
            }

            if (chicaLocationIndex != game.chicaLocation || chicaLocationState != game.chicaLocationState)
            {
                chicaLocations[chicaLocationIndex].SetState(-1);
                chicaLocationIndex = game.chicaLocation;
                chicaLocationState = game.chicaLocationState;
                chicaLocations[chicaLocationIndex].SetState(chicaLocationState);
            }

            if (bonnieLocationIndex != game.bonnieLocation || bonnieLocationState != game.bonnieLocationState)
            {
                bonnieLocations[bonnieLocationIndex].SetState(-1);
                bonnieLocationIndex = game.bonnieLocation;
                bonnieLocationState = game.bonnieLocationState;
                bonnieLocations[bonnieLocationIndex].SetState(bonnieLocationState);
            }

            if (foxyLocationIndex != game.foxyLocation || foxyLocationState != game.foxyLocationState)
            {
                foxyLocations[foxyLocationIndex].SetState(-1);
                foxyLocationIndex = game.foxyLocation;
                foxyLocationState = game.foxyLocationState;
                foxyLocations[foxyLocationIndex].SetState(foxyLocationState);
            }
        }
        else if (e.eventType == "officeChange")
        {
            if (rightDoorDown != game.rightDoor)
            {
                doorSound.Play();
                if (game.rightDoor)
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

            if (leftDoorDown != game.leftDoor)
            {
                doorSound.Play();
                if (game.leftDoor)
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

            rightLight = game.rightLight;
            leftLight = game.leftLight;
            rightDoorDown = game.rightDoor;
            leftDoorDown = game.leftDoor;
            guardCurrentCamera = game.selectedCameraNumber;
        }
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

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, rightLight, rightDoorDown, monitorOpen ? currentCamera : -1);
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
        FNAFClient.Instance.FNAF1RequestOfficeChange(!leftLight, leftDoorDown, rightLight, rightDoorDown, guardCurrentCamera);
    }

    public void LeftDoorToggle()
    {
        if (monitorOpen) return;
        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, !leftDoorDown, rightLight, rightDoorDown, guardCurrentCamera);
    }

    public void RightLightToggle()
    {
        if (monitorOpen) return;
        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, !rightLight, rightDoorDown, guardCurrentCamera);
    }

    public void RightDoorToggle()
    {
        if (monitorOpen) return;

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, rightLight, !rightDoorDown, guardCurrentCamera);
    }

    public void SwitchCamera(int index)
    {
        if (currentCamera == index) return;

        cameraSwitchSound.Play();
        cameraSwitchEffect.Play();
        currentCamera = index;
        cameraNameText.text = cameras[index].cameraName;

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, rightLight, rightDoorDown, index);

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
