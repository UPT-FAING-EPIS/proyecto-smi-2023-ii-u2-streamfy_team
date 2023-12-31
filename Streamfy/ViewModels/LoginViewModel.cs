﻿using Firebase.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Streamfy.ViewModels
{
    internal class LoginViewModel : INotifyPropertyChanged
    {
        public string webApiKey = "AIzaSyAj9XyHXUn0KPsQVRYjZnxc_kHrrSdZH6Y";

        private INavigation _navigation;
        private string userName;
        private string userPassword;

        public event PropertyChangedEventHandler PropertyChanged;

        public Command RegisterBtn { get; }
        public Command LoginBtn { get; }

        public string UserName
        {
            get => userName; set
            {
                userName = value;
                RaisePropertyChanged("UserName");
            }
        }

        public string UserPassword
        {
            get => userPassword; set
            {
                userPassword = value;
                RaisePropertyChanged("UserPassword");
            }
        }

        public LoginViewModel(INavigation navigation)
        {
            this._navigation = navigation;
            RegisterBtn = new Command(RegisterBtnTappedAsync);
            LoginBtn = new Command(LoginBtnTappedAsync);
        }

        private async void LoginBtnTappedAsync(object obj)
        {
            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(webApiKey));

            try
            {
                var auth = await authProvider.SignInWithEmailAndPasswordAsync(UserName, UserPassword);
                var content = await auth.GetFreshAuthAsync();
                var serializedContent = JsonConvert.SerializeObject(content);
                Preferences.Set("FreshFirebaseToken", serializedContent);
                await this._navigation.PushAsync(new InicioPage());
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.Reason == AuthErrorReason.EmailExists)
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Ingrese correctamente sus credenciales", "OK");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Ingrese correctamente sus credenciales", "OK");
                }

                if (this._navigation != null)
                {
                    await this._navigation.PopAsync();
                }
            }


        }

        private async void RegisterBtnTappedAsync(object obj)
        {
            await this._navigation.PushAsync(new RegisterPage());
        }

        private void RaisePropertyChanged(string v)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(v));
        }
    }
}
