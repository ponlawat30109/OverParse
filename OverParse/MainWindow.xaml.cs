using System;
using System.Collections.Generic;
using System.Diagnostics;
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
<<<<<<< HEAD
using HotKeyFrame;
using System.Net;
using System.Text;
using System.Globalization;  //ummm....
=======
using NHotkey;
using NHotkey.Wpf;
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa

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
<<<<<<< HEAD
        private HotKey hotkey1, hotkey2, hotkey3, hotkey4;
=======
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
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
<<<<<<< HEAD
                /* change */
                MessageBox.Show("OverParseにアクセス権が無く、ログの保存が出来ません！\n管理者としてOverParseを実行してみるか、システムのアクセス権を確認して下さい！\nOverParseを別のフォルダーに移動してみるのも良いかも知れません。", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

=======
                MessageBox.Show("Overparse cannot save logs at the moment. \n\nPlease check that you are running Overparse as an administrator or that your account has read/write access to this directory", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }


>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
            if (Properties.Settings.Default.UpgradeRequired && !Properties.Settings.Default.ResetInvoked)
            {
                //Console.WriteLine("Upgrading settings");
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
                Top = 50;
                Left = 50;
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

            ShowDamageGraph.IsChecked = Properties.Settings.Default.ShowDamageGraph; ShowDamageGraph_Click(null, null);
<<<<<<< HEAD
            HighlightYourDamage.IsChecked = Properties.Settings.Default.HighlightYourDamage; HighlightYourDamage_Click(null, null);
            Clock.IsChecked = Properties.Settings.Default.Clock; Clock_Click(null, null);
            HandleWindowOpacity(); HandleListOpacity();
            SeparateAIS_Click(null, null);
            SeparateDB_Click(null, null);
            SeparateRide_Click(null, null);
            SeparatePwp_Click(null, null);
            SeparateLsw_Click(null, null);
            LoadListColumn();
=======
            DTcfg.IsChecked = Properties.Settings.Default.DTcfg; DamageTaken_Click(null, null);
            Pctcfg.IsChecked = Properties.Settings.Default.Pctcfg; Percent_Click(null, null);
            JAcfg.IsChecked = Properties.Settings.Default.JAcfg; JA_Click(null, null);
            Cricfg.IsChecked = Properties.Settings.Default.Criticalcfg; Critical_Click(null, null);
            CompactMode.IsChecked = Properties.Settings.Default.CompactMode; CompactMode_Click(null, null);
            AnonymizeNames.IsChecked = Properties.Settings.Default.AnonymizeNames; AnonymizeNames_Click(null, null);
            HighlightYourDamage.IsChecked = Properties.Settings.Default.HighlightYourDamage; HighlightYourDamage_Click(null, null);
            Clock.IsChecked = Properties.Settings.Default.Clock; Clock_Click(null, null);
            HandleWindowOpacity(); HandleListOpacity(); SeparateAIS_Click(null, null);
            HandleWindowOpacity(); HandleListOpacity(); SeparateDB_Click(null, null);
            HandleWindowOpacity(); HandleListOpacity(); SeparateRide_Click(null, null);
            HandleWindowOpacity(); HandleListOpacity(); SeparatePwp_Click(null, null);
            HandleWindowOpacity(); HandleListOpacity(); SeparateLsw_Click(null, null);
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa

            if (Properties.Settings.Default.Maximized) { WindowState = WindowState.Maximized; }

            try
            {
<<<<<<< HEAD
                hotkey1 = new HotKey(this);
                hotkey2 = new HotKey(this);
                hotkey3 = new HotKey(this);
                hotkey4 = new HotKey(this);
                hotkey1.Regist(ModifierKeys.Control | ModifierKeys.Shift, Key.E, new EventHandler(EndEncounter_Key), 0x0071);
                hotkey2.Regist(ModifierKeys.Control | ModifierKeys.Shift, Key.R, new EventHandler(EndEncounterNoLog_Key), 0x0072);
                hotkey3.Regist(ModifierKeys.Control | ModifierKeys.Shift, Key.D, new EventHandler(DefaultWindowSize_Key), 0x0073);
                hotkey4.Regist(ModifierKeys.Control | ModifierKeys.Shift, Key.A, new EventHandler(AlwaysOnTop_Key), 0x0074);
=======
                HotkeyManager.Current.AddOrReplace("End Encounter", Key.E, ModifierKeys.Control | ModifierKeys.Shift, EndEncounter_Key);
                HotkeyManager.Current.AddOrReplace("End Encounter (No log)", Key.R, ModifierKeys.Control | ModifierKeys.Shift, EndEncounterNoLog_Key);
                HotkeyManager.Current.AddOrReplace("Default Window Size", Key.D, ModifierKeys.Control | ModifierKeys.Shift, DefaultWindowSize_Key);
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
            } catch {
                MessageBox.Show("Hot keys are currently not working for this instance of Overparse. \n\nPlease check that you are not running multiple instances of Overparse", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            }

<<<<<<< HEAD

            //new_version_check
            try
            {
                const string url = "https://api.github.com/repos/remon-7l/overparse/releases/latest";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla / 5.0 OverParse / 3.1.0";
                request.GetResponseAsync().ContinueWith(task => {
                    var response = task.Result;
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string content = reader.ReadToEnd();
                        var m = Regex.Match(content, @"tag_name.........");
                        var v = Regex.Match(m.Value, @"\d.\d.\d");
                        var newVersion = Version.Parse(v.ToString());
                        var nowVersion = Version.Parse("3.1.0");
                        if (newVersion <= nowVersion) { updatemsg = ""; }
                        if (nowVersion < newVersion) { updatemsg = " - New version available(" + v.ToString() + ")"; }
                    }
                });
            } catch {
            }

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
=======
            //skills.csv
            Console.WriteLine("Updating skills.csv");
            string[] tmp;
            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("https://raw.githubusercontent.com/VariantXYZ/PSO2ACT/master/PSO2ACT/skills.csv");
                StreamReader webreader = new StreamReader(stream);
                String content = webreader.ReadToEnd();

                tmp = content.Split('\n');
                File.WriteAllText("skills.csv", content);

                webreader.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"skills.csv update failed: {ex.ToString()}");
                if (File.Exists("skills.csv"))
                {
                    MessageBox.Show("OverParse failed to update its skill mappings. This usually means your connection hiccuped for a moment.\n\nA local copy will be used instead. If you'd like to try and update again, please use the 'Force Update Skills' option within the 'Other' menu.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    tmp = File.ReadAllLines("skills.csv");
                }
                else
                {
                    MessageBox.Show("OverParse failed to update its skill mappings. This usually means your connection hiccuped for a moment.\n\nSince you have no skill mappings downloaded, all attacks will be marked as \"Unknown\". If you'd like to try and update again, please use the 'Force update of skills.csv' option within the 'Other' menu.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    tmp = new string[0];
                }
            }
            //ignoreskills.csv
            try
            {
                WebClient client = new WebClient();
                client.DownloadFile("https://raw.githubusercontent.com/SkrubZer0/OverParse/master/OverParse/Other_Files/ignoreskills.csv", "ignoreskills.csv");
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
            }
            catch
            {
                MessageBox.Show("Cannot update your local ignoreskills.csv please be warned that JA data might be wrong.\n\nA local copy will be used instead. If you'd like to try and update again, please use the 'Force Update Skills' option within the 'Other' menu.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ignoreskill = File.ReadAllLines("ignoreskills.csv");

            Console.WriteLine("Parsing skills.csv");

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
            if (!Properties.Settings.Default.ListName) { CombatantView.Columns.Remove(NameColumn); NameHC.Width = temp; }
            if (Properties.Settings.Default.Variable)
            {
                if (Properties.Settings.Default.ListPct) { PercentHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CombatantView.Columns.Remove(PercentColumn); PercentHC.Width = temp; }
                if (Properties.Settings.Default.ListDmg) { DmgHC.Width = new GridLength(0.8, GridUnitType.Star); } else { CombatantView.Columns.Remove(DamageColumn); DmgHC.Width = temp; }
                if (Properties.Settings.Default.ListDmgd) { DmgDHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CombatantView.Columns.Remove(DamagedColumn); DmgDHC.Width = temp; }
                if (Properties.Settings.Default.ListDPS) { DPSHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CombatantView.Columns.Remove(DPSColumn); DPSHC.Width = temp; }
                if (Properties.Settings.Default.ListJA) { JAHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CombatantView.Columns.Remove(JAColumn); JAHC.Width = temp; }
                if (Properties.Settings.Default.ListCri) { CriHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CombatantView.Columns.Remove(CriColumn); CriHC.Width = temp; }
                if (Properties.Settings.Default.ListHit) { MdmgHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CombatantView.Columns.Remove(HColumn); MdmgHC.Width = temp; }
            } else {
                if (Properties.Settings.Default.ListPct) { PercentHC.Width = new GridLength(39); } else { CombatantView.Columns.Remove(PercentColumn); PercentHC.Width = temp; }
                if (Properties.Settings.Default.ListDmg) { DmgHC.Width = new GridLength(78); } else { CombatantView.Columns.Remove(DamageColumn); DmgHC.Width = temp; }
                if (Properties.Settings.Default.ListDmgd) { DmgDHC.Width = new GridLength(56); } else { CombatantView.Columns.Remove(DamagedColumn); DmgDHC.Width = temp; }
                if (Properties.Settings.Default.ListDPS) { DPSHC.Width = new GridLength(56); } else { CombatantView.Columns.Remove(DPSColumn); DPSHC.Width = temp; }
                if (Properties.Settings.Default.ListJA) { JAHC.Width = new GridLength(39); } else { CombatantView.Columns.Remove(JAColumn); JAHC.Width = temp; }
                if (Properties.Settings.Default.ListCri) { CriHC.Width = new GridLength(39); } else { CombatantView.Columns.Remove(CriColumn); CriHC.Width = temp; }
                if (Properties.Settings.Default.ListHit) { MdmgHC.Width = new GridLength(62); } else { CombatantView.Columns.Remove(HColumn); MdmgHC.Width = temp; }
            }
            if (!Properties.Settings.Default.ListAtk) { CombatantView.Columns.Remove(MaxHitColumn); AtkHC.Width = temp; }
            if (!Properties.Settings.Default.ListTab) { TabHC.Width = temp; }
        }

        private void Panic(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try { Directory.CreateDirectory("ErrorLogs"); }
            catch { MessageBox.Show("OverParse has failed to create the directory: <ErrorLogs>"); }
            string datetime = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
            string filename = $"ErrorLogs/ErrorLogs - {datetime}.txt";
            //=== UNHANDLED EXCEPTION ===
            //e.Exception.ToString()
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
            foreach (Combatant c in targetList) { workingList.Add(c); }

            // clear out the list
            CombatantData.Items.Clear();

            int elapsed = Log.ActiveTime;

<<<<<<< HEAD
            //Separate Part
=======
            // Separation...
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
            if (Properties.Settings.Default.SeparateAIS)
            {
                List<Combatant> pendingCombatants = new List<Combatant>();

                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly) { continue; }
                    if (c.AisDamage > 0)
                    {
                        Combatant AISHolder = new Combatant(c.ID, "AIS|" + c.Name, "AIS");
                        c.Attacks = c.Attacks.Except(c.AisAttacks).ToList();
                        AISHolder.Attacks.AddRange(c.AisAttacks);
                        pendingCombatants.Add(AISHolder);
                    }
                }
                workingList.AddRange(pendingCombatants);
            }

            if (Properties.Settings.Default.SeparateDB)
            {
                List<Combatant> pendingDBCombatants = new List<Combatant>();

                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly)
                        continue;
                    if (c.DBDamage > 0)
                    {
                        Combatant DBHolder = new Combatant(c.ID, "DB|" + c.Name, "DB");
                        c.Attacks = c.Attacks.Except(c.DBAttacks).ToList();
                        DBHolder.Attacks.AddRange(c.DBAttacks);
                        pendingDBCombatants.Add(DBHolder);
                    }
                }
                workingList.AddRange(pendingDBCombatants);
            }

            if (Properties.Settings.Default.SeparateRide)
            {
                List<Combatant> pendingRideCombatants = new List<Combatant>();

                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly)
                        continue;
                    if (c.RideDamage > 0)
                        {
                        Combatant RideHolder = new Combatant(c.ID, "Ride|" + c.Name, "Ride");
                        c.Attacks = c.Attacks.Except(c.RideAttacks).ToList();
                        RideHolder.Attacks.AddRange(c.RideAttacks);
                        pendingRideCombatants.Add(RideHolder);
                    }
                }
                workingList.AddRange(pendingRideCombatants);
            }

            if (Properties.Settings.Default.SeparatePwp)
            {
                List<Combatant> pendingPwpCombatants = new List<Combatant>();

                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly)
                        continue;
                    if (c.PwpDamage > 0)
                    {
                        Combatant PhotonHolder = new Combatant(c.ID, "Pwp|" + c.Name, "Pwp");
                        c.Attacks = c.Attacks.Except(c.PwpAttacks).ToList();
                        PhotonHolder.Attacks.AddRange(c.PwpAttacks);
                        pendingPwpCombatants.Add(PhotonHolder);
                    }
                }
                workingList.AddRange(pendingPwpCombatants);
            }

            if (Properties.Settings.Default.SeparateLsw)
            {
                List<Combatant> pendingLswCombatants = new List<Combatant>();

                foreach (Combatant c in workingList)
                {
                    if (!c.IsAlly)
                        continue;
                    if (c.LswDamage > 0)
                    {
                        Combatant LswHolder = new Combatant(c.ID, "Lsw|" + c.Name, "Lsw");
                        c.Attacks = c.Attacks.Except(c.LswAttacks).ToList();
                        LswHolder.Attacks.AddRange(c.LswAttacks);
                        pendingLswCombatants.Add(LswHolder);
                    }
                }
                workingList.AddRange(pendingLswCombatants);
            }

<<<<<<< HEAD
            //分けたものを含めて再ソート
            if (SeparateTab.SelectedIndex == 0) { workingList.Sort((x, y) => y.ReadDamage.CompareTo(x.ReadDamage)); }
=======
            // Re-sort everything
            workingList.Sort((x, y) => y.ReadDamage.CompareTo(x.ReadDamage));
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa

            //ザンバースをプレイヤーとして認識させ、元の計算から除外
            Int64 totalZanverse = workingList.Sum(x => x.ZvsDamage);
            Int64 totalFinish = workingList.Sum(x => x.HTFDamage);

            if (Properties.Settings.Default.SeparateFinish && 0 < totalFinish)
            {
                Combatant finishHolder = new Combatant("99999998", "HTF Attacks", "HTF Attacks");
                foreach (Combatant c in workingList)
                {
                    finishHolder.Attacks.AddRange(c.HTFAttacks);
                    c.Attacks = c.Attacks.Except(c.HTFAttacks).ToList();
                }
                workingList.Add(finishHolder);
            }

            if (Properties.Settings.Default.SeparateZanverse && 0 < totalZanverse)
            {
                Combatant zanverseHolder = new Combatant("99999999", "Zanverse", "Zanverse");
                foreach (Combatant c in workingList)
                {
                    zanverseHolder.Attacks.AddRange(c.ZvsAttacks);
                    c.Attacks = c.Attacks.Except(c.ZvsAttacks).ToList();
                    zanverseHolder.ZvsDamage = c.ZvsDamage;
                }
                workingList.Add(zanverseHolder);
            }

            // get group damage totals
<<<<<<< HEAD
            Int64 totalReadDamage = workingList.Sum(x => x.Damage);
            Int64 totalAllyDamage = workingList.Where(c => c.IsAlly).Sum(x => x.ReadDamage);
            Int64 totalDBDamage = workingList.Sum(x => x.DBDamage);
            Int64 totalLswDamage = workingList.Sum(x => x.LswDamage);
            Int64 totalPwpDamage = workingList.Sum(x => x.PwpDamage);
            Int64 totalAisDamage = workingList.Sum(x => x.AisDamage);
            Int64 totalRideDamage = workingList.Sum(x => x.RideDamage);
=======
            int totalReadDamage = workingList.Where(c => c.IsAlly).Sum(x => x.Damage);
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa

            // dps calcs!
            foreach (Combatant c in workingList)
            {
                c.PercentReadDPS = c.ReadDamage / (float)totalReadDamage * 100;
<<<<<<< HEAD
                c.AllyPct = c.AllyDamage / (float)totalAllyDamage * 100;
                c.DBPct = c.DBDamage / (float)totalDBDamage * 100;
                c.LswPct = c.LswDamage / (float)totalLswDamage * 100;
                c.PwpPct = c.PwpDamage / (float)totalPwpDamage * 100;
                c.AisPct = c.AisDamage / (float)totalAisDamage * 100;
                c.RidePct = c.RideDamage / (float)totalRideDamage * 100;
=======
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
            }

            // status pane updates
            if (!encounterlog.running || (encounterlog.valid && encounterlog.notEmpty))
            {
<<<<<<< HEAD
                EncounterIndicator.Fill = new SolidColorBrush(Color.FromArgb(192, 255, 128, 128));
                EncounterStatus.Content = encounterlog.LogStatus();
=======
                if ((c.IsAlly) && c.ReadDamage > Combatant.maxShare)
                    Combatant.maxShare = c.ReadDamage;

                bool filtered = true;
                if (Properties.Settings.Default.SeparateAIS || Properties.Settings.Default.SeparateDB || Properties.Settings.Default.SeparateRide || Properties.Settings.Default.SeparatePwp || Properties.Settings.Default.SeparateLsw)
                {
                    if (c.IsAlly && c.isTemporary == "no" && !HidePlayers.IsChecked)
                        filtered = false;
                    if (c.IsAlly && c.isTemporary == "AIS" && !HideAIS.IsChecked)
                        filtered = false;
                    if (c.IsAlly && c.isTemporary == "DB" && !HideDB.IsChecked)
                        filtered = false;
                    if (c.IsAlly && c.isTemporary == "Ride" && !HideRide.IsChecked)
                        filtered = false;
                    if (c.IsAlly && c.isTemporary == "Pwp" && !HidePwp.IsChecked)
                        filtered = false;
                    if (c.IsAlly && c.isTemporary == "Lsw" && !HideLsw.IsChecked)
                        filtered = false;
                    if (c.IsZanverse)
                        filtered = false;
                    if (c.IsFinish)
                        filtered = false;
                }
                else
                {
                    if ((c.IsAlly || c.IsZanverse || c.IsFinish || !FilterPlayers.IsChecked) && (c.Damage > 0))
                        filtered = false;
                }

                if (!filtered && c.Damage > 0) CombatantData.Items.Add(c);

>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
            }
            if (encounterlog.valid && encounterlog.notEmpty)
            {
                EncounterIndicator.Fill = new SolidColorBrush(Color.FromArgb(192, 64, 192, 64));
                EncounterStatus.Content = $"Waiting - {lastStatus}";
<<<<<<< HEAD
                if (lastStatus == "") { EncounterStatus.Content = "Waiting... - " + encounterlog.filename + updatemsg; }
=======
                if (lastStatus == "")
                    EncounterStatus.Content = "Waiting... - " + encounterlog.filename;

                CombatantData.Items.Refresh();
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
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

<<<<<<< HEAD
            // damage graph stuff
            Combatant.maxShare = 0;

            foreach (Combatant c in workingList)
            {
                if ((c.IsAlly) && c.ReadDamage > Combatant.maxShare) { Combatant.maxShare = c.ReadDamage; }

                bool filtered = true;
                if (Properties.Settings.Default.SeparateAIS || Properties.Settings.Default.SeparateDB || Properties.Settings.Default.SeparateRide || Properties.Settings.Default.SeparatePwp || Properties.Settings.Default.SeparateLsw)
                {
                    if (c.IsAlly && c.isTemporary == "no" && !HidePlayers.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "AIS" && !HideAIS.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "DB" && !HideDB.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "Ride" && !HideRide.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "Pwp" && !HidePwp.IsChecked) { filtered = false; }
                    if (c.IsAlly && c.isTemporary == "Lsw" && !HideLsw.IsChecked) { filtered = false; }
                    if (c.IsZanverse) { filtered = false; }
                    if (c.IsFinish) { filtered = false; }
                } else {
                    if ((c.IsAlly || c.IsZanverse || c.IsFinish) && (c.Damage > 0)) { filtered = false; }
                }
=======
                if (totalDPS > 0)
                    EncounterStatus.Content += $" - {totalDPS.ToString("N2")} DPS - {totalReadDamage.ToString("N0")} Total Damage";
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa

                if (!filtered && (c.Damage > 0) && (SeparateTab.SelectedIndex == 0)) { CombatantData.Items.Add(c); }
                if ((c.AllyDamage > 0) && (SeparateTab.SelectedIndex == 1)) { workingList.Sort((x, y) => y.AllyDamage.CompareTo(x.AllyDamage)); AllyData.Items.Add(c); }
                if ((c.DBDamage > 0) && (SeparateTab.SelectedIndex == 2) ) { workingList.Sort((x, y) => y.DBDamage.CompareTo(x.DBDamage)); DBData.Items.Add(c); }
                if ((c.LswDamage > 0) && (SeparateTab.SelectedIndex == 3)) { workingList.Sort((x, y) => y.LswDamage.CompareTo(x.LswDamage)); LswData.Items.Add(c); }
                if ((c.PwpDamage > 0) && (SeparateTab.SelectedIndex == 4)) { workingList.Sort((x, y) => y.PwpDamage.CompareTo(x.PwpDamage)); PwpData.Items.Add(c); }
                if ((c.AisDamage > 0) && (SeparateTab.SelectedIndex == 5)) { workingList.Sort((x, y) => y.AisDamage.CompareTo(x.AisDamage)); AisData.Items.Add(c); }
                if ((c.RideDamage > 0) && (SeparateTab.SelectedIndex == 6)) { workingList.Sort((x, y) => y.RideDamage.CompareTo(x.RideDamage)); RideData.Items.Add(c); }
            }

            // autoend
<<<<<<< HEAD
            if (Properties.Settings.Default.AutoEndEncounters && encounterlog.running)
            {
                int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                if ((unixTimestamp - Log.newTimestamp) >= Properties.Settings.Default.EncounterTimeout) { EndEncounter_Click(null, null); }
=======
            if (encounterlog.running && Properties.Settings.Default.AutoEndEncounters)
            {
                int unixTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                if ((unixTimestamp - encounterlog.newTimestamp) >= Properties.Settings.Default.EncounterTimeout)
                {
                    //Automatically ending an encounter
                    EndEncounter_Click(null, null);
                }
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
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

        private void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Application.Current.Shutdown();
        }

<<<<<<< HEAD
=======
        public void EndEncounter_Key(object sender, HotkeyEventArgs e)
        {
            //Encounter hotkey pressed
            EndEncounter_Click(null, null);
            e.Handled = true;
        }

        public void EndEncounterNoLog_Key(object sender, HotkeyEventArgs e)
        {
            //Encounter hotkey (no log) pressed
            EndEncounterNoLog_Click(null, null);
            e.Handled = true;
        }

        public void DefaultWindowSize_Key(object sender, HotkeyEventArgs e)
        {
            DefaultWindowSize_Click(null, null);
            e.Handled = true;
        }

>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) { DragMove(); }
        }

        private void ListViewItem_MouseRightClick(object sender, MouseButtonEventArgs e)
        {
<<<<<<< HEAD
            ListViewItem data = sender as ListViewItem;
            Combatant data2 = (Combatant)data.DataContext;
            Detalis f = new Detalis(data2) { Owner = this };
=======
            var data = (ListViewItem)sender;
            var item = CombatantData.ContainerFromElement((DependencyObject)e.OriginalSource) as ListViewItem;
            Details f = new Details(data.ToString(), "value") { Owner = this };
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
            f.Show();
        }
    }
}
