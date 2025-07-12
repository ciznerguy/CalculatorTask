using System;
using Microsoft.Maui.Controls;               // Entry, ContentPage, FocusEventArgs
using CalculatorTask.ViewModels;

namespace CalculatorTask.Views
{
    public partial class FractionCalculatorPage : ContentPage
    {
        // Explicit cast guarantees ViewModel is never null at runtime 
        private FractionCalculatorViewModel ViewModel
            => (FractionCalculatorViewModel)BindingContext;

        public FractionCalculatorPage()
        {
            InitializeComponent();
        }

        // When an Entry gains focus, direct digit-button input there
        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            if (sender is not Entry entry)
                return;

            if (entry == Num1Entry)
                ViewModel.TargetField = nameof(ViewModel.Num1);
            else if (entry == Den1Entry)
                ViewModel.TargetField = nameof(ViewModel.Den1);
            else if (entry == Num2Entry)
                ViewModel.TargetField = nameof(ViewModel.Num2);
            else if (entry == Den2Entry)
                ViewModel.TargetField = nameof(ViewModel.Den2);
        }
    }
}
