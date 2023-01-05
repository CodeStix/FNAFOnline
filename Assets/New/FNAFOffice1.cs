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
    public AudioSource doorKnockSound;
    public GameObject fan;
    public AudioSource ambientSound;
    public AudioClip[] ambientClips;
    public AudioSource carnavalSound;
    [Range(0f, 1f)]
    public float lightRandomThreshold = 0.85f;
    [Space]
    public FNAFBetweenAnimation monitor;
    public AudioSource monitorSound;
    public GameObject monitorEnableObject;
    public AudioSource cameraSwitchSound;
    [Tooltip("1A, 1B, 1C, 2A, 2B, 3, 4A, 4B, 5, 6, 7")]
    public FNAFOffice1Camera[] cameras;
    public Image[] cameraButtons;
    public Sprite cameraButtonNormal;
    public Sprite cameraButtonActive;
    public Sprite cameraButtonLookedAt;
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
    public Text nightText;
    public Text secondNightText;
    public Text roleText;
    public GameObject nightStartOverlay;
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
    public GameObject caughtMovingObject;
    public Color staticColor;
    public AudioSource signalLostSound;
    public AudioSource phoneGuySound;
    public AudioClip[] phoneGuyClips;
    public GameObject phone;
    public Button yourOfficeButton;
    public Button otherOfficeButton;
    public GameObject[] attackerMonitorObjects;
    [Range(0f, 1f)]
    public float randomStaticNoise = 0.98f;
    public float staticReturnSpeed = 1f;
    public Image staticImage;
    public float fullStaticReturnSpeed = 1f;
    public Image fullStaticImage;
    public AudioSource switchMonitorSound;
    public GameObject itsMe;
    public Image powerIssueOverlay;
    public AudioSource powerIssueSound;
    public Button[] distractionButtons;
    public GameObject distractionLoading;
    [Space]
    public UnityEvent onMoveAnyone;
    public UnityEvent onJumpscare;
    public UnityEvent onGuardWin;
    public UnityEvent onAftonWin;
    public UnityEvent onGuardLose;
    public UnityEvent onAftonLose;

    private FNAF1OfficeState yourOfficeState = new FNAF1OfficeState();
    private FNAF1OfficeState attackedOfficeState = new FNAF1OfficeState(); // Used only when gamemode is free for all

    private bool canToggleMonitor = true;
    private bool monitorOpen = false;
    private int currentCamera = 0;
    private bool sawChica = false;
    private bool sawBonnie = false;
    private float moveTimer = 0f;
    private bool moveTimerDone = true;
    private float cameraButtonBlink = 0f;
    private float caughtMovingTime = 0f;
    private float distortedSignalTime = 0f;
    private float disableFanTime = 0f;
    private float sabotageButtonsTime = 0f;
    private float itsMeTime = 0f;
    private float powerIssueTime = 0f;
    private float distractionLoadingTime = 0f;
    private bool monitorYourOffice = true;

    public bool MayMove => moveTimerDone;

    private FNAFUser User => FNAFClient.Instance.GetUser();
    private FNAFRoom Room => FNAFClient.Instance.GetRoom();
    private FNAFGamePlayer Player => Room.game.players.First((e) => e.user.id == User.id);

    private readonly string[] HOUR_NAMES = new[] { "12 AM", "1 AM", "2 AM", "3 AM", "4 AM", "5 AM", "6 AM", "7 AM", "8 AM", "9 AM", "10 AM", "11 AM", "12 PM", "1 PM", "2 PM", "3 PM", "4 PM" };
    public const string CLASSIC_GAMEMODE = "classic";
    public const string FREE_FOR_ALL_GAMEMODE = "freeForAll";

    private void Start()
    {
        if (FNAFClient.Instance == null || FNAFClient.Instance.GetRoom() == null)
        {
            Debug.Log("Joining random new room for testing");
            Invoke(nameof(TestJoinRoom), 1.0f);
        }
        else
        {
            StartGame();
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

        PlayRandomAmbient();
        ShuffleDistractions();
    }

    public void PlayRandomAmbient()
    {
        if (ambientSound.isPlaying)
            ambientSound.Stop();
        ambientSound.clip = ambientClips[Random.Range(0, ambientClips.Length)];
        ambientSound.Play();
    }

    private void StartGame()
    {
        Debug.Log("Starting game, controlled by " + Player.controlledByUser);

        nightStartOverlay.SetActive(true);
        Invoke(nameof(RemoveStartOverlay), 8.0f);

        string nightString = "Night " + FNAFClient.Instance.GetUser().night;
        secondNightText.text = nightString;
        nightText.text = nightString;

        if (Room.game.gameMode == CLASSIC_GAMEMODE)
        {
            yourOfficeButton.gameObject.SetActive(false);
            otherOfficeButton.gameObject.SetActive(false);

            if (Player.controlledByUser == null)
            {
                StartTimer(Room.game.startingMoveTime);
                SetAttackerMonitorObjects(true);
            }
            else
            {
                SetAttackerMonitorObjects(false);
            }

            roleText.text = Player.controlledByUser == null ? "You control the animatronics." : "You are the guard.";
        }
        else if (Room.game.gameMode == FREE_FOR_ALL_GAMEMODE)
        {
            yourOfficeButton.gameObject.SetActive(true);
            otherOfficeButton.gameObject.SetActive(true);
            StartTimer(Room.game.startingMoveTime);
            SetAttackerMonitorObjects(false);

            FNAFGamePlayer attackingPlayer = Room.game.players.FirstOrDefault((e) => e.controlledByUser.id == User.id);
            roleText.text = "You are a guard. But you must also control\n" + attackingPlayer.user.name + "'s animatronics.";
        }
        else
        {
            throw new System.NotImplementedException();
        }

        freddyLocations[0].SetMoveButton(true);
        chicaLocations[0].SetMoveButton(true);
        bonnieLocations[0].SetMoveButton(true);
        foxyLocations[0].SetMoveButton(true);
        distractionLoadingTime = Room.game.startingMoveTime + 45f;
    }

    private void SetAttackerMonitorObjects(bool enabled)
    {
        foreach(GameObject obj in attackerMonitorObjects)
        {
            obj.SetActive(enabled);
        }
    }

    private void RemoveStartOverlay()
    {
        nightStartOverlay.SetActive(false);
    }

    private void TestJoinRoom()
    {
        FNAFClient.Instance.JoinRoom(null);
        Invoke(nameof(TestStartGame), 1.0f);
    }

    private void TestStartGame()
    {
        FNAFClient.Instance.OnFNAF1MoveResponse += Instance_OnFNAF1MoveResponse;
        FNAFClient.Instance.OnFNAF1AttackEvent += Instance_OnFNAF1AttackEvent;
        FNAFClient.Instance.OnFNAF1DistractEvent += Instance_OnFNAF1DistractEvent;
        FNAFClient.Instance.OnFNAF1DistractResponse += Instance_OnFNAF1DistractResponse;
        FNAFClient.Instance.OnFNAF1AttackResponse += Instance_OnFNAF1AttackResponse;
        FNAFClient.Instance.OnFNAF1OfficeEvent += Instance_OnFNAF1OfficeEvent;
        FNAFClient.Instance.OnFNAF1AttackingOfficeEvent += Instance_OnFNAF1AttackingOfficeEvent;
        FNAFClient.Instance.OnFNAFEndEvent += Instance_OnFNAFEndEvent;
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;
        FNAFClient.Instance.ReadyRequest(true);
        Invoke(nameof(StartGame), 4.0f);
    }

    private void OnEnable()
    {
        FNAFClient.Instance.OnFNAF1MoveResponse += Instance_OnFNAF1MoveResponse;
        FNAFClient.Instance.OnFNAF1AttackEvent += Instance_OnFNAF1AttackEvent;
        FNAFClient.Instance.OnFNAF1DistractEvent += Instance_OnFNAF1DistractEvent;
        FNAFClient.Instance.OnFNAF1DistractResponse += Instance_OnFNAF1DistractResponse;
        FNAFClient.Instance.OnFNAF1AttackResponse += Instance_OnFNAF1AttackResponse;
        FNAFClient.Instance.OnFNAF1OfficeEvent += Instance_OnFNAF1OfficeEvent;
        FNAFClient.Instance.OnFNAF1AttackingOfficeEvent += Instance_OnFNAF1AttackingOfficeEvent;
        FNAFClient.Instance.OnFNAFEndEvent += Instance_OnFNAFEndEvent;
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;
    }

    private void OnDisable()
    {
        FNAFClient.Instance.OnFNAF1MoveResponse -= Instance_OnFNAF1MoveResponse;
        FNAFClient.Instance.OnFNAF1AttackEvent -= Instance_OnFNAF1AttackEvent;
        FNAFClient.Instance.OnFNAF1DistractEvent -= Instance_OnFNAF1DistractEvent;
        FNAFClient.Instance.OnFNAF1DistractResponse -= Instance_OnFNAF1DistractResponse;
        FNAFClient.Instance.OnFNAF1AttackResponse -= Instance_OnFNAF1AttackResponse;
        FNAFClient.Instance.OnFNAF1OfficeEvent -= Instance_OnFNAF1OfficeEvent;
        FNAFClient.Instance.OnFNAF1AttackingOfficeEvent -= Instance_OnFNAF1AttackingOfficeEvent;
        FNAFClient.Instance.OnFNAFEndEvent -= Instance_OnFNAFEndEvent;
        FNAFClient.Instance.OnRoomChangeEvent -= Instance_OnRoomChangeEvent;
    }

    private void Instance_OnFNAFEndEvent(object sender, System.EventArgs e)
    {
        StartCoroutine(GameEndSequence());
    }

    private void Instance_OnRoomChangeEvent(object sender, FNAFRoomChangeEvent e)
    {
        //if (e.eventType == "leave")
        //{
        //}
    }

    private void Instance_OnFNAF1AttackResponse(object sender, FNAF1AttackResponse e)
    {
        if (e.ok)
        {
            StartCoroutine(AftonJumpscareSequence(e.monster));
        }
    }

    private void Instance_OnFNAF1DistractResponse(object sender, FNAF1DistractResponse e)
    {
        if (e.ok)
        {
            Debug.Log("distraction cooldownTime = " + e.cooldownTime);
            distractionLoadingTime = e.cooldownTime;
            ShuffleDistractions();
        }
    }

    private void Instance_OnFNAF1AttackEvent(object sender, FNAF1AttackEvent e)
    {
        StartCoroutine(JumpscareSequence(e.monster));
    }

    private void Instance_OnFNAF1DistractEvent(object sender, FNAF1DistractEvent e)
    {
        Debug.Log("Received distraction event: " + e.distraction);
        switch (e.distraction)
        {
            case "phoneGuy": // Phone guy
                PlayPhoneGuy();
                break;
            case "lostSignal": // Lost signal for 10 seconds
                DistortSignal(10f);
                break;
            case "itsMe": // Itsme distraction
                ItsMeDistraction(4f);
                break;
            case "fakeMoveSound": // Plays a random animatronic move sound no matter where it is
                PlayRandomMoveSound();
                break;
            case "sabotageButtons": // Buttons are unreliable and don't always work (play the error sound)
                SabotageButtons(10f);
                break;
            case "carnavalMusic": // Plays carnaval music
                PlayCarnavalMusic();
                break;
            case "turnOffFan": // Turns off the fan temporary
                TurnOffFan(10f);
                break;
            case "ambienceChange": // Change ambience sound
                PlayRandomAmbient();
                break;
            case "powerIssue":
                PowerIssue(5f);
                break;
                //case "10power": // Is handled on server
                //    break;
                //case "disableRandomCamera": // Idea: Disables a random camera button
                //    break;
                //case "goldenFreddy": // Idea: Golden freddy appears in the office, use monitor to make him dissapear
                //    break;
                //case "rogue": // Idea: all the cameras show animatronics
                //    break;
                //case "moveAll": // Idea: all the animatronics move in one random direction
                //    break;
                //case "mirror": // Idea: the office gets mirrored
                //    break;
        }
    }

    private void Instance_OnFNAF1MoveResponse(object sender, FNAF1MoveResponse e)
    {
        // This function gets called if the server processed this clients animatronic move request
        if (e.ok)
        {
            onMoveAnyone?.Invoke();
            StartTimer(e.cooldownTime);
            if (e.gotCaught)
            {
                caughtMovingTime = 8f;
            }
        }
    }

    public void SetMonitorOffice(bool yourOffice)
    {
        if (monitorYourOffice == yourOffice)
        {
            return;
        }
        monitorYourOffice = yourOffice;
        fullStaticImage.color = Color.white;
        switchMonitorSound.Play();
        switchMonitorSound.pitch = Random.Range(0.93f, 1.03f);
        switchMonitorSound.time = Random.Range(0, 0.2f);

        if (yourOffice)
        {
            // Disable movement UI
            SetAttackerMonitorObjects(false);
            UpdateMonitorMonsters(attackedOfficeState, yourOfficeState, false);
            ClearCamerasBeingLookedAt();

            // Look again at current camera
            FNAFClient.Instance.FNAF1RequestOfficeChange(yourOfficeState.leftLight, yourOfficeState.leftDoor, yourOfficeState.rightLight, yourOfficeState.rightDoor, currentCamera);
        }
        else
        {
            // Enable movement UI
            SetAttackerMonitorObjects(true);
            UpdateMonitorMonsters(yourOfficeState, attackedOfficeState, false);
            UpdateCamerasBeingLookedAt(attackedOfficeState.camerasLookedAt);

            // Not looking at camera anymore
            FNAFClient.Instance.FNAF1RequestOfficeChange(yourOfficeState.leftLight, yourOfficeState.leftDoor, yourOfficeState.rightLight, yourOfficeState.rightDoor, -1);
        }
    }

    private void UpdateMonitorMonsters(FNAF1OfficeState previousState, FNAF1OfficeState newState, bool isOwnOffice)
    {
        if (previousState.freddyLocation != newState.freddyLocation || previousState.freddyLocationState != newState.freddyLocationState)
        {
            staticImage.color = Color.white;
            freddyLocations[previousState.freddyLocation].SetState(-1);
            freddyLocations[newState.freddyLocation].SetState(newState.freddyLocationState);

            if (isOwnOffice)
            {
                if (freddyLocations[newState.freddyLocation].isFar)
                {
                    freddyMoveSounds.PlayFar();
                }
                else
                {
                    freddyMoveSounds.PlayClose();
                    if (Random.value > 0.75f)
                        PowerIssue(0.5f);
                }
            }
        }

        if (previousState.chicaLocation != newState.chicaLocation || previousState.chicaLocationState != newState.chicaLocationState)
        {
            staticImage.color = Color.white;
            if (isOwnOffice)
            {
                sawChica = false;
                if (isOwnOffice && previousState.chicaLocation == chicaAttackLocationIndex && previousState.rightLight)
                {
                    // If moved away from window, disable light
                    FNAFClient.Instance.FNAF1RequestOfficeChange(newState.leftLight, newState.leftDoor, false, newState.rightDoor, monitorOpen ? currentCamera : -1);
                }
            }

            chicaLocations[previousState.chicaLocation].SetState(-1);
            chicaLocations[newState.chicaLocation].SetState(newState.chicaLocationState);

            if (isOwnOffice)
            {
                if (chicaLocations[newState.chicaLocation].isFar)
                {
                    chicaMoveSounds.PlayFar();
                }
                else
                {
                    chicaMoveSounds.PlayClose();
                    if (Random.value > 0.75f)
                        PowerIssue(0.5f);
                }
            }
        }

        if (previousState.bonnieLocation != newState.bonnieLocation || previousState.bonnieLocationState != newState.bonnieLocationState)
        {
            staticImage.color = Color.white;
            if (isOwnOffice)
            {
                sawBonnie = false;
                if (previousState.bonnieLocation == bonnieAttackLocationIndex && previousState.leftLight)
                {
                    // If moved away from window, disable light
                    FNAFClient.Instance.FNAF1RequestOfficeChange(false, newState.leftDoor, newState.rightLight, newState.rightDoor, monitorOpen ? currentCamera : -1);
                }
            }

            bonnieLocations[previousState.bonnieLocation].SetState(-1);
            bonnieLocations[newState.bonnieLocation].SetState(newState.bonnieLocationState);
            
            if (isOwnOffice)
            {
                if (bonnieLocations[newState.bonnieLocation].isFar)
                {
                    bonnieMoveSounds.PlayFar();
                }
                else
                {
                    bonnieMoveSounds.PlayClose();
                    if (Random.value > 0.75f)
                        PowerIssue(0.5f);
                }
            }
        }

        if (previousState.foxyLocation != newState.foxyLocation || previousState.foxyLocationState != newState.foxyLocationState)
        {
            staticImage.color = Color.white;
            if (isOwnOffice && previousState.foxyLocation == foxyAttackLocationIndex && newState.foxyLocation != foxyAttackLocationIndex)
            {
                doorKnockSound.Play();
            }

            foxyLocations[previousState.foxyLocation].SetState(-1);
            foxyLocations[newState.foxyLocation].SetState(newState.foxyLocationState);

            if (isOwnOffice)
            {
                if (foxyLocations[newState.foxyLocation].isFar)
                {
                    foxyMoveSounds.PlayFar();
                }
                else
                {
                    foxyMoveSounds.PlayClose();
                    if (Random.value > 0.75f)
                        PowerIssue(0.5f);
                }
            }
        }
    }

    private void ClearCamerasBeingLookedAt()
    {
        for (int i = 0; i < cameraButtons.Length; i++)
            if (cameraButtons[i].sprite == cameraButtonLookedAt)
                cameraButtons[i].sprite = cameraButtonNormal;
    }

    private void UpdateCamerasBeingLookedAt(List<FNAF1OfficeCameraState> camerasLookedAt)
    {
        ClearCamerasBeingLookedAt();

        foreach (var camera in camerasLookedAt)
            cameraButtons[camera.cameraIndex].sprite = cameraButtonLookedAt;
    }


    private void Instance_OnFNAF1AttackingOfficeEvent(object sender, FNAF1OfficeEvent e)
    {
        FNAF1OfficeState newAttackedOfficeState = e.office;

        // Update the monitor monsters if the others office is selected
        if (!monitorYourOffice)
        {
            UpdateMonitorMonsters(attackedOfficeState, newAttackedOfficeState, false);
            UpdateCamerasBeingLookedAt(newAttackedOfficeState.camerasLookedAt);
        }

        attackedOfficeState = newAttackedOfficeState;
    }

    private void Instance_OnFNAF1OfficeEvent(object sender, FNAF1OfficeEvent e)
    {
        // This event is received if something in this player's office should change
        FNAF1OfficeState newOfficeState = e.office;
        
        // Update the monitor monsters if your office is selected
        if (monitorYourOffice)
        {
            UpdateMonitorMonsters(yourOfficeState, newOfficeState, true);
        }

        if (yourOfficeState.rightDoor != newOfficeState.rightDoor)
        {
            doorSound.Play();
            if (newOfficeState.rightDoor)
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

        if (yourOfficeState.leftDoor != newOfficeState.leftDoor)
        {
            doorSound.Play();
            if (newOfficeState.leftDoor)
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

        // Show cameras being looked at if this player is an attacker
        if (Player.controlledByUser == null)
        {
            UpdateCamerasBeingLookedAt(newOfficeState.camerasLookedAt);
        }

        powerLeftText.text = "Power left: <b>" + Mathf.Floor(newOfficeState.powerLeft) + "</b>%";

        if (newOfficeState.powerLeft <= 0f && yourOfficeState.powerLeft > 0f)
        {
            powerDownSound.Play();
            if (newOfficeState.rightDoor)
            {
                rightDoor.Start();
                doorSound.Play();
            }
            if (newOfficeState.leftDoor)
            {
                leftDoor.Start();
                doorSound.Play();
            }

            //foreach (GameObject obj in isAfton ? requiresPowerAfton : requiresPower)
            //{
            //    obj.SetActive(false);
            //}
        }
        else if (newOfficeState.powerLeft > 0f && yourOfficeState.powerLeft <= 0f)
        {
            //foreach (GameObject obj in isAfton ? requiresPowerAfton : requiresPower)
            //{
            //    obj.SetActive(true);
            //}
        }

        float currentHour = e.currentHour;
        hourText.text = HOUR_NAMES[(int)currentHour];

        if (e.cancelPhoneGuy)
        {
            if (phoneGuySound.isPlaying)
            {
                phoneGuySound.Stop();
                cameraSwitchSound.Play();
            }
        }

        yourOfficeState = newOfficeState;
        if (Room.game.gameMode == CLASSIC_GAMEMODE)
        {
            attackedOfficeState = newOfficeState;
        }
    }

    private string GetNameForDistraction(string id)
    {
        switch(id)
        {
            case "phoneGuy":
                return "Call Phone Guy";
            case "lostSignal":
                return "Distort Signal";
            case "itsMe":
                return "Its Me Distraction";
            case "fakeMoveSound":
                return "Play Movement Sounds";
            case "sabotageButtons":
                return "Sabotage Buttons";
            case "carnavalMusic":
                return "Play Circus Music";
            case "turnOffFan":
                return "Turn Off Fan";
            case "ambienceChange":
                return "Change Background Sound";
            case "powerIssue":
                return "Create Power Issue";
            case "10power":
                return "Send 10% power";
            default:
                return "Invalid distraction";
        }
    }

    public void ShuffleDistractions()
    {
        string[] distractions = new[] { "phoneGuy", "lostSignal", "itsMe", "fakeMoveSound", "sabotageButtons", "carnavalMusic", "turnOffFan", "ambienceChange", "powerIssue", "10power" };
        for(int i = 0; i < distractions.Length; i++)
        {
            int n = Random.Range(0, distractions.Length);
            string temp = distractions[n];
            distractions[n] = distractions[i];
            distractions[i] = temp;
        }

        for(int i = 0; i < distractionButtons.Length; i++)
        {
            string distraction = distractions[i];
            Button button = distractionButtons[i];
            button.GetComponentInChildren<Text>().text = GetNameForDistraction(distraction);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SendDistraction(distraction));
        }
    }

    public void SendDistraction(string distractionName)
    {
        distractionLoadingTime = 5000000f;
        onMoveAnyone?.Invoke();

        Debug.Log("Send distraction " + distractionName);
        FNAFClient.Instance.FNAF1RequestDistract(distractionName);
    }

    public void PowerIssue(float time)
    {
        powerIssueTime = time;
    }

    public void ItsMeDistraction(float time)
    {
        if (monitorOpen)
            ToggleMonitor();
        itsMeTime = time;
    }

    public void TurnOffFan(float time)
    {
        disableFanTime = time;
    }

    public void SabotageButtons(float time)
    {
        sabotageButtonsTime = time;
    }

    public void PlayRandomMoveSound()
    {
        switch(Random.Range(0, 4))
        {
            case 0:
                freddyMoveSounds.PlayClose();
                break;
            case 1:
                chicaMoveSounds.PlayClose();
                break;
            case 2:
                foxyMoveSounds.PlayClose();
                break;
            case 3:
                bonnieMoveSounds.PlayClose();
                break;
        }
    }

    public void PlayPhoneGuy()
    {
        if (!phoneGuySound.isPlaying)
        {
            phoneGuySound.clip = phoneGuyClips[Random.Range(0, phoneGuyClips.Length)];
            phoneGuySound.Play();
        }
    }

    public void DistortSignal(float time)
    {
        if (time > distortedSignalTime)
            distortedSignalTime = time;
    }

    public void PlayCarnavalMusic()
    {
        if (!carnavalSound.isPlaying)
            carnavalSound.Play();
    }

    public void StopPhoneGuy()
    {
        FNAFClient.Instance.FNAF1RequestOfficeChange(yourOfficeState.leftLight, yourOfficeState.leftDoor, yourOfficeState.rightLight, yourOfficeState.rightDoor, monitorOpen ? currentCamera : -1, true);
    }

    private IEnumerator GameEndSequence()
    {
        yield return new WaitForSeconds(5.0f);

        if (Room.game.gameMode == CLASSIC_GAMEMODE)
        {
            bool somePlayerAlive = Room.game.players.Any((e) => e.alive && e.controlledByUser != null);
            if (Player.controlledByUser != null)
            {
                // Guard
                if (somePlayerAlive)
                    onGuardWin.Invoke();
                else
                    onGuardLose.Invoke();
            }
            else
            {
                // Controller
                if (somePlayerAlive)
                    onAftonLose.Invoke();
                else
                    onAftonWin.Invoke();
            }
        }
        else if (Room.game.gameMode == FREE_FOR_ALL_GAMEMODE)
        {
            if (Player.alive)
                onGuardWin.Invoke();
            else
                onGuardLose.Invoke();
        }

        yield return new WaitForSeconds(10.0f);

        LoadingScreen.LoadScene("Lobby");
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

        if (Room.game.gameMode == FREE_FOR_ALL_GAMEMODE)
        {
            yield return new WaitForSeconds(3.0f);

            aftonDeathOverlay.SetActive(false);
            aftonGuardView.SetActive(false);
            SetMonitorOffice(true);
        }
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

        if (Room.game.gameMode == FREE_FOR_ALL_GAMEMODE)
        {
            // Return to lobby early because the player would have to stare at a static noise until all other players are dead
            yield return new WaitForSeconds(5.0f);
            onGuardLose.Invoke();

            yield return new WaitForSeconds(10.0f);
            LoadingScreen.LoadScene("Lobby");
        }
    }

    private void Update()
    {
        int usage = 0;
        if (yourOfficeState.rightDoor)
            usage++;
        if (yourOfficeState.leftDoor)
            usage++;
        if (yourOfficeState.rightLight)
            usage++;
        if (yourOfficeState.leftLight)
            usage++;

        if (Room != null)
        {
            if (Room.game.gameMode == CLASSIC_GAMEMODE)
            {
                if (yourOfficeState.camerasLookedAt.Count > 0)
                    usage++;

                cameraNameText.text = cameras[currentCamera].cameraName;
            }
            else if (Room.game.gameMode == FREE_FOR_ALL_GAMEMODE)
            {
                yourOfficeButton.interactable = !monitorYourOffice;
                otherOfficeButton.interactable = monitorYourOffice;

                if (currentCamera >= 0 && monitorOpen)
                    usage++;

                FNAFGamePlayer attackingPlayer = Room.game.players.FirstOrDefault((e) => e.controlledByUser.id == User.id);
                if (attackingPlayer != null)
                {
                    cameraNameText.text = cameras[currentCamera].cameraName + (monitorYourOffice ? " (your office)" : " (" + attackingPlayer.user.name + "'s office)");
                }
                else
                {
                    Debug.LogWarning("attackingPlayer is null");
                }
            }
        }

        usageImage.sprite = usageSprites[usage];

        officeRenderer.sprite = yourOfficeState.powerLeft > 0f ? normalOfficeSprite : darkOfficeSprite;
        freddyAttackButton.interactable = moveTimerDone && !attackedOfficeState.rightDoor && attackedOfficeState.freddyLocation == freddyAttackLocationIndex;
        chicaAttackButton.interactable = moveTimerDone && !attackedOfficeState.rightDoor && attackedOfficeState.chicaLocation == chicaAttackLocationIndex;
        bonnieAttackButton.interactable = moveTimerDone && !attackedOfficeState.leftDoor && attackedOfficeState.bonnieLocation == bonnieAttackLocationIndex;
        foxyAttackButton.interactable = moveTimerDone && !attackedOfficeState.leftDoor && attackedOfficeState.foxyLocation == foxyAttackLocationIndex;

        staticImage.color = Color.Lerp(staticImage.color, staticColor, Time.deltaTime * staticReturnSpeed);
        fullStaticImage.color = Color.Lerp(fullStaticImage.color, Color.clear, Time.deltaTime * fullStaticReturnSpeed);

        if (Random.value > randomStaticNoise)
        {
            staticImage.color = Color.white;
        }

        if (distortedSignalTime > 0f)
        {
            distortedSignalTime -= Time.deltaTime;
            if (!signalLostSound.isPlaying)
            {
                signalLostSound.Play();
                signalLostSound.time = Random.value * signalLostSound.clip.length;
            }

            if (Player.controlledByUser != null)
            {
                staticImage.color = new Color(1f, 1f, 1f, 0.85f);
                signalLostSound.volume = monitorOpen ? 0.5f : 0.08f;
            }
            else
            {
                signalLostSound.volume = 0.08f;
            }
        }
        else
        {
            if (signalLostSound.isPlaying)
                signalLostSound.Stop();
        }

        if (sabotageButtonsTime > 0f)
            sabotageButtonsTime -= Time.deltaTime;

        if (disableFanTime > 0f)
            disableFanTime -= Time.deltaTime;
        fan.SetActive(disableFanTime <= 0f && yourOfficeState.powerLeft > 0f);

        if (distractionLoadingTime > 0f)
            distractionLoadingTime -= Time.deltaTime;
        foreach(Button button in distractionButtons)
            button.gameObject.SetActive(distractionLoadingTime <= 0f);
        distractionLoading.SetActive(distractionLoadingTime > 0f);

        if (!phoneGuySound.isPlaying)
            phone.SetActive(false);

        if (itsMeTime > 0f)
            itsMeTime -= Time.deltaTime;
        itsMe.SetActive(itsMeTime > 0f && Player.controlledByUser != null);

        if (powerIssueTime > 0f)
        {
            powerIssueTime -= Time.deltaTime;
            if (Random.value > 0.8f)
            {
                powerIssueOverlay.gameObject.SetActive(Random.value > 0.8f);
                if (!powerIssueSound.isPlaying && Random.value > 0.8f)
                    powerIssueSound.Play();
            }
        }
        else
        {
            powerIssueOverlay.gameObject.SetActive(false);
        }

        if (Application.isEditor)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                DistortSignal(10f);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                TurnOffFan(10f);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                PlayPhoneGuy();
            if (Input.GetKeyDown(KeyCode.Alpha4))
                PlayCarnavalMusic();
            if (Input.GetKeyDown(KeyCode.Alpha5))
                PlayRandomAmbient();
            if (Input.GetKeyDown(KeyCode.Alpha6))
                SabotageButtons(10f);
            if (Input.GetKeyDown(KeyCode.Alpha7))
                ItsMeDistraction(2f);
            if (Input.GetKeyDown(KeyCode.Alpha8))
                PowerIssue(5f);
        }

        if (cameraButtonBlink > 0f)
        {
            cameraButtonBlink -= Time.deltaTime;
        }
        else
        {
            cameraButtons[currentCamera].sprite = cameraButtons[currentCamera].sprite == cameraButtonActive ? cameraButtonNormal : cameraButtonActive;
            
            cameraButtonBlink = 0.5f;
        }

        if (caughtMovingTime > 0f)
        {
            caughtMovingTime -= Time.deltaTime;
            caughtMovingObject.SetActive(true);
        }
        else
        {
            caughtMovingObject.SetActive(false);
        }

        if (yourOfficeState.leftLight && yourOfficeState.powerLeft > 0f)
        {
            if (yourOfficeState.bonnieLocation == bonnieAttackLocationIndex && !sawBonnie)
            {
                windowScareSound.Play();
                sawBonnie = true;
            }

            leftOfficeRenderer.sprite = Random.value > lightRandomThreshold ? 
                (yourOfficeState.bonnieLocation == bonnieAttackLocationIndex ? leftOfficeMonsterSprite : leftOfficeLightSprite) : leftOfficeNormalSprite;
            leftLightButtonRenderer.sprite = leftLightButtonOnSprite;
        }
        else
        {
            leftOfficeRenderer.sprite = leftOfficeNormalSprite;
            leftLightButtonRenderer.sprite = leftLightButtonOffSprite;
        }

        if (yourOfficeState.rightLight && yourOfficeState.powerLeft > 0f)
        {
            if (yourOfficeState.chicaLocation == chicaAttackLocationIndex && !sawChica)
            {
                windowScareSound.Play();
                sawChica = true;
            }

            rightOfficeRenderer.sprite = Random.value > lightRandomThreshold ? 
                (yourOfficeState.chicaLocation == chicaAttackLocationIndex ? rightOfficeMonsterSprite : rightOfficeLightSprite) : rightOfficeNormalSprite;
            rightLightButtonRenderer.sprite = rightLightButtonOnSprite;
        }
        else
        {
            rightOfficeRenderer.sprite = rightOfficeNormalSprite;
            rightLightButtonRenderer.sprite = rightLightButtonOffSprite;
        }

        timerSound.volume = monitorOpen ? 1f : 0f;
        timerDoneSound.volume = monitorOpen ? 0.8f : 0.4f;

        if ((yourOfficeState.leftLight || yourOfficeState.rightLight) && yourOfficeState.powerLeft > 0f)
        {
            if (!lightSound.isPlaying)
                lightSound.Play();
        }
        else
        {
            if (lightSound.isPlaying)
                lightSound.Stop();
        }

        if (moveTimer > 0f && yourOfficeState.powerLeft > 0f)
        {
            moveTimer -= Time.deltaTime;
            timerText.text = "" + Mathf.Ceil(moveTimer);
        }
        else
        {
            if (!moveTimerDone)
                EndTimer();
        }

        if (yourOfficeState.powerLeft <= 0f && monitorOpen)
        {
            DisableMonitor();
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
        if (!canToggleMonitor || itsMeTime > 0f || yourOfficeState.powerLeft <= 0f)
        {
            return;
        }
            
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
            if (Player.controlledByUser != null && phoneGuySound.isPlaying && phoneGuySound.time > 10f)
            {
                phone.transform.position = new Vector2(Random.Range(-8f, 8f), Random.Range(-3.6f, 3.6f));
                phone.SetActive(Random.value <= 0.5f);
            }

            DisableMonitor();
            Invoke(nameof(ReenableMonitor), 0.25f);
        }

        if (Player.controlledByUser != null)
        {
            FNAFClient.Instance.FNAF1RequestOfficeChange(yourOfficeState.leftLight, yourOfficeState.leftDoor, yourOfficeState.rightLight, yourOfficeState.rightDoor, monitorOpen ? currentCamera : -1);
        }
    }

    private void ReenableMonitor()
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

    private void DisableMonitor()
    {
        monitorOpen = false;
        monitorSound.Stop();
        monitor.Start();
        monitorEnableObject.SetActive(false);
        canToggleMonitor = false;
    }

    private bool CheckButton()
    {
        if (monitorOpen)
        {
            return false;
        }

        if (yourOfficeState.powerLeft <= 0f || Player.controlledByUser == null || (sabotageButtonsTime > 0f && Random.value > 0.5f))
        {
            powerErrorSound.Play();
            return false;
        }
        else
        {
            return true;
        }
    }


    public void LeftLightToggle()
    {
        if (!CheckButton()) return;

        FNAFClient.Instance.FNAF1RequestOfficeChange(!yourOfficeState.leftLight, yourOfficeState.leftDoor, yourOfficeState.rightLight, yourOfficeState.rightDoor, monitorOpen ? currentCamera : -1);
    }

    public void LeftDoorToggle()
    {
        if (!CheckButton()) return;

        FNAFClient.Instance.FNAF1RequestOfficeChange(yourOfficeState.leftLight, !yourOfficeState.leftDoor, yourOfficeState.rightLight, yourOfficeState.rightDoor, monitorOpen ? currentCamera : -1);
    }


    public void RightLightToggle()
    {
        if (!CheckButton()) return;

        FNAFClient.Instance.FNAF1RequestOfficeChange(yourOfficeState.leftLight, yourOfficeState.leftDoor, !yourOfficeState.rightLight, yourOfficeState.rightDoor, monitorOpen ? currentCamera : -1);
    }

    public void RightDoorToggle()
    {
        if (!CheckButton()) return;

        FNAFClient.Instance.FNAF1RequestOfficeChange(yourOfficeState.leftLight, yourOfficeState.leftDoor, yourOfficeState.rightLight, !yourOfficeState.rightDoor, monitorOpen ? currentCamera : -1);
    }

    public void SwitchCamera(int index)
    {
        if (currentCamera == index) return;

        cameraButtons[currentCamera].sprite = cameraButtonNormal;
        cameraButtonBlink = 0.5f;

        cameraSwitchSound.Play();
        cameraSwitchEffect.Play();
        currentCamera = index;

        cameraButtons[currentCamera].sprite = cameraButtonActive;

        if (Player.controlledByUser != null && monitorYourOffice)
        {
            FNAFClient.Instance.FNAF1RequestOfficeChange(yourOfficeState.leftLight, yourOfficeState.leftDoor, yourOfficeState.rightLight, yourOfficeState.rightDoor, index);
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
            }
            else
            {
                cameras[i].gameObject.SetActive(false);
            }
        }
    }
}
