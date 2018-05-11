using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Ookii.Dialogs.Wpf;

namespace OverParse
{
    public class Log
    {
        public static int startTimestamp = 0;
        public static int newTimestamp = 0;
        public static int nowTimestamp = 0;
        public static int diffTime = 0;
        public static int ActiveTime = 0;
        public static int backupTime = 0;
        private string encounterData;
        public string filename;
        private List<int> instances = new List<int>();
        public List<Combatant> combatants = new List<Combatant>();
        public List<Combatant> backupCombatants = new List<Combatant>();
        private const int pluginVersion = 6;
        public bool valid, notEmpty, running;
        public DirectoryInfo logDirectory;
        private StreamReader logReader;

        public Log(string attemptDirectory)
        {
            bool nagMe = false;
            valid = false;
            notEmpty = false;
            running = false;

            if (Properties.Settings.Default.BanWarning)
            {
                MessageBoxResult panicResult = MessageBox.Show(Properties.Resources.BanWarning, "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (panicResult == MessageBoxResult.No) { Environment.Exit(-1); }
                Properties.Settings.Default.BanWarning = false;
            }

            while (!File.Exists($"{attemptDirectory}\\pso2.exe"))
            {
                if (nagMe)
                {
                    MessageBox.Show(Properties.Resources.Noexe, "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.Selectbin, "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    nagMe = true;
                }

                VistaFolderBrowserDialog dialog = new VistaFolderBrowserDialog();
                if ((bool)dialog.ShowDialog())
                {
                    attemptDirectory = dialog.SelectedPath;
                    Properties.Settings.Default.Path = attemptDirectory;
                }
                else
                {
                    MessageBox.Show(Properties.Resources.Noinstall, "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    Environment.Exit(-1);
                    break;
                }
            }

            if (!File.Exists($"{attemptDirectory}\\pso2.exe")) { return; }
            valid = true;
            logDirectory = new DirectoryInfo($"{attemptDirectory}\\damagelogs");

            //- - - - 

            bool pluginsExist = File.Exists(attemptDirectory + "\\pso2h.dll") && File.Exists(attemptDirectory + "\\plugins" + "\\PSO2DamageDump.dll");
            if (!pluginsExist) { Properties.Settings.Default.InstalledPluginVersion = -1; }

            if (Properties.Settings.Default.InstalledPluginVersion < pluginVersion)
            {
                MessageBoxResult selfdestructResult;

                if (pluginsExist)
                {
                    selfdestructResult = MessageBox.Show(Properties.Resources.Newdll, "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }
                else
                {
                    selfdestructResult = MessageBox.Show(Properties.Resources.Installdll, "OverParse Setup", MessageBoxButton.YesNo, MessageBoxImage.Question);
                }

                if (selfdestructResult == MessageBoxResult.No && !pluginsExist)
                {
                    MessageBox.Show(Properties.Resources.Needdll, "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                    Environment.Exit(-1);
                    return;
                }
                else if (selfdestructResult == MessageBoxResult.Yes)
                {
                    bool success = UpdatePlugin(attemptDirectory);
                    if (!pluginsExist && !success) { Environment.Exit(-1); }
                }
            }

            Properties.Settings.Default.FirstRun = false;

            if (!logDirectory.Exists) { return; }
            if (logDirectory.GetFiles().Count() == 0) { return; }

            notEmpty = true;

            FileInfo log = logDirectory.GetFiles().Where(f => Regex.IsMatch(f.Name, @"\d+\.")).OrderByDescending(f => f.Name).First();
            filename = log.Name;
            FileStream fileStream = File.Open(log.DirectoryName + "\\" + log.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(0, SeekOrigin.Begin);
            logReader = new StreamReader(fileStream);

            string existingLines = logReader.ReadToEnd();
            string[] result = existingLines.Split('\n');
            foreach (string s in result)
            {
                if (s == "") { continue; }
                string[] parts = s.Split(',');
                if (parts[0] == "0" && parts[3] == "YOU") { Combatant.currentPlayerID = parts[2]; }
            }
        }

        public bool UpdatePlugin(string attemptDirectory)
        {
            try
            {
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\pso2h.dll", attemptDirectory + "\\pso2h.dll", true);
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\ddraw.dll", attemptDirectory + "\\ddraw.dll", true);
                Directory.CreateDirectory(attemptDirectory + "\\plugins");
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\PSO2DamageDump.dll", attemptDirectory + "\\plugins" + "\\PSO2DamageDump.dll", true);
                File.Copy(Directory.GetCurrentDirectory() + "\\resources\\PSO2DamageDump.cfg", attemptDirectory + "\\plugins" + "\\PSO2DamageDump.cfg", true);
                Properties.Settings.Default.InstalledPluginVersion = pluginVersion;
                MessageBox.Show(Properties.Resources.Donedll, "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            } catch {
                MessageBox.Show(Properties.Resources.Errordll, "OverParse Setup", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public void WriteClipboard()
        {
            string log = "";
            foreach (Combatant c in combatants)
            {
                if (c.IsAlly)
                {
                    string shortname = c.Name;
                    if (c.Name.Length > 6) { shortname = c.Name.Substring(0, 6); }

                    log += $"{shortname} {(c.Damage).ToString("N0")} | ";
                }
            }

            if (log == "") { return; }
            log = log.Substring(0, log.Length - 2);

            try
            {
                Clipboard.SetText(log);
            }
            catch
            {
                MessageBox.Show(Properties.Resources.Cantclip);
            }

        }

        public string WriteLog()
        {
            if (combatants.Count != 0)
            {
                int elapsed = ActiveTime;
                if (ActiveTime == 0) { ActiveTime = 1; }
                TimeSpan timespan = TimeSpan.FromSeconds(elapsed);
                string timer = timespan.ToString(@"mm\:ss");
                string totalDamage = combatants.Sum(x => x.Damage).ToString("N0");
                string log = DateTime.Now.ToString("F") + " | " + timer + " | TotalDamage : " + totalDamage + Environment.NewLine + Environment.NewLine;

                foreach (Combatant c in combatants)
                {
                    try
                    {
                        if (c.IsAlly || c.IsZanverse || c.IsFinish)
                        {
                            log += $"{c.Name} | {c.RatioPercent}% | {c.ReadDamage.ToString("N0")} dmg | {c.ReadDamaged} dmgd | {c.DPS} DPS | JA : {c.WJAPercent}% | Critical : {c.WCRIPercent}% | Max:{c.MaxHitdmg} ({c.MaxHit})" + Environment.NewLine;
                        }
                    }
                    catch {/* 今の所何もしないっぽい */}
                }

                log += Environment.NewLine + Environment.NewLine;

                foreach (Combatant c in combatants)
                {
                    if (c.IsAlly || c.IsZanverse || c.IsFinish)
                    {
                        string header = $"[ {c.Name} - {c.RatioPercent}% - {c.ReadDamage.ToString("N0")} dmg ]";
                        log += header + Environment.NewLine + Environment.NewLine;

                        List<string> attackNames = new List<string>();
                        List<string> finishNames = new List<string>();
                        List<Tuple<string, List<Int64>, List<int>, List<int>>> attackData = new List<Tuple<string, List<Int64>, List<int>, List<int>>>();

                        if (c.IsZanverse && Properties.Settings.Default.SeparateZanverse)
                        {
                            foreach (Combatant c2 in backupCombatants) { attackNames.Add(c2.ID); }
                            foreach (string s in attackNames)
                            {
                                Combatant targetCombatant = backupCombatants.First(x => x.ID == s);
                                List<Int64> matchingAttacks = targetCombatant.Attacks.Where(a => a.ID == "2106601422").Select(a => a.Damage).ToList();
                                List<int> jaPercents = c.Attacks.Where(a => a.ID == "2106601422").Select(a => a.JA).ToList();
                                List<int> criPercents = c.Attacks.Where(a => a.ID == "2106601422").Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<Int64>, List<int>, List<int>>(targetCombatant.Name, matchingAttacks, jaPercents, criPercents));
                            }
                        }

                        else if (c.IsFinish && Properties.Settings.Default.SeparateFinish)
                        {
                            foreach (Combatant c3 in backupCombatants)
                            {
                                if (c3.HTFDamage > 0) { finishNames.Add(c3.ID); }
                            }

                            foreach (string htf in finishNames)
                            {
                                Combatant tCombatant = backupCombatants.First(x => x.ID == htf);
                                List<Int64> fmatchingAttacks = tCombatant.Attacks.Where(a => Sepid.HTFAtkID.Contains(a.ID)).Select(a => a.Damage).ToList();
                                List<int> jaPercents = c.Attacks.Where(a => Sepid.HTFAtkID.Contains(a.ID)).Select(a => a.JA).ToList();
                                List<int> criPercents = c.Attacks.Where(a => Sepid.HTFAtkID.Contains(a.ID)).Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<Int64>, List<int>, List<int>>(tCombatant.Name, fmatchingAttacks, jaPercents, criPercents));
                            }

                        }
                        else
                        {
                            foreach (Attack a in c.Attacks)
                            {
                                if ((a.ID == "2106601422" && Properties.Settings.Default.SeparateZanverse) || (Sepid.HTFAtkID.Contains(a.ID) && Properties.Settings.Default.SeparateFinish)) { continue; } //ザンバースの場合に何もしない
                                if (MainWindow.skillDict.ContainsKey(a.ID)) { a.ID = MainWindow.skillDict[a.ID]; } // these are getting disposed anyway, no 1 cur
                                if (!attackNames.Contains(a.ID.ToString())) { attackNames.Add(a.ID); }
                            }

                            foreach (string s in attackNames)
                            {
                                //マッチングアタックからダメージを選択するだけ
                                List<Int64> matchingAttacks = c.Attacks.Where(a => a.ID == s).Select(a => a.Damage).ToList();
                                List<int> jaPercents = c.Attacks.Where(a => a.ID == s).Select(a => a.JA).ToList();
                                List<int> criPercents = c.Attacks.Where(a => a.ID == s).Select(a => a.Cri).ToList();
                                attackData.Add(new Tuple<string, List<Int64>, List<int>, List<int>>(s, matchingAttacks, jaPercents, criPercents));
                            }
                        }

                        attackData = attackData.OrderByDescending(x => x.Item2.Sum()).ToList();

                        try
                        {
                            foreach (var i in attackData)
                            {
                                double percent = i.Item2.Sum() * 100d / c.Damage;
                                string spacer = (percent >= 9) ? "" : " ";

                                string paddedPercent = percent.ToString("00.00").Substring(0, 5);
                                string hits = i.Item2.Count().ToString("N0");
                                string sum = i.Item2.Sum().ToString("N0");
                                string min = i.Item2.Min().ToString("N0");
                                string max = i.Item2.Max().ToString("N0");
                                string avg = i.Item2.Average().ToString("N0");
                                string ja = (i.Item3.Average() * 100).ToString("N2");
                                string cri = (i.Item4.Average() * 100).ToString("N2");
                                log += $"{paddedPercent}%	| {i.Item1} - {sum} dmg";
                                log += $" - JA : {ja}% - Critical : {cri}%";
                                log += Environment.NewLine;
                                log += $"	|   {hits} hits - {min} min, {avg} avg, {max} max" + Environment.NewLine;
                            }
                        }
                        catch { }

                        log += Environment.NewLine;
                    }
                }


                log += "Instance IDs: " + String.Join(", ", instances.ToArray());

                DateTime thisDate = DateTime.Now;
                string directory = string.Format("{0:yyyy-MM-dd}", DateTime.Now);
                Directory.CreateDirectory($"Logs/{directory}");
                string datetime = string.Format("{0:yyyy-MM-dd_HH-mm-ss}", DateTime.Now);
                string filename = $"Logs/{directory}/OverParse - {datetime}.txt";
                File.WriteAllText(filename, log);

                return filename;
            }
            return null;
        }

        public string LogStatus()
        {
            if (!valid) { return "Error. OverParse Reset Please  Other -> Reset OverParse..."; }
            if (!notEmpty) { return "Directory No logs: Start PSO2 , Enable plugin and check pso2_bin!"; }
            if (!running) { return $"Waiting..."; }
            return encounterData;
        }

        public async Task UpdateLog(object sender, EventArgs e)
        {
            if (!valid || !notEmpty) { return; }

            string newLines = logReader.ReadToEnd();
            if (newLines == "") { await Task.CompletedTask; return; }

            string[] result = newLines.Split('\n');
            foreach (string str in result)
            {
                if (str == "") { continue; }
                string[] parts = str.Split(',');
                int lineTimestamp = int.Parse(parts[0]);
                int instanceID = int.Parse(parts[1]);
                string sourceID = parts[2];
                string sourceName = parts[3];
                string targetID = parts[4];
                string targetName = parts[5];
                string attackID = parts[6]; //WriteLog()にてID<->Nameの相互変換がある為int化が無理
                Int64 hitDamage = Int64.Parse(parts[7]);
                int JA = int.Parse(parts[8]);
                int Cri = int.Parse(parts[9]);
                //string isMultiHit = parts[10];
                //string isMisc = parts[11];
                //string isMisc2 = parts[12];
                int index = -1;

                if (parts[0] == "0" && parts[3] == "YOU") { Combatant.currentPlayerID = parts[2]; continue; }
                if (!instances.Contains(instanceID)) { instances.Add(instanceID); }
                if (hitDamage < 0) { continue; }
                if (sourceID == "0" || attackID == "0") { continue; }
                if (Properties.Settings.Default.Onlyme && sourceID != Combatant.currentPlayerID) { continue; }

                //処理スタート
                if (10000000 < int.Parse(sourceID)) //Player->Enemy
                {
                    newTimestamp = lineTimestamp;
                    if (startTimestamp == 0)
                    {
                        startTimestamp = newTimestamp;
                        nowTimestamp = newTimestamp;
                    }


                    if (newTimestamp - nowTimestamp >= 1)
                    {
                        diffTime = diffTime + 1;
                        nowTimestamp = newTimestamp;
                    }

                    if (Properties.Settings.Default.QuestTime) { ActiveTime = diffTime; }
                    else { ActiveTime = newTimestamp - startTimestamp; }

                    foreach (Combatant x in combatants) { if (x.ID == sourceID && x.isTemporary == "raw") { index = combatants.IndexOf(x); } }
                    if (index == -1)
                    {
                        combatants.Add(new Combatant(sourceID, sourceName, "raw"));
                        index = combatants.Count - 1;
                    }

                    Combatant source = combatants[index];
                    if (Sepid.DBAtkID.Contains(attackID)) { source.DBDamage += hitDamage; source.DBAttacks.Add(new Attack(attackID, hitDamage, JA, Cri)); }
                    else if (Sepid.LswAtkID.Contains(attackID)) { source.LswDamage += hitDamage; source.LswAttacks.Add(new Attack(attackID, hitDamage, JA, Cri)); }
                    else if (Sepid.PwpAtkID.Contains(attackID)) { source.PwpDamage += hitDamage; source.PwpAttacks.Add(new Attack(attackID, hitDamage, JA, Cri)); }
                    else if (Sepid.AISAtkID.Contains(attackID)) { source.AisDamage += hitDamage; source.AisAttacks.Add(new Attack(attackID, hitDamage, JA, Cri)); }
                    else if (Sepid.RideAtkID.Contains(attackID)) { source.RideDamage += hitDamage; source.RideAttacks.Add(new Attack(attackID, hitDamage, JA, Cri)); }
                    else { source.AllyDamage += hitDamage; source.AllyAttacks.Add(new Attack(attackID, hitDamage, JA, Cri)); }
                    source.Attacks.Add(new Attack(attackID, hitDamage, JA, Cri));
                    running = true;
                } else {
                    if (10000000 < int.Parse(targetID)) //Enemy->Player
                    {
                        foreach (Combatant x in combatants) { if (x.ID == targetID && x.isTemporary == "raw") { index = combatants.IndexOf(x); } }
                        if (index == -1)
                        {
                            combatants.Add(new Combatant(targetID, targetName, "raw"));
                            index = combatants.Count - 1;
                        }
                        Combatant source = combatants[index];
                        source.Damaged += hitDamage;
                        running = true;
                    }
                }
            }

            combatants.Sort((x, y) => y.ReadDamage.CompareTo(x.ReadDamage));
            if (startTimestamp != 0) { encounterData = "0:00:00 - ∞ DPS"; }

            await Task.CompletedTask;
        }
    }
}
