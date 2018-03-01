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
                foreach (Attack a in c.Attacks) { temp2.Attacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.AllyAttacks) { temp2.AllyAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.DBAttacks) { temp2.DBAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.LswAttacks) { temp2.LswAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.PwpAttacks) { temp2.PwpAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.AisAttacks) { temp2.AisAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                foreach (Attack a in c.RideAttacks) { temp2.RideAttacks.Add(new Attack(a.TimeID, a.ID, a.Damage, a.JA, a.Cri)); }
                temp2.Damaged = c.Damaged;
                temp2.AllyDamage = c.AllyDamage;
                temp2.DBDamage = c.DBDamage;
                temp2.LswDamage = c.LswDamage;
                temp2.PwpDamage = c.PwpDamage;
                temp2.AisDamage = c.AisDamage;
                temp2.RideDamage = c.RideDamage;
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

        public void EndEncounter_Key(object sender, EventArgs e) => EndEncounter_Click(null, null);

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

        private void EndEncounterNoLog_Key(object sender, EventArgs e) => EndEncounterNoLog_Click(null, null);

        private void AutoEndEncounters_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.AutoEndEncounters = AutoEndEncounters.IsChecked;
            SetEncounterTimeout.IsEnabled = AutoEndEncounters.IsChecked;
        }

        private void SetEncounterTimeout_Click(object sender, RoutedEventArgs e)
        {
            AlwaysOnTop.IsChecked = false;
            Inputbox input = new Inputbox("Encounter Timeout", "何秒経過すればエンカウントを終了させますか？", Properties.Settings.Default.EncounterTimeout.ToString()) { Owner = this };
            input.ShowDialog();
            if (Int32.TryParse(input.ResultText, out int x))
            {
                if (0 < x) { Properties.Settings.Default.EncounterTimeout = x; }
                else { MessageBox.Show("Error"); }
            }
            else
            {
                if (input.ResultText.Length > 0) { MessageBox.Show("Couldn't parse your input. Enter only a number."); }
            }

            AlwaysOnTop.IsChecked = Properties.Settings.Default.AlwaysOnTop;
        }

        private void LogToClipboard_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.LogToClipboard = LogToClipboard.IsChecked;

        private void OpenLogsFolder_Click(object sender, RoutedEventArgs e) => Process.Start(Directory.GetCurrentDirectory() + "\\Logs");

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

            AlwaysOnTop.IsChecked = Properties.Settings.Default.AlwaysOnTop;
        }

        private void QuestTime_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.QuestTime = QuestTime.IsChecked;
            if(Properties.Settings.Default.QuestTime)
            {
                Log.ActiveTime = Log.diffTime;
            } else {
                Log.ActiveTime = Log.newTimestamp - Log.startTimestamp;
            }
        }

        private void DefaultWindowSize_Click(object sender, RoutedEventArgs e)
        {
            Height = 275; Width = 670;
        }

        private void DefaultWindowSize_Key(object sender, EventArgs e)
        {
            Height = 275; Width = 670;
        }

        private void Japanese_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.Language = "ja-JP";

        private void English_Click(object sender, RoutedEventArgs e) => Properties.Settings.Default.Language = "en-US";

        private void SelectColumn_Click(object sender, RoutedEventArgs e)
        {
            GridLength temp = new GridLength(0);
            bool Name, Pct, Dmg, Dmgd, DPS, JA, Cri, Hit, Atk, Tab, Vrb;
            Name = Pct = Dmg = Dmgd = DPS = JA = Cri = Hit = Atk = Tab = true;
            Vrb = Properties.Settings.Default.Variable; 
            if (CNameHC.Width == temp) { Name = false; }
            if (CPercentHC.Width == temp) { Pct = false; }
            if (CDmgHC.Width == temp) { Dmg = false; }
            if (CDmgDHC.Width == temp) { Dmgd = false; }
            if (CDPSHC.Width == temp) { DPS = false; }
            if (CJAHC.Width == temp) { JA = false; }
            if (CCriHC.Width == temp) { Cri = false; }
            if (CMdmgHC.Width == temp) { Hit = false; }
            if (CAtkHC.Width == temp) { Atk = false; }
            if (TabHC.Width == temp) { Tab = false; }
            SelectColumn selectColumn = new SelectColumn(Name, Pct, Dmg, Dmgd, DPS, JA, Cri, Hit, Atk, Tab, Vrb) { Owner = this };
            selectColumn.ShowDialog();
            if (!(bool)selectColumn.DialogResult) { return; }
            CombatantView.Columns.Clear();

            if (selectColumn.ResultName) { CombatantView.Columns.Add(NameColumn); CNameHC.Width = new GridLength(1, GridUnitType.Star); } else { CNameHC.Width = temp; }
            if (selectColumn.Vrb)
            {
                if (selectColumn.Pct) { CombatantView.Columns.Add(PercentColumn); CPercentHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CPercentHC.Width = temp; }
                if (selectColumn.Dmg) { CombatantView.Columns.Add(DamageColumn); CDmgHC.Width = new GridLength(0.8, GridUnitType.Star); } else { CDmgHC.Width = temp; }
                if (selectColumn.Dmgd) { CombatantView.Columns.Add(DamagedColumn); CDmgDHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CDmgDHC.Width = temp; }
                if (selectColumn.DPS) { CombatantView.Columns.Add(DPSColumn); CDPSHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CDPSHC.Width = temp; }
                if (selectColumn.JA) { CombatantView.Columns.Add(JAColumn); CJAHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CJAHC.Width = temp; }
                if (selectColumn.Cri) { CombatantView.Columns.Add(CriColumn); CCriHC.Width = new GridLength(0.4, GridUnitType.Star); } else { CCriHC.Width = temp; }
                if (selectColumn.Hit) { CombatantView.Columns.Add(HColumn); CMdmgHC.Width = new GridLength(0.6, GridUnitType.Star); } else { CMdmgHC.Width = temp; }
            } else {
                if (selectColumn.Pct) { CombatantView.Columns.Add(PercentColumn); CPercentHC.Width = new GridLength(39); } else { CPercentHC.Width = temp; }
                if (selectColumn.Dmg) { CombatantView.Columns.Add(DamageColumn); CDmgHC.Width = new GridLength(78); } else { CDmgHC.Width = temp; }
                if (selectColumn.Dmgd) { CombatantView.Columns.Add(DamagedColumn); CDmgDHC.Width = new GridLength(56); } else { CDmgDHC.Width = temp; }
                if (selectColumn.DPS) { CombatantView.Columns.Add(DPSColumn); CDPSHC.Width = new GridLength(56); } else { CDPSHC.Width = temp; }
                if (selectColumn.JA) { CombatantView.Columns.Add(JAColumn); CJAHC.Width = new GridLength(39); } else { CJAHC.Width = temp; }
                if (selectColumn.Cri) { CombatantView.Columns.Add(CriColumn); CCriHC.Width = new GridLength(39); } else { CCriHC.Width = temp; }
                if (selectColumn.Hit) { CombatantView.Columns.Add(HColumn); CMdmgHC.Width = new GridLength(62); } else { CMdmgHC.Width = temp; }
            }
            if (selectColumn.Atk) { CombatantView.Columns.Add(MaxHitColumn); CAtkHC.Width = new GridLength(1.7, GridUnitType.Star); } else { CAtkHC.Width = temp; }
            if (selectColumn.Tab) { TabHC.Width = new GridLength(30); CTabHC.Width = new GridLength(30); HealTabHC.Width = new GridLength(30); } else { TabHC.Width = temp; CTabHC.Width = temp; HealTabHC.Width = temp; }
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
                MessageBox.Show("これにより、PSO2またはOverParseがフォアグラウンドにない時は、OverParseのウィンドゥが非表示になります。\nウィンドゥを表示するには、Alt+TabでOverParseにするか、タスクバーのアイコンをクリックします。", "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
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
            MessageBox.Show($"OverParse v{version}\n簡易的な自己監視ツール。\n\nShoutouts to WaifuDfnseForce.\nAdditional shoutouts to Variant, AIDA, and everyone else who makes the Tweaker plugin possible.\n\nPlease use damage information responsibly.", "OverParse");
        }

        private void LowResources_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.LowResources = LowResources.IsChecked;
            if (Properties.Settings.Default.LowResources)
            {
                thisProcess.PriorityClass = ProcessPriorityClass.Idle;
                MessageBox.Show("OverParseの基本優先度を低に設定しました。\n殆どのCPUではあまり影響ありませんが、CPU使用率が100%になるようなPCスペックの場合にOverParseの動作を止め、PSO2や画面キャプチャ等の他のプログラムを優先させます。\nOverParseが応答不能になる可能性があることを覚えておいて下さい。","OverParse");
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
                MessageBox.Show("OverParseの画面描画をCPU処理(ソフトウェアレンダリング)に変更しました。\nグラフィックボード搭載のPCでは逆効果ですが、CPUのみ高性能で内蔵GPUを使用する大部分の日本メーカー製ノートPCではある程度効果があります。\nIntel HD Graphicsを使用している場合や0.1%でもGPUの負荷を減らしたい場合これを有効にして下さい。", "OverParse");
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

        private void Discord_Click(object sender, RoutedEventArgs e) => Process.Start("https://discord.gg/pTCq443");

        private void Github_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/Remon-7L/OverParse");

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
                    Stream stream = client.OpenRead("https://remon-7l.github.io/skills_ja.csv");
                    StreamReader streamReader = new StreamReader(stream);
                    String content = streamReader.ReadToEnd();
                    File.WriteAllText("skills_ja.csv", content);
                }
                } catch {
                MessageBox.Show("skills.csvの取得に失敗しました。");
            }
        }

        private void ResetOverParse(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("OverParseをリセットしますか？\n設定は消去されますが、ログは消去されません。", "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result != MessageBoxResult.Yes)
                return;

            //Resetting
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.ResetInvoked = true;
            Properties.Settings.Default.Save();

            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

    }
}
