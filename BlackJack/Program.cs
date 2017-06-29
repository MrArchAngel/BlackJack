using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlackJack
{
    class Blackjack
    {
        public enum Suit
        {
            Hearts, Diamonds, Spades, Clubs
        }

        public enum Face
        {
            Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King
        }

        public class Card
        {
            public Suit Suit { get; set; }
            public Face Face { get; set; }
            public int Value { get; set; }
        }

        public class Deck
        {
            private List<Card> cards;

            public Deck()
            {
                this.Play();
            }

            public void Play()
            {
                cards = new List<Card>();

                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 13; j++)
                    {
                        cards.Add(new Card() { Suit = (Suit)i, Face = (Face)j });

                        if (j <= 8)
                        {
                            cards[cards.Count - 1].Value = j + 1;
                        }
                        else if (j >= 9)
                        {
                            cards[cards.Count - 1].Value = 10;
                        }
                    }
                }
            }

            public void Shuffle()
            {
                Random rng = new Random();
                int n = cards.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    Card card = cards[k];
                    cards[k] = cards[n];
                    cards[n] = card;
                }
            }

            public Card DrawACard()
            {
                if (cards.Count <= 0)
                {
                    this.Play();
                    this.Shuffle();
                }

                Card cardToReturn = cards[cards.Count - 1];
                cards.RemoveAt(cards.Count - 1);
                return cardToReturn;
            }

            public int GetAmountOfRemainingCards()
            {
                return cards.Count;
            }

            public void PrintDeck()
            {
                int i = 1;
                foreach (Card card in cards)
                {
                    Console.WriteLine("Card {i}: {1} of {2}. Value: {3}", i, card.Face, card.Suit, card.Value);
                    i++;
                }
            }
        }

        class Program
        {
            static int chips;
            static Deck deck;
            static List<Card> playerHand;
            static List<Card> dealerHand;

            static void Main(string[] args)
            {
                Console.WriteLine("Welcome to BlackJack\n");

                chips = 100;
                deck = new Deck();
                deck.Shuffle();

                while (chips > 0)
                {
                    DealHand();
                    Console.WriteLine("\nPress any key for the next hand...\n");
                    ConsoleKeyInfo userInput = Console.ReadKey(true);
                }

                Console.WriteLine("You Lost. Play Again.");
                Console.ReadLine();
            }

            static void DealHand()
            {
                if (deck.GetAmountOfRemainingCards() < 8)
                {
                    deck.Play();
                    deck.Shuffle();
                }

                Console.WriteLine("Remaining Cards: {0}", deck.GetAmountOfRemainingCards());
                Console.WriteLine("Current Chips: {0}", chips);
                Console.WriteLine("How much would you like to bet? (1 - {0})", chips);
                string input = Console.ReadLine().Trim().Replace(" ", "");
                int betAmount;
                while (!Int32.TryParse(input, out betAmount) || betAmount < 1 || betAmount > chips)
                {
                    Console.WriteLine("Illegal bet amount. How much would you like to bet? (1 - {0})", chips);
                    input = Console.ReadLine().Trim().Replace(" ", "");
                }
                Console.WriteLine();

                playerHand = new List<Card>();
                playerHand.Add(deck.DrawACard());
                playerHand.Add(deck.DrawACard());

                

                Console.WriteLine("[Players Hand]");
                Console.WriteLine("Card 1: {0} of {1}", playerHand[0].Face, playerHand[0].Suit);
                Console.WriteLine("Card 2: {0} of {1}", playerHand[1].Face, playerHand[1].Suit);

                foreach (Card card in playerHand)
                {
                    if (card.Face == Face.Ace)
                    {
                        Console.WriteLine("Is your Ace worth 1 or 11?");
                        string val = Console.ReadLine();

                        if (val == "1" || val == "11")
                        {
                            card.Value = Convert.ToInt32(val);
                            break;
                        }
                    }
                }
                Console.WriteLine("Total: {0}\n", playerHand[0].Value + playerHand[1].Value);

                dealerHand = new List<Card>();
                dealerHand.Add(deck.DrawACard());
                dealerHand.Add(deck.DrawACard());

                foreach (Card card in dealerHand)
                {
                    if (card.Face == Face.Ace)
                    {
                        card.Value += 10;
                        break;
                    }
                }

                Console.WriteLine("[Dealers Hand]");
                Console.WriteLine("Card 1: {0} of {1}", dealerHand[0].Face, dealerHand[1].Suit);
                Console.WriteLine("Card 2: [Hole Card]");
                Console.WriteLine("Total: {0}\n", dealerHand[0].Value);

                bool insurance = false;

                if (dealerHand[0].Face == Face.Ace)
                {
                    Console.WriteLine("Insurance? (y / n)");
                    string userInput = Console.ReadLine();

                    while (userInput != "y" && userInput != "n")
                    {
                        Console.WriteLine("I did not understand that. Insurance? (y / n)");
                        userInput = Console.ReadLine();
                    }

                    if (userInput == "y")
                    {
                        insurance = true;
                        //chips -= betAmount / 2;
                        Console.WriteLine("\n[Insurance Accepted]\n");
                    }
                    else
                    {
                        insurance = false;
                        Console.WriteLine("\n[Insurance Rejected]\n");
                    }
                }

                if (dealerHand[0].Face == Face.Ace || dealerHand[0].Value == 10)
                {
                    Console.WriteLine("Dealer checks for blackjack...\n");
                    Thread.Sleep(1500);
                    if (dealerHand[0].Value + dealerHand[1].Value == 21)
                    {
                        Console.WriteLine("[Dealers Hand]");
                        Console.WriteLine("Card 1: {0} of {1}", dealerHand[0].Face, dealerHand[1].Suit);
                        Console.WriteLine("Card 2: {0} of {1}", dealerHand[1].Face, dealerHand[1].Suit);
                        Console.WriteLine("Total: {0}\n", dealerHand[0].Value + dealerHand[1].Value);

                        Thread.Sleep(1500);

                        int amountLost = 0;

                        if (playerHand[0].Value + playerHand[1].Value == 21 && insurance)
                        {
                            amountLost = betAmount / 2;
                            chips -= betAmount / 2;
                        }
                        else if (playerHand[0].Value + playerHand[1].Value != 21 && !insurance)
                        {
                            amountLost = betAmount + betAmount / 2;
                            chips -= betAmount + betAmount / 2;
                        }

                        Console.WriteLine("You lost {0} chips.", amountLost);
                        Thread.Sleep(1500);
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Dealer does not have a blackjack.\n");
                    }
                }

                if (playerHand[0].Value + playerHand[1].Value == 21)
                {
                    Console.WriteLine("Blackjack, You Won ({0} chips.)\n", betAmount + betAmount / 2);
                    chips += betAmount + betAmount / 2;
                    return;
                }

                do
                {
                    Console.WriteLine("[(S)tand (H)it]");
                    ConsoleKeyInfo userOption = Console.ReadKey(true);
                    while (userOption.Key != ConsoleKey.H && userOption.Key != ConsoleKey.S)
                    {
                        Console.WriteLine("Illegal choice. [(S)tand (H)it]");
                        userOption = Console.ReadKey(true);
                    }
                    Console.WriteLine();

                    switch (userOption.Key)
                    {
                        case ConsoleKey.H:
                            Console.WriteLine("Card 1: {0} of {1}", playerHand[0].Face, playerHand[0].Suit);
                            Console.WriteLine("Card 2: {0} of {1}", playerHand[1].Face, playerHand[1].Suit);
                            playerHand.Add(deck.DrawACard());
                            int totalCardsValue = 0;
                            foreach (Card card in playerHand)
                            {
                                totalCardsValue += card.Value;
                            }
                            Console.WriteLine("Card {0}: {1} of {2}", playerHand.Count, playerHand[playerHand.Count - 1].Face, playerHand[playerHand.Count - 1].Suit);
                            Console.WriteLine("Total: {0}\n", totalCardsValue);

                            if (totalCardsValue > 21)
                            {
                                if (playerHand[0].Value == 11)
                                {
                                    playerHand[0].Value = 1;
                                }
                                else if (playerHand[1].Value == 11)
                                {
                                    playerHand[1].Value = 1;
                                }
                                else if (playerHand[2].Value == 11)
                                {
                                    playerHand[2].Value = 1;
                                }
                                else
                                {
                                    Console.Write("Busted\n");
                                    chips -= betAmount;
                                    Thread.Sleep(1500);
                                    return;
                                }
                                continue;
                            }

                            else if (totalCardsValue == 21)
                            {
                                Console.WriteLine("21, You should stand....\n");
                                Thread.Sleep(1500);
                                continue;
                            }
                            else
                            {
                                continue;
                            }

                        case ConsoleKey.S:

                            Console.WriteLine("[Dealers Hand]");
                            Console.WriteLine("Card 1: {0} of {1}", dealerHand[0].Face, dealerHand[1].Suit);
                            Console.WriteLine("Card 2: {0} of {1}", dealerHand[1].Face, dealerHand[1].Suit);

                            int dealerCardsValue = 0;
                            foreach (Card card in dealerHand)
                            {
                                dealerCardsValue += card.Value;
                            }

                            while (dealerCardsValue < 17)
                            {
                                Thread.Sleep(1500);
                                dealerHand.Add(deck.DrawACard());
                                dealerCardsValue = 0;
                                foreach (Card card in dealerHand)
                                {
                                    dealerCardsValue += card.Value;
                                }
                                Console.WriteLine("Card {0}: {1} of {2}", dealerHand.Count, dealerHand[dealerHand.Count - 1].Face, dealerHand[dealerHand.Count - 1].Suit);
                            }

                            dealerCardsValue = 0;
                            foreach (Card card in dealerHand)
                            {
                                dealerCardsValue += card.Value;
                            }
                            Console.WriteLine("Total: {0}\n", dealerCardsValue);

                            if (dealerCardsValue > 21)
                            {
                                Console.WriteLine("Dealer busts! You win {0} chips.", betAmount);
                                chips += betAmount;
                                return;
                            }
                            else
                            {
                                int playerCardsValue = 0;
                                foreach (Card card in playerHand)
                                {
                                    playerCardsValue += card.Value;
                                }

                                if (dealerCardsValue > playerCardsValue)
                                {
                                    Console.WriteLine("Players hand {0}   Dealers hand {1}    You lose {2} chips!", playerCardsValue, dealerCardsValue, betAmount);
                                    chips -= betAmount;
                                    return;
                                }
                                else if (playerCardsValue > dealerCardsValue)
                                {
                                    Console.WriteLine("Players hand {0}     Dealers hand {1}    You win {2} chips!", playerCardsValue, dealerCardsValue, betAmount);
                                    chips += betAmount;
                                    return;
                                }
                                else
                                {
                                    Console.WriteLine("Players hand {0}     Dealers hand {1}    Push! You neither win nor lose!!", playerCardsValue, dealerCardsValue);
                                    return;
                                }
                            }

                        default:
                            break;
                    }

                    Console.ReadLine();
                }
                while (true);
            }
        }
    }
}