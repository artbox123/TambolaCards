using UnityEngine;
using TMPro;

namespace ArtboxGames
{
    public class DoNotDestroy : MonoBehaviour
    {
        public static DoNotDestroy Instance;

        [HideInInspector] public GameObject loadingPanel;
        [HideInInspector] public GameObject messageBox;
        [HideInInspector] public TextMeshProUGUI message;
        public Sprite[] playerImages;
        public Sprite[] cardImages;

        [Header("Sound")]
        public AudioSource Music;
        public AudioSource Sound;

        public AudioClip[] sounds;
        public AudioClip[] numbers;
        public AudioClip[] colors;
        public bool gameStarted;

        // Start is called before the first frame update
        void Start()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
        }
    }
}