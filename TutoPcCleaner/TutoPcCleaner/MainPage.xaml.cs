using Plugin.LocalNotification;
using System.Runtime.InteropServices;
using TutoPcCleaner.Helpers;

namespace TutoPcCleaner
{
    public partial class MainPage : ContentPage
    {
        Sysinfos Sysinfos = new();

        bool chkbFichiersTempChecked = true;
        bool chkbCorbeilleChecked = true;
        bool chkbWinUpdateChecked = true;
        bool chkbErrorsChecked = true;
        bool chkbLogsChecked = true;
        long totalCleanedSize = 0;
        int version = 1;
        public MainPage()
        {
            InitializeComponent();
            CHeckVersion();
            ShowSystemInfos();
            InitChkbStates();
        }

        private async void InfoButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                Uri uri = new Uri("https://www.anthony-cardinale.fr/tools/pccleaner/");
                await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }catch(Exception ex)
            {

            }
        }


        public void ShowSystemInfos()
        {
            osVersion.Text = Sysinfos.GetWinVer();
            hardware.Text = Sysinfos.GetHardwareInfos();
        }

        public void InitChkbStates()
        {
            chkbFichiersTempChecked = Preferences.Get("chkbFichierstempChecked", true);
            chkbCorbeilleChecked = Preferences.Get("chkbCorbeilleChecked", true);
            chkbWinUpdateChecked = Preferences.Get("chkbWinUpdateChecked", true);
            chkbErrorsChecked = Preferences.Get("chkbErrorsChecked", true);
            chkbLogsChecked = Preferences.Get("chkbLogsChecked", true);

            // Appliquer les valeurs aux cases à cocher
            chkbFichiersTemp.IsChecked = chkbFichiersTempChecked;
            chkbCorbeille.IsChecked = chkbCorbeilleChecked;
            chkbWinUpdate.IsChecked = chkbWinUpdateChecked;
            chkbErrors.IsChecked = chkbErrorsChecked;
            chkbLogs.IsChecked = chkbLogsChecked;
        }

        private void ButtonClean_Clicked(object sender, EventArgs e)
        {

            ResetValues();
            infos.IsVisible = false;

            //Fichiers temp
            if (chkbFichiersTempChecked)
            {
                ClearWindowsTempFolder();
            }
            //Vider corbeille
            if (chkbCorbeilleChecked)
            {
                EmptyRecycleBin();
            }
            //MAJ
            if (chkbWinUpdateChecked)
            {
                ClearWinUpdate();
            }
            //Errors
            if (chkbErrorsChecked)
            {
                ClearWinWer();
            }
            //Logs
            if (chkbLogsChecked)
            {
                ClearWinLogs();
            }
            progression.Progress = 1;
            tableRecap.IsVisible = true;

            long totalCleanedSizeInMb = totalCleanedSize / 1000000;
            if(totalCleanedSizeInMb < 0)
            {
                totalCleanedSizeInMb = 0;
            }
            if(totalCleanedSizeInMb < 50)
            {
                totalSize.Text = "< 50 MB supprimés";
            }
            else
            {
                totalSize.Text = "~" + totalCleanedSizeInMb + "MB supprimés !";
            }

        }

        public void EmptyRecycleBin()
        {
            //Flag pour annuler la confirmation de suppression
            const int SHERB_NOCONFIRMATION = 0x00000001;
            try
            {
                SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION);
                detailCorbeille.Detail = "Les fichiers de la corbeille ont été supprimés !";
            }catch (Exception ex) { }
        }
        [DllImport("shell32.dll")]
        static extern int SHEmptyRecycleBin(IntPtr hWnd, string pszRootPath, uint dwFlags);

        public void ClearWindowsTempFolder()
        {
            string path = @"C:\Windows\Temp";
            if (Directory.Exists(path))
            {
                detailFichiersTemp.Detail = GetFilesCountInFolder(path) + " fichiers supprimés.";
                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize += size;
                ProcessDirectory(path);
            }
        }
        public int GetFilesCountInFolder(string path)
        {
            int count = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).Count();
            return count;
        }
        public void ProcessDirectory(string targetPath)
        {
            string[] fileEntries = Directory.GetFiles(targetPath);
            foreach(string fileName in fileEntries)
            {
                processFile(fileName);
            }
            string[] subdirectoryEntries = Directory.GetDirectories(targetPath);
            foreach(string subdirectory in subdirectoryEntries)
            {
                //ProcessDirectory(subdirectory);
                Directory.Delete(subdirectory,true);
            }
        }

        public void processFile(string path)
        {
            try
            {
                if (path.Contains("\\Temp"))
                {
                    File.Delete(path);
                }
                else if (path.Contains("\\SoftwareDistribution"))
                {
                    File.Delete(path);
                }
                else if (path.Contains("\\winevt\\Logs"))
                {

                    File.Delete(path);
                }
                else if (path.Contains("\\Windows\\WER"))
                {
                    File.Delete(path);
                }
                
            }
            catch(Exception ex) 
            { 
                FileInfo fi = new FileInfo(path);
                totalCleanedSize -= fi.Length;
                Console.WriteLine("The process failed: {0}",ex.Message);
            }
        }

        public void ClearWinUpdate()
        {
            string path = @"C:\Windows\SoftwareDistribution\Download";
            if (Directory.Exists(path))
            {
                detailWinUpdate.Detail = GetFilesCountInFolder(path) + " fichiers supprimés";
                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize += size;
                ProcessDirectory(path);
            }
        }
        public void ClearWinWer()
        {
            string path = @"C:\ProgramData\Microsoft\Windows\WER";
            if (Directory.Exists(path))
            {
                detailErrors.Detail = GetFilesCountInFolder(path) + " fichiers supprimés.";
                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize += size;
                ProcessDirectory(path);
            }
        }

        public void ClearWinLogs()
        {
            string path = @"C:\Windows\System32\winevt\Logs";
            if (Directory.Exists(path))
            {
                detailLogs.Detail = GetFilesCountInFolder(path) + " fichiers supprimés.";
                var size = DirSize(new DirectoryInfo(path));
                totalCleanedSize += size;
                ProcessDirectory(path);
            }
        }

        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;
            FileInfo[] fis = d.GetFiles();
            foreach(FileInfo fi in fis)
            {
                size += fi.Length;
            }

            DirectoryInfo[] dis = d.GetDirectories();
            foreach(DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }
            return size;
        }

        /// <summary>
        /// Reinitialiser le tableau Recap
        /// </summary>
        public void ResetValues()
        {
            totalCleanedSize = 0;
            progression.Progress = 0;
            tableRecap.IsVisible = false;
            totalSize.Text = "";

            detailCorbeille.Detail = "Ignoré";
            detailErrors.Detail = "Ignoré";
            detailFichiersTemp.Detail = "Ignoré";
            detailLogs.Detail = "Ignoré";
            detailWinUpdate.Detail = "Ignoré";
        }

        /// <summary>
        /// Recuperer la dernière version du logiciel
        /// </summary>
        public async void CHeckVersion()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = "https://www.anthony-cardinale.fr/_public/_dev/v2.txt";
                    string s = await client.GetStringAsync(url);
                    int lastVersion = int.Parse(s);
                    bool paramShowNotif = Preferences.Get("paramSearchMaj", true);
                    if (lastVersion > version && paramShowNotif)
                    {
                        ShowNotif();
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Afficher une notification à l'utilisateur
        /// </summary>
        public void ShowNotif()
        {
            var notif = new NotificationRequest
            {
                NotificationId = 1,
                Title = "Mise à jour disponible",
                Subtitle = "Téléchargez la dernière version de PC Cleaner",
                BadgeNumber = 1,
                Schedule = new NotificationRequestSchedule
                {
                    NotifyTime = DateTime.Now.AddSeconds(1),
                }
            };

            LocalNotificationCenter.Current.Show(notif);
        }



        //Liste des évènements "CheckBox modifiée"
        private void chkbFichiersTemp_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbFichiersTempChecked = e.Value;
            Preferences.Set("chkbFichierstempChecked", chkbFichiersTempChecked);
        }

        private void chkbCorbeille_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbCorbeilleChecked = e.Value;
            Preferences.Set("chkbCorbeilleChecked", chkbCorbeilleChecked);

        }

        private void chkbWinUpdate_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbWinUpdateChecked = e.Value;
            Preferences.Set("chkbWinUpdateChecked", chkbWinUpdateChecked);

        }

        private void chkbErrors_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbErrorsChecked = e.Value;
            Preferences.Set("chkbErrorsChecked", chkbErrorsChecked);

        }

        private void chkbLogs_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            chkbLogsChecked = e.Value;
            Preferences.Set("chkbLogsChecked", chkbLogsChecked);

        }

        private void chkbShaders_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

        }

        private void chkbWinOld_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

        }

        //Events de click sur le menu de gauche de page
        private async void ImageButtonOptions_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new OptionsPage());
        }

        private async void ImageButtonUpdate_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UpdatePage());
        }
        private async void ImageButtonOutils_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ToolsPage());
        }
        private async void ImageButtonRam_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RamPage());
        }
        private async void ImageButtonClean_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }
    }

}
