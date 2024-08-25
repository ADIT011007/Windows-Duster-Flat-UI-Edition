using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace Windows_Duster_FlatUi_Edition
{
    public partial class Form1 : Form
    {
        private DriveInfo[] drives;
        private bool isnetworkworking = false;
        private bool isworkingonupdateorscan = false;
        public Form1()
        {
            InitializeComponent();
            timer1 = new Timer();
            timer1.Interval = 10000; // 10 seconds
            timer1.Tick += timer1_Tick;
            timer1.Start();
            ShowTipWithAnimation();
            CheckAndRestartAsAdmin();
            metroListView1.View = View.Details;
            metroListView1.Columns.Add("Operation", 150);
            metroListView1.Columns.Add("Status", 159);
            CheckForHDD();
        }
        private readonly string[] tips = new[]
        {
            "Tip 1: Use Windows + L to quickly lock your PC.",
            "Tip 2: Use Ctrl + Shift + Esc to open Task Manager.",
            "Tip 3: Use Alt + Tab to switch between open applications.",
            "Tip 4: Use Windows + D to show or hide the desktop.",
"Tip 5: Use Windows + E to open File Explorer quickly.",
"Tip 6: Use Windows + I to open the Settings app.",
"Tip 7: Use Windows + X to open the Quick Link menu (right-click on the Start menu).",
"Tip 8: Use Ctrl + Shift + N to create a new folder in File Explorer.",
"Tip 9: Use Windows + R to open the Run dialog box.",
"Tip 10: Keep your system updated with the latest Windows updates for better security and performance.",
"Tip 11: Regularly clean up temporary files and unused programs to free up disk space.",
"Tip 12: Use a strong password or enable Windows Hello for better security.",
"Tip 13: Backup your important files regularly to avoid data loss.",
"Tip 14: Use Windows Defender or another antivirus program to protect your PC from malware.",
"Tip 15: Enable System Restore to recover your system in case of issues.",
"Tip 16: Defragment your hard drive regularly if you are using an HDD.",
"Tip 17: Use Windows + P to quickly switch between display modes (e.g., duplicate, extend, etc.).",
"Tip 18: Use Windows + S to quickly search for apps, files, or settings.",
"Tip 19: Disable startup programs you don't need to speed up boot time.",
"Tip 20: Keep your drivers up to date for better hardware performance.",
"Tip 21: Use Windows + Arrow keys to snap windows to different parts of the screen.",
"Tip 22: Enable Night Light in display settings to reduce eye strain during night use.",
"Tip 23: Use Windows + V to access clipboard history.",
"Tip 24: Use Task Manager to monitor system performance and troubleshoot issues.",
"Tip 25: Use the Snipping Tool or Windows + Shift + S to take screenshots.",
"Tip 26: Use Disk Cleanup to remove unnecessary files and free up space.",
"Tip 27: Turn on automatic maintenance in Security and Maintenance settings for regular system checks.",
"Tip 28: Customize your Start menu and taskbar for quick access to frequently used apps.",
"Tip 29: Use virtual desktops (Windows + Ctrl + D) to organize your workspace.",
"Tip 30: Adjust your power settings for better battery life or performance.",
"Tip 31: Use Windows + Spacebar to quickly switch input languages or keyboard layouts.",
"Tip 32: Regularly update your browser and clear its cache to maintain browsing speed.",
"Tip 33: Use Windows + G to open the Xbox Game Bar for gaming features.",
"Tip 34: Use Alt + F4 to close the currently active window or app.",
"Tip 35: Create a system image backup for a full recovery in case of a major issue.",
"Tip 36: Use Windows + U to quickly access the Ease of Access settings.",
"Tip 37: Set up a PIN or fingerprint login for faster sign-in.",
"Tip 38: Use Windows + L to lock your screen when stepping away.",
"Tip 39: Consider using BitLocker to encrypt your drive for added security.",
"Tip 40: Use Windows + , (comma) to peek at the desktop temporarily.",
"Tip 41: Adjust your display scaling in settings for better readability.",
"Tip 42: Keep your BIOS/UEFI firmware up to date for system stability.",
"Tip 43: Set up automatic backups using File History in Windows.",
"Tip 44: Use Focus Assist to minimize distractions during work or presentations.",
"Tip 45: Use Windows + 1, 2, 3, etc., to quickly launch taskbar pinned apps.",
"Tip 46: Configure your network settings to prioritize a wired connection for faster internet.",
"Tip 47: Regularly check your storage using the Storage Sense feature.",
"Tip 48: Use PowerShell or Command Prompt for advanced system management.",
"Tip 49: Learn and use keyboard shortcuts to enhance your productivity.",
"Tip 50: Schedule regular scans with Windows Defender or your antivirus to keep your system secure."
        };

        private int currentTipIndex = 0;




        private void CheckAndRestartAsAdmin()
        {
            if (!IsRunningAsAdmin())
            {
                RestartAsAdmin();
                Environment.Exit(0); // Ensure the non-admin instance exits immediately
            }
        }

        private bool IsRunningAsAdmin()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void RestartAsAdmin()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    UseShellExecute = true,
                    FileName = Application.ExecutablePath,
                    Verb = "runas" // This prompts for admin privileges
                };

                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("This application must be run as Administrator. Restarting failed.");
            }
        }
      
        private void iconButton2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("This May Dissconnect You From Your Network Until The Procesess Is Done. ","Hence Are You Still Willing To Proceed ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                timer2.Start();
                iconButton1.Enabled = false;
                iconButton4.Enabled = false;
                iconButton2.Enabled = false;
                iconButton3.Enabled = false;
                RunIpConfigCommands();
                label3.Text = "Optimising Network";
                isnetworkworking = true;
            }
            else
            {
                //Nothing
            }
        }
        string ipconfigoutput;
        private void RunIpConfigCommands()
        {
            label3.Text = "Optimising Network";
            string[] commands = { "ipconfig /release", "ipconfig /renew", "ipconfig /flushdns", "ipconfig /registerdns" };
            progressBar1.Maximum = commands.Length;
            progressBar1.Value = 0;

            foreach (string command in commands)
            {
                ExecuteCommand(command);
                progressBar1.Value += 1;
                // Check for elevation requirement
                if (ipconfigoutput.Contains("The requested operation requires elevation."))
                {
                    MessageBox.Show("Please run the application as an administrator.", "Elevation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    break;
                }
            }
           
        }

        private void ExecuteCommand(string command)
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd", "/c " + command)
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                ipconfigoutput = (result + Environment.NewLine);
            }
            label3.Text = "Completed Optimising Network";
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            iconButton1.Enabled = false;
            iconButton4.Enabled = false;
            iconButton2.Enabled = false;
            iconButton3.Enabled = false;

            string[] tempDirectories = new string[]
    {
        Path.GetTempPath(),
        Environment.GetFolderPath(Environment.SpecialFolder.InternetCache),
        Environment.GetFolderPath(Environment.SpecialFolder.Recent),
        Environment.GetFolderPath(Environment.SpecialFolder.Cookies),
        Environment.GetFolderPath(Environment.SpecialFolder.History),
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Recent",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Caches",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Edge\User Data\Default\Cache",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cache",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Mozilla\Firefox\Profiles",
        @"C:\Windows\Temp",
        @"C:\Windows\Prefetch"
    };

            foreach (string dir in tempDirectories)
            {
                if (Directory.Exists(dir))
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // Skip the file or log the issue if needed
                            }
                            catch (Exception ex)
                            {
                                // Handle other exceptions if needed
                            }
                        }

                        foreach (string subDir in Directory.GetDirectories(dir))
                        {
                            try
                            {
                                Directory.Delete(subDir, true);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // Skip the directory or log the issue if needed
                            }
                            catch (Exception ex)
                            {
                                // Handle other exceptions if needed
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip the directory entirely if access is denied
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions if needed
                    }
                }
            }
            ShowTempFilesSize();
            label3.Text = "Cleaned Temp Files";
            iconButton1.Enabled = true;
            iconButton4.Enabled = true;
            iconButton2.Enabled = true;
            iconButton3.Enabled = true;
        }


        private void cleantempfiles_oneclick_optimiser()
        {
            string[] tempDirectories = new string[]
    {
        Path.GetTempPath(),
        Environment.GetFolderPath(Environment.SpecialFolder.InternetCache),
        Environment.GetFolderPath(Environment.SpecialFolder.Cookies),
        Environment.GetFolderPath(Environment.SpecialFolder.History),
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Recent",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Caches",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Edge\User Data\Default\Cache",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cache",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Mozilla\Firefox\Profiles",
        @"C:\Windows\Temp",
        @"C:\Windows\Prefetch"
          
    };

            foreach (string dir in tempDirectories)
            {
                if (Directory.Exists(dir))
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // Skip the file or log the issue if needed
                            }
                            catch (Exception ex)
                            {
                                // Handle other exceptions if needed
                            }
                        }

                        foreach (string subDir in Directory.GetDirectories(dir))
                        {
                            try
                            {
                                Directory.Delete(subDir, true);
                            }
                            catch (UnauthorizedAccessException)
                            {
                                // Skip the directory or log the issue if needed
                            }
                            catch (Exception ex)
                            {
                                // Handle other exceptions if needed
                            }
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip the directory entirely if access is denied
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions if needed
                    }
                }
            }
            ShowTempFilesSize();
        }
        private void ShowTempFilesSize()
        {
            string[] tempDirectories = new string[]
            {
        Path.GetTempPath(),
        Environment.GetFolderPath(Environment.SpecialFolder.InternetCache),
        Environment.GetFolderPath(Environment.SpecialFolder.Cookies),
        Environment.GetFolderPath(Environment.SpecialFolder.History),
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Recent",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Temp",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Caches",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Edge\User Data\Default\Cache",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Google\Chrome\User Data\Default\Cache",
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Mozilla\Firefox\Profiles",
        @"C:\Windows\Temp",
        @"C:\Windows\Prefetch"
            };

            long totalSize = 0;

            foreach (string dir in tempDirectories)
            {
                if (Directory.Exists(dir))
                {
                    try
                    {
                        totalSize += Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Sum(f => new FileInfo(f).Length);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Skip this directory if access is denied
                    }
                    catch (Exception ex)
                    {
                        // Handle other exceptions if needed
                    }
                }
            }

            label2.Text = $"Total Temp Files Size: {totalSize / (1024 * 1024)} MB";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer2.Start();
            metroLabel1.Visible = false;
            Form2 frm2 = new Form2();
            ShowTempFilesSize();
            if(this.Visible == false && frm2.Visible)
            {
                //do Nothing
            }
            else if(this.Visible == true && frm2.Visible == false)
            {
                frm2.Close();
            }
        }

        private void KillAllDismProcesses()
        {
            try
            {
                ProcessStartInfo processInfo = new ProcessStartInfo
                {
                    FileName = "taskkill",
                    Arguments = "/F /IM dism.exe /T",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (Process process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.Start();
                    process.WaitForExit();
                }

                metroListView1.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void iconButton1_Click(object sender, EventArgs e)
        {
            button1.Visible = true;
            string errorMessage;
            bool isNetworkAvailable = NetworkHelper.IsNetworkAvailable(out errorMessage);
            
            
            DialogResult result = MessageBox.Show(
        "1. Scan Health: Checks the Windows image for component store corruption.\r\n\r\n" +
        "2. Check Health: Detects whether any corruption has been detected in the Windows image.\r\n\r\n" +
        "3. Restore Health: Scans the Windows image for corruption and performs a repair automatically.\r\n\r\n" +
        "4. Analyze Component Store: Analyzes the Windows component store to show which components can be cleaned up.\r\n\r\n" +
        "5. Component Cleanup: Cleans up the Windows component store to free up disk space by removing superseded components.\r\n\r\n" +
        "6. Reset Base: Removes superseded versions of every component in the component store, effectively resetting it.\r\n\r\n" +
        "7. Repair Windows Image: Repairs the Windows image using a specified source file to replace corrupted or missing files.\r\n\r\n" +
        "8. Enable Windows Features: Enables optional Windows features, like the .NET Framework 3.5.\r\n\r\n" +
        "9. Check System File Integrity: Verifies the integrity of system files to ensure they are not corrupted or altered.\r\n\r\n" +
        "10. List Available Features: Lists all Windows features available on the system that can be enabled or disabled.\r\n\r\n" +
        "11. Check Driver Status: Displays a list of all installed drivers and their status on the system.\r\n\r\n" +
        "12. Remove Superseded Updates: Removes all superseded updates to reduce the size of the component store and free up disk space.\r\n\r\n" +
        "13. Enable Optional Features: Enables multiple optional features, such as .NET Framework versions and other Windows components.\r\n\r\n" +
        "14. Check Disk for Errors: Scans the disk for errors and attempts to repair them, which can help with system stability.\r\n\r\n" +
        "15. Update Windows Image: Updates the Windows image to the latest version, ensuring all components are up to date.\r\n\r\n" +
        "16. Scan for Malware: Scans for malware within the system image using integrated anti-malware tools.",
        "DISM Features", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

            // Check the result and perform actions based on the user's choice
            if (result == DialogResult.OK)
            {
                await Task.Delay(1000);
                isworkingonupdateorscan = true;
                button1.Visible = true;
                iconButton4.Enabled = false;
                iconButton2.Enabled = false;
                iconButton3.Enabled = false;
                metroListView1.Visible = true;
                if (isNetworkAvailable)
                {

                    KillAllDismProcesses();
                    // List of DISM commands to execute
                    var dismCommands = new Dictionary<string, string>
{

    {"Scan Health", "/Cleanup-Image /ScanHealth"},
    {"Check Health", "/Cleanup-Image /CheckHealth"},
    {"Restore Health", "/Cleanup-Image /RestoreHealth"},
    {"Analyze Component Store", "/cleanup-image /AnalyzeComponentStore"},
    {"Component Cleanup", "/cleanup-image /StartComponentCleanup"},
    {"Reset Base", "/cleanup-image /ResetBase"},
    {"Repair Windows Image", "/Online /Cleanup-Image /RestoreHealth /Source:wim:X:\\sources\\install.wim:1 /LimitAccess"},
    {"Enable Windows Features", "/Online /Enable-Feature /FeatureName:NetFx3 /All"},
    {"Check System File Integrity", "/Online /Cleanup-Image /CheckIntegrity"},
    {"List Available Features", "/Online /Get-Features"},
    {"Check Driver Status", "/Online /Get-Drivers /Format:Table"},
    {"Remove Superseded Updates", "/Online /Cleanup-Image /StartComponentCleanup /ResetBase"},
    {"Enable Optional Features", "/Online /Enable-Feature /All /FeatureName:NetFx3,NetFx4"},
    {"Check Disk for Errors", "/Online /Cleanup-Image /CheckSpill /EnablePolicy"},
    {"Update Windows Image", "/Online /Cleanup-Image /Update-Image"},
    {"Scan for Malware", "/Online /Cleanup-Image /ScanForMalware"},
};

                    // Clear existing items in the ListView
                    metroListView1.Items.Clear();

                    // Add items to ListView
                    foreach (var command in dismCommands)
                    {
                        ListViewItem item = new ListViewItem(new[] { command.Key, "Pending" });
                        metroListView1.Items.Add(item);
                    }

                    // Execute DISM commands
                    int totalProgress = 0; // Initialize total progress to 0
           

                    // Execute DISM commands
                    foreach (var command in dismCommands)
                    {
                        await ExecuteDismCommandAsync(command.Key, command.Value);
                        totalProgress += 100; // Assuming each command contributes 100% to total progress
                    }


                }
                else
                {

                    MessageBox.Show("It appears that you are not connected to the internet. An internet connection is required to start fixing Windows corrupted files.", "Network Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            else if (result == DialogResult.Cancel)
            {
                button1.Visible = false;
                MessageBox.Show("Cancled");
            }

           

        }

        private async Task ExecuteDismCommandAsync(string commandKey, string commandValue)
        {
            // UI updates should always be on the UI thread
            BeginInvoke(new Action(() =>
            {
                iconButton1.Enabled = false;
                iconButton4.Enabled = false;
                iconButton2.Enabled = false;
                iconButton3.Enabled = false;
            }));

            await Task.Run(() =>
            {
                Console.WriteLine($"Executing DISM command: {commandKey}");

                // Create process to execute DISM command
                Process dismProcess = new Process();
                dismProcess.StartInfo.FileName = "dism.exe";
                dismProcess.StartInfo.Arguments = $"/Online {commandValue}";
                dismProcess.StartInfo.UseShellExecute = false;
                dismProcess.StartInfo.RedirectStandardOutput = true;
                dismProcess.StartInfo.RedirectStandardError = true;
                dismProcess.StartInfo.CreateNoWindow = true;
                dismProcess.EnableRaisingEvents = true;

                // Event handler for capturing output
                dismProcess.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        Console.WriteLine(e.Data);

                        BeginInvoke(new Action(async () =>
                        {
                            // Find the corresponding ListViewItem by searching for the command key
                            var item = metroListView1.Items
                                .Cast<ListViewItem>()
                                .FirstOrDefault(i => i.SubItems[0].Text == commandKey);

                            if (item != null)
                            {
                                item.SubItems[1].Text = "In Progress"; // Update status
                                                                       // Update ListView with command status
                                if (e.Data.Contains("Actual Size of Component Store"))
                                {
                                    metroLabel1.Visible = true;
                                    metroLabel1.Text = e.Data;
                                    await Task.Delay(10000);
                                    metroLabel1.Visible = false;

                                }

                                // Example of parsing progress or update UI based on the data
                                UpdateProgress(e.Data, item);
                            }
                        }));
                    }
                };

                // Start the process and begin reading output
                dismProcess.Start();
                dismProcess.BeginOutputReadLine();

                // Wait for process to exit asynchronously
                dismProcess.WaitForExit();
                int cmmandcount = +1;
                if(cmmandcount  == 16)
                {
                    // Prompt user for confirmation
                    DialogResult result = MessageBox.Show("Warning: A full system scan requires a system restart. Please save all your work and close any open applications before proceeding .","Hence Are You Still WIlling To Contineou ?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        // Start the process to restart the system
                        ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/r /t 0");
                        psi.CreateNoWindow = true;
                        psi.UseShellExecute = false;
                        Process.Start(psi);
                    }
                }

                Console.WriteLine($"DISM command {commandKey} completed.");
            });

            // Re-enable buttons on the UI thread after the command completes
            BeginInvoke(new Action(() =>
            {
                iconButton1.Enabled = true;
                iconButton4.Enabled = true;
                iconButton2.Enabled = true;
                iconButton3.Enabled = true;

                // Update ListView item status to "Completed"
                var item = metroListView1.Items
                    .Cast<ListViewItem>()
                    .FirstOrDefault(i => i.SubItems[0].Text == commandKey);
                if (item != null)
                {
                    item.SubItems[1].Text = "Completed";
                }
            }));
        }




        private void UpdateProgress(string output, ListViewItem item)
        {
            // Search for progress percentage in output
            // Assuming progress is represented as a number followed by '%'
            int startIndex = output.IndexOf('%') - 4;
            if (startIndex >= 0)
            {
                string progressStr = output.Substring(startIndex, 4);
                if (double.TryParse(progressStr, out double progress))
                {

                    // Update progress in ListView
                    item.SubItems[1].Text = $"{progress}%";

                }
            }
        }

      

        private void metroButton1_Click(object sender, EventArgs e)
        {
            KillAllDismProcesses();
            timer1.Stop();
            timer2.Stop();
            this.Dispose();
            this.Close();
            Application.Exit();

        }

        private async Task ShowTipWithAnimation()
        {
            textBox1.Clear();
            string tip = tips[currentTipIndex];
            foreach (char c in tip)
            {
                if (this.IsDisposed || textBox1.IsDisposed)
                {
                    return; // Exit if disposed
                }
                textBox1.AppendText(c.ToString());

                await Task.Delay(50); // Adjust delay for typing speed
            }
        }
        private async void timer1_Tick(object sender, EventArgs e)
        {
            currentTipIndex = (currentTipIndex + 1) % tips.Length;
            await ShowTipWithAnimation();

        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (progressBar1.Value == 4)
            {
                label3.Text = "Network Optimization Done";
                iconButton1.Enabled = true;
                iconButton4.Enabled = true;
                iconButton2.Enabled = true;
                iconButton3.Enabled = true;
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop and dispose of Timer1
            if (timer1 != null)
            {
                timer1.Stop();
                timer1.Dispose();
            }

            // Stop and dispose of Timer2
            if (timer2 != null)
            {
                timer2.Stop();
                timer2.Dispose();
            }

            // It's generally unnecessary to call Refresh or Update on textBox1 in the closing event.
            // If you need to do some last-minute data processing or saving, consider doing it here instead.
        }


        private async void iconButton4_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Warning: Defragmenting your HDD may take up to an hour or longer, depending on the current health and fragmentation level of your disk. ","Please ensure your device is plugged in and avoid using it heavily during the process." , MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
            if (result == DialogResult.OK)
            {
                iconButton4.Enabled = false;
                metroLabel1.Visible = true;
                CheckForHDD();
                await Task.Delay(2000);
                StartDefragmentation();
            }
                
          
        }

        private async void StartDefragmentation()
        {
            // Check if hddDrives is initialized and contains drives
            if (hddDrives == null || hddDrives.Count == 0)
            {
                MessageBox.Show("No HDDs available for defragmentation.");
                iconButton4.Enabled = true;
                return;
            }

            metroLabel1.Text =($"Found {hddDrives.Count} drive(s) to defragment.");

            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnimationSpeed = 30;

            foreach (DriveInfo drive in hddDrives)
            {
                if (drive == null || !drive.IsReady) continue; // Skip if the drive is null or not ready

                string driveLetter = drive.Name.TrimEnd('\\'); // Remove trailing backslash
                metroLabel1.ForeColor = Color.Green;
                metroLabel1.Text =($"Defragmenting drive: {driveLetter}");

                string defragArguments = $"/O /U {driveLetter}";
                Process defragProcess = new Process();
                defragProcess.StartInfo.FileName = "defrag.exe";
                defragProcess.StartInfo.Arguments = defragArguments;
                defragProcess.StartInfo.UseShellExecute = false;
                defragProcess.StartInfo.RedirectStandardOutput = true;
                defragProcess.StartInfo.RedirectStandardError = true;
                defragProcess.StartInfo.CreateNoWindow = true;

                try
                {
                    defragProcess.Start();

                    // Capture output and errors
                    string output = defragProcess.StandardOutput.ReadToEnd();
                    string error = defragProcess.StandardError.ReadToEnd();

                    defragProcess.WaitForExit();

                    // Check if there were any errors
                    if (defragProcess.ExitCode != 0)
                    {
                        metroLabel1.ForeColor= Color.Red;
                        metroLabel1.Text = ($"Defragmentation of {driveLetter} failed: {error}");
                        await Task.Delay(10000);
                        metroLabel1.Visible = false;
                    }
                    else
                    {
                        metroLabel1.ForeColor = (Color.Green);
                        metroLabel1.Text = ($"Defragmentation of {driveLetter} completed successfully.");
                        await Task.Delay(10000);
                        metroLabel1.Visible = false;
                     
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error during defragmentation of {driveLetter}: {ex.Message}");
                }
            }

            // After processing all drives
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.MarqueeAnimationSpeed = 0;
            metroLabel1.Text = ("Defragmentation complete for all detected HDDs.");
            progressBar1.Visible = false;

        }


        private List<DriveInfo> hddDrives;
        private void CheckForHDD()
        {
            drives = DriveInfo.GetDrives();
            hddDrives = new List<DriveInfo>();

            foreach (DriveInfo drive in drives)
            {
                // Log all detected drives with more detail
                string message = $"Drive: {drive.Name}, Type: {drive.DriveType}, Ready: {drive.IsReady}, " +
                                 $"Format: {drive.DriveFormat}, Volume Label: {drive.VolumeLabel}, Total Size: {drive.TotalSize}";
                metroLabel1.Text = (message);

                // Add all drives except CDRom and Unknown types
                if (drive.IsReady && drive.DriveType != DriveType.CDRom && drive.DriveType != DriveType.Unknown)
                {
                    hddDrives.Add(drive);
                }
            }

            if (hddDrives.Count == 0)
            {
                metroLabel1.Text = ("No HDD found.");
                iconButton4.Enabled = true;
            }
            else
            {
                iconButton4.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KillAllDismProcesses();
            button1.Visible = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
