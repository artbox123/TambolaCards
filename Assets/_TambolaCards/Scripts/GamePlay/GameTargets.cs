using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ArtboxGames
{
    public class GameTargets : GameManager
    {
        //it will return trail or set
        public List<string> GetTrailOrSet()
        {
            List<string> trailOrSetList = new List<string>();

            List<string> cardColors = new List<string> {
            "Heart",
            "Spade",
            "Diamond",
            "Club"
            };

            List<string> cardCombo = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                string color = cardColors[Random.Range(0, cardColors.Count)];
                cardCombo.Add(color);
                cardColors.Remove(color);
            }

            int Card = Random.Range(1, 14);
            //string str = cardCombo[0] + "_" + Card + "," + cardCombo[1] + "_" + Card + "," + cardCombo[2] + "_" + Card;
            //Debug.Log("1.Trail or Set : " + str);

            trailOrSetList.Add(cardCombo[0] + "_" + Card);
            trailOrSetList.Add(cardCombo[1] + "_" + Card);
            trailOrSetList.Add(cardCombo[2] + "_" + Card);
            return trailOrSetList;
        }

        //it will return pure sequence
        public List<string> GetPureSequence()
        {
            List<string> pureSequenceList = new List<string>();

            List<string> cardColors = new List<string> {
            "Heart",
            "Spade",
            "Diamond",
            "Club"
        };
            string cardsColor = cardColors[Random.Range(0, cardColors.Count)];
            List<int> rankCombo = new List<int>();

            int cardNo = Random.Range(1, 14);
            if (cardNo == 13)
                cardNo = 1;
            for (int i = 0; i < 3; i++)
            {
                rankCombo.Add(cardNo);
                if (cardNo == 13)
                {
                    cardNo = 1;
                }
                else
                {
                    cardNo++;
                }
            }
            //string str1 = cardsColor + "_" + rankCombo[0] + "," + cardsColor + "_" + rankCombo[1] + "," + cardsColor + "_" + rankCombo[2];
            //Debug.Log("2.Pure Sequence : " + str1);

            pureSequenceList.Add(cardsColor + "_" + rankCombo[0]);
            pureSequenceList.Add(cardsColor + "_" + rankCombo[1]);
            pureSequenceList.Add(cardsColor + "_" + rankCombo[2]);

            return pureSequenceList;
        }

        //it will return only sequence
        public List<string> GetSequence()
        {
            List<string> sequenceList = new List<string>();

            List<string> cardColors = new List<string> {
            "Heart",
            "Spade",
            "Diamond",
            "Club",
            "Heart",
            "Spade",
            "Diamond",
            "Club"
        };

            List<string> cardCombo = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                string color = cardColors[Random.Range(0, cardColors.Count)];
                cardCombo.Add(color);
                cardColors.Remove(color);
            }

            List<int> rankCombo = new List<int>();

            int cardNo = Random.Range(1, 14);
            if (cardNo == 13)
                cardNo = 1;
            for (int i = 0; i < 3; i++)
            {
                rankCombo.Add(cardNo);
                if (cardNo == 13)
                {
                    cardNo = 1;
                }
                else
                {
                    cardNo++;
                }
            }
            //string str1 = cardCombo[0] + "_" + rankCombo[0] + "," + cardCombo[1] + "_" + rankCombo[1] + "," + cardCombo[2] + "_" + rankCombo[2];
            //Debug.Log("3.Only Sequence : " + str1);

            sequenceList.Add(cardCombo[0] + "_" + rankCombo[0]);
            sequenceList.Add(cardCombo[1] + "_" + rankCombo[1]);
            sequenceList.Add(cardCombo[2] + "_" + rankCombo[2]);

            return sequenceList;
        }

        //it will return color
        public List<string> GetColor()
        {
            List<string> colorList = new List<string>();

            List<string> cardColors = new List<string> {
            "Heart",
            "Spade",
            "Diamond",
            "Club"
        };
            string cardColor = cardColors[Random.Range(0, cardColors.Count)];
            List<int> cardNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

            List<int> rankCombo = new List<int>();

        getCardAgain:
            for (int j = 0; j < 3; j++)
            {
                int rank = cardNumber[Random.Range(0, cardNumber.Count)];
                rankCombo.Add(rank);
                cardNumber.Remove(rank);
            }
            rankCombo = rankCombo.OrderBy(x => x).ToList();
            if (rankCombo[2] - rankCombo[1] == 1 && rankCombo[1] - rankCombo[0] == 1)
            {
                //Debug.Log ("called again : " + rankCombo [2] + " , " + rankCombo [1] + " , " + rankCombo [0]);
                cardNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
                goto getCardAgain;
            }
            else
            {
                //string str = cardColor + "_" + rankCombo[0] + "," + cardColor + "_" + rankCombo[1] + "," + cardColor + "_" + rankCombo[2];
                //Debug.Log("4.Color : " + str);
            }

            colorList.Add(cardColor + "_" + rankCombo[0]);
            colorList.Add(cardColor + "_" + rankCombo[1]);
            colorList.Add(cardColor + "_" + rankCombo[2]);

            return colorList;
        }

        //it will return pair
        public List<string> GetPair()
        {
            List<string> pairList = new List<string>();

        getColorAgain:
            List<string> cardColors = new List<string> {
            "Heart",
            "Spade",
            "Diamond",
            "Club",
            "Heart",
            "Spade",
            "Diamond",
            "Club"
        };
            List<string> cardCombo = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                string color = cardColors[Random.Range(0, cardColors.Count)];
                cardCombo.Add(color);
                cardColors.Remove(color);
            }
            if (cardCombo[0] == cardCombo[1])
            {
                cardCombo.Clear();
                goto getColorAgain;
            }

            int firstRank = Random.Range(1, 14);

        getRankAgain:
            int secondRank = Random.Range(1, 14);
            if (secondRank == firstRank)
            {
                goto getRankAgain;
            }
            else
            {
                //string str = cardCombo[0] + "_" + firstRank + "," + cardCombo[1] + "_" + firstRank + "," + cardCombo[2] + "_" + secondRank;
                //Debug.Log("5.Pair : " + str);
            }
            if (secondRank > firstRank)
            {
                pairList.Add(cardCombo[0] + "_" + firstRank);
                pairList.Add(cardCombo[1] + "_" + firstRank);
                pairList.Add(cardCombo[2] + "_" + secondRank);
            }
            else
            {
                pairList.Add(cardCombo[0] + "_" + secondRank);
                pairList.Add(cardCombo[1] + "_" + firstRank);
                pairList.Add(cardCombo[2] + "_" + firstRank);
            }
            return pairList;
        }

        //it will return high cards
        public List<string> GetHighCard()
        {
            List<string> highCardsList = new List<string>();

            List<string> cardColors = new List<string> {
            "Heart",
            "Spade",
            "Diamond",
            "Club",
            "Heart",
            "Spade",
            "Diamond",
            "Club"
        };
            List<int> CardNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };

            List<string> cardCombo = new List<string>();
            List<int> rankCombo = new List<int>();

            for (int i = 0; i < 3; i++)
            {
                string color = cardColors[Random.Range(0, cardColors.Count)];
                cardCombo.Add(color);
                cardColors.Remove(color);
            }

        getCardAgain:
            for (int j = 0; j < 3; j++)
            {
                int rank = CardNumber[Random.Range(0, CardNumber.Count)];
                rankCombo.Add(rank);
                CardNumber.Remove(rank);
            }

            rankCombo = rankCombo.OrderBy(x => x).ToList();
            if (rankCombo[2] - rankCombo[1] == 1 && rankCombo[1] - rankCombo[0] == 1)
            {
                //Debug.Log ("called again : " + rankCombo [2] + " , " + rankCombo [1] + " , " + rankCombo [0]);
                CardNumber = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
                goto getCardAgain;
            }
            else
            {
                highCardsList.Add(cardCombo[0] + "_" + rankCombo[0]);
                highCardsList.Add(cardCombo[1] + "_" + rankCombo[1]);
                highCardsList.Add(cardCombo[2] + "_" + rankCombo[2]);
                //string str = cardCombo[0] + "_" + rankCombo[0] + "," + cardCombo[1] + "_" + rankCombo[1] + "," + cardCombo[2] + "_" + rankCombo[2];
                //Debug.Log("6.High Cards : " + str);
            }
            return highCardsList;
        }
    }
}