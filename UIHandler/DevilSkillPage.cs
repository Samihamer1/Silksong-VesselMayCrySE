using System;
using System.Collections.Generic;
using System.Text;

namespace VesselMayCrySE.UIHandler
{
    internal class DevilSkillPage
    {
        internal class SkillPageData
        {
            public string name;
            public List<SkillData> skills;

            public SkillPageData(string name, List<SkillData> keys)
            {
                this.name = name;
                skills = keys;
            }
        }

        internal enum SkillType
        {
            UpSpell,
            DownSpell,
            HorizontalSpell,
            Passive,
            Slash,
            GreatSlash,
            CycloneSlash,
            SprintSlash,
            NeutralSpell,
            DownSlash,
            DashStab
        }

        internal class SkillData
        {
            public string key;
            public SkillType type;

            public SkillData(string key, SkillType type)
            {
                this.key = key;
                this.type = type;
            }
        }

        private static SkillPageData DevilTriggerSkillPage = new SkillPageData(
            "DEVILTRIGGER",
            new List<SkillData>()
            {
                new SkillData("DEVILTRIGGER_BUILDUP", SkillType.Passive),
                new SkillData("DEVILTRIGGER_STATE", SkillType.Passive),
                new SkillData("DEVILTRIGGER_CHASERBLADES", SkillType.Passive),
            });

        private static SkillPageData StyleSkillPage = new SkillPageData(
            "STYLE",
            new List<SkillData>()
            {
                new SkillData("STYLE_BUILDUP", SkillType.Passive),
                new SkillData("STYLE_RANKS", SkillType.Passive),
                new SkillData("STYLE_LOSS", SkillType.Passive),
            });

        private static SkillPageData GeneralSkillPage = new SkillPageData(
            "GENERAL",
            new List<SkillData>()
            {
                new SkillData("GENERAL_SWAP", SkillType.NeutralSpell),
                new SkillData("GENERAL_HORIZONTALSPELL", SkillType.HorizontalSpell),
                new SkillData("GENERAL_UPSPELL", SkillType.UpSpell),
                new SkillData("GENERAL_DOWNSPELL", SkillType.DownSpell),
                new SkillData("GENERAL_GREATSLASH", SkillType.GreatSlash),
                new SkillData("GENERAL_CYCLONESLASH", SkillType.CycloneSlash),
                new SkillData("GENERAL_SPRINTSLASH", SkillType.SprintSlash),
            });

        private static SkillPageData DevilSwordSkillPage = new SkillPageData(
            "DEVILSWORD",
            new List<SkillData>()
            {
                new SkillData("DEVILSWORD_MASTERY", SkillType.Passive),
                new SkillData("DEVILSWORD_COMBO", SkillType.Slash),
                new SkillData("DEVILSWORD_DOWNSLASH", SkillType.DownSlash),
                new SkillData("DEVILSWORD_DASHSTAB", SkillType.DashStab),
                new SkillData("DEVILSWORD_DRIVE", SkillType.HorizontalSpell),
                new SkillData("DEVILSWORD_ROUNDTRIP", SkillType.UpSpell),
                new SkillData("DEVILSWORD_REACTOR", SkillType.DownSpell),
                new SkillData("DEVILSWORD_FORMATION", SkillType.GreatSlash),
                new SkillData("DEVILSWORD_HIGHTIME", SkillType.CycloneSlash),
                new SkillData("DEVILSWORD_MILLIONSTAB", SkillType.SprintSlash),
            }
        );

        private static SkillPageData CerberusSkillPage = new SkillPageData(
            "CERBERUS",
            new List<SkillData>()
            {
                new SkillData("CERBERUS_MASTERY", SkillType.Passive),
                new SkillData("CERBERUS_COMBO", SkillType.Slash),
                new SkillData("CERBERUS_DASHSTAB", SkillType.DashStab),
                new SkillData("CERBERUS_THUNDERCLAP", SkillType.HorizontalSpell),
                new SkillData("CERBERUS_ICEAGE", SkillType.UpSpell),
                new SkillData("CERBERUS_SWING", SkillType.DownSpell),
                new SkillData("CERBERUS_HOTSTUFF", SkillType.GreatSlash),
                new SkillData("CERBERUS_REVOLVER", SkillType.CycloneSlash),
                new SkillData("CERBERUS_KINGSLAYER", SkillType.SprintSlash)
            });        

        public static Dictionary<string, SkillPageData> SkillPages = new Dictionary<string, SkillPageData>()
        {
            ["GENERAL"] = GeneralSkillPage,
            ["DEVILTRIGGER"] = DevilTriggerSkillPage,
            ["STYLE"] = StyleSkillPage,
            ["DEVILSWORD"] = DevilSwordSkillPage,
            ["CERBERUS"] = CerberusSkillPage,
        };
    }
}
