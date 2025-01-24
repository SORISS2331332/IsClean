using Plugin.LocalNotification;
using System.Windows.Input;
using TutoPcCleaner.Helpers;
namespace TutoPcCleaner;

public partial class UpdatePage : ContentPage
{
    Sysinfos Sysinfos = new();
    public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
    int version = 1;
    public UpdatePage()
	{
		InitializeComponent();
        ShowSystemInfos();
        BindingContext = this;
        CHeckVersion();
    }




    public void ShowSystemInfos()
    {
        osVersion.Text = Sysinfos.GetWinVer();
        hardware.Text = Sysinfos.GetHardwareInfos();
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
                loadingGraph.IsVisible = false;
                loadingText.IsVisible = false;

                if (lastVersion > version)
                {
                    ShowUpdatePage();
                }
                else
                {
                    ShowDefaultPage();
                }
            }
        }
        catch (Exception ex)
        {

        }
    }

    public void ShowUpdatePage()
    {
        updateTitle.IsVisible = true;
        updateSub.IsVisible = true;
        updateVersion.IsVisible = true;
        updateLink.IsVisible = true;
    }

    public void ShowDefaultPage()
    {
        defaultTitle.IsVisible = true;
        defaultSub.IsVisible = true;
        defaultVersion.IsVisible = true;
        defaultLink.IsVisible = true;
    }


    private async void ButtonUpdateSoft_Clicked(object sender, EventArgs e)
    {
        try
        {
            Uri uri = new Uri("http://anthony-cardinale.fr/tools/pccleaner?from=pccleaner&update=true");
            await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
        }
        catch (Exception ex)
        {

        }
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