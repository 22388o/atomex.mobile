﻿using System;
using System.Threading.Tasks;
using atomex.ViewModel;
using Xamarin.Forms;

namespace atomex
{
    public partial class AddressesPage : ContentPage
    {
        Color selectedTxBackgroundColor;

        public AddressesPage(AddressesViewModel addressesViewModel)
        {
            InitializeComponent();
            BindingContext = addressesViewModel;

            string selectedColorName = "ListViewSelectedBackgroundColor";

            if (Application.Current.RequestedTheme == OSAppTheme.Dark)
                selectedColorName = "ListViewSelectedBackgroundColorDark";

            Application.Current.Resources.TryGetValue(selectedColorName, out var selectedColor);
            selectedTxBackgroundColor = (Color)selectedColor;
        }

        private async void OnItemTapped(object sender, EventArgs args)
        {
            Grid selectedTx = (Grid)sender;
            selectedTx.IsEnabled = false;
            Color initColor = selectedTx.BackgroundColor;
            selectedTx.BackgroundColor = selectedTxBackgroundColor;

            await Task.Delay(500);

            selectedTx.BackgroundColor = initColor;
            selectedTx.IsEnabled = true;
        }
    }
}
