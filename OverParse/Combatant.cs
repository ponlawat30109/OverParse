using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace OverParse
{
    public class Combatant
    {
        public static string currentPlayerID , currentPlayerName;
        private const float maxBGopacity = 0.6f;
        public string ID, isTemporary;
        public string Name { get; set; }
        public float PercentDPS, PercentReadDPS, AllyPct, DBPct, LswPct, PwpPct, AisPct, RidePct;
        //public int ActiveTime;
        public static string Log;
        public List<Attack> Attacks, ZvsAttacks, HTFAttacks, DBAttacks, LswAttacks, PwpAttacks, AisAttacks, RideAttacks;
        public Int64 CDamage, Damaged, ZvsDamage, HTFDamage, DBDamage, LswDamage, PwpDamage, AisDamage, RideDamage;
        public static string[] FinishAttackIDs = new string[] { "2268332858", "170999070", "2268332813", "1266101764", "11556353", "1233721870", "1233722348", "3480338695" };
        public static string[] PhotonAttackIDs = new string[] { "2414748436", "1954812953", "2822784832", "3339644659", "2676260123", "224805109" , "1913897098" };
        public static string[] LaconiumAttackIDs = new string[] { "2235773608", "2235773610", "2235773611", "2235773818", "2235773926", "2235773927", "2235773944", "2618804663", "2619614461", "3607718359" };
        public static string[] AISAttackIDs = new string[] { "119505187", "79965782", "79965783", "79965784", "80047171", "434705298", "79964675", "1460054769", "4081218683", "3298256598", "2826401717" };
        public static string[] DBAttackIDs = new string[] { "267911699", "262346668", "265285249", "264996390", "311089933", "3988916155", "265781051", "3141577094", "2289473436", "517914866", "517914869", "1117313539", "1611279117", "3283361988", "1117313602", "395090797", "2429416220", "1697271546", "1117313924" };
        public static string[] RideAttackIDs = new string[] { "3491866260", "2056025809", "2534881408", "2600476838", "1247666429", "3750571080", "3642240295", "651750924", "2452463220", "1732461796", "3809261131", "1876785244", "3765765641", "3642969286", "1258041436" };


        public bool IsYou => (ID == currentPlayerID);
        public string DisplayName
        {
            get
            {
                return Name;
            }
        }

        public string RatioPercent => $"{PercentReadDPS:00.00}";

        public Int64 Damage => Attacks.Sum(x => x.Damage);

        public string DamageReadout => ReadDamage.ToString("N0");
        public string ReadDamaged => Damaged.ToString("N0");

        public double DPS => Damage / OverParse.Log.ActiveTime;
        public double ReadDPS => Math.Round(ReadDamage / (double)OverParse.Log.ActiveTime);
        public string StringDPS => ReadDPS.ToString("N0");
        public string FDPSReadout
        {
            get
            {
                if (Properties.Settings.Default.DPSformat)
                {
                    return FormatNumber(ReadDPS);
                } else {
                    return StringDPS;
                }
            }
        }

        public static float maxShare = 0;

        public string JAPercent
        {
            get
            {
                try {
                    if (Properties.Settings.Default.Nodecimal) { return ((Attacks.Where(a => !MainWindow.jaignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N0"); }
                    else { return ((Attacks.Where(a => !MainWindow.jaignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N2"); }
                }
                catch { return "Error"; }
            }
        }

        public string CRIPercent
        {
            get
            {
                try {
                    if (Properties.Settings.Default.Nodecimal) { return ((Attacks.Where(a => !MainWindow.critignoreskill.Contains(a.ID)).Average(x => x.Cri)) * 100).ToString("N0"); }
                    else { return ((Attacks.Where(a => !MainWindow.critignoreskill.Contains(a.ID)).Average(x => x.Cri)) * 100).ToString("N2"); }
                }
                catch { return "Error"; }
            }
        }

        public string WJAPercent
        {
            get
            {
                try
                {
                    return ((Attacks.Where(a => !MainWindow.jaignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N2");
                }
                catch
                {
                    return "Error";
                }
            }
        }

        public string WCRIPercent
        {
            get
            {
                try
                {
                    return ((Attacks.Average(a => a.Cri)) * 100).ToString("00.00");
                }
                catch
                { return "Error"; }
            }
        }

        public bool IsAlly => (int.Parse(ID) >= 10000000) && !IsZanverse && !IsFinish;
        public bool IsZanverse => (isTemporary == "Zanverse");
        public bool IsFinish => (isTemporary == "HTF Attacks");
        public bool IsDB => (isTemporary == "DB");
        public bool IsLsw => (isTemporary == "Lsw");
        public bool IsPwp => (isTemporary == "Pwp");
        public bool IsAIS => (isTemporary == "AIS");
        public bool IsRide => (isTemporary == "Ride");

        public string MaxHitdmg => MaxHitAttack.Damage.ToString("N0");

        public Int64 MaxHitNum => MaxHitAttack.Damage;

        public string MaxHitID => MaxHitAttack.ID;

        //Ally Data
        public string AllyReadPct => AllyPct.ToString("N2");
        public Int64 AllyDamage
        {
            get
            {
                Int64 temp = Damage - DBDamage - LswDamage - PwpDamage - AisDamage - RideDamage; 
                if (Properties.Settings.Default.SeparateZanverse) { temp -= ZvsDamage; }
                if (Properties.Settings.Default.SeparateFinish) { temp -= HTFDamage; }
                return temp;
            }
        }
        public string AllyReadDamage => AllyDamage.ToString("N0");
        public string AllyDPS => Math.Round(AllyDamage / (double)OverParse.Log.ActiveTime).ToString("N0");
        public string AllyJAPct => (Attacks.Where(a => !DBAttackIDs.Contains(a.ID) && !PhotonAttackIDs.Contains(a.ID) && !AISAttackIDs.Contains(a.ID) && !RideAttackIDs.Contains(a.ID) && !MainWindow.jaignoreskill.Contains(a.ID)).Average(x => x.JA) * 100).ToString("N2");
        public string AllyCriPct => (Attacks.Where(a => !DBAttackIDs.Contains(a.ID) && !PhotonAttackIDs.Contains(a.ID) && !AISAttackIDs.Contains(a.ID) && !RideAttackIDs.Contains(a.ID)).Average(x => x.Cri) * 100).ToString("N2");
        public string AllyMaxHitdmg => AllyMaxHit.Damage.ToString("N0");
        public string AllyAtkName
        {
            get
            {
                if (AllyMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(AllyMaxHit.ID)) { attack = MainWindow.skillDict[AllyMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack AllyMaxHit
        {
            get
            {
                Attacks.RemoveAll(a => (DBAttackIDs.Contains(a.ID) || PhotonAttackIDs.Contains(a.ID) || AISAttackIDs.Contains(a.ID) || RideAttackIDs.Contains(a.ID)));
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return Attacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        //DarkBlast Data
        public string DBReadPct => DBPct.ToString("N2");
        public string DBReadDamage => DBDamage.ToString("N0");
        public string DBDPS => Math.Round(DBDamage / (double)OverParse.Log.ActiveTime).ToString("N0");
        public string DBJAPct => (DBAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string DBCriPct => (DBAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string DBMaxHitdmg => DBMaxHit.Damage.ToString("N0");
        public string DBAtkName
        {
            get
            {
                if (DBMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(DBMaxHit.ID)) { attack = MainWindow.skillDict[DBMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack DBMaxHit
        {
            get
            {
                DBAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return DBAttacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        //Laconium sword Data
        public string LswReadPct => LswPct.ToString("N2");
        public string LswReadDamage => LswDamage.ToString("N0");
        public string LswDPS => Math.Round(LswDamage / (double)OverParse.Log.ActiveTime).ToString("N0");
        public string LswJAPct => (LswAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string LswCriPct => (LswAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string LswMaxHitdmg => LswMaxHit.Damage.ToString("N0");
        public string LswAtkName
        {
            get
            {
                if (LswMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(LswMaxHit.ID)) { attack = MainWindow.skillDict[LswMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack LswMaxHit
        {
            get
            {
                LswAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return LswAttacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        //PhotonWeapon Data
        public string PwpReadPct => PwpPct.ToString("N2");
        public string PwpReadDamage => PwpDamage.ToString("N0");
        public string PwpDPS => Math.Round(PwpDamage / (double)OverParse.Log.ActiveTime).ToString("N0");
        public string PwpJAPct => (PwpAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string PwpCriPct => (PwpAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string PwpMaxHitdmg => PwpMaxHit.Damage.ToString("N0");
        public string PwpAtkName
        {
            get
            {
                if (PwpMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(PwpMaxHit.ID)) { attack = MainWindow.skillDict[PwpMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack PwpMaxHit
        {
            get
            {
                PwpAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return PwpAttacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        //AIS Data
        public string AisReadPct => AisPct.ToString("N2");
        public string AisReadDamage => AisDamage.ToString("N0");
        public string AisDPS => Math.Round(AisDamage / (double)OverParse.Log.ActiveTime).ToString("N0");
        public string AisJAPct => (AisAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string AisCriPct => (AisAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string AisMaxHitdmg => AisMaxHit.Damage.ToString("N0");
        public string AisAtkName
        {
            get
            {
                if (AisMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(AisMaxHit.ID)) { attack = MainWindow.skillDict[AisMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack AisMaxHit
        {
            get
            {
                AisAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return AisAttacks.FirstOrDefault();
                } else {
                    return null;
                }
            }
        }

        //Ridroid Data
        public string RideReadPct => RidePct.ToString("N2");
        public string RideReadDamage => RideDamage.ToString("N0");
        public string RideDPS => Math.Round(RideDamage / (double)OverParse.Log.ActiveTime).ToString("N0");
        public string RideJAPct => (RideAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string RideCriPct => (RideAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string RideMaxHitdmg => RideMaxHit.Damage.ToString("N0");
        public string RideAtkName
        {
            get
            {
                if (RideMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(RideMaxHit.ID)) { attack = MainWindow.skillDict[RideMaxHit.ID]; }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }
        public Attack RideMaxHit
        {
            get
            {
                RideAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return RideAttacks.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }


        public Int64 ReadDamage
        {
            get
            {
                if (IsZanverse || IsFinish || IsAIS || IsPwp || IsDB || IsRide) { return Damage; }

                Int64 temp = Damage;
                if (Properties.Settings.Default.SeparateZanverse) { temp -= ZvsDamage; }
                if (Properties.Settings.Default.SeparateFinish) { temp -= HTFDamage; }
                if (Properties.Settings.Default.SeparatePwp) { temp -= PwpDamage; }
                if (Properties.Settings.Default.SeparateAIS)  { temp -= AisDamage; }
                if (Properties.Settings.Default.SeparateDB) { temp -= DBDamage; }
                if (Properties.Settings.Default.SeparateRide) { temp -= RideDamage; }

                return temp;
            }
        }

        private String FormatNumber(double value)
        {
            int num = (int)Math.Round(value);
            if (value >= 100000000) { return (value / 1000000).ToString("#,0") + "M"; }
            if (value >= 1000000) { return (value / 1000000D).ToString("0.0") + "M"; }
            if (value >= 100000) { return (value / 1000).ToString("#,0") + "K"; }
            if (value >= 1000) { return (value / 1000D).ToString("0.0") + "K"; }
            return value.ToString("#,0");
        }

        public Brush Brush
        {
            get
            {
                if (Properties.Settings.Default.ShowDamageGraph && IsAlly)
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 128, 128), Color.FromArgb(128, 30, 30, 30));
                } else {
                    return new SolidColorBrush(Color.FromArgb(127, 30, 30, 30));
                }
            }
        }

        public Brush Brush2
        {
            get
            {
                if (Properties.Settings.Default.ShowDamageGraph && IsAlly)
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 64, 64), Color.FromArgb(0, 0, 0, 0));
                } else {
                    return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                }
            }
        }

        LinearGradientBrush GenerateBarBrush(Color c, Color c2)
        {
            if (!Properties.Settings.Default.ShowDamageGraph) { c = new Color(); }
            if (IsYou && Properties.Settings.Default.HighlightYourDamage) { c = Color.FromArgb(128, 0, 255, 255); }


            LinearGradientBrush lgb = new LinearGradientBrush
            {
                StartPoint = new System.Windows.Point(0, 0),
                EndPoint = new System.Windows.Point(1, 0)
            };
            lgb.GradientStops.Add(new GradientStop(c, 0));
            lgb.GradientStops.Add(new GradientStop(c, ReadDamage / maxShare));
            lgb.GradientStops.Add(new GradientStop(c2, ReadDamage / maxShare));
            lgb.GradientStops.Add(new GradientStop(c2, 1));
            lgb.SpreadMethod = GradientSpreadMethod.Repeat;
            return lgb;
        }

        public Attack MaxHitAttack
        {
            get
            {
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                return Attacks.FirstOrDefault();
            }
        }

        public string MaxHit
        {
            get
            {
                if (MaxHitAttack == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(MaxHitID))
                {
                    attack = MainWindow.skillDict[MaxHitID];
                }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }

        public Combatant(string id, string name)
        {
            ID = id;
            Name = name;
            PercentDPS = -1;
            Attacks = new List<Attack>();
            ZvsAttacks = new List<Attack>();
            HTFAttacks = new List<Attack>();
            DBAttacks = new List<Attack>();
            LswAttacks = new List<Attack>();
            PwpAttacks = new List<Attack>();
            AisAttacks = new List<Attack>();
            RideAttacks = new List<Attack>();
            isTemporary = "no";
            PercentReadDPS = 0;
            CDamage = 0;
            Damaged = 0;
            ZvsDamage = 0;
            HTFDamage = 0;
            DBDamage = 0;
            LswDamage = 0;
            PwpDamage = 0;
            AisDamage = 0;
            RideDamage = 0;
        }

        public Combatant(string id, string name, string temp)
        {
            ID = id;
            Name = name;
            PercentDPS = -1;
            Attacks = new List<Attack>();
            ZvsAttacks = new List<Attack>();
            HTFAttacks = new List<Attack>();
            DBAttacks = new List<Attack>();
            LswAttacks = new List<Attack>();
            PwpAttacks = new List<Attack>();
            AisAttacks = new List<Attack>();
            RideAttacks = new List<Attack>();
            isTemporary = temp;
            PercentReadDPS = 0;
            CDamage = 0;
            Damaged = 0;
            ZvsDamage = 0;
            HTFDamage = 0;
            DBDamage = 0;
            LswDamage = 0;
            PwpDamage = 0;
            AisDamage = 0;
            RideDamage = 0;
        }

    }

    public class Attack
    {
        public string ID, TargetID;
        public Int64 Damage;
        public int JA, Cri;

        public Attack(string initID, Int64 initDamage, int justAttack, int critical, string targetID)
        {
            ID = initID;
            Damage = initDamage;
            JA = justAttack;
            Cri = critical;
            TargetID = targetID;
        }
    }

}
