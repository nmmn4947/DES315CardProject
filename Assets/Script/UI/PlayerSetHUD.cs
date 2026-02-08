using TMPro;
using UnityEngine;

namespace CardProject
{
    public class PlayerSetHUD : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _score;
        [SerializeField] private TextMeshProUGUI _cardsLeft;

        public void SetUpText(string name, int score, int cardN)
        {
            EditNameText(name);
            EditScoreNumber(score);
            EditCardLeftNumber(cardN);
        }
        
        public void EditNameText(string text)
        {
            _nameText.text = text;
        }
        public void EditScoreNumber(int n)
        {
            string s = "Score : ";
            s += n;
            _score.text = s;
        }
        public void EditCardLeftNumber(int n)
        {
            string s = "Cards : ";
            s += n;
            _cardsLeft.text = s;
        }
    }
}
