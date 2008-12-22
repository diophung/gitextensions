﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;

using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Reflection;
using System.IO;

namespace GitUI
{
    public partial class FormSettigns : Form
    {
        public FormSettigns()
        {
            InitializeComponent();

            CheckSettings();

            GitCommands.GitCommands gitCommands = new GitCommands.GitCommands();

            UserName.Text = GitCommands.GitCommands.GetSetting("user.name");
            UserEmail.Text = GitCommands.GitCommands.GetSetting("user.email");
            Editor.Text = GitCommands.GitCommands.GetSetting("core.editor");
            MergeTool.Text = GitCommands.GitCommands.GetSetting("merge.tool");
            KeepMergeBackup.Checked = GitCommands.GitCommands.GetSetting("merge.keepBackup").Trim() == "true";
            

            GlobalUserName.Text = gitCommands.GetGlobalSetting("user.name");
            GlobalUserEmail.Text = gitCommands.GetGlobalSetting("user.email");
            GlobalEditor.Text = gitCommands.GetGlobalSetting("core.editor");
            GlobalMergeTool.Text = gitCommands.GetGlobalSetting("merge.tool");
            GlobalKeepMergeBackup.Checked = gitCommands.GetGlobalSetting("merge.keepBackup").Trim() == "true";
        }

        private void UserName_TextChanged(object sender, EventArgs e)
        {
        }

        private void UserEmail_TextChanged(object sender, EventArgs e)
        {
        }

        private void Ok_Click(object sender, EventArgs e)
        {
            GitCommands.GitCommands gitCommands = new GitCommands.GitCommands();

            GitCommands.GitCommands.SetSetting("user.name", UserName.Text);
            GitCommands.GitCommands.SetSetting("user.email", UserEmail.Text);
            GitCommands.GitCommands.SetSetting("core.editor", Editor.Text);
            GitCommands.GitCommands.SetSetting("merge.tool", MergeTool.Text);
            if (KeepMergeBackup.Checked)
                GitCommands.GitCommands.SetSetting("merge.keepBackup", "true");
            else
                GitCommands.GitCommands.SetSetting("merge.keepBackup", "false");


            gitCommands.SetGlobalSetting("user.name", GlobalUserName.Text);
            gitCommands.SetGlobalSetting("user.email", GlobalUserEmail.Text);
            gitCommands.SetGlobalSetting("core.editor", GlobalEditor.Text);
            gitCommands.SetGlobalSetting("merge.tool", GlobalMergeTool.Text);

            if (GlobalKeepMergeBackup.Checked)
                gitCommands.SetGlobalSetting("merge.keepBackup", "true");
            else
                gitCommands.SetGlobalSetting("merge.keepBackup", "false");

            Close();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        protected string GetRegistryValue(RegistryKey root, string subkey, string key)
        {
            try
            {
                RegistryKey rk;
                rk = root.OpenSubKey(subkey, false);

                string value = "";

                if (rk != null && rk.GetValue(key) is string)
                {
                    value = rk.GetValue(key).ToString();
                    rk.Flush();
                    rk.Close();
                }

                return value;
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show("GitExtensions has insufficient permisions to check the registry.");
            }
            return "";
        }


        protected void SetRegistryValue(RegistryKey root, string subkey, string key, string value)
        {
            try
            {
                RegistryKey rk;
                rk = root.OpenSubKey(subkey, false);

                if (rk != null)
                {
                    rk.SetValue(key, value);
                    rk.Flush();
                    rk.Close();
                }

            }
            catch(UnauthorizedAccessException ex)
            {
                MessageBox.Show("GitExtensions has insufficient permisions to modify the registry.\nPlease add this key to the registry manually.\nPath:   " + root.ToString() + "\\" + subkey +"\nValue:  " + key + " = " + value);
            }
        }

        public bool CheckSettings()
        {
            bool bValid = true;
            try
            {
                if (string.IsNullOrEmpty(GetRegistryValue(Registry.LocalMachine, "Software\\GitExtensions", "InstallDir")) || !File.Exists(GetRegistryValue(Registry.LocalMachine, "Software\\GitExtensions", "InstallDir") + "\\GitExtensions.exe"))
                {
                    GitExtensionsInstall.BackColor = Color.LightSalmon;
                    GitExtensionsInstall.Text = "Registry entry missing [HKEY_LOCAL_MACHINE\\SOFTWARE\\GitExtensions\\InstallDir].";
                    bValid = false;
                }
                else
                {
                    GitExtensionsInstall.BackColor = Color.LightGreen;
                    GitExtensionsInstall.Text = "GitExtensions is properly registered.";
                }

                if (string.IsNullOrEmpty(GetRegistryValue(Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Shell Extensions\\Approved", "{3C16B20A-BA16-4156-916F-0A375ECFFE24}")) ||
                    string.IsNullOrEmpty(GetRegistryValue(Registry.ClassesRoot, "*\\shellex\\ContextMenuHandlers\\GitExtensions2", null)) ||
                    string.IsNullOrEmpty(GetRegistryValue(Registry.ClassesRoot, "Directory\\shellex\\ContextMenuHandlers\\GitExtensions2", null)) ||
                    string.IsNullOrEmpty(GetRegistryValue(Registry.ClassesRoot, "Directory\\Background\\shellex\\ContextMenuHandlers\\GitExtensions2", null)))
                {
                    ShellExtensionsRegistered.BackColor = Color.LightSalmon;
                    ShellExtensionsRegistered.Text = "GitExtensionsShellEx.dll needs to be registered in order to use the shell extensions.";
                    bValid = false;
                }
                else
                {
                    ShellExtensionsRegistered.BackColor = Color.LightGreen;
                    ShellExtensionsRegistered.Text = "Shell extensions registered properly.";
                }

                GitCommands.GitCommands gitCommands = new GitCommands.GitCommands();
                if (string.IsNullOrEmpty(gitCommands.GetGlobalSetting("user.name")) ||
                    string.IsNullOrEmpty(gitCommands.GetGlobalSetting("user.email")))
                {
                    UserNameSet.BackColor = Color.LightSalmon;
                    UserNameSet.Text = "You need to configure a user name and an email address.";
                    bValid = false;
                }
                else
                {
                    UserNameSet.BackColor = Color.LightGreen;
                    UserNameSet.Text = "There is a user name and an email address configured.";
                }

                if (string.IsNullOrEmpty(gitCommands.GetGlobalSetting("merge.tool")))
                {
                    DiffTool.BackColor = Color.LightSalmon;
                    DiffTool.Text = "You need to configure merge tool in order to solve mergeconflicts (kdiff3 for example).";
                    bValid = false;
                }
                else
                {
                    DiffTool.BackColor = Color.LightGreen;
                    DiffTool.Text = "There is a mergetool configured.";
                }

                if (string.IsNullOrEmpty(GitCommands.GitCommands.RunCmd("git.exe", "status")))
                {
                    GitFound.BackColor = Color.LightSalmon;
                    GitFound.Text = "Git.exe needs to be in the system path. To solve this problem you can add git.exe to the path or reinstall git.";
                    bValid = false;
                }
                else
                {
                    GitFound.BackColor = Color.LightGreen;
                    GitFound.Text = "Git.exe is found on your computer.";
                }
            }
            catch
            {
            }
            if ((Application.UserAppDataRegistry.GetValue("checksettings") == null ||
                  Application.UserAppDataRegistry.GetValue("checksettings").ToString() == "true"))
            {
                CheckAtStartup.Checked = true;
            }
            else
            {
                CheckAtStartup.Checked = false;
            }


            return bValid;
        }

        private void GitExtensionsInstall_Click(object sender, EventArgs e)
        {
            string fileName = Assembly.GetAssembly(GetType()).Location;
            fileName = fileName.Substring(0, fileName.LastIndexOfAny(new char[]{'\\', '/'}));

            if (File.Exists(fileName+"\\GitExtensions.exe"))
                SetRegistryValue(Registry.LocalMachine, "Software\\GitExtensions", "InstallDir", fileName);

            CheckSettings();
        }

        private void ShellExtensionsRegistered_Click(object sender, EventArgs e)
        {

            if (File.Exists(GetRegistryValue(Registry.LocalMachine, "Software\\GitExtensions", "InstallDir") + "\\GitExtensionsShellEx.dll"))
                GitCommands.GitCommands.RunCmd("regsvr32", "\"" + GetRegistryValue(Registry.LocalMachine, "Software\\GitExtensions", "InstallDir") + "\\GitExtensionsShellEx.dll\"");
            else
                {
                    string fileName = Assembly.GetAssembly(GetType()).Location;
                    fileName = fileName.Substring(0, fileName.LastIndexOfAny(new char[] { '\\', '/' })) + "\\GitExtensionsShellEx.dll";

                    if (File.Exists(fileName))
                        GitCommands.GitCommands.RunCmd("regsvr32", fileName);
                }

            CheckSettings();
        }

        private void UserNameSet_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab("GlobalSettingsPage");
        }

        private void DiffTool_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab("GlobalSettingsPage");
        }

        private void GitFound_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This problem cannot be solved by GitExtensions.\nYou need to add git.exe to the path manually or reinstall Git.");
        }

        private void FormSettigns_Load(object sender, EventArgs e)
        {

        }

        private void CheckAtStartup_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckAtStartup.Checked)
                Application.UserAppDataRegistry.SetValue("checksettings", "true");
            else
                Application.UserAppDataRegistry.SetValue("checksettings", "false");
        }

        private void Rescan_Click(object sender, EventArgs e)
        {
            CheckSettings();
        }
    }
}
