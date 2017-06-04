// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NXT.Models;
using NXTWebService.Models;

namespace NXT.Helpers
{
    public class Settings : INotifyPropertyChanged
    {
        private static Lazy<Settings> SettingsInstance = new Lazy<Settings>(() => new Settings());

        public static Settings Current => SettingsInstance.Value;

        private Settings()
        {
        }

        private static ISettings AppSettings
        {
            get { return CrossSettings.Current; }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (!string.IsNullOrWhiteSpace(propertyName))
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        bool SetProperty<T>(T value, T defaultValue = default(T), [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName)) return false;

            if (Equals(AppSettings.GetValueOrDefault<T>(propertyName, defaultValue), value)) return false;

            AppSettings.AddOrUpdateValue(propertyName, value);
            RaisePropertyChanged(propertyName);

            return true;
        }

        #endregion INotifyPropertyChanged

        T GetProperty<T>(T defaultValue = default(T), [CallerMemberName]string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return defaultValue;

            return AppSettings.GetValueOrDefault(propertyName, defaultValue);
        }

        public string UserName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public Guid UserGuid
        {
            get { return GetProperty<Guid>(); }
            set { SetProperty(value); }
        }

        public AuthType UserAuth
        {
            get { return GetProperty<AuthType>(); }
            set { SetProperty(value); }
        }

        public string SocialUserID
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public string UserFirstName
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public string UserEmail
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public byte[] Avatar
        {
            get { return GetProperty<byte[]>(); }
            set { SetProperty(value); }
        }

        public string AvatarUrl
        {
            get { return GetProperty<String>(); }
            set { SetProperty(value); }
        }

        public string Colour
        {
            get { return GetProperty<string>(); }
            set { SetProperty(value); }
        }

        public UserDto User
        {
            get
            {
                UserDto u = new UserDto()
                {
                    UserName = UserName,
                    ID = UserGuid,
                    Email = UserEmail,
                    AvatarUrl = AvatarUrl,
                    Colour = Colour,
                    //UserFirstName = value.UserName;
                };

                switch (UserAuth)
                {
                    case AuthType.Facebook:
                        u.AuthType = AuthType.Facebook;
                        u.SocialID = SocialUserID;
                        break;
                }
                return u;
            }
            set
            {
                UserName = value.UserName;
                UserGuid = value.ID;
                SocialUserID = value.SocialID;
                UserFirstName = value.UserName;
                UserEmail = value.Email;
                AvatarUrl = value.AvatarUrl;
                UserAuth = value.AuthType;
                Colour = value.Colour;
            }
        }

        public ObservableCollection<GroupDto> Groups { get; set; }
    }
}