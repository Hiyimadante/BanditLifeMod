using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace BanditLifeMod.Models
{
    public class BanditPlayerData
    {
        [SaveableProperty(1)]
        public CampaignTime BecameBanditTime { get; set; }

        [SaveableProperty(2)]
        public int BanditsRecruited { get; set; }

        [SaveableProperty(3)]
        public int VillagesRaided { get; set; }

        [SaveableProperty(4)]
        public int CaravansAttacked { get; set; }

        public BanditPlayerData()
        {
            BanditsRecruited = 0;
            VillagesRaided = 0;
            CaravansAttacked = 0;
        }
    }
}