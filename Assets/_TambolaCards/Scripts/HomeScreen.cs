using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ArtboxGames
{
    public class HomeScreen : GameManager
    {
        public static HomeScreen Instance;

        public TextMeshProUGUI playerLevel;
        public TextMeshProUGUI playerName;
        public Image playerImage;
        public TextMeshProUGUI playerCoins;
        [SerializeField] private GameObject loader;

        [Header("Message Box")]
        [SerializeField] private GameObject m_loadingPanel;
        [SerializeField] private GameObject m_messageBox;
        [SerializeField] private TextMeshProUGUI m_message;

        [Header("Popups")]
        public GameObject playerWaiting;
        public GameObject createOrJoin;
        [SerializeField] private GameObject playerProfile;
        [SerializeField] private GameObject playConfirmation;
        [SerializeField] private GameObject settings;
        [SerializeField] private GameObject coinHistory;
        [SerializeField] private GameObject achievement;
        [SerializeField] private GameObject dailySpin;
        [SerializeField] private GameObject dailyBonus;
        [SerializeField] private GameObject howToPlay;
        [SerializeField] private GameObject WatchVideo;
        [SerializeField] private GameObject rateUs;
        [SerializeField] private GameObject quitGame;

        private void Awake()
        {
            Instance = this;
            InitializeMessageBox(m_loadingPanel, m_messageBox, m_message);
        }

        // Start is called before the first frame update
        void Start()
        {
            SetScreen();
            SetPlayerProfile();

            if (!DoNotDestroy.Instance.gameStarted)
            {
                PlaySound(4);
                DoNotDestroy.Instance.gameStarted = true;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (m_loadingPanel.activeSelf)
                    return;
                if (m_messageBox.activeSelf)
                {
                    m_messageBox.SetActive(false);
                }
                else if (playerWaiting.activeSelf)
                {

                }
                else if (createOrJoin.activeSelf)
                {
                    createOrJoin.SetActive(false);
                }
                else if (playerProfile.activeSelf)
                {
                    playerProfile.SetActive(false);
                }
                else if (playConfirmation.activeSelf)
                {
                    playConfirmation.SetActive(false);
                }
                else if (settings.activeSelf)
                {
                    settings.SetActive(false);
                }
                else if (coinHistory.activeSelf)
                {
                    coinHistory.SetActive(false);
                }
                else if (achievement.activeSelf)
                {
                    achievement.SetActive(false);
                }
                else if (dailySpin.activeSelf)
                {
                    dailySpin.SetActive(false);
                }
                else if (dailyBonus.activeSelf)
                {
                    dailyBonus.SetActive(false);
                }
                else if (howToPlay.activeSelf)
                {
                    howToPlay.SetActive(false);
                }
                else if (WatchVideo.activeSelf)
                {
                    WatchVideo.SetActive(false);
                }
                else if (rateUs.activeSelf)
                {
                    rateUs.SetActive(false);
                }
                else if (quitGame.activeSelf)
                {
                    quitGame.SetActive(false);
                }
                else
                {
                    quitGame.SetActive(true);
                }
            }
        }

        private void SetPlayerProfile()
        {
            if (string.IsNullOrEmpty(PlayerInfo.Instance.userName))
                LoadScene("Login", m_loadingPanel);
            playerLevel.text = "Level : " + GetLevel().ToString("00");
            playerName.text = PlayerInfo.Instance.userName;
            playerCoins.text = PlayerInfo.Instance.coins.ToString();
            StartCoroutine(LoadImageFromPath(PlayerInfo.Instance.userImage, playerImage));
        }

        public void MoreGame()
        {
            Application.OpenURL("https://play.google.com/store/apps/developer?id=Artbox+Infotech");
        }

        public void Share()
        {
            NativeShare share = new NativeShare();
            share.SetSubject("Share").SetTitle("Share").SetText("I want to play Tambola Cards with you! Please install from : " + ShareURL() + " Play with your friends and family.").Share();
        }

        // set screen ui with screen current resolution
        public void SetScreen()
        {
            float ratio = (float)Screen.height / (float)Screen.width;
            //Debug.Log("=== ratio : " + ratio);
            if (ratio >= 0.65f)
            {
                GetComponent<CanvasScaler>().matchWidthOrHeight = 0.1f;
            }
            else if (ratio <= 0.5f)
            {
                GetComponent<CanvasScaler>().matchWidthOrHeight = 0.9f;
            }
        }
    }
}