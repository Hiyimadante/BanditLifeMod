using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using BanditLifeMod.Models;

namespace BanditLifeMod.Behaviors
{
    public class BanditLifeBehavior : CampaignBehaviorBase
    {
        private BanditPlayerData _banditData;
        private bool _isBandit = false;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSettlementLeftEvent.AddNonSerializedListener(this, OnSettlementLeft);
            CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, OnDailyTick);
            CampaignEvents.OnPartyInteraction.AddNonSerializedListener(this, OnPartyInteraction);
        }

        public override void SyncData(IDataStore dataStore)
        {
            dataStore.SyncData("bandit_life_is_bandit", ref _isBandit);
            dataStore.SyncData("bandit_life_player_data", ref _banditData);
        }

        public void BecomeBandit()
        {
            if (_isBandit)
            {
                InformationManager.DisplayMessage(new InformationMessage(
                    "Ya eres un bandido!", 
                    Colors.Red));
                return;
            }

            _isBandit = true;
            _banditData = new BanditPlayerData
            {
                BecameBanditTime = CampaignTime.Now
            };

            DeclareWarToAllFactions();
            SetPlayerAsBandit();

            InformationManager.DisplayMessage(new InformationMessage(
                "¡Te has convertido en un bandido! Todas las facciones te declaran guerra.",
                Colors.Yellow));
        }

        private void DeclareWarToAllFactions()
        {
            var playerFaction = Hero.MainHero.MapFaction;
            var mainFactions = Campaign.Current.Factions
                .Where(f => f.IsKingdomFaction || f.IsBanditFaction)
                .ToList();

            foreach (var faction in mainFactions)
            {
                if (faction == playerFaction || faction.Leader == Hero.MainHero)
                    continue;

                if (faction.IsAtWarWith(playerFaction))
                    continue;

                FactionManager.DeclareWar(playerFaction, faction);
            }
        }

        private void SetPlayerAsBandit()
        {
            var playerHero = Hero.MainHero;
            var banditFaction = Campaign.Current.Factions.FirstOrDefault(f => f.IsBanditFaction);
            if (banditFaction != null)
            {
                playerHero.SetNewOccupation(Occupation.Bandit);
            }
        }

        private void OnSettlementLeft(Settlement settlement, MobileParty mobileParty)
        {
            if (mobileParty?.LeaderHero == Hero.MainHero && _isBandit)
            {
                ApplyBanditEffects();
            }
        }

        private void OnDailyTick()
        {
            if (!_isBandit || Hero.MainHero == null)
                return;

            var playerParty = MobileParty.MainParty;
            if (playerParty != null)
            {
                EnforceBanditLife();
            }
        }

        private void OnPartyInteraction(MobileParty mobileParty)
        {
            if (!_isBandit || mobileParty == null)
                return;

            if (mobileParty.IsBandit || (mobileParty.LeaderHero != null && 
                mobileParty.LeaderHero.Occupation == Occupation.Bandit))
            {
                if (MobileParty.MainParty.Morale > mobileParty.Morale)
                {
                    TransferBanditsToPlayerParty(mobileParty);
                }
            }
        }

        private void TransferBanditsToPlayerParty(MobileParty banditParty)
        {
            if (banditParty.MemberRoster.TotalMancount > 0)
            {
                var troopsToTransfer = (int)(banditParty.MemberRoster.TotalMancount * 0.3f);
                troopsToTransfer = MBMath.ClampInt(troopsToTransfer, 1, banditParty.MemberRoster.TotalMancount - 1);

                for (int i = 0; i < troopsToTransfer; i++)
                {
                    var character = banditParty.MemberRoster.GetCharacterAtIndex(0);
                    if (character != null)
                    {
                        MobileParty.MainParty.MemberRoster.AddToCounts(character, 1);
                        banditParty.MemberRoster.AddToCounts(character, -1);
                    }
                }

                InformationManager.DisplayMessage(new InformationMessage(
                    $"Reclutaste {troopsToTransfer} bandidos",
                    Colors.Green));
            }
        }

        private void ApplyBanditEffects()
        {
            if (MobileParty.MainParty != null)
            {
                MobileParty.MainParty.RecentlyBattled = false;
            }
        }

        private void EnforceBanditLife()
        {
            var playerHero = Hero.MainHero;
            if (playerHero.Occupation != Occupation.Bandit && _isBandit)
            {
                playerHero.SetNewOccupation(Occupation.Bandit);
            }
        }

        public bool IsBandit => _isBandit;
        public BanditPlayerData BanditData => _banditData;
    }
}