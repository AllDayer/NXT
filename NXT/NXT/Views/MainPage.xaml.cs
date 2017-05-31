using Prism.Navigation;
using Xamarin.Forms;

namespace NXT.Views
{
    public partial class MainPage : MasterDetailPage, IMasterDetailPageOptions
    {
        public MainPage()
        {
            InitializeComponent();
        }
        public bool IsPresentedAfterNavigation => Device.Idiom != TargetIdiom.Phone;

    }
}
