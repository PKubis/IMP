using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using IMP.Services; // Importowanie serwisu
using Firebase.Database;

namespace IMP.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private string email;
        private string password;
        private string repeatPassword;
        private readonly RealtimeDatabaseService _realtimeDatabaseService;  // Referencja do serwisu

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand RegisterUser { get; }

        public string Email
        {
            get => email;
            set
            {
                email = value;
                RaisePropertyChanged(nameof(Email));
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                RaisePropertyChanged(nameof(Password));
            }
        }

        public string RepeatPassword
        {
            get => repeatPassword;
            set
            {
                repeatPassword = value;
                RaisePropertyChanged(nameof(RepeatPassword));
            }
        }

        public RegisterViewModel()
        {
            _realtimeDatabaseService = new RealtimeDatabaseService();  // Inicjalizacja serwisu
            RegisterUser = new Command(RegisterUserTappedAsync);
        }

        private async void RegisterUserTappedAsync(object obj)
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(RepeatPassword))
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Please enter email and both password fields.", "OK");
                return;
            }

            if (Password != RepeatPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Passwords do not match.", "OK");
                return;
            }

            try
            {
                // Firebase Authentication - Rejestracja użytkownika
                var authProvider = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDNtwI02aWPPvuGGK22Hm8LskD6soyIpZY"));
                var auth = await authProvider.CreateUserWithEmailAndPasswordAsync(Email, Password);

                string userId = auth.User.LocalId;
                string email = auth.User.Email;

                // Zapisanie użytkownika do Realtime Database za pomocą serwisu
                await _realtimeDatabaseService.AddUserAsync(userId, email);  // Dodaj użytkownika do Realtime Database

                // Przekierowanie na stronę główną po udanej rejestracji
                await Application.Current.MainPage.Navigation.PushAsync(new HomePage(userId));  // Używamy userId, aby przekazać go do HomePage

                await Application.Current.MainPage.DisplayAlert("Alert", "User Registered successfully", "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
