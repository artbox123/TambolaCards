using TMPro;
using UnityEngine;

namespace ArtboxGames
{
    public class LoginScreen : GameManager
    {
        [Header("Message Box")]
        [SerializeField] private GameObject m_loadingPanel;
        [SerializeField] private GameObject m_messageBox;
        [SerializeField] private TextMeshProUGUI m_message;

        [SerializeField] private GameObject LoginPanel = default;

        private void Awake()
        {
            InitializeMessageBox(m_loadingPanel, m_messageBox, m_message);
        }

        // Start is called before the first frame update
        void Start()
        {
#if UNITY_IOS
        LoginPanel.SetActive(false);
        LoginAsGuest();
#endif
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (m_messageBox.activeSelf)
                {
                    m_messageBox.SetActive(false);
                }
                else
                {
                    Application.Quit();
                }
            }
        }

        public void LoginAsGuest()
        {
            PlayerInfo.Instance.login = 1;
            PlayerInfo.Instance.userID = SystemInfo.deviceUniqueIdentifier;
            PlayerInfo.Instance.userName = "Guest_" + Random.Range(100, 1000);
            PlayerInfo.Instance.userImage = "0";
            LoadScene("Home", m_loadingPanel, 1f);
        }

        public void LoginWithGoogle()
        {
            DoNotDestroy.Instance.loadingPanel.SetActive(true);
            StartCoroutine(CheckInternet((isConnected) =>
            {
                if (isConnected)
                {
                    SocialLogin.Instance.OnSignIn();
                }
                else
                {
                    ShowMessageBox("No internet connection!");
                }
            }));
        }

        public void LoginWithFacebook()
        {
            DoNotDestroy.Instance.loadingPanel.SetActive(true);
            StartCoroutine(CheckInternet((isConnected) =>
            {
                if (isConnected)
                {
                    //SocialLogin.Instance.LoginWithFacebook();
                }
                else
                {
                    ShowMessageBox("No internet connection!");
                }
            }));
        }
    }
}