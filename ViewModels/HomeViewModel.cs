using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Firebase.Auth;

namespace IMP.ViewModels
{
    public class HomeViewModel : BindableObject
    {
        private readonly INavigation _navigation;
        private readonly string _userId; // Zmiana z userEmail na userId

        public ICommand NavigateToSectionsCommand { get; }

        public HomeViewModel(INavigation navigation, string userId)
        {
            _navigation = navigation;
            _userId = userId; // Przypisujemy userId
            NavigateToSectionsCommand = new Command(async () => await NavigateToSections());
        }

        private async Task NavigateToSections()
        {
            await _navigation.PushAsync(new SectionsPage(_userId)); // Przekazujemy userId do SectionsPage
        }
    }
}
