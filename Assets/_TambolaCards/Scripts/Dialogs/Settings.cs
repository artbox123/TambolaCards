using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace ArtboxGames
{
    public class Settings : GameManager
    {
        [SerializeField] private GameObject soundOff;
        [SerializeField] private GameObject vibrateOff;
        [SerializeField] private TextMeshProUGUI soundText;
        [SerializeField] private TextMeshProUGUI vibrateText;

        [SerializeField] private GameObject logoutBtn;

        // Start is called before the first frame update
        void Start()
        {
            Sound(PlayerInfo.Instance.sound);
            Vibrate(PlayerInfo.Instance.vibration);

#if UNITY_IOS
        logoutBtn.SetActive(false);
#endif
        }

        public void Sound(int status)
        {
            GameObject obj = EventSystem.current.currentSelectedGameObject;
            PlayerInfo.Instance.sound = status;
            if (status == 0)
            {
                soundOff.SetActive(true);
                soundText.text = "Sound Off";
            }
            else
            {
                soundOff.SetActive(false);
                soundText.text = "Sound On";
            }
            if (obj.name.StartsWith("Sound"))
                Vibration.VibratePop();
        }

        public void Vibrate(int status)
        {
            GameObject obj = EventSystem.current.currentSelectedGameObject;
            PlayerInfo.Instance.vibration = status;
            if (status == 0)
            {
                vibrateOff.SetActive(true);
                vibrateText.text = "Vibrate Off";
            }
            else
            {
                vibrateOff.SetActive(false);
                vibrateText.text = "Vibrate On";
            }
            if (obj.name.StartsWith("Vibrate"))
                Vibration.VibratePop();
        }

        public void PrivacyPolicy()
        {
#if UNITY_IOS
        Application.OpenURL("https://dineshladumore.wordpress.com");
#elif UNITY_ANDROID
            Application.OpenURL("https://tambola-cards-0.flycricket.io/privacy.html");
#endif
        }

        public void VideoTutorial()
        {
            Application.OpenURL("https://youtu.be/8UssK9pn7sM");
        }

        public void Logout()
        {
            if (PlayerInfo.Instance.login == 2)
                SocialLogin.Instance.OnSignOut();

            PlayerInfo.Instance.userName = "";
            PlayerInfo.Instance.userImage = "0";
            PlayerInfo.Instance.login = 0;
            LoadScene("Login", DoNotDestroy.Instance.loadingPanel);
        }

        public void Close()
        {
            AdsManager.Instance?.ShowInterstitial();
        }
    }
}