using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;                   // for Colors
using CalculatorTask.ViewModels;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

namespace CalculatorTask.Views
{
    [SuppressMessage("Interoperability", "CA1422:Validate platform compatibility",
                     Justification = "We only ship to supported Windows versions")]
    public class SamePageWithCodeBehind : ContentPage
    {
        private readonly Entry _num1Entry, _den1Entry, _num2Entry, _den2Entry;

        [SupportedOSPlatform("windows10.0.17763.0")]
        public SamePageWithCodeBehind()
        {
            // 1. Always set the ViewModel so SelectedOperation is initialized
            BindingContext = new FractionCalculatorViewModel();

            // 2. Check Windows version—but do NOT throw; just skip any Windows-only features
            if (!OperatingSystem.IsWindowsVersionAtLeast(10, 0, 17763))
            {
                // On non-Windows or older Windows, we simply continue;
                // Windows-specific APIs are only used where guarded by SupportedOSPlatform.
            }

            Title = "מחשבון שברים מעורבים";
            BackgroundColor = Colors.Black;

            // 3. Header label
            var header = new Label
            {
                Text = Title,
                FontSize = 22,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center
            };

            // 4. Create and bind Entries
            _num1Entry = CreateEntry(nameof(FractionCalculatorViewModel.Num1));
            _den1Entry = CreateEntry(nameof(FractionCalculatorViewModel.Den1));
            _num2Entry = CreateEntry(nameof(FractionCalculatorViewModel.Num2));
            _den2Entry = CreateEntry(nameof(FractionCalculatorViewModel.Den2));

            // Move focus automatically
            _num1Entry.Completed += (_, _) => _den1Entry.Focus();
            _den1Entry.Completed += (_, _) => _num2Entry.Focus();
            _num2Entry.Completed += (_, _) => _den2Entry.Focus();

            // 5. Build fraction inputs + picker
            var frac1 = CreateFractionGrid(_num1Entry, _den1Entry);
            var picker = new Picker
            {
                Title = "בחר פעולה",
                WidthRequest = 100,
                BackgroundColor = Color.FromArgb("#2D2D2D"),
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center
            };
            picker.SetBinding(Picker.ItemsSourceProperty, "Operations");
            picker.SetBinding(Picker.SelectedItemProperty, "SelectedOperation");
            var frac2 = CreateFractionGrid(_num2Entry, _den2Entry);

            var fractionsRow = new Grid
            {
                ColumnSpacing = 30,
                ColumnDefinitions =
                {
                    new ColumnDefinition(), new ColumnDefinition(), new ColumnDefinition()
                },
                HorizontalOptions = LayoutOptions.Center
            };
            fractionsRow.Add(frac1, 0, 0);
            fractionsRow.Add(picker, 1, 0);
            fractionsRow.Add(frac2, 2, 0);

            // 6. Result display
            var resultTitle = new Label
            {
                Text = "תוצאה:",
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center
            };

            var resultWhole = new Label
            {
                FontSize = 40,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };
            resultWhole.SetBinding(Label.TextProperty, "ResultWhole");
            resultWhole.SetBinding(IsVisibleProperty, "IsResultVisible");

            var resultNumerator = new Label
            {
                FontSize = 24,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };
            resultNumerator.SetBinding(Label.TextProperty, "ResultNumerator");

            var resultDenominator = new Label
            {
                FontSize = 24,
                TextColor = Colors.White,
                HorizontalTextAlignment = TextAlignment.Center
            };
            resultDenominator.SetBinding(Label.TextProperty, "ResultDenominator");

            var fracResult = new Grid
            {
                RowSpacing = 3,
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition { Height = 1 },
                    new RowDefinition()
                }
            };
            fracResult.Add(resultNumerator, 0, 0);
            fracResult.Add(new BoxView { Color = Colors.White }, 0, 1);
            fracResult.Add(resultDenominator, 0, 2);
            fracResult.SetBinding(IsVisibleProperty, "IsResultVisible");

            var resultGrid = new Grid
            {
                ColumnDefinitions = { new ColumnDefinition(), new ColumnDefinition() },
                ColumnSpacing = 10,
                HorizontalOptions = LayoutOptions.Center
            };
            resultGrid.Add(resultWhole, 0, 0);
            resultGrid.Add(fracResult, 1, 0);

            var errorLabel = new Label
            {
                FontSize = 18,
                TextColor = Colors.OrangeRed,
                HorizontalTextAlignment = TextAlignment.Center
            };
            errorLabel.SetBinding(Label.TextProperty, "ResultError");
            errorLabel.SetBinding(IsVisibleProperty, "IsErrorVisible");
            resultGrid.Add(errorLabel, 0, 1);
            Grid.SetColumnSpan(errorLabel, 2);

            // 7. Action buttons
            var clearBtn = CreateButton("C", "ClearCommand", "#D9534F");
            var calcBtn = CreateButton("חשב", "CalculateCommand", "#F0AD4E");
            var actions = new HorizontalStackLayout
            {
                HorizontalOptions = LayoutOptions.Center,
                Spacing = 20,
                Children = { clearBtn, calcBtn }
            };

            // 8. Keypad
            var keypad = CreateKeypad();

            // 9. Assemble the page
            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 20,
                    Spacing = 25,
                    Children =
                    {
                        header,
                        fractionsRow,
                        resultTitle,
                        resultGrid,
                        actions,
                        keypad
                    }
                }
            };
        }

        // Creates a bound Entry (text only)
        private static Entry CreateEntry(string propertyName)
        {
            var entry = new Entry
            {
                BackgroundColor = Color.FromArgb("#1E1E1E"),
                TextColor = Colors.White,
                PlaceholderColor = Colors.Gray,
                WidthRequest = 60,
                HorizontalTextAlignment = TextAlignment.Center
            };
            entry.SetBinding(Entry.TextProperty, propertyName);
            return entry;
        }

        // Fraction layout: numerator / line / denominator
        [SupportedOSPlatform("windows10.0.17763.0")]
        private static Grid CreateFractionGrid(Entry numerator, Entry denominator)
        {
            var grid = new Grid
            {
                RowSpacing = 3,
                RowDefinitions =
                {
                    new RowDefinition(),
                    new RowDefinition { Height = 1 },
                    new RowDefinition()
                }
            };
            grid.Add(numerator, 0, 0);
            grid.Add(new BoxView { Color = Colors.White }, 0, 1);
            grid.Add(denominator, 0, 2);
            return grid;
        }

        // Button helper
        private static Button CreateButton(string text, string command, string color)
        {
            var btn = new Button
            {
                Text = text,
                BackgroundColor = Color.FromArgb(color),
                TextColor = Colors.White,
                WidthRequest = 130,
                HeightRequest = 65,
                CornerRadius = 32
            };
            btn.SetBinding(Button.CommandProperty, command);
            return btn;
        }

        // On-screen numeric keypad
        private Grid CreateKeypad()
        {
            var grid = new Grid
            {
                RowSpacing = 10,
                ColumnSpacing = 10,
                Padding = 10,
                HorizontalOptions = LayoutOptions.Center,
                ColumnDefinitions =
                {
                    new ColumnDefinition(), new ColumnDefinition(), new ColumnDefinition()
                },
                RowDefinitions =
                {
                    new RowDefinition(), new RowDefinition(),
                    new RowDefinition(), new RowDefinition()
                }
            };

            string[] digits = { "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            for (int i = 0; i < digits.Length; i++)
                grid.Add(CreateDigitButton(digits[i]), i % 3, i / 3);

            var zero = CreateDigitButton("0");
            grid.Add(zero, 0, 3);
            Grid.SetColumnSpan(zero, 3);

            return grid;
        }

        // Digit-button helper
        private Button CreateDigitButton(string digit)
        {
            var btn = new Button
            {
                Text = digit,
                FontSize = 22,
                WidthRequest = 91,
                HeightRequest = 91,
                CornerRadius = 45,
                BackgroundColor = Color.FromArgb("#2D2D2D"),
                TextColor = Colors.White,
                CommandParameter = digit
            };
            btn.SetBinding(Button.CommandProperty, "DigitCommand");
            return btn;
        }
    }
}
