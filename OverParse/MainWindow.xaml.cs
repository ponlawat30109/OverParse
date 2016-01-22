﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace OverParse
{
    public partial class MainWindow : Window
    {
        Log encounterlog;

        public MainWindow()
        {
            InitializeComponent();

            encounterlog = new Log(Properties.Settings.Default.Path);
            Console.WriteLine(Properties.Settings.Default.Path);

            UpdateForm(null, null);
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(UpdateForm);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = AlwaysOnTop.IsChecked;
        }

        public void UpdateForm(object sender, EventArgs e)
        {
            encounterlog.UpdateLog(this, null);
            EncounterData.Content = encounterlog.logStatus();
            //Application.Current.MainWindow.Title = "OverParse WDF Alpha - " + encounterlog.logStatus();
            CombatantData.Items.Clear();

            foreach (Combatant c in encounterlog.combatants)
            {
                if (Int32.Parse(c.ID) >= 10000000 || FilterPlayers.IsChecked)
                {
                    CombatantData.Items.Add(c);
                }
            }      
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CommonOpenFileDialog();
            dlg.Title = "Select your pso2_bin folder...";
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = Directory.GetCurrentDirectory();

            dlg.AddToMostRecentlyUsedList = false;
            dlg.AllowNonFileSystemItems = false;
            dlg.DefaultDirectory = Directory.GetCurrentDirectory();
            dlg.EnsureFileExists = true;
            dlg.EnsurePathExists = true;
            dlg.EnsureReadOnly = false;
            dlg.EnsureValidNames = true;
            dlg.Multiselect = false;
            dlg.ShowPlacesList = true;

            if (dlg.ShowDialog() != CommonFileDialogResult.Ok) { return; }

            var folder = dlg.FileName;
            Console.WriteLine(folder);
            Properties.Settings.Default.Path = folder;
            Console.WriteLine(encounterlog.logStatus());
            encounterlog = new Log(folder);
            Console.WriteLine(encounterlog.logStatus());
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void EndEncounter_Click(object sender, RoutedEventArgs e)
        {
            encounterlog = new Log(Properties.Settings.Default.Path);
        }

        private void AlwaysOnTop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This doesn't actually do anything yet.");
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("OverParse v.0.0.0.1 - WDF Alpha\nAnything and everything may be broken.\n\nPlease use damage information responsibly.");
        }

        private void Website_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.tyronesama.moe/");
        }
    }

    public class Combatant
    {
        public int Damage { get; set; }
        public int Healing { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }
        public int MaxHitNum { get; set; }
        public string MaxHitID { get; set; }
        public float DPS { get; set; }
        public string PercentDPS { get; set; }

        public string MaxHit
        {
            get {return MaxHitNum.ToString();}
        }

        public string DPSReadout
        {
            get {return $"{PercentDPS}%";}
        }

        public string DamageReadout
        {
            get { return Damage.ToString("N0"); }
        }

        public Combatant(string id, string name)
        {
            ID = id;
            Name = name;
            Damage = 0;
            Healing = 0;
            MaxHitNum = 0;
            MaxHitID = "none";
            DPS = 0;
            PercentDPS = "-";
        }
    }

    public class Log
    {
        bool valid;
        bool running;
        int startTimestamp = 0;
        int newTimestamp = 0;
        int partyDPS;
        string encounterData;
        StreamReader logreader;
        public List<Combatant> combatants = new List<Combatant>();

        public Log(string attemptDirectory)
        {
            Console.WriteLine($"NEW LOG: {attemptDirectory}");
            valid = false;
            DirectoryInfo directory = new DirectoryInfo($"{attemptDirectory}\\damagelogs");
            if (!directory.Exists) { return; }
            if (directory.GetFiles().Count() == 0) { return; }

            valid = true;
            running = false;

            Console.WriteLine($"VALID: {valid} - {attemptDirectory}");

            FileInfo log = directory.GetFiles().OrderByDescending(f => f.LastWriteTime).First();
            Console.WriteLine($"Reading from {log.DirectoryName}\\{log.Name}");
            FileStream fileStream = File.Open(log.DirectoryName + "\\" + log.Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fileStream.Seek(0, SeekOrigin.End);
            logreader = new StreamReader(fileStream);
        }

        public string logStatus()
        {
            if (!valid)
            {
                return "No damagelogs found...";
            }
            if (!running)
            {
                return "Waiting for combat data...";
            }
            return encounterData;
        }

        public void UpdateLog(object sender, EventArgs e)
        {
            Console.WriteLine($"updatelog called - {valid.ToString()}");
            if (!valid) { return; }
            Console.WriteLine("valid check");
            string newLines = logreader.ReadToEnd();
            if (newLines != "")
            {
                Console.WriteLine("new stuff");
                string[] result = newLines.Split('\n');
                foreach (string str in result)
                {
                    if (str != "")
                    {
                        string[] parts = str.Split(',');

                        string lineTimestamp = parts[0];
                        string sourceID = parts[2];
                        string sourceName = parts[3];
                        int hitDamage = Int32.Parse(parts[7]);
                        string attackID = parts[6];
                        string isMisc = parts[11];

                        int index = -1;
                        foreach (Combatant x in combatants)
                        {
                            if (x.ID == sourceID) { index = combatants.IndexOf(x); }
                        }

                        if (index == -1)
                        {
                            combatants.Add(new Combatant(sourceID, sourceName));
                            index = combatants.Count - 1;
                        }

                        Combatant source = combatants[index];

                        if (hitDamage > 0)
                        {
                            source.Damage += hitDamage;
                            newTimestamp = Int32.Parse(lineTimestamp);
                            if (startTimestamp == 0) { startTimestamp = newTimestamp; }
                            running = true;
                        }
                        else
                        {
                            if (startTimestamp != 0) { source.Healing -= hitDamage; }
                        }

                        if (source.MaxHitNum < hitDamage)
                        {
                            source.MaxHitNum = hitDamage;
                            source.MaxHitID = attackID;
                        }
                    }
                }


                combatants.Sort((x, y) => y.Damage.CompareTo(x.Damage));

                if (startTimestamp != 0 && newTimestamp != startTimestamp)
                {
                    int elapsed = newTimestamp - startTimestamp;
                    float partyDPS = 0;
                    int filtered = 0;

                    TimeSpan timespan = TimeSpan.FromSeconds(elapsed);
                    string timer = timespan.ToString(@"mm\:ss");

                    encounterData = $"{timer}";

                    foreach (Combatant x in combatants)
                    {
                        if (Int32.Parse(x.ID) >= 10000000)
                        {

                            float dps = x.Damage / (newTimestamp - startTimestamp);
                            x.DPS = dps;
                            partyDPS += dps;
                        }
                        else
                        {
                            filtered++;
                        }
                    }

                    foreach (Combatant x in combatants)
                    {
                        if (Int32.Parse(x.ID) >= 10000000)
                        {
                            x.PercentDPS = (x.DPS / partyDPS * 100).ToString();
                        }
                        else
                        {
                            x.PercentDPS = "-";
                        }
                    }

                    encounterData += $" - {partyDPS.ToString("0.00")} DPS";
                }

            }
        }
    }
}
