using System;                                // מרחב שמות בסיסי של .NET
using System.Collections.Generic;            // עבור List<T>
using System.Collections.ObjectModel;        // עבור ObservableCollection<T>
using System.ComponentModel;                 // עבור INotifyPropertyChanged
using System.Runtime.CompilerServices;       // תומך ב-[CallerMemberName]
using System.Windows.Input;                  // ממשק ICommand
using Microsoft.Maui.Controls;               // רכיבי MAUI כמו Command
using CalculatorTask.Models;                 // כולל את המחלקות Fraction ו-FractionOperation

namespace CalculatorTask.ViewModels
{
    public class FractionCalculatorViewModel : INotifyPropertyChanged
    {
        // --- שדות קלט לשבר הראשון והשני ---
        public string Whole1 { get; set; } = string.Empty; // שלם ראשון
        public string Num1 { get; set; } = string.Empty;   // מונה ראשון
        public string Den1 { get; set; } = string.Empty;   // מכנה ראשון
        public string Whole2 { get; set; } = string.Empty; // שלם שני
        public string Num2 { get; set; } = string.Empty;   // מונה שני
        public string Den2 { get; set; } = string.Empty;   // מכנה שני

        // --- שדות תוצאה ---
        private string _resultWhole = string.Empty;
        public string ResultWhole
        {
            get => _resultWhole;
            set { _resultWhole = value; OnPropertyChanged(); } // שלם בתוצאה
        }

        private string _resultNumerator = string.Empty;
        public string ResultNumerator
        {
            get => _resultNumerator;
            set { _resultNumerator = value; OnPropertyChanged(); } // מונה בתוצאה
        }

        private string _resultDenominator = string.Empty;
        public string ResultDenominator
        {
            get => _resultDenominator;
            set { _resultDenominator = value; OnPropertyChanged(); } // מכנה בתוצאה
        }

        // --- שדות שגיאה ---
        private string _resultError = string.Empty;
        public string ResultError
        {
            get => _resultError;
            set { _resultError = value; OnPropertyChanged(); } // טקסט שגיאה
        }

        // שליטה על תצוגה של תוצאה או שגיאה
        private bool _isResultVisible;
        public bool IsResultVisible
        {
            get => _isResultVisible;
            set { _isResultVisible = value; OnPropertyChanged(); }
        }

        private bool _isErrorVisible;
        public bool IsErrorVisible
        {
            get => _isErrorVisible;
            set { _isErrorVisible = value; OnPropertyChanged(); }
        }

        // --- פעולות אפשריות ---
        public ObservableCollection<FractionOperation> Operations { get; } =
            new ObservableCollection<FractionOperation>
        {
            new FractionOperation("חיבור", (a, b) => new Fraction(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator)),
            new FractionOperation("חיסור", (a, b) => new Fraction(a.Numerator * b.Denominator - b.Numerator * a.Denominator, a.Denominator * b.Denominator)),
            new FractionOperation("כפל",   (a, b) => new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator)),
            new FractionOperation("חילוק", (a, b) => new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator)),
        };

        public FractionOperation SelectedOperation { get; set; } // פעולה נבחרת

        // --- פקודות לכפתורים ---
        public ICommand CalculateCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand DigitCommand { get; }

        // שדה היעד הנוכחי לקלט מהמספרים
        private string _targetField = nameof(Num1);
        public string TargetField
        {
            get => _targetField;
            set { _targetField = value; OnPropertyChanged(); }
        }

        // --- בנאי: מקשר פקודות לפונקציות ---
        public FractionCalculatorViewModel()
        {
            CalculateCommand = new Command(Calculate);                  // פקודת חישוב
            ClearCommand     = new Command(Clear);                      // פקודת ניקוי
            DigitCommand     = new Command<string>(AppendDigit);        // הוספת ספרה
        }

        // --- חישוב ---
        private void Calculate()
        {
            try
            {
                if (SelectedOperation == null)
                {
                    ShowError("בחר פעולה לפני החישוב"); // פעולה לא נבחרה
                    return;
                }

                // בדיקת שדות חסרים
                var missing = new List<string>();
                if (string.IsNullOrWhiteSpace(Num1)) missing.Add("מונה ראשון");
                if (string.IsNullOrWhiteSpace(Den1)) missing.Add("מכנה ראשון");
                if (string.IsNullOrWhiteSpace(Num2)) missing.Add("מונה שני");
                if (string.IsNullOrWhiteSpace(Den2)) missing.Add("מכנה שני");

                if (missing.Count > 0)
                {
                    ShowError("שדות חסרים: " + string.Join(", ", missing));
                    return;
                }

                // בדיקת מכנים אפס
                if ((int.TryParse(Den1, out var d1) && d1 == 0) ||
                    (int.TryParse(Den2, out var d2) && d2 == 0))
                {
                    ShowError("מכנה לא יכול להיות אפס");
                    return;
                }

                // בניית שברים מהקלט
                var f1 = BuildFraction(Whole1, Num1, Den1);
                var f2 = BuildFraction(Whole2, Num2, Den2);

                // חישוב לפי פעולה נבחרת
                SelectedOperation.SetFractions(f1, f2);
                var result = SelectedOperation.Execute();

                // פיצול תוצאה לשלם ושבר
                ResultWhole      = result.WholePart != 0 ? result.WholePart.ToString() : string.Empty;
                var rem = result.Numerator % result.Denominator;
                ResultNumerator   = rem != 0 ? rem.ToString() : string.Empty;
                ResultDenominator = rem != 0 ? result.Denominator.ToString() : string.Empty;

                ShowResult(); // הצג תוצאה
            }
            catch
            {
                ShowError("שגיאה כללית בחישוב"); // חריגה כללית
            }
        }

        // --- בניית אובייקט Fraction ---
        private Fraction BuildFraction(string whole, string num, string den)
        {
            var w = int.TryParse(whole, out var wi) ? wi : 0;
            var n = int.TryParse(num, out var ni) ? ni : 0;
            var d = int.TryParse(den, out var di) ? di : 1;

            return new Fraction(w * d + n, d); // הפיכת שבר מעורב לשבר רגיל
        }

        // --- הוספת ספרה לשדה נבחר ---
        private void AppendDigit(string digit)
        {
            switch (TargetField)
            {
                case nameof(Num1): Num1 += digit; OnPropertyChanged(nameof(Num1)); break;
                case nameof(Den1): Den1 += digit; OnPropertyChanged(nameof(Den1)); break;
                case nameof(Num2): Num2 += digit; OnPropertyChanged(nameof(Num2)); break;
                case nameof(Den2): Den2 += digit; OnPropertyChanged(nameof(Den2)); break;
            }
        }

        // --- איפוס שדות ---
        private void Clear()
        {
            Whole1 = Num1 = Den1 =
            Whole2 = Num2 = Den2 =
            ResultWhole = ResultNumerator = ResultDenominator =
            ResultError = string.Empty;

            IsErrorVisible = false;
            IsResultVisible = false;

            OnPropertyChanged(string.Empty); // רענון כל הקשרים (bindings)
        }

        // --- הצגת שגיאה ---
        private void ShowError(string msg)
        {
            ResultError     = msg;
            IsErrorVisible  = true;
            IsResultVisible = false;
        }

        // --- הצגת תוצאה ---
        private void ShowResult()
        {
            ResultError     = string.Empty;
            IsErrorVisible  = false;
            IsResultVisible = true;
        }

        // --- מימוש INotifyPropertyChanged ---
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
