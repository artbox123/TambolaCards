using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

namespace ArtboxGames
{
    public class GameManager : MonoBehaviour
    {
        private string characters = "123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";

        public IEnumerator LoadAsyncScene(string sceneName, GameObject loading = null, float waitTime = 0, Image fillImage = null, TextMeshProUGUI fillPercent = null)
        {
            // The Application loads the Scene in the background as the current Scene runs.
            // This is particularly good for creating loading screens.
            // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
            // a sceneBuildIndex of 1 as shown in Build Settings.
            if (loading != null)
                loading.SetActive(true);
            yield return new WaitForSeconds(waitTime);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            // Wait until the asynchronous scene fully loads
            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 1f);
                if (fillImage != null)
                {
                    fillImage.fillAmount = progress;
                    if (fillPercent != null)
                        fillPercent.text = "Loading... " + (asyncLoad.progress * 100f).ToString("00") + "%";
                }
                yield return null;
            }
            if (asyncLoad.isDone)
            {
                if (fillImage != null)
                {
                    fillImage.fillAmount = 1f;
                }
            }
        }

        public void LoadScene(string sceneName, GameObject loading = null, float waitTime = 0, Image fillImage = null, TextMeshProUGUI fillPercent = null)
        {
            StartCoroutine(LoadAsyncScene(sceneName, loading, waitTime, fillImage, fillPercent));
        }

        public IEnumerator CheckInternet(System.Action<bool> action)
        {
            WWW www = new WWW("http://google.com");
            yield return www;
            if (www.error != null)
            {
                action(false);
            }
            else
            {
                action(true);
            }
        }

        public IEnumerator LoadImageFromPath(string path, Image _userImage)
        {
            if (path.StartsWith("https"))
            {
                int h_w = 90;
                if (path.StartsWith("https://graph"))
                    h_w = 200;

                WWW url = new WWW(path);
                Texture2D textFb2 = new Texture2D(h_w, h_w, TextureFormat.RGBA32, false); //TextureFormat must be DXT5

                yield return url;
                //Debug.Log("GetUserImage : " + url);

                float width = (float)textFb2.width;
                float height = (float)textFb2.height;
                Rect rect = new Rect(0, 0, width, height);

                if (url.error == null)
                {
                    _userImage.sprite = Sprite.Create(textFb2, rect, new Vector2(0, 0), 1);
                    url.LoadImageIntoTexture(textFb2);
                }
            }
            else if (path.Length > 1)
            {
                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    yield return null;
                }
                float width = (float)texture.width;
                float height = (float)texture.height;
                Rect rect = new Rect(0, 0, width, height);

                _userImage.sprite = Sprite.Create(texture, rect, new Vector2(0, 0), 1);
            }
            else if (path.Length == 1)
            {
                int index = int.Parse(path);
                _userImage.sprite = GetPlayerImage(index);
            }
        }

        public Sprite GetPlayerImage(int index)
        {
            return DoNotDestroy.Instance.playerImages[index];
        }

        public void InitializeMessageBox(GameObject m_loadingPanel, GameObject m_messageBox, TextMeshProUGUI m_message)
        {
            DoNotDestroy.Instance.loadingPanel = m_loadingPanel;
            DoNotDestroy.Instance.messageBox = m_messageBox;
            DoNotDestroy.Instance.message = m_message;
        }

        public void ShowMessageBox(string msg)
        {
            DoNotDestroy.Instance.messageBox.SetActive(true);
            DoNotDestroy.Instance.loadingPanel.SetActive(false);
            DoNotDestroy.Instance.message.text = msg;
        }

        public void DisconnectPlayer()
        {
            ServerCode.Instance.SendDisconnect();
            ServerCode.Instance.joinedPlayer.Clear();
            ServerCode.Instance.isAdmin = false;
            ServerCode.Instance.roomCode = "";
        }

        public bool CheckBalance(int bootAmount)
        {
            if (PlayerInfo.Instance.coins >= bootAmount)
                return true;
            else
            {
                ShowMessageBox("You dont have enough coins!");
                return false;
            }
        }

        public string GenerateRoomId(int length = 6)
        {
            string roomId = "";
            for (int i = 0; i < length; i++)
            {
                int a = Random.Range(0, characters.Length);
                roomId = roomId + characters[a];
            }
            //Debug.Log("=== room id : " + roomId);
            return roomId;
        }

        public void AddCoinHistory(Actions action, int coins, TransactionType type)
        {
            CoinHistory c_history = new CoinHistory();
            c_history.date = System.DateTime.Now.ToString();
            c_history.action = action;
            c_history.coins = coins.ToString();
            c_history.type = type.ToString();
            PlayerInfo.Instance.coinHistory.Add(c_history);

            if (PlayerInfo.Instance.coinHistory.Count > 10)
            {
                PlayerInfo.Instance.coinHistory.RemoveAt(0);
            }

            string str_history = JsonConvert.SerializeObject(PlayerInfo.Instance.coinHistory);
            PlayerPrefs.SetString("coinhistory", str_history);
            PlayerPrefs.Save();
        }

        public Sprite CardImage(string card)
        {
            if (card.StartsWith("Spade"))
            {
                return DoNotDestroy.Instance.cardImages[0];
            }
            else if (card.StartsWith("Club"))
            {
                return DoNotDestroy.Instance.cardImages[1];
            }
            else if (card.StartsWith("Heart"))
            {
                return DoNotDestroy.Instance.cardImages[2];
            }
            else if (card.StartsWith("Diamond"))
            {
                return DoNotDestroy.Instance.cardImages[3];
            }
            return DoNotDestroy.Instance.cardImages[0];
        }

        public string CardNumber(string card, bool getNumber = false)
        {
            string[] splitCard = card.Split('_');
            string cardNum = splitCard[1];

            if (!getNumber)
            {
                if (cardNum == "1")
                    cardNum = "A";
                else if (cardNum == "11")
                    cardNum = "J";
                else if (cardNum == "12")
                    cardNum = "Q";
                else if (cardNum == "13")
                    cardNum = "K";
            }
            return cardNum;
        }

        public Color32 NumberColor(string card)
        {
            if (card.StartsWith("Spade"))
            {
                return new Color32(244, 188, 57, 255);
            }
            else if (card.StartsWith("Club"))
            {
                return new Color32(0, 199, 146, 255);
            }
            else if (card.StartsWith("Heart"))
            {
                return new Color32(254, 129, 234, 255);
            }
            else if (card.StartsWith("Diamond"))
            {
                return new Color32(5, 207, 237, 255);
            }
            return new Color32(244, 188, 57, 255);
        }

        public int GetLevel()
        {
            int level = (PlayerInfo.Instance.coins / 10000);
            if (level <= 1)
                return 1;
            else
                return level;
        }

        public void PlaySound(int index = 0)
        {
            if (PlayerInfo.Instance.sound == 1)
            {
                DoNotDestroy.Instance.Sound.PlayOneShot(DoNotDestroy.Instance.sounds[index]);
            }
        }

        public void PlayVoice(int number, string color)
        {
            if (PlayerInfo.Instance.sound == 1)
            {
                StartCoroutine(PlayVoiceWithColor(number, color));
            }
        }

        private IEnumerator PlayVoiceWithColor(int number, string strcolor)
        {
            DoNotDestroy.Instance.Sound.PlayOneShot(DoNotDestroy.Instance.numbers[number - 1]);
            yield return new WaitForSeconds(1f);
            int color = 0;
            if (strcolor.StartsWith("Spade"))
                color = 0;
            else if (strcolor.StartsWith("Club"))
                color = 1;
            else if (strcolor.StartsWith("Heart"))
                color = 2;
            else if (strcolor.StartsWith("Diamond"))
                color = 3;
            DoNotDestroy.Instance.Sound.PlayOneShot(DoNotDestroy.Instance.colors[color]);
        }

        public string ShareURL()
        {
#if UNITY_IOS
        return "https://apps.apple.com/us/developer/dixit-rathod/id1581604214";
#elif UNITY_ANDROID
            return "https://play.google.com/store/apps/details?id=" + Application.identifier;
#endif
        }
    }
}