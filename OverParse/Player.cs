using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace OverParse
{
    public static class Sepid
    {
        public static readonly string[] HTFAtkID = new string[] { "2268332858", "170999070", "2268332813", "1266101764", "11556353", "1233721870", "1233722348", "3480338695" };
        public static readonly string[] PwpAtkID = new string[] { "2414748436", "1954812953", "2822784832", "3339644659", "2676260123", "224805109", "1913897098" };
        public static readonly string[] LswAtkID = new string[] { "2235773608", "2235773610", "2235773611", "2235773818", "2235773926", "2235773927", "2235773944", "2618804663", "2619614461", "3607718359" };
        public static readonly string[] AISAtkID = new string[] { "119505187", "79965782", "79965783", "79965784", "80047171", "434705298", "79964675", "1460054769", "4081218683", "3298256598", "2826401717" };
        public static readonly string[] DBAtkID = new string[] { "267911699", "262346668", "265285249", "264996390", "311089933", "3988916155", "265781051", "3141577094", "2289473436", "517914866", "517914869", "1117313539", "1611279117", "3283361988", "1117313602", "395090797", "2429416220", "1697271546", "1117313924", "2743071591", "2743062721", "1783571383", "2928504078", "1783571188", "2849190450", "1223455602", "651603449", "2970658149", "2191939386", "2091027507", "4078260742" };
        public static readonly string[] RideAtkID = new string[] { "3491866260", "2056025809", "2534881408", "2600476838", "1247666429", "3750571080", "3642240295", "651750924", "2452463220", "1732461796", "3809261131", "1876785244", "3765765641", "3642969286", "1258041436" };
    }

    public struct Hit
    {
        public string ID;
        public Int64 Damage;
        public byte JA, Cri;
        public Hit(string paid, Int64 dmg, byte ja, byte cri) { ID = paid; Damage = dmg; JA = ja; Cri = cri; }

        public string ReadID => ID;
        public string IDName
        {
            get
            {
                try
                {
                    return MainWindow.skillDict[ID];
                } catch
                {
                    return ID;
                }
            }
        }

        public string ReadDamage => Damage.ToString("N0");
        public string IsJA { get { if (JA == 1) { return "True"; } else { return "False"; } } }
        public string IsCri { get { if (Cri == 1) { return "True"; } else { return "False"; } } }
    }

    public class Player //Welcome to Property Hell
    {
        private const float maxBGopacity = 0.6f;
        public string ID, Name, isTemporary;
        public float PercentDPS, PercentReadDPS, AllyPct, DBPct, LswPct, PwpPct, AisPct, RidePct;
        public double TScore;
        public List<Hit> Attacks, AllyAttacks, DBAttacks, LswAttacks, PwpAttacks, AisAttacks, RideAttacks;
        public Int64 Damage, Damaged, ZvsDamage, HTFDamage, AllyDamage, DBDamage, LswDamage, PwpDamage, AisDamage, RideDamage;

        public bool IsYou => ID == MainWindow.currentPlayerID;
        public string DisplayName => Name;

        public string RatioPercent => $"{PercentReadDPS:00.00}";
        public string ReadTScore
        {
            get
            {
                if (isTemporary == "raw") { return $"{TScore:00.00}"; } else { return "--"; }
            }
        }

        //public Int64 Damage => Attacks.Sum(x => x.Damage);
        //public Int64 ZvsDamage => Attacks.Where(a => a.ID == "2106601422").Sum(x => x.Damage);
        //public Int64 HTFDamage => Attacks.Where(a => Sepid.HTFAtkID.Contains(a.ID)).Sum(x => x.Damage);
        public Int64 ReadDamage
        {
            get
            {
                if (IsZanverse || IsFinish || IsDB || IsLsw || IsPwp || IsAIS || IsRide) { return Damage; }

                Int64 temp = Damage;
                if (Properties.Settings.Default.SeparateFinish) { temp -= HTFDamage; }
                if (Properties.Settings.Default.SeparateZanverse) { temp -= ZvsDamage; }
                if (Properties.Settings.Default.SeparateDB) { temp -= DBDamage; }
                if (Properties.Settings.Default.SeparateLsw) { temp -= LswDamage; }
                if (Properties.Settings.Default.SeparatePwp) { temp -= PwpDamage; }
                if (Properties.Settings.Default.SeparateAIS) { temp -= AisDamage; }
                if (Properties.Settings.Default.SeparateRide) { temp -= RideDamage; }
                return temp;
            }
        }

        public string BindDamage
        {
            get
            {
                if (Properties.Settings.Default.DamageSI)
                {
                    return FormatDmg(ReadDamage);
                } else { return ReadDamage.ToString("N0"); }
            }
        }

        public string BindDamaged => Damaged.ToString("N0");

        public double DPS => Damage / MainWindow.current.ActiveTime;
        public double ReadDPS => Math.Round(ReadDamage / (double)MainWindow.current.ActiveTime);
        public string StringDPS => ReadDPS.ToString("N0");
        public string FDPSReadout
        {
            get
            {
                if (Properties.Settings.Default.DPSSI)
                {
                    return FormatNumber(ReadDPS);
                } else
                {
                    return StringDPS;
                }
            }
        }

        public string JAPercent
        {
            get
            {
                try
                {
                    if (Properties.Settings.Default.Nodecimal) { return ((Attacks.Where(a => !MainWindow.jaignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N0"); } else { return ((Attacks.Where(a => !MainWindow.jaignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N2"); }
                } catch { return "Error"; }
            }
        }

        public string WJAPercent
        {
            get
            {
                try { return ((Attacks.Where(a => !MainWindow.jaignoreskill.Contains(a.ID)).Average(x => x.JA)) * 100).ToString("N2"); } catch { return "Error"; }
            }
        }

        public string CRIPercent
        {
            get
            {
                try
                {
                    if (Properties.Settings.Default.Nodecimal) { return ((Attacks.Where(a => !MainWindow.critignoreskill.Contains(a.ID)).Average(x => x.Cri)) * 100).ToString("N0"); } else { return ((Attacks.Where(a => !MainWindow.critignoreskill.Contains(a.ID)).Average(x => x.Cri)) * 100).ToString("N2"); }
                } catch { return "Error"; }
            }
        }

        public string WCRIPercent
        {
            get
            {
                try { return ((Attacks.Average(a => a.Cri)) * 100).ToString("00.00"); } catch { return "Error"; }
            }
        }

        public bool IsAlly => (int.Parse(ID) >= 10000000) && !IsZanverse && !IsFinish;
        public bool IsZanverse => (isTemporary == "Zvs");
        public bool IsFinish => (isTemporary == "HTF");
        public bool IsDB => (isTemporary == "DB");
        public bool IsLsw => (isTemporary == "Lsw");
        public bool IsPwp => (isTemporary == "Pwp");
        public bool IsAIS => (isTemporary == "AIS");
        public bool IsRide => (isTemporary == "Ride");

        public Hit MaxHitAttack
        {
            get
            {
                Attacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                return Attacks.FirstOrDefault();
            }
        }

        public string MaxHitdmg
        {
            get
            {
                try
                {
                    if (Properties.Settings.Default.MaxSI)
                    {
                        return FormatDmg(MaxHitAttack.Damage);
                    } else { return MaxHitAttack.Damage.ToString("N0"); }
                } catch { return "Error"; }
            }
        }

        public string MaxHit
        {
            get
            {
                //if (MaxHitAttack == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(MaxHitAttack.ID))
                {
                    attack = MainWindow.skillDict[MaxHitAttack.ID];
                }
                return MaxHitAttack.Damage.ToString(attack);
            }
        }


        //Separate
        #region 

        //Ally Data
        public string AllyReadPct => AllyPct.ToString("N2");
        public string AllyReadDamage => AllyDamage.ToString("N0");
        public string AllyDPS => Math.Round(AllyDamage / (double)MainWindow.current.ActiveTime).ToString("N0");
        public string AllyJAPct => (AllyAttacks.Where(a => !MainWindow.jaignoreskill.Contains(a.ID)).Average(x => x.JA) * 100).ToString("N2");
        public string AllyCriPct => (AllyAttacks.Where(a => !MainWindow.critignoreskill.Contains(a.ID)).Average(x => x.Cri) * 100).ToString("N2");
        public string AllyMaxHitdmg => AllyMaxHit.Damage.ToString("N0");
        public string AllyAtkName
        {
            get
            {
                //if (AllyMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(AllyMaxHit.ID)) { attack = MainWindow.skillDict[AllyMaxHit.ID]; }
                return AllyMaxHit.Damage.ToString(attack);
            }
        }
        public Hit AllyMaxHit
        {
            get
            {
                AllyAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (AllyAttacks != null)
                {
                    return AllyAttacks.FirstOrDefault();
                } else
                {
                    return new Hit();
                }
            }
        }

        //DarkBlast Data
        public string DBReadPct => DBPct.ToString("N2");
        public string DBReadDamage => DBDamage.ToString("N0");
        public string DBDPS => Math.Round(DBDamage / (double)MainWindow.current.ActiveTime).ToString("N0");
        public string DBJAPct => (DBAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string DBCriPct => (DBAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string DBMaxHitdmg => DBMaxHit.Damage.ToString("N0");
        public string DBAtkName
        {
            get
            {
                //if (DBMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(DBMaxHit.ID)) { attack = MainWindow.skillDict[DBMaxHit.ID]; }
                return DBMaxHit.Damage.ToString(attack);
            }
        }
        public Hit DBMaxHit
        {
            get
            {
                DBAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (DBAttacks != null)
                {
                    return DBAttacks.FirstOrDefault();
                } else
                {
                    return new Hit();
                    //return null;
                }
            }
        }

        //Laconium sword Data
        public string LswReadPct => LswPct.ToString("N2");
        public string LswReadDamage => LswDamage.ToString("N0");
        public string LswDPS => Math.Round(LswDamage / (double)MainWindow.current.ActiveTime).ToString("N0");
        public string LswJAPct => (LswAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string LswCriPct => (LswAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string LswMaxHitdmg => LswMaxHit.Damage.ToString("N0");
        public string LswAtkName
        {
            get
            {
                //if (LswMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(LswMaxHit.ID)) { attack = MainWindow.skillDict[LswMaxHit.ID]; }
                return LswMaxHit.Damage.ToString(attack);
            }
        }
        public Hit LswMaxHit
        {
            get
            {
                LswAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return LswAttacks.FirstOrDefault();
                } else
                {
                    return new Hit();
                    //return null;
                }
            }
        }

        //PhotonWeapon Data
        public string PwpReadPct => PwpPct.ToString("N2");
        public string PwpReadDamage => PwpDamage.ToString("N0");
        public string PwpDPS => Math.Round(PwpDamage / (double)MainWindow.current.ActiveTime).ToString("N0");
        public string PwpJAPct => (PwpAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string PwpCriPct => (PwpAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string PwpMaxHitdmg => PwpMaxHit.Damage.ToString("N0");
        public string PwpAtkName
        {
            get
            {
                //if (PwpMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(PwpMaxHit.ID)) { attack = MainWindow.skillDict[PwpMaxHit.ID]; }
                return PwpMaxHit.Damage.ToString(attack);
            }
        }
        public Hit PwpMaxHit
        {
            get
            {
                PwpAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return PwpAttacks.FirstOrDefault();
                } else
                {
                    return new Hit();
                    //return null;
                }
            }
        }

        //AIS Data
        public string AisReadPct => AisPct.ToString("N2");
        public string AisReadDamage => AisDamage.ToString("N0");
        public string AisDPS => Math.Round(AisDamage / (double)MainWindow.current.ActiveTime).ToString("N0");
        public string AisJAPct => (AisAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string AisCriPct => (AisAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string AisMaxHitdmg => AisMaxHit.Damage.ToString("N0");
        public string AisAtkName
        {
            get
            {
                //if (AisMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(AisMaxHit.ID)) { attack = MainWindow.skillDict[AisMaxHit.ID]; }
                return AisMaxHit.Damage.ToString(attack);
            }
        }
        public Hit AisMaxHit
        {
            get
            {
                AisAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return AisAttacks.FirstOrDefault();
                } else
                {
                    return new Hit();
                    //return null;
                }
            }
        }

        //Ridroid Data
        public string RideReadPct => RidePct.ToString("N2");
        public string RideReadDamage => RideDamage.ToString("N0");
        public string RideDPS => Math.Round(RideDamage / (double)MainWindow.current.ActiveTime).ToString("N0");
        public string RideJAPct => (RideAttacks.Average(x => x.JA) * 100).ToString("N2");
        public string RideCriPct => (RideAttacks.Average(x => x.Cri) * 100).ToString("N2");
        public string RideMaxHitdmg => RideMaxHit.Damage.ToString("N0");
        public string RideAtkName
        {
            get
            {
                //if (RideMaxHit == null) { return "--"; }
                string attack = "Unknown";
                if (MainWindow.skillDict.ContainsKey(RideMaxHit.ID)) { attack = MainWindow.skillDict[RideMaxHit.ID]; }
                return RideMaxHit.Damage.ToString(attack);
            }
        }
        public Hit RideMaxHit
        {
            get
            {
                RideAttacks.Sort((x, y) => y.Damage.CompareTo(x.Damage));
                if (Attacks != null)
                {
                    return RideAttacks.FirstOrDefault();
                } else
                {
                    return new Hit();
                    //return null;
                }
            }
        }
        #endregion

        private String FormatDmg(Int64 value)
        {
            if (value >= 1000000000) { return (value / 1000000000D).ToString("0.00") + "G"; } else if (value >= 100000000) { return (value / 1000000D).ToString("0.0") + "M"; } else if (value >= 1000000) { return (value / 1000000D).ToString("0.00") + "M"; } else if (value >= 100000) { return (value / 1000).ToString("#,0") + "K"; } else if (value >= 10000) { return (value / 1000D).ToString("0.0") + "K"; }
            return value.ToString("#,0");
        }

        private String FormatNumber(double value)
        {
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
                if (Properties.Settings.Default.ShowDamageGraph && (IsAlly))
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 128, 128), Color.FromArgb(0, 30, 30, 30));
                } else
                {
                    if (IsYou && Properties.Settings.Default.HighlightYourDamage) { return new SolidColorBrush(Color.FromArgb(128, 0, 255, 255)); }

                    return new SolidColorBrush(Color.FromArgb(0, 30, 30, 30));
                }
            }
        }

        public Brush Brush2
        {
            get
            {
                if (Properties.Settings.Default.ShowDamageGraph && (IsAlly && !IsZanverse))
                {
                    return GenerateBarBrush(Color.FromArgb(128, 0, 64, 64), Color.FromArgb(0, 0, 0, 0));
                } else
                {
                    if (IsYou && Properties.Settings.Default.HighlightYourDamage) { return new SolidColorBrush(Color.FromArgb(0, 0, 64, 64)); }
                    return new SolidColorBrush(new Color());
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
            lgb.GradientStops.Add(new GradientStop(c, ReadDamage / MainWindow.current.firstDamage));
            lgb.GradientStops.Add(new GradientStop(c2, ReadDamage / MainWindow.current.firstDamage));
            lgb.GradientStops.Add(new GradientStop(c2, 1));
            lgb.SpreadMethod = GradientSpreadMethod.Repeat;
            return lgb;
        }

        public Player(string id, string name, string temp)
        {
            ID = id;
            Name = name;
            isTemporary = temp;
            PercentDPS = -1;
            Attacks = new List<Hit>();
            AllyAttacks = new List<Hit>();
            DBAttacks = new List<Hit>();
            LswAttacks = new List<Hit>();
            PwpAttacks = new List<Hit>();
            AisAttacks = new List<Hit>();
            RideAttacks = new List<Hit>();
            PercentReadDPS = 0;
            AllyDamage = 0;
            Damaged = 0;
            DBDamage = 0;
            LswDamage = 0;
            PwpDamage = 0;
            AisDamage = 0;
            RideDamage = 0;
            ZvsDamage = 0;
        }
    }

    public class Session
    {
        public int startTimestamp, newTimestamp, nowTimestamp, diffTime, ActiveTime;
        public List<int> instances = new List<int>();
        public Int64 totalDamage, totalAllyDamage, totalDBDamage, totalLswDamage, totalPwpDamage, totalAisDamage, totalRideDamage, totalZanverse, totalFinish;
        public double totalSD, firstDamage;
        public List<Player> players = new List<Player>();

        public Session()
        {

        }
    }
}
