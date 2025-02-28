﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using atomex.Views.BuyCurrency;
using Atomex;
using Atomex.Common;
using Atomex.Core;
using Atomex.Cryptography;
using Atomex.Wallet.Abstract;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace atomex.ViewModel
{
    public class BuyViewModel : BaseViewModel
    {
        private IAtomexApp AtomexApp { get; }

        public INavigation Navigation { get; set; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(nameof(IsLoading)); }
        }

        private Network _network;
        public Network Network
        {
            get => _network;
            set
            {
                _network = value;
                OnPropertyChanged(nameof(Network));
            }
        }

        private string _url;
        public string Url
        {
            get => _url;
            set { _url = value; OnPropertyChanged(nameof(Url)); }
        }

        private string _userId;

        private string[] _redirectedUrls = { "https://widget.wert.io/terms-and-conditions",
                                             "https://widget.wert.io/privacy-policy",
                                             "https://support.wert.io/",
                                             "https://sandbox.wert.io/terms-and-conditions",
                                             "https://sandbox.wert.io/privacy-policy" };

        public ObservableCollection<string> Currencies { get; } = new ObservableCollection<string> { "BTC", "ETH", "XTZ" };

        public BuyViewModel(IAtomexApp app)
        {
            AtomexApp = app ?? throw new ArgumentNullException(nameof(AtomexApp));
            IsLoading = true;
            _userId = GetUserId();
            Network = app.Account.Network;
        }


        private ICommand _selectCurrencyCommand;
        public ICommand SelectCurrencyCommand => _selectCurrencyCommand ??= new Command<string>(async (value) => await LoadWebView(value));

        private async Task LoadWebView(string currency)
        {
            string appTheme = Application.Current.RequestedTheme.ToString().ToLower();
            string address = GetDefaultAddress(currency);

            var baseUri = _network == Network.MainNet
                ? "https://widget.wert.io/atomex"
                : "https://sandbox.wert.io/01F298K3HP4DY326AH1NS3MM3M";

            Url = $"{baseUri}/widget" +
                $"?commodity={currency}" +
                $"&address={address}" +
                $"&click_id=user:{_userId}/network:{_network}" +
                $"&theme={appTheme}";

            OnPropertyChanged(nameof(Url));

            await Navigation.PushAsync(new BuyPage(this));
        }

        private ICommand _canExecuteCommand;
        public ICommand CanExecuteCommand => _canExecuteCommand ??= new Command<WebNavigatingEventArgs>((args) => CanExecute(args));

        private void CanExecute(WebNavigatingEventArgs args)
        {
            IsLoading = false;

            if (_redirectedUrls.Any(u => args.Url.StartsWith(u)))
            {
                Launcher.OpenAsync(new Uri(args.Url));
                args.Cancel = true;
            }
        }

        private string GetUserId()
        {
            using var servicePublicKey = AtomexApp.Account.Wallet.GetServicePublicKey(AtomexApp.Account.UserSettings.AuthenticationKeyIndex);
            using var publicKey = servicePublicKey.ToUnsecuredBytes();
            return Sha256.Compute(Sha256.Compute(publicKey)).ToHexString();
        }

        public string GetDefaultAddress(string currency)
        {
            if (currency == "ETH" || currency == "XTZ")
            {
                var account = AtomexApp.Account.GetCurrencyAccount<ILegacyCurrencyAccount>(currency);

                var activeTokenAddresses = account
                    .GetUnspentTokenAddressesAsync()
                    .WaitForResult()
                    .ToList();

                var activeAddresses = AtomexApp.Account
                    .GetUnspentAddressesAsync(currency)
                    .WaitForResult()
                    .ToList();

                activeTokenAddresses
                    .ForEach(a => a.Balance = activeAddresses
                    .Find(b => b.Address == a.Address)?.Balance ?? 0m);

                activeAddresses = activeAddresses
                    .Where(a => activeTokenAddresses
                    .FirstOrDefault(b => b.Address == a.Address) == null)
                    .ToList();

                var receiveAddress = activeTokenAddresses
                    .Concat(activeAddresses)
                    .OrderByDescending(w => w.AvailableBalance())
                    .FirstOrDefault(w => w.HasActivity);

                if (receiveAddress != null)
                    return receiveAddress.Address;
            }

            return AtomexApp.Account
                .GetFreeExternalAddressAsync(currency)
                .WaitForResult()
                .Address;
        }
    }
}

