using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtboxGames
{
    public class PlayerInfo : GameManager
    {
        public static PlayerInfo Instance;
        public List<CoinHistory> coinHistory = new List<CoinHistory>();
        public List<Achievements> achievements = new List<Achievements>();
        public List<string> playerName;
        public PlayType playType;

        [SerializeField] private int defaultCoins;
        [SerializeField] private TextAsset playerNameFile;

        private int _login;
        private string _userID;
        private string _userName;
        private int _coins;
        private string _userImage;
        private int _sound;
        private int _vibration;
        private int _totalPlayed;
        private int _totalWin;

        private void Start()
        {
            if (PlayerPrefs.HasKey("coinhistory"))
            {
                coinHistory = JsonConvert.DeserializeObject<List<CoinHistory>>(PlayerPrefs.GetString("coinhistory"));
            }

            if (playerNameFile != null)
            {
                playerName = (playerNameFile.text.Split('\n')).ToList();
            }
        }

        public int login
        {
            get
            {
                return _login;
            }
            set
            {
                _login = value;
                PlayerPrefs.SetInt("login", _login);
                PlayerPrefs.Save();
            }
        }

        public string userID
        {
            get
            {
                return _userID;
            }
            set
            {
                _userID = value;
                PlayerPrefs.SetString("userid", _userID);
                PlayerPrefs.Save();
            }
        }

        public string userName
        {
            get
            {
                return _userName;
            }
            set
            {
                _userName = value;
                PlayerPrefs.SetString("username", _userName);
                PlayerPrefs.Save();
            }
        }

        public int coins
        {
            get
            {
                return _coins;
            }
            set
            {
                _coins = value;
                PlayerPrefs.SetInt("Coins", _coins);
                PlayerPrefs.Save();

                if (HomeScreen.Instance != null)
                {
                    HomeScreen.Instance.playerCoins.text = _coins.ToString();
                    HomeScreen.Instance.playerLevel.text = "Level : " + GetLevel().ToString("00");
                }
            }
        }

        public string userImage
        {
            get
            {
                return _userImage;
            }
            set
            {
                _userImage = value;
                PlayerPrefs.SetString("userimage", _userImage);
                PlayerPrefs.Save();
            }
        }

        public int sound
        {
            get
            {
                return _sound;
            }
            set
            {
                _sound = value;
                PlayerPrefs.SetInt("sound", _sound);
                PlayerPrefs.Save();
            }
        }

        public int vibration
        {
            get
            {
                return _vibration;
            }
            set
            {
                _vibration = value;
                PlayerPrefs.SetInt("vibration", _vibration);
                PlayerPrefs.Save();
            }
        }

        public int totalPlayed
        {
            get
            {
                return _totalPlayed;
            }
            set
            {
                _totalPlayed = value;
                PlayerPrefs.SetInt("totalplayed", _totalPlayed);
                PlayerPrefs.Save();
            }
        }

        public int totalWin
        {
            get
            {
                return _totalWin;
            }
            set
            {
                _totalWin = value;
                PlayerPrefs.SetInt("totalwin", _totalWin);
                PlayerPrefs.Save();
            }
        }

        void Awake()
        {
            Instance = this;

            if (!PlayerPrefs.HasKey("login"))
                login = 0;
            else
                login = PlayerPrefs.GetInt("login");

            if (!PlayerPrefs.HasKey("userid"))
                userID = SystemInfo.deviceUniqueIdentifier;
            else
                userID = PlayerPrefs.GetString("userid");

            if (!PlayerPrefs.HasKey("username"))
                userName = "Guest_" + Random.Range(100, 1000);
            else
                userName = PlayerPrefs.GetString("username");

            if (!PlayerPrefs.HasKey("Coins"))
                coins = defaultCoins;
            else
                coins = PlayerPrefs.GetInt("Coins");

            if (!PlayerPrefs.HasKey("userimage"))
                userImage = "0";
            else
                userImage = PlayerPrefs.GetString("userimage");

            if (!PlayerPrefs.HasKey("sound"))
                sound = 1;
            else
                sound = PlayerPrefs.GetInt("sound");

            if (!PlayerPrefs.HasKey("vibration"))
                vibration = 1;
            else
                vibration = PlayerPrefs.GetInt("vibration");

            if (!PlayerPrefs.HasKey("totalplayed"))
                totalPlayed = 0;
            else
                totalPlayed = PlayerPrefs.GetInt("totalplayed");

            if (!PlayerPrefs.HasKey("totalwin"))
                totalWin = 0;
            else
                totalWin = PlayerPrefs.GetInt("totalwin");

            if (!PlayerPrefs.HasKey("achievements"))
                InitializeAchievements();
            else
                achievements = JsonConvert.DeserializeObject<List<Achievements>>(PlayerPrefs.GetString("achievements"));

            if (!PlayerPrefs.HasKey("facebook"))
            {
                PlayerPrefs.SetInt("facebook", 0);
                PlayerPrefs.Save();
            }
        }

        public void UpdateCoins(CoinAction action, int amount)
        {
            if (action == CoinAction.Add)
                coins += amount;
            else if (action == CoinAction.Minuse)
            {
                coins -= amount;
                if (coins <= 0)
                    coins = 0;
            }
        }

        private void InitializeAchievements()
        {
            // achievement 1
            //Achievements achievement1 = new Achievements();
            //achievement1.id = "achievement1";
            //achievement1.name = "Facebook Login";
            //achievement1.description = "Get 1000 coins on first Facebook login";
            //achievement1.limit = 1f;
            //achievement1.fillAmount = 0f;
            //achievement1.status = false;
            //achievement1.winAmount = 1000;

            // achievement 2
            Achievements achievement2 = new Achievements();
            achievement2.id = "achievement2";
            achievement2.name = "Complete 5 days bonus";
            achievement2.description = "Get 5000 coins by completing 5 days bonus";
            achievement2.limit = 1f;
            achievement2.fillAmount = 0f;
            achievement2.status = false;
            achievement2.winAmount = 5000;

            // achievement 3
            Achievements achievement3 = new Achievements();
            achievement3.id = "achievement3";
            achievement3.name = "Win first 10 games";
            achievement3.description = "Get 5000 coins by winning first 10 games";
            achievement3.limit = 10f;
            achievement3.fillAmount = 0f;
            achievement3.status = false;
            achievement3.winAmount = 5000;

            // achievement 4
            Achievements achievement4 = new Achievements();
            achievement4.id = "achievement4";
            achievement4.name = "Win first 20 games";
            achievement4.description = "Get 10,000 coins by winning first 20 games";
            achievement4.limit = 20f;
            achievement4.fillAmount = 0f;
            achievement4.status = false;
            achievement4.winAmount = 10000;

            // achievement 5
            Achievements achievement5 = new Achievements();
            achievement5.id = "achievement5";
            achievement5.name = "Win first 30 games";
            achievement5.description = "Get 20,000 coins by winning first 30 games";
            achievement5.limit = 30f;
            achievement5.fillAmount = 0f;
            achievement5.status = false;
            achievement5.winAmount = 20000;

            //achievements.Add(achievement1);
            achievements.Add(achievement2);
            achievements.Add(achievement3);
            achievements.Add(achievement4);
            achievements.Add(achievement5);

            SaveAchievements();
        }

        public void SaveAchievements()
        {
            string str_achievements = JsonConvert.SerializeObject(achievements);
            PlayerPrefs.SetString("achievements", str_achievements);
            PlayerPrefs.Save();
        }
    }

    public enum CoinAction
    {
        Add,
        Minuse
    }

    public enum PlayType
    {
        Play = 0,
        Party = 1,
    }

    public class CoinHistory
    {
        public string date { get; set; }
        public Actions action { get; set; }
        public string coins { get; set; }
        public string type { get; set; }
    }

    public class Achievements
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public float limit { get; set; }
        public float fillAmount { get; set; }
        public bool status { get; set; }
        public int winAmount { get; set; }
    }

    public enum Actions
    {
        CoinsStore,
        ChipsStore,
        FreeVideo,
        DailyBonus,
        DailySpin,
        Achievement,
        GameWin,
        GameLose,
        Play,
        Party
    }

    public enum TransactionType
    {
        Credit,
        Debit
    }

    public enum Targets
    {
        TrailOrSet,
        PureSequence,
        Sequence,
        Color,
        Pair,
        HighCard
    }
}