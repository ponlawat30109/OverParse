using HotKeyFrame;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;  //ummm....
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace OverParse
{
    public partial class MainWindow : Window
    {
        public static Dictionary<string, string> skillDict = new Dictionary<string, string>();
        public static string[] jaignoreskill, critignoreskill;
        public DispatcherTimer damageTimer = new DispatcherTimer();
        private Log encounterlog;
        private List<Combatant> lastCombatants = new List<Combatant>();
        private List<Combatant> workingList = new List<Combatant>();
        private List<string> sessionLogFilenames = new List<string>();
        private string lastStatus = "";
        private HotKey hotkey1, hotkey2, hotkey3, hotkey4;
        private IntPtr hwndcontainer;
        private string updatemsg = " - Update check Error.";
        private Process thisProcess = Process.GetCurrentProcess();

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            IntPtr hwnd = new WindowInteropHelper(this).Handle;  // Get this window's handle
            hwndcontainer = hwnd;
        }

        public MainWindow()
        {
            Properties.Resources.Culture = CultureInfo.GetCultureInfo(Properties.Settings.Default.Language);
            InitializeComponent();
            Dispatcher.UnhandledException += Panic;
            LowResources.IsChecked = Properties.Settings.Default.LowResources;
            CPUdraw.IsChecked = Properties.Settings.Default.CPUdraw;
            if (Properties.Settings.Default.LowResources) { thisProcess.PriorityClass = ProcessPriorityClass.Idle; }
            if (Properties.Settings.Default.CPUdraw) { RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly; }

            try { Directory.CreateDirectory("Logs"); }
            catch
            {
                MessageBox.Show("OverParseにアクセス権が無く、ログの保存が出来ません！\n管理者としてOverParseを実行してみるか、システムのアクセス権を確認して下さい！\nOverParseを別のフォルダーに移動してみるのも良いかも知れません。", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            if (Properties.Settings.Default.UpgradeRequired && !Properties.Settings.Default.ResetInvoked)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
            }

            Properties.Settings.Default.ResetInvoked = false;

            Top = Properties.Settings.Default.Top;
            Left = Properties.Settings.Default.Left;
            Height = Properties.Settings.Default.Height;
            Width = Properties.Settings.Default.Width;

            bool outOfBounds = (Left <= SystemParameters.VirtualScreenLeft - Width) ||
                (Top <= SystemParameters.VirtualScreenTop - Height) ||
                (SystemParameters.VirtualScreenLeft + SystemParameters.VirtualScreenWidth <= Left) ||
                (SystemParameters.VirtualScreenTop + SystemParameters.VirtualScreenHeight <= Top);

            if (outOfBounds)
            {
                Top = 50; Left = 50;
            }

            AutoEndEncounters.IsChecked = Properties.Settings.Default.AutoEndEncounters;
            SetEncounterTimeout.IsEnabled = AutoEndEncounters.IsChecked;
            SeparateZanverse.IsChecked = Properties.Settings.Default.SeparateZanverse;
            SeparateFinish.IsChecked = Properties.Settings.Default.SeparateFinish;
            SeparateAIS.IsChecked = Properties.Settings.Default.SeparateAIS;
            SeparateDB.IsChecked = Properties.Settings.Default.SeparateDB;
            SeparateRide.IsChecked = Properties.Settings.Default.SeparateRide;
            SeparatePwp.IsChecked = Properties.Settings.Default.SeparatePwp;
            SeparateLsw.IsChecked = Properties.Settings.Default.SeparateLsw;
            Onlyme.IsChecked = Properties.Settings.Default.Onlyme;
            DPSFormat.IsChecked = Properties.Settings.Default.DPSformat;
            Nodecimal.IsChecked = Properties.Settings.Default.Nodecimal;
            ClickthroughMode.IsChecked = Properties.Settings.Default.ClickthroughEnabled;
            LogToClipboard.IsChecked = Properties.Settings.Default.LogToClipboard;
            AlwaysOnTop.IsChecked = Properties.Settings.Default.AlwaysOnTop;
            AutoHideWindow.IsChecked = Properties.Settings.Default.AutoHideWindow;
            QuestTime.IsChecked = Properties.Settings.Default.QuestTime;

            ShowDamageGraph.IsChecked = Properties.Settings.Default.ShowDamageGraph; ShowDamageGraph_Click(null, null);
            HighlightYourDamage.IsChecked = Properties.Settings.Default.HighlightYourDamage; HighlightYourDamage_Click(null, null);
            Clock.IsChecked = Properties.Settings.Default.Clock; Clock_Click(null, null);
            HandleWindowOpacity(); HandleListOpacity();
            SeparateAIS_Click(null, null);
            SeparateDB_Click(null, null);
            SeparateRide_Click(null, null);
            SeparatePwp_Click(null, null);
            SeparateLsw_Click(null, null);
            LoadListColumn();

            if (Properties.Settings.Default.Maximized) { WindowState = WindowState.Maximized; }

            try
            {
                hotkey1 = new HotKey(this);
                hotkey2 = new HotKey(this);
                hotkey3 = new HotKey(this);
                hotkey4 = new HotKey(this);
                hotkey1.Regist(ModifierKeys.Control | ModifierKeys.Shift, Key.E, new EventHandler(EndEncounter_Key), 0x0071);
                hotkey2.Regist(ModifierKeys.Control | ModifierKeys.Shift, Key.R, new EventHandler(EndEncounterNoLog_Key), 0x0072);
                hotkey3.Regist(ModifierKeys.Control | ModifierKeys.Shift, Key.D, new EventHandler(DefaultWindowSize_Key), 0x0073);
                hotkey4.Regist(ModifierKeys.Control | ModifierKeys.Shift, Key.A, new EventHandler(AlwaysOnTop_Key), 0x0074);
            } catch {
                MessageBox.Show("OverParseはホットキーを初期化出来ませんでした。　多重起動していないか確認して下さい！\nプログラムは引き続き使用できますが、ホットキーは反応しません。", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            //new_version_check
            try
            {
                const string url = "https://api.github.com/repos/remon-7l/overparse/releases/latest";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.KeepAlive = false;
                request.UserAgent = "Mozilla / 5.0 OverParse / 3.1.5";
                request.GetResponseAsync().ContinueWith(task => {
                    var response = task.Result;
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        var m = Regex.Match(content, @"tag_name.........");
                        var v = Regex.Match(m.Value, @"\d.\d.\d");
                        var newVersion = Version.Parse(v.ToString());
                        var nowVersion = Version.Parse("3.1.5");
                        if (newVersion <= nowVersion) { updatemsg = ""; }
                        if (nowVersion < newVersion) { updatemsg = " - New version available(" + v.ToString() + ")"; }
                    }
                });
            } catch { }

        //skills.csv
        string[] skills = new string[0];
            try
            {
                if (Properties.Settings.Default.Language == "ja-JP") { skills = File.ReadAllLines("skills_ja.csv"); }
                if (Properties.Settings.Default.Language == "en-US") { skills = File.ReadAllLines("skills_en.csv"); }
            } catch {
                    MessageBox.Show("skills.csvが存在しません。\n全ての最大ダメージはUnknownとなります。", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            try {
                jaignoreskill = File.ReadAllLines("jaignoreskills.csv");
            } catch (Exception e) {
                MessageBox.Show(e.ToString());
                jaignoreskill = new string[] { "12345678900" }; //nullだとエラーが出るので適当な値
            }

            try {
                critignoreskill = File.ReadAllLines("critignoreskills.csv");
            } catch (Exception e) {
                MessageBox.Show(e.ToString());
                critignoreskill = new string[] { "12345678900" }; //nullだとエラーが出るので適当な値
            }

            foreach (string s in skills)
            {
                string[] split = s.Split(',');
                if (split.Length > 1)
                {
                    skillDict.Add(split[1], split[0]);
                }
            }

            //Initializing default log
            //and installing...
            encounterlog = new Log(Properties.Settings.Default.Path);
            UpdateForm(null, null);

            //Initializing damageTimer
            damageTimer.Tick += new EventHandler(UpdateForm);
            damageTimer.Interval = new TimeSpan(0, 0, 0, 0, Properties.Settings.Default.Updateinv);
            damageTimer.Start();

            //Initializing inactiveTimer
            DispatcherTimer inactiveTimer = new DispatcherTimer();
            inactiveTimer.Tick += new EventHandler(HideIfInactive);
            inactiveTimer.Interval = new TimeSpan(0, 0, 1);
            inactiveTimer.Start();

            //Initializing logCheckTimer
            DispatcherTimer logCheckTimer = new DispatcherTimer();
            logCheckTimer.Tick += new EventHandler(CheckForNewLog);
            logCheckTimer.Interval = new TimeSpan(0, 0, 1);
            logCheckTimer.Start();
        }

        private void HideIfInactive(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.AutoHideWindow) { return; }
            string title = WindowsServices.GetActiveWindowTitle();
            string[] relevant = { "OverParse", "OverParse Setup", "OverParse Error", "Encounter Timeout", "Phantasy Star Online 2" };
            if (!relevant.Contains(title))
            {
                Opacity = 0;
            } else {
                HandleWindowOpacity();
            }
        }

        private void CheckForNewLog(object sender, EventArgs e)
        {
            DirectoryInfo directory = encounterlog.logDirectory;
            if (!directory.Exists) { return; }
            if (directory.GetFiles().Count() == 0) { return; }
            FileInfo log = directory.GetFiles().Where(f => Regex.IsMatch(f.Name, @"\d+\.csv")).OrderByDescending(f => f.Name).First();
            if (log.Name != encounterlog.filename) { encounterlog = new Log(Properties.Settings.Default.Path); }
        }

        private void LoadListColumn()
        {
            GridLength temp = new GridLength(0);
            if (!Properties.Settings.Default.ListName) { CombatantView.Columns.Remove(NameColumn); CNameHC.Width = temp; }
            if (Properties.Settings.Default.Variable)
            {
                if (Properties.Settings.Default.ListPct) { CPercentHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CombatantView.Columns.Remove(PercentColumn); CPercentHC.Width = temp; }
                if (Properties.Settings.Default.ListDmg) { CDmgHC.Width = new GridLength(0.8, GridUnitType.Star); } else { CombatantView.Columns.Remove(DamageColumn); CDmgHC.Width = temp; }
                if (Properties.Settings.Default.ListDmgd) { CDmgDHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CombatantView.Columns.Remove(DamagedColumn); CDmgDHC.Width = temp; }
                if (Properties.Settings.Default.ListDPS) { CDPSHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CombatantView.Columns.Remove(DPSColumn); CDPSHC.Width = temp; }
                if (Properties.Settings.Default.ListJA) { CJAHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CombatantView.Columns.Remove(JAColumn); CJAHC.Width = temp; }
                if (Properties.Settings.Default.ListCri) { CCriHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CombatantView.Columns.Remove(CriColumn); CCriHC.Width = temp; }
                if (Properties.Settings.Default.ListHit) { CMdmgHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CombatantView.Columns.Remove(HColumn); CMdmgHC.Width = temp; }
            } else {
                if (Properties.Settings.Default.ListPct) { CPercentHC.Width = new GridLength(39); } else { CombatantView.Columns.Remove(PercentColumn); CPercentHC.Width = temp; }
                if (Properties.Settings.Default.ListDmg) { CDmgHC.Width = new GridLength(78); } else { CombatantView.Columns.Remove(DamageColumn); CDmgHC.Width = temp; }
                if (Properties.Settings.Default.ListDmgd) { CDmgDHC.Width = new GridLength(56); } else { CombatantView.Columns.Remove(DamagedColumn); CDmgDHC.Width = temp; }
                if (Properties.Settings.Default.ListDPS) { CDPSHC.Width = new GridLength(56); } else { CombatantView.Columns.Remove(DPSColumn); CDPSHC.Width = temp; }
                if (Properties.Settings.Default.ListJA) { CJAHC.Width = new GridLength(39); } else { CombatantView.Columns.Remove(JAColumn); CJAHC.Width = temp; }
                if (Properties.Settings.Default.ListCri) { CCriHC.Width = new GridLength(39); } else { CombatantView.Columns.Remove(CriColumn); CCriHC.Width = temp; }
                if (Properties.Settings.Default.ListHit) { CMdmgHC.Width = new GridLength(62); } else { CombatantView.Columns.Remove(HColumn); CMdmgHC.Width = temp; }
            }
            if (!Properties.Settings.Default.ListAtk) { CombatantView.Columns.Remove(MaxHitColumn); CAtkHC.Width = temp; }
            if (!Properties.Settings.Default.ListTab) { TabHC.Width = temp; CTabHC.Width = temp; }
        }

        private void Panic(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try { Directory.CreateDirectory("ErrorLogs"); }
            catch { MessageBox.Show("OverParseはDirectory<ErrorLogs>の作成に失敗しました。"); }
            string datetime = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
            string filename = $"ErrorLogs/ErrorLogs - {datetime}.txt";
            File.WriteAllText(filename, e.Exception.ToString());
        }

        public void HandleWindowOpacity()
        {
            TheWindow.Opacity = Properties.Settings.Default.WindowOpacity;
            // ACHTUNG ACHTUNG ACHTUNG ACHTUNG ACHTUNG ACHTUNG ACHTUNG ACHTUNG
            WinOpacity_0.IsChecked = false;
            WinOpacity_25.IsChecked = false;
            Winopacity_50.IsChecked = false;
            WinOpacity_75.IsChecked = false;
            WinOpacity_100.IsChecked = false;

            if (Properties.Settings.Default.WindowOpacity == 0) { WinOpacity_0.IsChecked = true; }
            else if (Properties.Settings.Default.WindowOpacity == .25) { WinOpacity_25.IsChecked = true; }
            else if (Properties.Settings.Default.WindowOpacity == .50) { Winopacity_50.IsChecked = true; }
            else if (Properties.Settings.Default.WindowOpacity == .75) { WinOpacity_75.IsChecked = true; }
            else if (Properties.Settings.Default.WindowOpacity == 1) { WinOpacity_100.IsChecked = true; }
        }

        public void HandleListOpacity()
        {
            Background.Opacity = Properties.Settings.Default.ListOpacity;
            ListOpacity_0.IsChecked = false;
            ListOpacity_25.IsChecked = false;
            Listopacity_50.IsChecked = false;
            ListOpacity_75.IsChecked = false;
            ListOpacity_100.IsChecked = false;

            if (Properties.Settings.Default.ListOpacity == 0) { ListOpacity_0.IsChecked = true; }
            else if (Properties.Settings.Default.ListOpacity == .25) { ListOpacity_25.IsChecked = true; }
            else if (Properties.Settings.Default.ListOpacity == .50) { Listopacity_50.IsChecked = true; }
            else if (Properties.Settings.Default.ListOpacity == .75) { ListOpacity_75.IsChecked = true; }
            else if (Properties.Settings.Default.ListOpacity == 1) { ListOpacity_100.IsChecked = true; }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = AlwaysOnTop.IsChecked;
            if (Properties.Settings.Default.ClickthroughEnabled)
            {
                int extendedStyle = WindowsServices.GetWindowLong(hwndcontainer, WindowsServices.GWL_EXSTYLE);
                WindowsServices.SetWindowLong(hwndcontainer, WindowsServices.GWL_EXSTYLE, extendedStyle | WindowsServices.WS_EX_TRANSPARENT);
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            HandleWindowOpacity();
            Window window = (Window)sender;
            window.Topmost = AlwaysOnTop.IsChecked;
            if (Properties.Settings.Default.ClickthroughEnabled)
            {
                int extendedStyle = WindowsServices.GetWindowLong(hwndcontainer, WindowsServices.GWL_EXSTYLE);
                WindowsServices.SetWindowLong(hwndcontainer, WindowsServices.GWL_EXSTYLE, extendedStyle & ~WindowsServices.WS_EX_TRANSPARENT);
            }
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized) { WindowState = WindowState.Normal; }
        }

        public void UpdateForm(object sender, EventArgs e)
        {
            if (encounterlog == null) { return; }
            if (Properties.Settings.Default.Clock) { Datetime.Content = DateTime.Now.ToString("HH:mm:ss.ff"); }

            encounterlog.UpdateLog(this, null);

            // get a copy of the right combatants
            List<Combatant> targetList = (encounterlog.running ? encounterlog.combatants : lastCombatants);
            workingList.Clear();
            foreach (Combatant c in targetList)
            {
                Combatant temp = new Combatant(c.ID, c.Name, c.isTemporary);
                foreach (Attack a in c.Attacks) { temp.Attacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.AllyAttacks) { temp.AllyAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.DBAttacks) { temp.DBAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.LswAttacks) { temp.LswAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.PwpAttacks) { temp.PwpAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.AisAttacks) { temp.AisAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.RideAttacks) { temp.RideAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                temp.Damaged = c.Damaged;
                temp.AllyDamage = c.AllyDamage;
                temp.DBDamage = c.DBDamage;
                temp.LswDamage = c.LswDamage;
                temp.PwpDamage = c.PwpDamage;
                temp.AisDamage = c.AisDamage;
                temp.RideDamage = c.RideDamage;
                temp.PercentReadDPS = c.PercentReadDPS;
                workingList.Add(temp);
            }

            // clear out the list
            CombatantData.Items.Clear();
            AllyData.Items.Clear();
            DBData.Items.Clear();
            LswData.Items.Clear();
            PwpData.Items.Clear();
            AisData.Items.Clear();
            RideData.Items.Clear();

            int elapsed = Log.ActiveTime;

            //Separate Part

            if (Properties.Settings.Default.SeparateDB)
            {
                List<Combatant> pendingDBCombatants = new List<Combatant>();
                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly) { continue; }
                    if (0 < c.DBDamage)
                    {
                        Combatant DBHolder = new Combatant(c.ID, "DB|" + c.Name, "DB");
                        List<Attack> targetAttacks = c.Attacks.Where(a => Sepid.DBAtkID.Contains(a.ID)).ToList();
                        c.Attacks = c.Attacks.Except(targetAttacks).ToList();
                        DBHolder.Attacks.AddRange(targetAttacks);
                        pendingDBCombatants.Add(DBHolder);
                    }
                }
                workingList.AddRange(pendingDBCombatants);
            }

            if (Properties.Settings.Default.SeparateLsw)
            {
                List<Combatant> pendingLswCombatants = new List<Combatant>();
                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly) { continue; }
                    if (0 < c.LswDamage)
                    {
                        Combatant LswHolder = new Combatant(c.ID, "Lsw|" + c.Name, "Lsw");
                        List<Attack> targetAttacks = c.Attacks.Where(a => Sepid.LswAtkID.Contains(a.ID)).ToList();
                        c.Attacks = c.Attacks.Except(targetAttacks).ToList();
                        LswHolder.Attacks.AddRange(targetAttacks);
                        pendingLswCombatants.Add(LswHolder);
                    }
                }
                workingList.AddRange(pendingLswCombatants);
            }

            if (Properties.Settings.Default.SeparatePwp)
            {
                List<Combatant> pendingPwpCombatants = new List<Combatant>();

                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly) { continue; }
                    if (0 < c.PwpDamage)
                    {
                        Combatant PhotonHolder = new Combatant(c.ID, "Pwp|" + c.Name, "Pwp");
                        List<Attack> targetAttacks = c.Attacks.Where(a => Sepid.PwpAtkID.Contains(a.ID)).ToList();
                        c.Attacks = c.Attacks.Except(targetAttacks).ToList();
                        PhotonHolder.Attacks.AddRange(targetAttacks);
                        pendingPwpCombatants.Add(PhotonHolder);
                    }
                }
                workingList.AddRange(pendingPwpCombatants);
            }

            if (Properties.Settings.Default.SeparateAIS)
            {
                List<Combatant> pendingCombatants = new List<Combatant>();
                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly) { continue; }
                    if (0 < c.AisDamage)
                    {
                        Combatant AISHolder = new Combatant(c.ID, "AIS|" + c.Name, "AIS");
                        List<Attack> targetAttacks = c.Attacks.Where(a => Sepid.AISAtkID.Contains(a.ID)).ToList();
                        c.Attacks = c.Attacks.Except(targetAttacks).ToList();
                        AISHolder.Attacks.AddRange(targetAttacks);
                        pendingCombatants.Add(AISHolder);
                    }
                }
                workingList.AddRange(pendingCombatants);
            }

            if (Properties.Settings.Default.SeparateRide)
            {
                List<Combatant> pendingRideCombatants = new List<Combatant>();
                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly) { continue; }
                    if (0 < c.RideDamage)
                        {
                        Combatant RideHolder = new Combatant(c.ID, "Ride|" + c.Name, "Ride");
                        List<Attack> targetAttacks = c.Attacks.Where(a => Sepid.RideAtkID.Contains(a.ID)).ToList();
                        c.Attacks = c.Attacks.Except(targetAttacks).ToList();
                        RideHolder.Attacks.AddRange(targetAttacks);
                        pendingRideCombatants.Add(RideHolder);
                    }
                }
                workingList.AddRange(pendingRideCombatants);
            }


            //分けたものを含めて再ソート
            if (SeparateTab.SelectedIndex == 0) { workingList.Sort((x, y) => y.ReadDamage.CompareTo(x.ReadDamage)); }

            Int64 totalZanverse = workingList.Sum(x => x.ZvsDamage);
            Int64 totalFinish = workingList.Sum(x => x.HTFDamage);

            if (Properties.Settings.Default.SeparateFinish && 0 < totalFinish)
            {
                Combatant finishHolder = new Combatant("99999998", "HTF Attacks", "HTF Attacks");
                foreach (Combatant c in workingList)
                {
                    if (c.IsAlly)
                    {
                        List<Attack> targetAttacks = c.Attacks.Where(a => Sepid.HTFAtkID.Contains(a.ID)).ToList();
                        finishHolder.Attacks.AddRange(targetAttacks);
                        c.Attacks = c.Attacks.Except(targetAttacks).ToList();
                    }
                }
                workingList.Add(finishHolder);
            }
            
            if (Properties.Settings.Default.SeparateZanverse && 0 < totalZanverse)
            {
                Combatant zanverseHolder = new Combatant("99999999", "Zanverse", "Zanverse");
                foreach (Combatant c in workingList)
                {
                    if (c.IsAlly)
                    {
                        List<Attack> targetAttacks = c.Attacks.Where(a => a.ID == "2106601422").ToList();
                        zanverseHolder.Attacks.AddRange(targetAttacks);
                        c.Attacks = c.Attacks.Except(targetAttacks).ToList();
                    }
                }
                workingList.Add(zanverseHolder);
            }

            // get group damage totals
            Int64 totalDamage = workingList.Sum(x => x.Damage);
            Int64 totalReadDamage = workingList.Sum(x => x.ReadDamage);
            Int64 totalAllyDamage = workingList.Sum(x => x.AllyDamage);
            Int64 totalDBDamage = workingList.Sum(x => x.DBDamage);
            Int64 totalLswDamage = workingList.Sum(x => x.LswDamage);
            Int64 totalPwpDamage = workingList.Sum(x => x.PwpDamage);
            Int64 totalAisDamage = workingList.Sum(x => x.AisDamage);
            Int64 totalRideDamage = workingList.Sum(x => x.RideDamage);

            // dps calcs!
            foreach (Combatant c in workingList)
            {
                c.PercentReadDPS = c.ReadDamage / (float)totalReadDamage * 100;
                c.AllyPct = c.AllyDamage / (float)totalAllyDamage * 100;
                c.DBPct = c.DBDamage / (float)totalDBDamage * 100;
                c.LswPct = c.LswDamage / (float)totalLswDamage * 100;
                c.PwpPct = c.PwpDamage / (float)totalPwpDamage * 100;
                c.AisPct = c.AisDamage / (float)totalAisDamage * 100;
                c.RidePct = c.RideDamage / (float)totalRideDamage * 100;
            }

            // status pane updates
            StatusUpdate(totalDamage,totalZanverse);

            // damage graph stuff
            Combatant.maxShare = 0;

            foreach (Combatant c in workingList)
            {
                if ((c.IsAlly) && c.ReadDamage > Combatant.maxShare) { Combatant.maxShare = c.ReadDamage; }

                bool filtered = true;
                if (Properties.Settings.Default.SeparateAIS || Properties.Settings.Default.SeparateDB || Properties.Settings.Default.SeparateRide || Properties.Settings.Default.SeparatePwp || Properties.Settings.Default.SeparateLsw)
                {
                    if (c.IsAlly && c.isTemporary == "raw" && !HidePlayers.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "AIS" && !HideAIS.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "DB" && !HideDB.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "Ride" && !HideRide.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "Pwp" && !HidePwp.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "Lsw" && !HideLsw.IsChecked) { filtered = false; }
                    if (c.IsZanverse) { filtered = false; }
                    if (c.IsFinish) { filtered = false; }
                } else {
                    if (c.IsAlly || c.IsZanverse || c.IsFinish) { filtered = false; }
                }

                if (!filtered && (0 < c.Damage) && (SeparateTab.SelectedIndex == 0)) { CombatantData.Items.Add(c); }
                if ((0 < c.AllyDamage) && (SeparateTab.SelectedIndex == 1)) { workingList.Sort((x, y) => y.AllyDamage.CompareTo(x.AllyDamage)); AllyData.Items.Add(c); }
                if ((0 < c.DBDamage) && (SeparateTab.SelectedIndex == 2) ) { workingList.Sort((x, y) => y.DBDamage.CompareTo(x.DBDamage)); DBData.Items.Add(c); }
                if ((0 < c.LswDamage) && (SeparateTab.SelectedIndex == 3)) { workingList.Sort((x, y) => y.LswDamage.CompareTo(x.LswDamage)); LswData.Items.Add(c); }
                if ((0 < c.PwpDamage) && (SeparateTab.SelectedIndex == 4)) { workingList.Sort((x, y) => y.PwpDamage.CompareTo(x.PwpDamage)); PwpData.Items.Add(c); }
                if ((0 < c.AisDamage) && (SeparateTab.SelectedIndex == 5)) { workingList.Sort((x, y) => y.AisDamage.CompareTo(x.AisDamage)); AisData.Items.Add(c); }
                if ((0 < c.RideDamage) && (SeparateTab.SelectedIndex == 6)) { workingList.Sort((x, y) => y.RideDamage.CompareTo(x.RideDamage)); RideData.Items.Add(c); }
            }

            // autoend
            if (Properties.Settings.Default.AutoEndEncounters && encounterlog.running)
            {
                int unixTimestamp = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if ((unixTimestamp - Log.newTimestamp) >= Properties.Settings.Default.EncounterTimeout) { EndEncounter_Click(null, null); }
            }
        }

        private void StatusUpdate(Int64 totalReadDamage,Int64 totalZanverse)
        {
            if (!encounterlog.running || (encounterlog.valid && encounterlog.notEmpty))
            {
                EncounterIndicator.Fill = new SolidColorBrush(Color.FromArgb(192, 255, 128, 128));
                EncounterStatus.Content = encounterlog.LogStatus();
            }
            if (encounterlog.valid && encounterlog.notEmpty)
            {
                EncounterIndicator.Fill = new SolidColorBrush(Color.FromArgb(192, 64, 192, 64));
                EncounterStatus.Content = $"Waiting - {lastStatus}";
                if (lastStatus == "") { EncounterStatus.Content = "Waiting... - " + encounterlog.filename + updatemsg; }
            }
            if (encounterlog.running)
            {
                EncounterIndicator.Fill = new SolidColorBrush(Color.FromArgb(192, 0, 192, 255));
                TimeSpan timespan = TimeSpan.FromSeconds(Log.ActiveTime);
                string timer = timespan.ToString(@"h\:mm\:ss");
                EncounterStatus.Content = $"{timer}";

                float totalDPS = totalReadDamage / (float)Log.ActiveTime;
                if (totalDPS > 0) { EncounterStatus.Content += $" - Total : {totalReadDamage.ToString("N0")}" + $" - {totalDPS.ToString("N0")} DPS"; }
                if (!Properties.Settings.Default.SeparateZanverse) { EncounterStatus.Content += $" - Zanverse : {totalZanverse.ToString("N0")}"; }
                lastStatus = EncounterStatus.Content.ToString();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //Closing...

            if (!Properties.Settings.Default.ResetInvoked)
            {
                if (WindowState == WindowState.Maximized)
                {
                    Properties.Settings.Default.Top = RestoreBounds.Top;
                    Properties.Settings.Default.Left = RestoreBounds.Left;
                    Properties.Settings.Default.Height = RestoreBounds.Height;
                    Properties.Settings.Default.Width = RestoreBounds.Width;
                    Properties.Settings.Default.Maximized = true;
                }
                else
                {
                    Properties.Settings.Default.Top = Top;
                    Properties.Settings.Default.Left = Left;
                    Properties.Settings.Default.Height = Height;
                    Properties.Settings.Default.Width = Width;
                    Properties.Settings.Default.Maximized = false;
                }
            }

            encounterlog.WriteLog();
            Properties.Settings.Default.Save();
        }


        private void OpenRecentLog_Click(object sender, RoutedEventArgs e)
        {
            string filename = sessionLogFilenames[SessionLogs.Items.IndexOf((e.OriginalSource as MenuItem))];
            //attempting to open
            Process.Start(Directory.GetCurrentDirectory() + "\\" + filename);
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        private void ListViewItem_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
            ListViewItem data = sender as ListViewItem;
            Combatant data2 = (Combatant)data.DataContext;
            Detalis f = new Detalis(data2) { Owner = this };
            f.Show();
        }

    }
}
