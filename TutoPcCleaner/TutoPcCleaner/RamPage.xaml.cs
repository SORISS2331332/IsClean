using System.Management;
using TutoPcCleaner.Helpers;
using Microsoft.Win32;
namespace TutoPcCleaner;

public partial class RamPage : ContentPage
{
    Sysinfos Sysinfos = new();
    public RamPage()
	{

		InitializeComponent();
        ShowSystemInfos();
        GetRamUsage();
    }




    public void ShowSystemInfos()
    {
        osVersion.Text = Sysinfos.GetWinVer();
        hardware.Text = Sysinfos.GetHardwareInfos();
    }
    public async void GetRamUsage()
    {
        try
        {
            ManagementObjectSearcher ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory " +
                "FROM Win32_OperatingSystem");
            ulong totalRam = 0;
            ulong frram = 0;
            foreach (ManagementObject objram in ramMonitor.Get())
            {
                totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]);
                frram = Convert.ToUInt64(objram["FreePhysicalMemory"]);
            }

            int fram2 = Convert.ToInt32(frram);
            int fram3 = Convert.ToInt32(totalRam);

            string fram4 = Convert.ToString(fram2);
            string fram5 = Convert.ToString(fram3);

            double fram6 = Convert.ToDouble(fram4);
            double fram7 = Convert.ToDouble(fram5);

            double percent = fram6 / fram7 * 100;
            int per2 = (int)Math.Round(percent);
            ramUsageTxt.Text = 100 - per2 + "%";

            graph.Progress = 1 - (percent / 100);

            totalRam = (totalRam / 1000000);
            frram = frram / 1000000;
            cellTotal.Detail = totalRam + " GB";
            cellFree.Detail = frram + " GB. ("+ per2 + "%).";
            cellUsed.Detail = (totalRam - frram) + " GB. ("+ (100 - per2) + "%).";
        }
        catch (Exception ex) { }
    }
    public async void OptimizeRam()
    {
        try
        {
            GC.Collect(1, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();
        }
        catch (Exception ex) { }

        await Task.Delay(TimeSpan.FromSeconds(2));
        graph.IsIndeterminate = false;
        ramCleaned.IsVisible = true;
        GetRamUsage();
    }

    private void ButtonRamCleaned_Clicked(object sender, EventArgs e)
    {
        graph.IsIndeterminate = true;
        OptimizeRam();
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