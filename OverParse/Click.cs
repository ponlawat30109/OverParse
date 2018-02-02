using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace OverParse
{
    public partial class MainWindow : Window
    {

        private void EndEncounter_Click(object sender, RoutedEventArgs e)
        {
            //Ending encounter
            bool temp = Properties.Settings.Default.AutoEndEncounters;
            Properties.Settings.Default.AutoEndEncounters = false;
            UpdateForm(null, null); // I'M FUCKING STUPID
            Properties.Settings.Default.AutoEndEncounters = temp;
            encounterlog.backupCombatants = encounterlog.combatants;
            Log.backupTime = Log.ActiveTime;

            List<Combatant> workingListCopy = new List<Combatant>();
            foreach (Combatant c in workingList)
            {
                Combatant temp2 = new Combatant(c.ID, c.Name, c.isTemporary);
                foreach (Attack a in c.Attacks) { temp2.Attacks.Add(new Attack(a.ID, a.Damage, a.JA, a.Cri, a.TargetID)); }
                temp2.Damaged = c.Damaged;
                temp2.PercentReadDPS = c.PercentReadDPS;
                workingListCopy.Add(temp2);
            }
            //Saving last combatant list"
            lastCombatants = encounterlog.combatants;
            encounterlog.combatants = workingListCopy;
            string filename = encounterlog.WriteLog();
            if (filename != null)
            {
                if ((SessionLogs.Items[0] as MenuItem).Name == "SessionLogPlaceholder") { SessionLogs.Items.Clear(); }
                int items = SessionLogs.Items.Count;
                string prettyName = filename.Split('/').LastOrDefault();
                sessionLogFilenames.Add(filename);
                var menuItem = new MenuItem() { Name = "SessionLog_" + items.ToString(), Header = prettyName };
                menuItem.Click += OpenRecentLog_Click;
                SessionLogs.Items.Add(menuItem);
            }
            if (Properties.Settings.Default.LogToClipboard) { encounterlog.WriteClipboard(); }

            encounterlog = new Log(Properties.Settings.Default.Path);
            UpdateForm(null, null);
            Log.startTimestamp = Log.nowTimestamp = Log.diffTime = 0;
        }

<<<<<<< HEAD
        public void EndEncounter_Key(object sender, EventArgs e) => EndEncounter_Click(null, null);
=======
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa

        private void EndEncounterNoLog_Click(object sender, RoutedEventArgs e)
        {
            Log.ActiveTime = Log.backupTime;
            bool temp = Properties.Settings.Default.AutoEndEncounters;
            Properties.Settings.Default.AutoEndEncounters = false;
            UpdateForm(null, null);
            Properties.Settings.Default.AutoEndEncounters = temp;
            //Reinitializing log
            encounterlog = new Log(Properties.Settings.Default.Path);
            UpdateForm(null, null);
            Log.startTimestamp = Log.nowTimestamp = Log.diffTime = 0;
        }

<<<<<<< HEAD
        private void EndEncounterNoLog_Key(object sender, EventArgs e) => EndEncounterNoLog_Click(null, null);

=======
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
        private void AutoEndEncounters_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoEndEncounters = AutoEndEncounters.IsChecked;
            SetEncounterTimeout.IsEnabled = AutoEndEncounters.IsChecked;
        }

        private void SetEncounterTimeout_Click(object sender, RoutedEventArgs e)
        {
            AlwaysOnTop.IsChecked = false;
            Inputbox input = new Inputbox("Encounter Timeout", "Please input the amount of seconds you would like for the Encounter Timeout", Properties.Settings.Default.EncounterTimeout.ToString()) { Owner = this };
            input.ShowDialog();
            if (Int32.TryParse(input.ResultText, out int x))
            {
                if (x > 0) { Properties.Settings.Default.EncounterTimeout = x; }
                else { MessageBox.Show("Error"); }
            }
            else
            {
                if (input.ResultText.Length > 0) { MessageBox.Show("Could not read your input. Please use numbers only."); }
            }

            AlwaysOnTop.IsChecked = Properties.Settings.Default.AlwaysOnTop;
        }

        private void LogToClipboard_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.LogToClipboard = LogToClipboard.IsChecked;

<<<<<<< HEAD
        private void OpenLogsFolder_Click(object sender, RoutedEventArgs e) => Process.Start(Directory.GetCurrentDirectory() + "\\Logs");
=======
        private void OpenLogsFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Directory.GetCurrentDirectory() + "\\Logs");
        }

        private void OpenRecentLog_Click(object sender, RoutedEventArgs e)
        {
            string filename = sessionLogFilenames[SessionLogs.Items.IndexOf((e.OriginalSource as MenuItem))];
            //attempting to open
            Process.Start(Directory.GetCurrentDirectory() + "\\" + filename);
        }

        private void FilterPlayers_Click(object sender, RoutedEventArgs e)
        {
            UpdateForm(null, null);
        }
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa

        private void SeparateZanverse_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateZanverse = SeparateZanverse.IsChecked;
            UpdateForm(null, null);
        }

        private void SeparateFinish_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateFinish = SeparateFinish.IsChecked;
            UpdateForm(null, null);
        }

        private void SeparateAIS_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateAIS = SeparateAIS.IsChecked;
            HideAIS.IsEnabled = SeparateAIS.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void SeparateDB_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateDB = SeparateDB.IsChecked;
            HideDB.IsEnabled = SeparateDB.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void SeparateRide_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateRide = SeparateRide.IsChecked;
            HideRide.IsEnabled = SeparateRide.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void SeparatePwp_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparatePwp = SeparatePwp.IsChecked;
            HidePwp.IsEnabled = SeparatePwp.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void SeparateLsw_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.SeparateLsw = SeparateLsw.IsChecked;
            HideLsw.IsEnabled = SeparateLsw.IsChecked;
            HidePlayers.IsEnabled = (SeparateAIS.IsChecked || SeparateDB.IsChecked || SeparateRide.IsChecked || SeparatePwp.IsChecked || SeparateLsw.IsChecked);
            UpdateForm(null, null);
        }

        private void HidePlayers_Click(object sender, RoutedEventArgs e)
        {
            if (HidePlayers.IsChecked)
            {
                HideAIS.IsChecked = false;
                HideDB.IsChecked = false;
                HideRide.IsChecked = false;
                HidePwp.IsChecked = false;
            }
            UpdateForm(null, null);
        }

        private void HideAIS_Click(object sender, RoutedEventArgs e)
        {
            if (HideAIS.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void HideDB_Click(object sender, RoutedEventArgs e)
        {
            if (HideDB.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void HideRide_Click(object sender, RoutedEventArgs e)
        {
            if (HideRide.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void HidePwp_Click(object sender, RoutedEventArgs e)
        {
            if (HidePwp.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void HideLsw_Click(object sender, RoutedEventArgs e)
        {
            if (HideLsw.IsChecked) { HidePlayers.IsChecked = false; }
            UpdateForm(null, null);
        }

        private void Onlyme_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Onlyme = Onlyme.IsChecked;
            UpdateForm(null, null);
        }

        private void DPSFormat_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DPSformat = DPSFormat.IsChecked;
            UpdateForm(null, null);
        }

        private void Nodecimal_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Nodecimal = Nodecimal.IsChecked;
            UpdateForm(null, null);
        }

        private void ChangeInterval_Click(object sender, RoutedEventArgs e)
        {
            AlwaysOnTop.IsChecked = false;
            Inputbox input = new Inputbox("OverParse", ".csvファイルの読み取り間隔を変更します... (単位:ms)", Properties.Settings.Default.Updateinv.ToString()) { Owner = this };
            input.ShowDialog();
            if (Int32.TryParse(input.ResultText, out int x))
            {
                if (x > 49)
                {
                    damageTimer.Interval = new TimeSpan(0, 0, 0, 0, x);
                    Properties.Settings.Default.Updateinv = x;
                }
                else { MessageBox.Show("Error"); }
            } else {
                if (input.ResultText.Length > 0) { MessageBox.Show("Couldn't parse your input. Enter only a number."); }
            }

<<<<<<< HEAD
            AlwaysOnTop.IsChecked = Properties.Settings.Default.AlwaysOnTop;
        }

        private void QuestTime_Click(object sender, RoutedEventArgs e)
=======
        private void ChangeFont_Click(object sender, RoutedEventArgs e)
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
        {
            Properties.Settings.Default.QuestTime = QuestTime.IsChecked;
            if(Properties.Settings.Default.QuestTime)
            {
                Log.ActiveTime = Log.diffTime;
            } else {
                Log.ActiveTime = Log.newTimestamp - Log.startTimestamp;
            }
        }

<<<<<<< HEAD
        private void DefaultWindowSize_Click(object sender, RoutedEventArgs e)
        {
            Height = 275; Width = 670;
=======
        private void DamageTaken_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.DTcfg = DTcfg.IsChecked;
            if (DTcfg.IsChecked)
            {
                DmgDHC.Width = new GridLength(0);

            }
            else
            {
                DmgDHC.Width = new GridLength(0.9, GridUnitType.Star);
            }
            UpdateForm(null, null);
        }

        private void Percent_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Pctcfg = Pctcfg.IsChecked;
            if (Pctcfg.IsChecked)
            {
                PercentHC.Width = new GridLength(0);

            }
            else
            {
                PercentHC.Width = new GridLength(52);
            }
            UpdateForm(null, null);
        }

        private void JA_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.JAcfg = JAcfg.IsChecked;
            if (JAcfg.IsChecked)
            {
                JAHC.Width = new GridLength(0);

            }
            else
            {
                JAHC.Width = new GridLength(52);
            }
            UpdateForm(null, null);
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
        }

        private void DefaultWindowSize_Key(object sender, EventArgs e)
        {
<<<<<<< HEAD
            Height = 275; Width = 670;
=======
            Properties.Settings.Default.Criticalcfg = Cricfg.IsChecked;
            if (Cricfg.IsChecked)
            {
                CriHC.Width = new GridLength(0);

            }
            else
            {
                CriHC.Width = new GridLength(52);
            }
            UpdateForm(null, null);
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
        }

        private void Japanese_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.Language = "ja-JP";

        private void English_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.Language = "en-US";

        private void SelectColumn_Click(object sender, RoutedEventArgs e)
        {
<<<<<<< HEAD
            GridLength temp = new GridLength(0);
            bool Name, Pct, Dmg, Dmgd, DPS, JA, Cri, Hit, Atk, Tab, Vrb;
            Name = Pct = Dmg = Dmgd = DPS = JA = Cri = Hit = Atk = Tab = true;
            Vrb = Properties.Settings.Default.Variable; 
            if (NameHC.Width == temp) { Name = false; }
            if (PercentHC.Width == temp) { Pct = false; }
            if (DmgHC.Width == temp) { Dmg = false; }
            if (DmgDHC.Width == temp) { Dmgd = false; }
            if (DPSHC.Width == temp) { DPS = false; }
            if (JAHC.Width == temp) { JA = false; }
            if (CriHC.Width == temp) { Cri = false; }
            if (MdmgHC.Width == temp) { Hit = false; }
            if (AtkHC.Width == temp) { Atk = false; }
            if (TabHC.Width == temp) { Tab = false; }
            SelectColumn selectColumn = new SelectColumn(Name, Pct, Dmg, Dmgd, DPS, JA, Cri, Hit, Atk, Tab, Vrb) { Owner = this };
            selectColumn.ShowDialog();
            if (!(bool)selectColumn.DialogResult) { return; }
            CombatantView.Columns.Clear();


            if (selectColumn.ResultName) { CombatantView.Columns.Add(NameColumn); NameHC.Width = new GridLength(1, GridUnitType.Star); } else { NameHC.Width = temp; }
            if (selectColumn.Vrb)
            {
                if (selectColumn.Pct) { CombatantView.Columns.Add(PercentColumn); PercentHC.Width = new GridLength(0.4, GridUnitType.Star); } else { PercentHC.Width = temp; }
                if (selectColumn.Dmg) { CombatantView.Columns.Add(DamageColumn); DmgHC.Width = new GridLength(0.8, GridUnitType.Star); } else { DmgHC.Width = temp; }
                if (selectColumn.Dmgd) { CombatantView.Columns.Add(DamagedColumn); DmgDHC.Width = new GridLength(0.6, GridUnitType.Star); } else { DmgDHC.Width = temp; }
                if (selectColumn.DPS) { CombatantView.Columns.Add(DPSColumn); DPSHC.Width = new GridLength(0.6, GridUnitType.Star); } else { DPSHC.Width = temp; }
                if (selectColumn.JA) { CombatantView.Columns.Add(JAColumn); JAHC.Width = new GridLength(0.4, GridUnitType.Star); } else { JAHC.Width = temp; }
                if (selectColumn.Cri) { CombatantView.Columns.Add(CriColumn); CriHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CriHC.Width = temp; }
                if (selectColumn.Hit) { CombatantView.Columns.Add(HColumn); MdmgHC.Width = new GridLength(0.6, GridUnitType.Star); } else { MdmgHC.Width = temp; }
            } else {
                if (selectColumn.Pct) { CombatantView.Columns.Add(PercentColumn); PercentHC.Width = new GridLength(39); } else { PercentHC.Width = temp; }
                if (selectColumn.Dmg) { CombatantView.Columns.Add(DamageColumn); DmgHC.Width = new GridLength(78); } else { DmgHC.Width = temp; }
                if (selectColumn.Dmgd) { CombatantView.Columns.Add(DamagedColumn); DmgDHC.Width = new GridLength(56); } else { DmgDHC.Width = temp; }
                if (selectColumn.DPS) { CombatantView.Columns.Add(DPSColumn); DPSHC.Width = new GridLength(56); } else { DPSHC.Width = temp; }
                if (selectColumn.JA) { CombatantView.Columns.Add(JAColumn); JAHC.Width = new GridLength(39); } else { JAHC.Width = temp; }
                if (selectColumn.Cri) { CombatantView.Columns.Add(CriColumn); CriHC.Width = new GridLength(39); } else { CriHC.Width = temp; }
                if (selectColumn.Hit) { CombatantView.Columns.Add(HColumn); MdmgHC.Width = new GridLength(62); } else { MdmgHC.Width = temp; }
            }
            if (selectColumn.Atk) { CombatantView.Columns.Add(MaxHitColumn); AtkHC.Width = new GridLength(1.7, GridUnitType.Star); } else { AtkHC.Width = temp; }
            if (selectColumn.Tab) { TabHC.Width = new GridLength(30); } else { TabHC.Width = temp; }
            Properties.Settings.Default.ListName = selectColumn.ResultName;
            Properties.Settings.Default.ListPct = selectColumn.Pct;
            Properties.Settings.Default.ListDmg = selectColumn.Dmg;
            Properties.Settings.Default.ListDmgd = selectColumn.Dmgd;
            Properties.Settings.Default.ListDPS = selectColumn.DPS;
            Properties.Settings.Default.ListJA = selectColumn.JA;
            Properties.Settings.Default.ListCri = selectColumn.Cri;
            Properties.Settings.Default.ListHit = selectColumn.Hit;
            Properties.Settings.Default.ListAtk = selectColumn.Atk;
            Properties.Settings.Default.ListHit = selectColumn.Hit;
            Properties.Settings.Default.ListTab = selectColumn.Tab;
            Properties.Settings.Default.Variable = selectColumn.Vrb;
=======
            Properties.Settings.Default.CompactMode = CompactMode.IsChecked;
            if (CompactMode.IsChecked)
            {
                AtkHC.Width = new GridLength(0);
            }
            else
            {
                AtkHC.Width = new GridLength(1.7, GridUnitType.Star);
            }
            UpdateForm(null, null);
        }

        private void VariableColumn_Click(object sender, RoutedEventArgs e)
        {
            PercentHC.Width = new GridLength(0.4, GridUnitType.Star);
            DmgHC.Width = new GridLength(0.8, GridUnitType.Star);
            DPSHC.Width = new GridLength(0.6, GridUnitType.Star);
            JAHC.Width = new GridLength(0.4, GridUnitType.Star);
            CriHC.Width = new GridLength(0.4, GridUnitType.Star);
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
        }

        private void ShowDamageGraph_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ShowDamageGraph = ShowDamageGraph.IsChecked;
            UpdateForm(null, null);
        }

        private void HighlightYourDamage_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.HighlightYourDamage = HighlightYourDamage.IsChecked;
            UpdateForm(null, null);
        }

        private void WindowOpacity_0_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = 0;
            HandleWindowOpacity();
        }

        private void WindowOpacity_25_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = .25;
            HandleWindowOpacity();
        }

        private void WindowOpacity_50_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = .50;
            HandleWindowOpacity();
        }

        private void WindowOpacity_75_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = .75;
            HandleWindowOpacity();
        }

        private void WindowOpacity_100_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.WindowOpacity = 1;
            HandleWindowOpacity();
        }

        private void ListOpacity_0_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = 0;
            HandleListOpacity();
        }

        private void ListOpacity_25_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = .25;
            HandleListOpacity();
        }

        private void ListOpacity_50_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = .50;
            HandleListOpacity();
        }

        private void ListOpacity_75_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = .75;
            HandleListOpacity();
        }

        private void ListOpacity_100_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ListOpacity = 1;
            HandleListOpacity();
        }

        private void AlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AlwaysOnTop = AlwaysOnTop.IsChecked;
            OnActivated(e);
        }

        public void AlwaysOnTop_Key(object sender, EventArgs e)
        {
            AlwaysOnTop.IsChecked = !AlwaysOnTop.IsChecked;
            IntPtr wasActive = WindowsServices.GetForegroundWindow();

            // hack for activating overparse window
            WindowState = WindowState.Minimized;
            Show();
            WindowState = WindowState.Normal;

            Topmost = AlwaysOnTop.IsChecked;
            AlwaysOnTop_Click(null, null);
            WindowsServices.SetForegroundWindow(wasActive);
        }

        private void AutoHideWindow_Click(object sender, RoutedEventArgs e)
        {
            if (AutoHideWindow.IsChecked && Properties.Settings.Default.AutoHideWindowWarning)
            {
                MessageBox.Show("When PSO2 or OverParse is not in the foreground, OverParse will be hidden. To display OverParse, use Alt + Tab to select OverParse, or click the taskbar icon.", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                Properties.Settings.Default.AutoHideWindowWarning = false;
            }
            Properties.Settings.Default.AutoHideWindow = AutoHideWindow.IsChecked;
        }

        private void ClickthroughToggle(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ClickthroughEnabled = ClickthroughMode.IsChecked;
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
<<<<<<< HEAD
            MessageBox.Show($"OverParse v{version}\n簡易的な自己監視ツール。\n\nShoutouts to WaifuDfnseForce.\nAdditional shoutouts to Variant, AIDA, and everyone else who makes the Tweaker plugin possible.\n\nPlease use damage information responsibly.", "OverParse");
=======
            MessageBox.Show($"OverParse v3\nSelf Monitoring tool. \n\nShoutouts to Variant, AIDA, and everyone else who makes the Tweaker plugin possible.\n\nPlease use damage information responsibly.", "OverParse");
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
        }

        private void LowResources_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LowResources = LowResources.IsChecked;
            if (Properties.Settings.Default.LowResources)
            {
                thisProcess.PriorityClass = ProcessPriorityClass.Idle;
                MessageBox.Show("Process priority of OverParse is now set to low. \n\nThis setting is only recommended if your CPU is a potato. Please remember that OverParse may become unresponsive due to this setting.", "OverParse");
            } else {
                thisProcess.PriorityClass = ProcessPriorityClass.Normal;
            }
        }

        private void CPUdraw_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.CPUdraw = CPUdraw.IsChecked;
            if (Properties.Settings.Default.CPUdraw)
            {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
                MessageBox.Show("OverParse is now using CPU rendering. \n\nThis setting is only recommended if you do not have a discrete GPU. \n Please remember that this setting may increase CPU load.", "OverParse");
            } else {
                RenderOptions.ProcessRenderMode = RenderMode.Default;
            }
        }

        private void Clock_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Clock = Clock.IsChecked;
            if (Properties.Settings.Default.Clock) { Datetime.Visibility = Visibility.Visible; }
            else { Datetime.Visibility = Visibility.Collapsed; }
        }

        private void Github_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/SkrubZer0/OverParse");

<<<<<<< HEAD
        private void Github_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/Remon-7L/OverParse");

=======
>>>>>>> cc0dadfeb20fcf257fc512232ff89cd46bc61faa
        private void SkipPlugin_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.InstalledPluginVersion = 5;

        private void ResetLogFolder_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Path = "A://BROKEN/FILE/PATH";
            EndEncounterNoLog_Click(this, null);
        }

        private void UpdatePlugin_Click(object sender, RoutedEventArgs e)
        {
            encounterlog.UpdatePlugin(Properties.Settings.Default.Path);
            EndEncounterNoLog_Click(this, null);
        }

        private void Updateskills_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    Stream stream = client.OpenRead("https://remon-7l.github.io/skills.csv");
                    StreamReader streamReader = new StreamReader(stream);
                    String content = streamReader.ReadToEnd();
                    File.WriteAllText("skills.csv", content);
                }
                } catch {
                MessageBox.Show("skills.csvの取得に失敗しました。");
            }
        }

        private void ResetOverParse(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to Reset OverParse? \n\nYour user settings will be deleted but your logs will remain in your log folder.", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result != MessageBoxResult.Yes)
                return;

            //Resetting
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.ResetInvoked = true;
            Properties.Settings.Default.Save();

            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void Updateskills_Click(object sender, RoutedEventArgs e)
        {
            string[] tmp;
            //skills.csv
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    Stream stream = client.OpenRead("https://raw.githubusercontent.com/VariantXYZ/PSO2ACT/master/PSO2ACT/skills.csv");
                    StreamReader webreader = new StreamReader(stream);
                    String content = webreader.ReadToEnd();

                    tmp = content.Split('\n');
                    File.WriteAllText("skills.csv", content);                    
                }

                skillDict.Clear();

                foreach (string s in tmp)
                {
                    string[] split = s.Split(',');
                    if (split.Length > 1)
                    {
                        skillDict.Add(split[1], split[0]);
                    }
                }

                MessageBox.Show("skills.csv has been successfully updated");
            }
            catch
            {
                MessageBox.Show("Failed to update skills.csv");
            }

            

            //ignoreskills.csv
            try
            {
                using (System.Net.WebClient client = new System.Net.WebClient())
                {
                    client.DownloadFile("https://raw.githubusercontent.com/SkrubZer0/OverParse/master/OverParse/Other_Files/ignoreskills.csv", "ignoreskills.csv");
                }

                ignoreskill = File.ReadAllLines("ignoreskills.csv");

                MessageBox.Show("ignoreskills.csv has been successfully updated");
            }
            catch
            {
                MessageBox.Show("Failed to update ignoreskills.csv", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
