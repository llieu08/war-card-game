using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace WarCardGame
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("=========================================================================");
            Console.WriteLine("|                     Welcome to the card game of War!                  |");
            Console.WriteLine("|                                                                       |");
            Console.WriteLine("|HOW TO PLAY                                                            |");
            Console.WriteLine("|This is a 2 player game. The deck is shuffled and each player is dealt |");
            Console.WriteLine("|half the deck. Each player reveals a card from the top of their decks. |");
            Console.WriteLine("|The player with the higher value wins both cards. Those cards are      |");
            Console.WriteLine("|returned to the winner's deck and gets reshuffled. If there is a draw, |");
            Console.WriteLine("|the players must go to war. Each player draws 3 cards face down and    |");
            Console.WriteLine("|reveal the 4th card. The player with the higher card wins all 10 cards.|");
            Console.WriteLine("|                                                                       |");
            Console.WriteLine("|HOW TO WIN                                                             |");
            Console.WriteLine("|The player with all the cards wins.                                    |");
            Console.WriteLine("|                                                                       |");
            Console.WriteLine("=========================================================================");
            Console.WriteLine("\nPress enter to begin...");
            Console.ReadLine();

            Stopwatch timer = new Stopwatch();
            timer.Start();

            PlayWar();
            
            timer.Stop();
            Console.WriteLine($"\nTime taken: {timer.ElapsedMilliseconds} ms");
            Console.ReadLine();
        }

        public static void PlayWar()
        {
            var deck = BuildDeckAndShuffle();
            List<Card> playerOne = new List<Card>();
            List<Card> playerTwo = new List<Card>();

            while (deck.Any())
            {
                playerOne.Add(deck.Dequeue());
                playerTwo.Add(deck.Dequeue());
            }

            var turns = Battle(playerOne, playerTwo);
            
            Console.WriteLine($"Player {(playerOne.Any() ? "one" : "two")} wins after {turns} turns!");
        }

        public static Queue<Card> BuildDeckAndShuffle()
        {
            Queue<Card> deck = new Queue<Card>();
            List<Suit> suits = new List<Suit> { Suit.Diamonds, Suit.Clubs, Suit.Hearts, Suit.Spades };
            foreach (var s in suits)
            {
                for (int i = 2; i <= 14; i++)
                {
                    deck.Enqueue(new Card { Suit = s, Value = i });
                }
            }

            return new Queue<Card>(deck.OrderBy(x => Guid.NewGuid()));
        }

        public static List<Card> Shuffle(List<Card> deck)
        {
            return new List<Card>(deck.OrderBy(x => Guid.NewGuid()));
        }

        public static int Battle(List<Card> a, List<Card> b)
        {
            int turns = 0;
            while (a.Any() && b.Any())
            {
                var aCard = a.First();
                var bCard = b.First();

                Console.Write($"{turns}. ");

                if (aCard.Value > bCard.Value)
                {
                    var shuffledCards = Shuffle(new List<Card> { aCard, bCard });
                    b.RemoveAt(0);
                    a.RemoveAt(0);
                    a.AddRange(shuffledCards);
                    Console.WriteLine($"A: {aCard.ToString()} beats B: {bCard.ToString()}");
                }
                else if (aCard.Value < bCard.Value)
                {
                    var shuffledCards = Shuffle(new List<Card> { aCard, bCard });
                    a.RemoveAt(0);
                    b.RemoveAt(0);
                    b.AddRange(shuffledCards);
                    Console.WriteLine($"A: {aCard.ToString()} loses to B: {bCard.ToString()}");
                }
                else
                {
                    Console.WriteLine($"A: {aCard.ToString()} equals B: {bCard.ToString()} I DECLARE WAR...");
                    GoToWar(a, b, new List<Card>());
                }
                turns++;
            }

            return turns;
        }

        public static void GoToWar(List<Card> deckA, List<Card> deckB, List<Card> warPile)
        {
            Card warCardA = null;
            Card warCardB = null;
            var deckACount = deckA.Count;
            var deckBCount = deckB.Count;

            // add warCards to the pile if there is enough for player one
            if (deckACount > 5)
            {
                warCardA = deckA[4];
                warPile.AddRange(deckA.Take(5));
                deckA.RemoveRange(0, 5);
            }
            // add all but the player one's last card (saved for next war if it occurs)
            else
            {
                warCardA = deckA[deckACount - 1];
                if (deckACount > 1)
                {
                    warPile.AddRange(deckA.Take(deckACount - 1));
                    deckA.RemoveRange(0, deckACount - 1);
                }
            }

            // add warCards to the pile if there is enough for player two
            if (deckBCount > 5)
            {
                warCardB = deckB[4];
                warPile.AddRange(deckB.Take(5));
                deckB.RemoveRange(0, 5);
            }
            // add all but the player two's last card (saved for next war if it occurs)
            else
            {
                warCardB = deckB[deckBCount - 1];
                if (deckBCount > 1)
                {
                    warPile.AddRange(deckB.Take(deckBCount - 1));
                    deckB.RemoveRange(0, deckBCount - 1);
                }
            }

            if (warCardA.Value > warCardB.Value)
            {
                Console.WriteLine($"A: {warCardA.ToString()} beats B: {warCardB.ToString()}");
                // add the last winning war card from player one's deck
                if (deckACount <= 5)
                {
                    warPile.Add(deckA.First());
                    deckA.Clear();
                }
                
                // add last losing war card from player two's deck
                if (deckBCount <= 5)
                {
                    warPile.Add(deckB.First());
                    deckB.Clear();
                }
                
                var shuffledCards = Shuffle(warPile);
                deckA.AddRange(shuffledCards);
            }
            else if (warCardA.Value < warCardB.Value)
            {
                Console.WriteLine($"A: {warCardA.ToString()} loses to B: {warCardB.ToString()}");
                if (deckACount <= 5)
                {
                    warPile.Add(deckA.First());
                    deckA.Clear();
                }

                if (deckBCount <= 5)
                {
                    warPile.Add(deckB.First());
                    deckB.Clear();
                }
                
                var shuffledCards = Shuffle(warPile);
                deckB.AddRange(shuffledCards);
            }
            else
            {
                Console.WriteLine($"A: {warCardA.ToString()} equals B: {warCardB.ToString()} I DECLARE WAR...");
                GoToWar(deckA, deckB, warPile);
            }
        }
    }
}
