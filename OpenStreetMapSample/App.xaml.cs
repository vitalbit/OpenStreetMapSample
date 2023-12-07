namespace OpenStreetMapSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            if (DeviceInfo.Idiom == DeviceIdiom.Phone)
                MainPage = new NavigationPage(new MainPage());
            else
                MainPage = new MainPageLarge();
        }
    }
}
