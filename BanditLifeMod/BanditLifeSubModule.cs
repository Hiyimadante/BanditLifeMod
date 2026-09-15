using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using BanditLifeMod.Behaviors;
using BanditLifeMod.Models;

namespace BanditLifeMod
{
    public class BanditLifeSubModule : MBSubModuleBase
    {
        private BanditLifeBehavior _banditLifeBehavior;

        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (!(gameStarterObject is CampaignGameStarter campaignStarter))
                return;

            AddGameMenus(campaignStarter);
            AddBehaviors(campaignStarter);
        }

        private void AddBehaviors(CampaignGameStarter campaignStarter)
        {
            _banditLifeBehavior = new BanditLifeBehavior();
            campaignStarter.AddBehavior(_banditLifeBehavior);
        }

        private void AddGameMenus(CampaignGameStarter campaignStarter)
        {
            campaignStarter.AddGameMenuOption(
                "hideout",
                "bandit_life_option",
                "{=bandit_option}Unirse a la vida de bandido",
                GameMenuCallbackDelegate.CreateFromFunction(
                    new GameMenuCallbackDelegate.OnConditionDelegate((args) => true),
                    new GameMenuCallbackDelegate.OnConsequenceDelegate((args) =>
                    {
                        if (_banditLifeBehavior != null)
                        {
                            _banditLifeBehavior.BecomeBandit();
                        }
                    })),
                false,
                -1,
                false);
        }
    }
}