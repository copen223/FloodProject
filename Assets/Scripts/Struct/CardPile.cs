using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Struct
{
    public class CardPile
    {
        public CardPile(string _name)
        {
            pile_name = _name;
        }
        public CardPile()
        {
            pile_name = "none";
        }


        public string Name
        {
            get
            {
                return pile_name;
            }
        }
        private string pile_name;

        public int Count
        {
            get
            {
                return cards_list.Count;
            }
        }

        public List<Card> cards_list = new List<Card>();

        public bool HasCard
        {
            get { return cards_list.Count > 0; }
            set { }
        }

        public void AddCard(Card card)
        {
            cards_list.Add(card);
        }
        public void RemoveCard(Card card)
        {
            cards_list.Remove(card);
        }
        public Card GetFirstCard()
        {
            return cards_list[0];
        }
        private void TranslateCardTo(CardPile to)
        {
            Card ct = cards_list[0];
            to.AddCard(ct);

            if (to.Name=="focusPile")
            {
                ct.isFocused = true;
                return;
            }

            this.RemoveCard(ct);
        }
        public void TranslateCardTo(Card card, CardPile to)
        {
            Card ct = card;
            to.AddCard(ct);

            if (to.Name == "focusPile")
            {
                ct.isFocused = true;
                return;
            }

            this.RemoveCard(ct);
        }
        public void TranslateCardsTo(CardPile to,int num)
        {
            for(int i=0;i<num;i++)
            {
                TranslateCardTo(to);
            }
        }

        public void SetHolder(ActorMono actor)
        {
            for(int i=0;i<cards_list.Count;i++)
            {
                Card card = cards_list[i];
                card.holder = actor;
            }
        }


        public void Shuffle()
        {
            Random random = new Random();

            for(int i = 0;i<cards_list.Count;i++)
            {
                int t = random.Next(0, cards_list.Count - 1);

                var temp = cards_list[t];
                cards_list[t] = cards_list[i];
                cards_list[i] = temp;
            }
        }

    }
}
