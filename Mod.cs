using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace CasinoCardNS
{
	public class CasinoCard : Mod
	{
		public override void Ready()
		{
			Logger.Log("Ready!");
		}
	}

	public class Casino : CardData
	{

		int CoinCount = 0;

		protected override bool CanHaveCard(CardData otherCard)
		{
			if (otherCard.Id == "gold")
				return true;
			return base.CanHaveCard(otherCard);
		}

		public void popCoins(int amount)
		{
			GameCard gameCard = WorldManager.instance.CreateCardStack(base.transform.position + Vector3.up * 0.2f, amount, "gold", checkAddToStack: false);
			WorldManager.instance.StackSend(gameCard.GetRootCard(), OutputDir, null, sendToChest: true);
			CoinCount -= amount;
		}

		public bool betGold(int amount)
		{
			return UnityEngine.Random.Range(0, 100) < 99;
		}

		[TimedAction("bet_action")]
		public void BetAction(int heldCoins)
		{

			bool wonBet = betGold(heldCoins);

			if (wonBet)
			{
				int doubled_coins = heldCoins * 2;
				popCoins(doubled_coins);
				WorldManager.instance.CreateSmoke(Position); // create smoke particles at the red pandas position

			}
			else
			{
				WorldManager.instance.CreateSmoke(Position); // create smoke particles at the red pandas position
			}
		}

		public override void UpdateCard()
		{
			var coins = ChildrenMatchingPredicate(childCard => childCard.Id == "gold");
			if (coins.Count > 0)
			{

				int heldCoins = 0;
				foreach (CardData coin in coins)
				{
					coin.MyGameCard.DestroyCard();
					heldCoins++;
				}
				MyGameCard.StartTimer(30f, () => BetAction(heldCoins), $"Betting {heldCoins} coin(s)", "betting");

			}
			base.UpdateCard();
		}
	}

}