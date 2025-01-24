using Microsoft.VisualBasic;
using TutoPcCleaner.Helpers;
namespace TutoPcCleaner;

public partial class ToolsPage : ContentPage
{
    Sysinfos Sysinfos = new();
    public ToolsPage()
	{
		InitializeComponent();
        ShowSystemInfos();
    }




    public void ShowSystemInfos()
    {
        osVersion.Text = Sysinfos.GetWinVer();
        hardware.Text = Sysinfos.GetHardwareInfos();
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

    private void ButtonCreateRestauPoint_Clicked(object sender, EventArgs e)
    {
        dynamic restPoint = Interaction.GetObject("winmgmts:\\\\.\\root\\default:Systemrestore");
        if(restPoint != null)
        {
            if(restPoint.CreateRestorePoint("PC Cleaner restore point", 0, 100) == 0)
            {
                restaureTxt.Text = "Point de restauration créé !";
            }
            else
            {
                restaureTxt.Text = "Echec lors de la création du point de restauration ! ";
            }
        }
    }
    private void ButtonScan_Clicked(object sender, EventArgs e)
    {

    }
}