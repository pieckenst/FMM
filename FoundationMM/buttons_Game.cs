﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace FoundationMM
{
    public partial class Window : Form
    {
        bool isRestoringVsBackingUp = false;

        private void launchDewritoClick(object sender, EventArgs e)
        {
            Process.Start(Path.Combine(System.IO.Directory.GetCurrentDirectory(), "eldorado.exe"), "-launcher");
        }

        private void applyClick(object sender, EventArgs e)
        {
            if (listView1.CheckedItems.Count == 0)
            {
                string fmmdat = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "fmm.dat");
                FileStream fmmdatWiper = File.Open(fmmdat, FileMode.OpenOrCreate);
                fmmdatWiper.SetLength(0);
                fmmdatWiper.Close();

                string mapsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "maps");
                DirectoryInfo dir1 = Directory.CreateDirectory(Path.Combine(mapsPath, "fmmbak"));
                DirectoryInfo dir2 = Directory.CreateDirectory(Path.Combine(mapsPath, "fmmbak", "fonts"));
                DirectoryInfo dir3 = Directory.CreateDirectory(Path.Combine(mapsPath, "fonts"));

                isRestoringVsBackingUp = false;

                if (File.Exists(Path.Combine(mapsPath, "fmmbak", "tags.dat")))
                {
                    if (restoreCleanWorker.IsBusy != true || !isFileLocked(new FileInfo(Path.Combine(mapsPath, "tags.dat"))))
                    {
                        button1.Enabled = false;
                        button2.Enabled = false;
                        openGameRoot.Enabled = false;
                        openMods.Enabled = false;
                        button5.Enabled = false;
                        button6.Enabled = false;
                        restoreCleanWorker.RunWorkerAsync(new string[] { mapsPath });
                    }
                }
                else
                {
                    MessageBox.Show("No clean files stored.");
                }
            }
            else
            {
                DialogResult confirmApply = MessageBox.Show("Are you sure you want to apply these mods?\nMods downloaded from unsafe locations may harm your computer.", "Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirmApply == DialogResult.No) { return; }

                string mapsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "maps");

                // Backup tags and stuff
                DirectoryInfo dir1 = Directory.CreateDirectory(Path.Combine(mapsPath, "fmmbak"));
                DirectoryInfo dir2 = Directory.CreateDirectory(Path.Combine(mapsPath, "fmmbak", "fonts"));
                DirectoryInfo dir3 = Directory.CreateDirectory(Path.Combine(mapsPath, "fonts"));

                isRestoringVsBackingUp = false;

                if (File.Exists(Path.Combine(mapsPath, "fmmbak", "tags.dat")))
                {
                    isRestoringVsBackingUp = true;
                }

                if (fileTransferWorker.IsBusy != true || !isFileLocked(new FileInfo(Path.Combine(mapsPath, "tags.dat"))))
                {
                    button1.Enabled = false;
                    button2.Enabled = false;
                    button3.Enabled = false;
                    button4.Enabled = false;
                    openGameRoot.Enabled = false;
                    openMods.Enabled = false;
                    button5.Enabled = false;
                    button6.Enabled = false;
                    tabControl1.Enabled = false;
                    fileTransferWorker.RunWorkerAsync(new string[] { mapsPath });
                }
            }
        }

        private bool isFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }
        
        private void button16_Click(object sender, EventArgs e)
        {
            tabControl1.Enabled = false;

            string remLocation = "https://github.com/Clef-0/FMM-Mods/trunk/" + listView2.SelectedItems[0].SubItems[5].Text;
            Debug.WriteLine(remLocation);
            string locLocation = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "mods", "tagmods", listView2.SelectedItems[0].SubItems[5].Text.Replace("/","\\"));
            Debug.WriteLine(locLocation);

            dlModWorker.RunWorkerAsync(new string[] { remLocation, locLocation });
        }
    }
}
