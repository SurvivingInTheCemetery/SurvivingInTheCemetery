using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Text = TMPro.TMP_Text;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Unity.VisualScripting;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : Manager<UIManager>
{
    public Button[] reaperButton; 
    public Button[] ghostButton; 

    public bool useCountdown = false;
    public float countdownTime = 5f;

    private bool isStarted;
    public bool IsStarted
    {
        get
        {
            return isStarted;
        }
    }

    private GameObject endingCanvas;
    public GameObject EndingCanvas
    {
        get
        {
            return endingCanvas;
        }
        set
        {
            endingCanvas = value;
        }
    }

    public Text timer;
    public Text notice;
    public GameObject ghostUI;
    public GameObject reaperUI;
    public Sprite completeKey;

    private float curtime = 0f;

    private Hashtable customProperties;
    private string playerKind = "";

    private void Start()
    {
        isStarted = false;
        countdownTime++;
    }

    private bool isSetUI = false;
    private bool isShowEnding = false;
    private void Update()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "GameScene")
        {
            Countdown(useCountdown);
            SetPlayerUI();
            UpdateCount();
            BindButton();
        }
        else
        {
            if (endingCanvas == null)
            {
                Debug.Log("======================================================================");
                endingCanvas = GameObject.Find("Canvas");
                if (photonView.IsMine)
                {
                    Transform ghost = endingCanvas.transform.Find("Ghost");
                    Transform reaper = endingCanvas.transform.Find("Reaper");
                    var prop = PhotonNetwork.LocalPlayer.CustomProperties;
                    while (prop != null)
                    {
                        prop = PhotonNetwork.LocalPlayer.CustomProperties;
                    }
                    if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isWin"])
                    {
                        if (playerKind == "Ghost")
                        {
                            ghost.gameObject.SetActive(true);
                            ghost.Find("Win").gameObject.SetActive(true);
                        }
                        else
                        {
                            reaper.gameObject.SetActive(true);
                            reaper.Find("Win").gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        if (playerKind == "Ghost")
                        {
                            ghost.gameObject.SetActive(true);
                            ghost.Find("Lose").gameObject.SetActive(true);
                        }
                        else
                        {
                            reaper.gameObject.SetActive(true);
                            reaper.Find("Lose").gameObject.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    void Countdown(bool isUse)
    {
        if (isUse)
        {
            if (curtime < countdownTime - 1)
            {
                curtime += Time.deltaTime;
                timer.text = ((int)(countdownTime - curtime)).ToString();
            }
            else if (!isStarted)
            {
                timer.gameObject.SetActive(false);
                isStarted = true;
            }
        }
        else
        {
            if(!isStarted) isStarted = true;
        }
    }

    void SetPlayerUI()
    {
        if (GameManager.Instance.IsPlayerCreated && !isSetUI)
        {
            if(PhotonNetwork.LocalPlayer.CustomProperties["isReaper"] != null)
            {
                if ((bool)PhotonNetwork.LocalPlayer.CustomProperties["isReaper"])
                {
                    reaperUI.SetActive(true);
                    playerKind = "Reaper";
                }
                else
                {
                    ghostUI.SetActive(true);
                    playerKind = "Ghost";
                }
                isSetUI = true;
            }
        }
    }

    private bool isCompleteKey = false;

    [PunRPC]
    void ShowNotice()
    {
        AndroidAPI.ToastMessage("��Ż�� �����Ǿ����ϴ�! ��Ż�� ���� ����ĥ �� �ֽ��ϴ�!");
    }

    void UpdateCount()
    {
        Text countUI = null;
        if(isSetUI)
        {
            if(playerKind == "Ghost")
            {
                if (countUI == null) countUI = ghostUI.transform.Find("KeyCount").Find("Count").GetComponent<Text>();
                int count = GameManager.Instance.BrokenKeyCount;
                countUI.text = count.ToString() + " / 4";
                if (count == 4 && !isCompleteKey)
                {
                    ghostUI.transform.Find("KeyCount").Find("Image").GetComponent<Image>().sprite = completeKey;
                    photonView.RPC("ShowNotice", RpcTarget.All);
                    isCompleteKey = true;
                }
            }
            else
            {
                if (countUI == null) countUI = reaperUI.transform.Find("SoulStack").Find("Count").GetComponent<Text>();
                int count = GameManager.Instance.Player.GetComponent<ReaperSkill_Blue>().skillCount;
                countUI.text = count.ToString() + " / 4";
            }
        }
    }
    private bool isBinded = false;
    void BindButton()
    {
        if (GameManager.Instance.Player != null && !isBinded)
        {
            if (playerKind == "Reaper")
            {
                reaperButton[0].onClick.AddListener(GameManager.Instance.Player.GetComponent<ReaperButtonInput>().Attack);
                reaperButton[1].onClick.AddListener(GameManager.Instance.Player.GetComponent<ReaperButtonInput>().Skill);
            }
            else
            {
                ghostButton[0].onClick.AddListener(GameManager.Instance.Player.GetComponent<GhostState>().Transformation);
                ghostButton[1].onClick.AddListener(GameManager.Instance.Player.GetComponent<GhostState>().Heist);
            }
            isBinded = true;
        }
    }
}
