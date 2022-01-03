using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class FNAFMoveSounds
{
    [Range(0f, 1f)]
    public float farChance = 0.3f;
    public AudioSource[] farSounds;
    [Range(0f, 1f)]
    public float closeChance = 0.7f;
    public AudioSource[] closeSounds;

    public void PlayFar()
    {
        if (Random.value > farChance) return;
        farSounds[Random.Range(0, farSounds.Length)].Play();
    }

    public void PlayClose()
    {
        if (Random.value > closeChance) return;
        closeSounds[Random.Range(0, closeSounds.Length)].Play();
    }
}

public class FNAFOffice1 : MonoBehaviour
{
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
    public AudioSource windowScareSound;
    [Range(0f, 1f)]
    public float lightRandomThreshold = 0.85f;
    [Space]
    public FNAFBetweenAnimation monitor;
    public AudioSource monitorSound;
    public GameObject monitorEnableObject;
    public AudioSource cameraSwitchSound;
    [Tooltip("1A, 1B, 1C, 2A, 2B, 3, 4A, 4B, 5, 6, 7")]
    public FNAFOffice1Camera[] cameras;
    public FNAFAnimatedSprite[] cameraButtons;
    public FNAFAnimatedSprite cameraSwitchEffect;
    public Text cameraNameText;
    public int bonnieAttackLocationIndex;
    public int chicaAttackLocationIndex;
    public int freddyAttackLocationIndex;
    public int foxyAttackLocationIndex;
    public FNAFMonsterLocation[] freddyLocations;
    public FNAFMonsterLocation[] chicaLocations;
    public FNAFMonsterLocation[] bonnieLocations;
    public FNAFMonsterLocation[] foxyLocations;
    public FNAFMoveSounds freddyMoveSounds;
    public FNAFMoveSounds chicaMoveSounds;
    public FNAFMoveSounds bonnieMoveSounds;
    public FNAFMoveSounds foxyMoveSounds;
    public Button freddyAttackButton;
    public Button chicaAttackButton;
    public Button bonnieAttackButton;
    public Button foxyAttackButton;
    public GameObject freddyJumpScare;
    public GameObject chicaJumpScare;
    public GameObject bonnieJumpScare;
    public GameObject foxyJumpScare;
    public GameObject deathOverlay;
    public GameObject aftonGuardView;
    public GameObject aftonFreddyJumpScare;
    public GameObject aftonChicaJumpScare;
    public GameObject aftonBonnieJumpScare;
    public GameObject aftonFoxyJumpScare;
    public GameObject aftonDeathOverlay;
    [Space]
    public Text hourText;
    public Text powerLeftText;
    public Image usageImage;
    public Sprite[] usageSprites;
    public Image timerImage;
    public Text timerText;
    public AudioSource timerSound;
    public AudioSource timerDoneSound;
    public GameObject[] requiresPower; // Objects that will be disabled when power is zero
    public GameObject[] requiresPowerAfton; 
    public AudioSource powerErrorSound;
    public AudioSource powerDownSound;
    [Space]
    public UnityEvent onMoveAnyone;
    public UnityEvent onJumpscare;
    public UnityEvent whenGuard;

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
    private bool leftLight = false;
    private bool leftDoorDown = false;
    private bool rightLight = false;
    private bool rightDoorDown = false;
    private int currentCamera = 0;
    private int guardCurrentCamera = 0;
    private bool sawChica = false;
    private bool sawBonnie = false;
    private float moveTimer = 0f;
    private bool moveTimerDone = true;
    private float powerLeft = 100f;

    private string role;
    private bool isAfton = false;

    public bool MayMove => moveTimerDone;

    private readonly string[] HOUR_NAMES = new[] { "12 PM", "1 AM", "2 AM", "3 AM", "4 AM", "5 AM", "6 AM", "7 AM", "8 AM", "9 AM", "10 AM", "11 AM", "12 AM", "1 PM", "2 PM", "3 PM", "4 PM" };

    private void Start()
    {
        if (FNAFClient.Instance == null || FNAFClient.Instance.GetRoom() == null)
        {
            Debug.Log("Joining random new room for testing");
            Invoke(nameof(TestJoinRoom), 1.0f);
        }
        else
        {
            role = FNAFClient.Instance.GetRoom().users.First((e) => e.user.id == FNAFClient.Instance.GetUser().id).role;
            isAfton = role == "afton";
            if (!isAfton) whenGuard.Invoke();

            if (isAfton)
                StartTimer(FNAFClient.Instance.GetRoom().game.settings.initialPower);

            Debug.Log("Starting game, role = " + role);
        }

        freddyAttackButton.onClick.AddListener(() => FNAFClient.Instance.FNAF1RequestAttack("Freddy"));
        chicaAttackButton.onClick.AddListener(() => FNAFClient.Instance.FNAF1RequestAttack("Chica"));
        bonnieAttackButton.onClick.AddListener(() => FNAFClient.Instance.FNAF1RequestAttack("Bonnie"));
        foxyAttackButton.onClick.AddListener(() => FNAFClient.Instance.FNAF1RequestAttack("Foxy"));

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
        FNAFClient.Instance.OnFNAF1MoveResponse += Instance_OnFNAF1MoveResponse;
        FNAFClient.Instance.StartGameRequest(true);
        Invoke(nameof(TestStartedGame), 2.0f);
    }

    private void TestStartedGame()
    {
        role = FNAFClient.Instance.GetRoom().users.First((e) => e.user.id == FNAFClient.Instance.GetUser().id).role;
        isAfton = role == "afton";
        if (!isAfton) whenGuard.Invoke();
        if (isAfton)
            StartTimer(FNAFClient.Instance.GetRoom().game.settings.startingMoveTime);
        Debug.Log("Joined random new room for testing, role = " + role);
    }

    private void OnEnable()
    {
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;
        FNAFClient.Instance.OnFNAF1MoveResponse += Instance_OnFNAF1MoveResponse;
    }

    private void OnDisable()
    {
        FNAFClient.Instance.OnRoomChangeEvent -= Instance_OnRoomChangeEvent;
        FNAFClient.Instance.OnFNAF1MoveResponse -= Instance_OnFNAF1MoveResponse;
    }

    private void Instance_OnFNAF1MoveResponse(object sender, FNAF1MoveResponse e)
    {
        // This function gets called if the server processed this clients animatronic move request
        if (e.ok)
        {
            onMoveAnyone?.Invoke();
            StartTimer(e.cooldownTime);
        }
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
                if (freddyLocations[freddyLocationIndex].isFar)
                    freddyMoveSounds.PlayFar();
                else
                    freddyMoveSounds.PlayClose();
            }

            if (chicaLocationIndex != game.chicaLocation || chicaLocationState != game.chicaLocationState)
            {
                sawChica = false;
                chicaLocations[chicaLocationIndex].SetState(-1);
                chicaLocationIndex = game.chicaLocation;
                chicaLocationState = game.chicaLocationState;
                chicaLocations[chicaLocationIndex].SetState(chicaLocationState);
                if (chicaLocations[chicaLocationIndex].isFar)
                    chicaMoveSounds.PlayFar();
                else
                    chicaMoveSounds.PlayClose();
            }

            if (bonnieLocationIndex != game.bonnieLocation || bonnieLocationState != game.bonnieLocationState)
            {
                sawBonnie = false;
                bonnieLocations[bonnieLocationIndex].SetState(-1);
                bonnieLocationIndex = game.bonnieLocation;
                bonnieLocationState = game.bonnieLocationState;
                bonnieLocations[bonnieLocationIndex].SetState(bonnieLocationState);
                if (bonnieLocations[bonnieLocationIndex].isFar)
                    bonnieMoveSounds.PlayFar();
                else
                    bonnieMoveSounds.PlayClose();
            }

            if (foxyLocationIndex != game.foxyLocation || foxyLocationState != game.foxyLocationState)
            {
                foxyLocations[foxyLocationIndex].SetState(-1);
                foxyLocationIndex = game.foxyLocation;
                foxyLocationState = game.foxyLocationState;
                foxyLocations[foxyLocationIndex].SetState(foxyLocationState);
                if (foxyLocations[foxyLocationIndex].isFar)
                    foxyMoveSounds.PlayFar();
                else
                    foxyMoveSounds.PlayClose();
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
        else if (e.eventType == "tick")
        {
            hourText.text = HOUR_NAMES[game.currentHour];
            powerLeftText.text = "Power left: <b>" + Mathf.Floor(game.powerLeft) + "</b>%";

            if (game.powerLeft <= 0f && powerLeft > 0f)
            {
                powerDownSound.Play();
                if (rightDoorDown)
                {
                    rightDoor.Start();
                    doorSound.Play();
                }
                if (leftDoorDown)
                {
                    leftDoor.Start();
                    doorSound.Play();
                }
                
                foreach (GameObject obj in isAfton ? requiresPowerAfton : requiresPower)
                {
                    obj.SetActive(false);
                }
            }
            else if (game.powerLeft > 0f && powerLeft <= 0f)
            {
                foreach (GameObject obj in isAfton ? requiresPowerAfton : requiresPower)
                {
                    obj.SetActive(true);
                }
            }

            powerLeft = game.powerLeft;
        }
        else if (e.eventType == "end")
        {
            LoadingScreen.LoadScene("Lobby");
        } 
        else if (e.eventType == "attack")
        {
            if (isAfton)
                StartCoroutine(AftonJumpscareSequence(game.attackingMonster));
            else
                StartCoroutine(JumpscareSequence(game.attackingMonster));
        }
    }

    private IEnumerator AftonJumpscareSequence(string monster)
    {
        aftonGuardView.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        switch (monster)
        {
            case "Freddy":
                aftonFreddyJumpScare.SetActive(true);
                break;
            case "Chica":
                aftonChicaJumpScare.SetActive(true);
                break;
            case "Bonnie":
                aftonBonnieJumpScare.SetActive(true);
                break;
            case "Foxy":
                aftonFoxyJumpScare.SetActive(true);
                break;
        }
        yield return new WaitForSeconds(1.0f);
        aftonFreddyJumpScare.SetActive(false);
        aftonChicaJumpScare.SetActive(false);
        aftonBonnieJumpScare.SetActive(false);
        aftonFoxyJumpScare.SetActive(false);
        aftonDeathOverlay.SetActive(true);
        yield return new WaitForSeconds(4.0f);
        //onWin?.Invoke();
    }

    private IEnumerator JumpscareSequence(string monster)
    {
        yield return new WaitForSeconds(2.0f);
        onJumpscare?.Invoke();
        switch (monster)
        {
            case "Freddy":
                freddyJumpScare.SetActive(true);
                break;
            case "Chica":
                chicaJumpScare.SetActive(true);
                break;
            case "Bonnie":
                bonnieJumpScare.SetActive(true);
                break;
            case "Foxy":
                foxyJumpScare.SetActive(true);
                break;
        }
        yield return new WaitForSeconds(1.0f);
        freddyJumpScare.SetActive(false);
        chicaJumpScare.SetActive(false);
        bonnieJumpScare.SetActive(false);
        foxyJumpScare.SetActive(false);
        deathOverlay.SetActive(true);
    }

    private void Update()
    {
        int usage = 0;
        if (rightDoorDown)
            usage++;
        if (leftDoorDown)
            usage++;
        if (rightLight)
            usage++;
        if (leftLight)
            usage++;
        if (guardCurrentCamera >= 0)
            usage++;
        usageImage.sprite = usageSprites[usage];

        officeRenderer.sprite = powerLeft > 0f ? normalOfficeSprite : darkOfficeSprite;
        freddyAttackButton.interactable = moveTimerDone && freddyLocationIndex == freddyAttackLocationIndex;
        chicaAttackButton.interactable = moveTimerDone && chicaLocationIndex == chicaAttackLocationIndex;
        bonnieAttackButton.interactable = moveTimerDone && bonnieLocationIndex == bonnieAttackLocationIndex;
        foxyAttackButton.interactable = moveTimerDone && foxyLocationIndex == foxyAttackLocationIndex;

        if (leftLight && powerLeft > 0f)
        {
            if (bonnieLocationIndex == bonnieAttackLocationIndex && !sawBonnie)
            {
                windowScareSound.Play();
                sawBonnie = true;
            }

            leftOfficeRenderer.sprite = Random.value > lightRandomThreshold ? 
                (bonnieLocationIndex == bonnieAttackLocationIndex ? leftOfficeMonsterSprite : leftOfficeLightSprite) : leftOfficeNormalSprite;
            leftLightButtonRenderer.sprite = leftLightButtonOnSprite;
        }
        else
        {
            leftOfficeRenderer.sprite = leftOfficeNormalSprite;
            leftLightButtonRenderer.sprite = leftLightButtonOffSprite;
        }

        if (rightLight && powerLeft > 0f)
        {
            if (chicaLocationIndex == chicaAttackLocationIndex && !sawChica)
            {
                windowScareSound.Play();
                sawChica = true;
            }

            rightOfficeRenderer.sprite = Random.value > lightRandomThreshold ? 
                (chicaLocationIndex == chicaAttackLocationIndex ? rightOfficeMonsterSprite : rightOfficeLightSprite) : rightOfficeNormalSprite;
            rightLightButtonRenderer.sprite = rightLightButtonOnSprite;
        }
        else
        {
            rightOfficeRenderer.sprite = rightOfficeNormalSprite;
            rightLightButtonRenderer.sprite = rightLightButtonOffSprite;
        }

        timerSound.volume = monitorOpen ? 1f : 0f;
        timerDoneSound.volume = monitorOpen ? 0.8f : 0.4f;

        if ((leftLight || rightLight) && powerLeft > 0f)
        {
            if (!lightSound.isPlaying)
                lightSound.Play();
        }
        else
        {
            if (lightSound.isPlaying)
                lightSound.Stop();
        }

        if (moveTimer > 0f && powerLeft > 0f)
        {
            moveTimer -= Time.deltaTime;
            timerText.text = "" + Mathf.Ceil(moveTimer);
        }
        else
        {
            if (!moveTimerDone)
                EndTimer();
        }
    }

    private void StartTimer(float time)
    {
        timerImage.enabled = true;
        timerSound.Play();
        moveTimer = time;
        moveTimerDone = false;
    }

    private void EndTimer()
    {
        timerSound.Stop();
        timerDoneSound.Play();
        timerImage.enabled = false;
        timerText.text = "Move!";
        moveTimerDone = true;
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

        if (!isAfton)
        {
            FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, rightLight, rightDoorDown, monitorOpen ? currentCamera : -1);
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

        if (powerLeft <= 0f || isAfton)
        {
            powerErrorSound.Play();
            return;
        }

        FNAFClient.Instance.FNAF1RequestOfficeChange(!leftLight, leftDoorDown, rightLight, rightDoorDown, guardCurrentCamera);
    }

    public void LeftDoorToggle()
    {
        if (monitorOpen) return;

        if (powerLeft <= 0f || isAfton)
        {
            powerErrorSound.Play();
            return;
        }

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, !leftDoorDown, rightLight, rightDoorDown, guardCurrentCamera);
    }

    public void RightLightToggle()
    {
        if (monitorOpen) return;

        if (powerLeft <= 0f || isAfton)
        {
            powerErrorSound.Play();
            return;
        }

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, !rightLight, rightDoorDown, guardCurrentCamera);
    }

    public void RightDoorToggle()
    {
        if (monitorOpen) return;

        if (powerLeft <= 0f || isAfton)
        {
            powerErrorSound.Play();
            return;
        }

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, rightLight, !rightDoorDown, guardCurrentCamera);
    }

    public void SwitchCamera(int index)
    {
        if (currentCamera == index) return;

        cameraSwitchSound.Play();
        cameraSwitchEffect.Play();
        currentCamera = index;
        cameraNameText.text = cameras[index].cameraName;

        if (!isAfton)
        {
            FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, rightLight, rightDoorDown, index);
        }

        UpdateCameras();
    }

    private void UpdateCameras()
    {
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == currentCamera)
            {
                cameras[i].gameObject.SetActive(true);
                cameraButtons[i].gameObject.SetActive(true);
            }
            else
            {
                cameras[i].gameObject.SetActive(false);
                cameraButtons[i].gameObject.SetActive(false);
            }
        }
    }
}
