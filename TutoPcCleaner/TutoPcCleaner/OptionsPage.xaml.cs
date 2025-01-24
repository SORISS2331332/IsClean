using System.Windows.Input;
using TutoPcCleaner.Helpers;

namespace TutoPcCleaner
{
    public partial class OptionsPage : ContentPage
    {
        Sysinfos Sysinfos = new();
        public ICommand TapCommand => new Command<string>(async (url) => await Launcher.OpenAsync(url));
        public OptionsPage()
        {
            InitializeComponent();
            ShowSystemInfos();
            BindingContext = this;
            paramSearchMaj.IsChecked = Preferences.Get("paramSearchMaj", true);
        }




        public void ShowSystemInfos()
        {
            osVersion.Text = Sysinfos.GetWinVer();
            hardware.Text = Sysinfos.GetHardwareInfos();
        }

        private void paramSearchMaj_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            Preferences.Set("paramSearchMaj", e.Value);
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

