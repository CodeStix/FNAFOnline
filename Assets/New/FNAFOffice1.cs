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
    [Range(0f, 1f)]
    public float randomStaticNoise = 0.98f;
    public float staticReturnSpeed = 1f;
    public Image staticImage;
    public GameObject itsMe;
    public Image powerIssueOverlay;
    public AudioSource powerIssueSound;
    public Button[] distractionButtons;
    public GameObject distractionLoading;
    [Space]
    public UnityEvent onMoveAnyone;
    public UnityEvent onJumpscare;
    public UnityEvent whenGuard;
    public UnityEvent onGuardWin;
    public UnityEvent onAftonWin;
    public UnityEvent onGuardLose;
    public UnityEvent onAftonLose;

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
    private bool sawChica = false;
    private bool sawBonnie = false;
    private float moveTimer = 0f;
    private bool moveTimerDone = true;
    private float powerLeft = 100f;
    private float cameraButtonBlink = 0f;
    private float caughtMovingTime = 0f;
    private float distortedSignalTime = 0f;
    private float disableFanTime = 0f;
    private float sabotageButtonsTime = 0f;
    private float itsMeTime = 0f;
    private float powerIssueTime = 0f;
    private float distractionLoadingTime = 0f;

    public bool MayMove => moveTimerDone;

    private FNAFUser User => FNAFClient.Instance.GetUser();
    private FNAFRoom Room => FNAFClient.Instance.GetRoom();
    private FNAFGamePlayer Player => Room.game.players.First((e) => e.id == User.id);

    private readonly string[] HOUR_NAMES = new[] { "12 AM", "1 AM", "2 AM", "3 AM", "4 AM", "5 AM", "6 AM", "7 AM", "8 AM", "9 AM", "10 AM", "11 AM", "12 PM", "1 PM", "2 PM", "3 PM", "4 PM" };

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
        Debug.Log("Starting game");

        nightStartOverlay.SetActive(true);
        Invoke(nameof(RemoveStartOverlay), 5.0f);

        string nightString = "Night " + FNAFClient.Instance.GetUser().night;
        secondNightText.text = nightString;
        nightText.text = nightString;

        FNAFRoom room = FNAFClient.Instance.GetRoom();
        FNAFUser user = FNAFClient.Instance.GetUser();
        FNAFGamePlayer player = room.game.players.First((e) => e.id == user.id);

        bool enableControlUI;
        if (room.settings.gameMode == "classic")
        {
            enableControlUI = player.controlledByPlayerId == 0;
            roleText.text = player.controlledByPlayerId == 0 ? "You control the animatronics." : "You are the guard.";
        }
        else if (room.settings.gameMode == "duel")
        {
            enableControlUI = true;
            roleText.text = "You are the guard and controller.\nSurvive and kill!";
        }
        else
        {
            throw new System.NotImplementedException();
        }

        if (enableControlUI)
        {
            StartTimer(FNAFClient.Instance.GetRoom().settings.startingMoveTime);
            freddyLocations[0].SetMoveButton(true);
            chicaLocations[0].SetMoveButton(true);
            bonnieLocations[0].SetMoveButton(true);
            foxyLocations[0].SetMoveButton(true);
        }
        else
        {
            whenGuard.Invoke();
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
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;
        FNAFClient.Instance.OnFNAF1MoveResponse += Instance_OnFNAF1MoveResponse;
        FNAFClient.Instance.OnFNAF1AttackEvent += Instance_OnFNAF1AttackEvent;
        FNAFClient.Instance.OnFNAF1DistractEvent += Instance_OnFNAF1DistractEvent;
        FNAFClient.Instance.OnFNAF1DistractResponse += Instance_OnFNAF1DistractResponse;
        FNAFClient.Instance.OnFNAF1MoveResponse += Instance_OnFNAF1MoveResponse1;
        FNAFClient.Instance.StartGameRequest(true);
        Invoke(nameof(StartGame), 2.0f);
    }

    private void OnEnable()
    {
        FNAFClient.Instance.OnRoomChangeEvent += Instance_OnRoomChangeEvent;
        FNAFClient.Instance.OnFNAF1MoveResponse += Instance_OnFNAF1MoveResponse;
        FNAFClient.Instance.OnFNAF1AttackEvent += Instance_OnFNAF1AttackEvent;
        FNAFClient.Instance.OnFNAF1DistractEvent += Instance_OnFNAF1DistractEvent;
        FNAFClient.Instance.OnFNAF1DistractResponse += Instance_OnFNAF1DistractResponse;
        FNAFClient.Instance.OnFNAF1MoveResponse += Instance_OnFNAF1MoveResponse1;
    }


    private void OnDisable()
    {
        FNAFClient.Instance.OnRoomChangeEvent -= Instance_OnRoomChangeEvent;
        FNAFClient.Instance.OnFNAF1MoveResponse -= Instance_OnFNAF1MoveResponse;
        FNAFClient.Instance.OnFNAF1AttackEvent -= Instance_OnFNAF1AttackEvent;
        FNAFClient.Instance.OnFNAF1DistractEvent -= Instance_OnFNAF1DistractEvent;
        FNAFClient.Instance.OnFNAF1DistractResponse -= Instance_OnFNAF1DistractResponse;
        FNAFClient.Instance.OnFNAF1MoveResponse -= Instance_OnFNAF1MoveResponse1;
    }

    private void Instance_OnFNAF1AttackEvent(object sender, FNAF1AttackEvent e)
    {
        
    }

    private void Instance_OnFNAF1MoveResponse1(object sender, FNAF1MoveResponse e)
    {
        if (!e.ok) return;

        FNAF1OfficeState controlsOffice = Room.settings.gameMode == "classic" ? Room.game.office : Room.game.players.First((e) => e.controlledByPlayerId == User.id).office;
        for (int i = 0; i < freddyLocations.Length; i++)
            freddyLocations[i].SetMoveButton(controlsOffice.freddyLocation == i);
        for (int i = 0; i < chicaLocations.Length; i++)
            chicaLocations[i].SetMoveButton(controlsOffice.chicaLocation == i);
        for (int i = 0; i < bonnieLocations.Length; i++)
            bonnieLocations[i].SetMoveButton(controlsOffice.bonnieLocation == i);
        for (int i = 0; i < foxyLocations.Length; i++)
            foxyLocations[i].SetMoveButton(controlsOffice.foxyLocation == i);
    }

    private void Instance_OnFNAF1DistractResponse(object sender, FNAF1DistractResponse e)
    {
        
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
            case "10power": // Sends 10 power
                break;
                //case "disableRandomCamera": // Disables a random camera button
                //    break;
                //case "goldenFreddy": // Golden freddy appears in the office, use monitor to make him dissapear
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

    private void Instance_OnRoomChangeEvent(object sender, FNAFRoomChangeEvent e)
    {
        // Sync office to received room
        FNAF1Game game = e.room.game;
        FNAFGamePlayer player = game.players.First((e) => e.id == FNAFClient.Instance.GetUser().id);

        FNAF1OfficeState office = e.room.settings.gameMode == "classic" ? game.office : player.office;
        if (e.eventType == "move")
        {
            if (freddyLocationIndex != office.freddyLocation || freddyLocationState != office.freddyLocationState)
            {
                freddyLocations[freddyLocationIndex].SetState(-1);
                freddyLocationIndex = office.freddyLocation;
                freddyLocationState = office.freddyLocationState;
                freddyLocations[freddyLocationIndex].SetState(freddyLocationState);
                if (freddyLocations[freddyLocationIndex].isFar)
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

            if (chicaLocationIndex != office.chicaLocation || chicaLocationState != office.chicaLocationState)
            {
                sawChica = false;
                if (chicaLocationIndex == chicaAttackLocationIndex && rightLight)
                {
                    // If moved away from window, disable light
                    FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, false, rightDoorDown, monitorOpen ? currentCamera : -1);
                }

                chicaLocations[chicaLocationIndex].SetState(-1);
                chicaLocationIndex = office.chicaLocation;
                chicaLocationState = office.chicaLocationState;
                chicaLocations[chicaLocationIndex].SetState(chicaLocationState);
                if (chicaLocations[chicaLocationIndex].isFar)
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

            if (bonnieLocationIndex != office.bonnieLocation || bonnieLocationState != office.bonnieLocationState)
            {
                sawBonnie = false;
                if (bonnieLocationIndex == bonnieAttackLocationIndex && leftLight)
                {
                    // If moved away from window, disable light
                    FNAFClient.Instance.FNAF1RequestOfficeChange(false, leftDoorDown, rightLight, rightDoorDown, monitorOpen ? currentCamera : -1);
                }

                bonnieLocations[bonnieLocationIndex].SetState(-1);
                bonnieLocationIndex = office.bonnieLocation;
                bonnieLocationState = office.bonnieLocationState;
                bonnieLocations[bonnieLocationIndex].SetState(bonnieLocationState);
                if (bonnieLocations[bonnieLocationIndex].isFar)
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

            if (foxyLocationIndex != office.foxyLocation || foxyLocationState != office.foxyLocationState)
            {
                if (foxyLocationIndex == foxyAttackLocationIndex && office.foxyLocation != foxyAttackLocationIndex)
                    doorKnockSound.Play();
                foxyLocations[foxyLocationIndex].SetState(-1);
                foxyLocationIndex = office.foxyLocation;
                foxyLocationState = office.foxyLocationState;
                foxyLocations[foxyLocationIndex].SetState(foxyLocationState);
                if (foxyLocations[foxyLocationIndex].isFar)
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
        else if (e.eventType == "officeChange")
        {
            if (rightDoorDown != office.rightDoor)
            {
                doorSound.Play();
                if (office.rightDoor)
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

            if (leftDoorDown != office.leftDoor)
            {
                doorSound.Play();
                if (office.leftDoor)
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

            rightLight = office.rightLight;
            leftLight = office.leftLight;
            rightDoorDown = office.rightDoor;
            leftDoorDown = office.leftDoor;

            if (Room.settings.gameMode == "classic")
            {
                for(int i = 0; i < cameraButtons.Length; i++)
                    cameraButtons[i].sprite = cameraButtonNormal;
                if (Player.controlledByPlayerId == 0)
                {
                    // Show the selected cameras to the controller
                    foreach (FNAFGamePlayer p in game.players)
                        if (p.selectedCameraNumber >= 0 && p.selectedCameraNumber < cameraButtons.Length && p.controlledByPlayerId != 0)
                            cameraButtons[p.selectedCameraNumber].sprite = cameraButtonLookedAt;
                }
            }
        }
        else if (e.eventType == "tick")
        {
            hourText.text = HOUR_NAMES[game.currentHour];
            powerLeftText.text = "Power left: <b>" + Mathf.Floor(office.powerLeft) + "</b>%";

            if (office.powerLeft <= 0f && powerLeft > 0f)
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
                
                //foreach (GameObject obj in isAfton ? requiresPowerAfton : requiresPower)
                //{
                //    obj.SetActive(false);
                //}
            }
            else if (office.powerLeft > 0f && powerLeft <= 0f)
            {
                //foreach (GameObject obj in isAfton ? requiresPowerAfton : requiresPower)
                //{
                //    obj.SetActive(true);
                //}
            }

            powerLeft = office.powerLeft;
        }
        else if (e.eventType == "end")
        {
            StartCoroutine(GameEndSequence());
            
        } 
        else if (e.eventType == "attack")
        {
            //if (Player.controlledByPlayerId != 0)
            //    StartCoroutine(AftonJumpscareSequence(game.attackingMonster));
            //else
            //    StartCoroutine(JumpscareSequence(game.attackingMonster));
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
            Button button = distractionButtons[i];
            button.onClick.RemoveAllListeners();
            button.GetComponentInChildren<Text>().text = GetNameForDistraction(distractions[i]);
            button.onClick.AddListener(() => SendDistraction(distractions[i]));
        }
    }

    public void SendDistraction(string distractionName)
    {
        onMoveAnyone?.Invoke();
        Debug.Log("Send distraction " + distractionName);
        distractionLoadingTime = 5f;
        ShuffleDistractions();
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
        if (phoneGuySound.isPlaying)
        {
            phoneGuySound.Stop();
            cameraSwitchSound.Play();
        }
    }

    private IEnumerator GameEndSequence()
    {
        yield return new WaitForSeconds(5.0f);

        if (Player.controlledByPlayerId != 0)
        {
            if (Player.alive)
                onAftonLose.Invoke();
            else
                onAftonWin.Invoke();
        }
        else
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
        if (Room.settings.gameMode == "classic")
        {
            if (Room.game.players.Any((e) => e.selectedCameraNumber >= 0 && e.controlledByPlayerId != 0))
                usage++;
        }
        else if (Room.settings.gameMode == "duel")
        {
            if (currentCamera >= 0)
                usage++;
        }
        usageImage.sprite = usageSprites[usage];

        officeRenderer.sprite = powerLeft > 0f ? normalOfficeSprite : darkOfficeSprite;
        freddyAttackButton.interactable = moveTimerDone && !rightDoorDown && freddyLocationIndex == freddyAttackLocationIndex;
        chicaAttackButton.interactable = moveTimerDone && !rightDoorDown && chicaLocationIndex == chicaAttackLocationIndex;
        bonnieAttackButton.interactable = moveTimerDone && !leftDoorDown && bonnieLocationIndex == bonnieAttackLocationIndex;
        foxyAttackButton.interactable = moveTimerDone && !leftDoorDown && foxyLocationIndex == foxyAttackLocationIndex;

        staticImage.color = Color.Lerp(staticImage.color, staticColor, Time.deltaTime * staticReturnSpeed);

        if (Random.value > randomStaticNoise)
        {
            staticImage.color = Color.white;
        }

        if (distortedSignalTime > 0f)
        {
            distortedSignalTime -= Time.deltaTime;
            staticImage.color = new Color(1f, 1f, 1f, 0.85f);
            if (!signalLostSound.isPlaying)
            {
                signalLostSound.Play();
                signalLostSound.time = Random.value * signalLostSound.clip.length;
            }
            signalLostSound.volume = monitorOpen ? 0.5f : 0.08f;
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
        fan.SetActive(disableFanTime <= 0f && powerLeft > 0f);

        if (distractionLoadingTime > 0f)
            distractionLoadingTime -= Time.deltaTime;
        foreach(Button button in distractionButtons)
            button.gameObject.SetActive(distractionLoadingTime <= 0f);
        distractionLoading.SetActive(distractionLoadingTime > 0f);

        if (!phoneGuySound.isPlaying)
            phone.SetActive(false);

        if (itsMeTime > 0f)
            itsMeTime -= Time.deltaTime;
        itsMe.SetActive(itsMeTime > 0f);

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
        if (!canToggleMonitor || itsMeTime > 0f) return;
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
            if (phoneGuySound.isPlaying && phoneGuySound.time > 10f)
            {
                phone.transform.position = new Vector2(Random.Range(-8f, 8f), Random.Range(-3.6f, 3.6f));
                phone.SetActive(Random.value <= 0.5f);
            }
            monitorSound.Stop();
            monitor.Start();
            monitorEnableObject.SetActive(false);
            Invoke(nameof(ReenableMonitor), 0.25f);
        }

        if (Player.controlledByPlayerId != 0)
        {
            FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, rightLight, rightDoorDown, monitorOpen ? currentCamera : -1);
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

    private bool CheckButton()
    {
        if (monitorOpen)
        {
            return false;
        }

        if (powerLeft <= 0f || Player.controlledByPlayerId == 0 || (sabotageButtonsTime > 0f && Random.value > 0.5f))
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

        FNAFClient.Instance.FNAF1RequestOfficeChange(!leftLight, leftDoorDown, rightLight, rightDoorDown, monitorOpen ? currentCamera : -1);
    }

    public void LeftDoorToggle()
    {
        if (!CheckButton()) return;

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, !leftDoorDown, rightLight, rightDoorDown, monitorOpen ? currentCamera : -1);
    }


    public void RightLightToggle()
    {
        if (!CheckButton()) return;

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, !rightLight, rightDoorDown, monitorOpen ? currentCamera : -1);
    }

    public void RightDoorToggle()
    {
        if (!CheckButton()) return;

        FNAFClient.Instance.FNAF1RequestOfficeChange(leftLight, leftDoorDown, rightLight, !rightDoorDown, monitorOpen ? currentCamera : -1);
    }

    public void SwitchCamera(int index)
    {
        if (currentCamera == index) return;

        cameraButtons[currentCamera].sprite = cameraButtonNormal;
        cameraButtonBlink = 0.5f;

        cameraSwitchSound.Play();
        cameraSwitchEffect.Play();
        currentCamera = index;
        cameraNameText.text = cameras[index].cameraName;

        cameraButtons[currentCamera].sprite = cameraButtonActive;

        if (Player.controlledByPlayerId != 0)
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
            }
            else
            {
                cameras[i].gameObject.SetActive(false);
            }
        }
    }
}
