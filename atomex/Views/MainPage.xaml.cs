﻿using System.ComponentModel;
using Xamarin.Forms;
using atomex.ViewModel;
using atomex.CustomElements;
using Atomex;
using System;

namespace atomex
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : CustomTabbedPage, INavigationService
    {

        private readonly NavigationPage navigationConversionPage;

        private MainViewModel _mainViewModel;

        public MainPage()
        {

            _mainViewModel = new MainViewModel();
            IAtomexApp AtomexApp = _mainViewModel.GetAtomexApp();
            var navigationWalletsListPage = new NavigationPage(new CurrenciesListPage(AtomexApp, _mainViewModel.CurrenciesViewModel, this));
            navigationWalletsListPage.IconImageSource = "NavBar__wallets";
            navigationWalletsListPage.Title = "Wallets";

            var navigationPortfolio = new NavigationPage(new Portfolio(_mainViewModel.CurrenciesViewModel));
            navigationPortfolio.IconImageSource = "NavBar__portfolio";
            navigationPortfolio.Title = "Portfolio";

            //navigationConversionPage = new NavigationPage(new ConversionPage(AtomexApp, _mainViewModel.ConversionViewModel));
            navigationConversionPage = new NavigationPage(new ConversionsListPage(AtomexApp, _mainViewModel.CurrenciesViewModel.Currencies[3], _mainViewModel.ConversionViewModel));
            navigationConversionPage.IconImageSource = "NavBar__conversion";
            navigationConversionPage.Title = "Conversion";

            var navigationSettingsPage = new NavigationPage(new SettingsPage(_mainViewModel.SettingsViewModel));
            navigationSettingsPage.IconImageSource = "NavBar__settings";
            navigationSettingsPage.Title = "Settings";

            Children.Add(navigationPortfolio);
            Children.Add(navigationWalletsListPage);
            Children.Add(navigationConversionPage);
            Children.Add(navigationSettingsPage);

        }

        public void ShowConversionPage(IAtomexApp app, CurrencyViewModel currency = null)
        {
            this.CurrentPage = navigationConversionPage;

            var conversionViewModel = navigationConversionPage.RootPage.BindingContext as ConversionViewModel;
            Console.WriteLine(conversionViewModel);
            if (conversionViewModel != null)
            {
                navigationConversionPage.PushAsync(new ConversionFirstStepPage(app, conversionViewModel));

                Console.WriteLine(currency);
                Console.WriteLine(conversionViewModel);
                Console.WriteLine(conversionViewModel.FromCurrency);
                conversionViewModel.FromCurrency = currency;
            }
        }
    }
}
