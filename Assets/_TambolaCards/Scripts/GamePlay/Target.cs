using System.Collections.Generic;
using UnityEngine;

namespace ArtboxGames
{
    public class Target : MonoBehaviour
    {
        public Targets target;
        public List<string> cards;
        public int winAmount;
        public GameObject loader;
        public GameObject claimDone;
        public bool isClaimed;
    }
}