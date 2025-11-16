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
    }
}