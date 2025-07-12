using System.ComponentModel;
using System.Runtime.CompilerServices;
using CalculatorTask.Models;

namespace CalculatorTask.ViewModels;

public class FractionTestViewModel : INotifyPropertyChanged
{
    private string _numeratorInput = string.Empty;
    private string _denominatorInput = string.Empty;
    private string _result = string.Empty;
    private string _wholePartText = string.Empty;

    public string NumeratorInput
    {
        get => _numeratorInput;
        set { _numeratorInput = value; OnPropertyChanged(); }
    }

    public string DenominatorInput
    {
        get => _denominatorInput;
        set { _denominatorInput = value; OnPropertyChanged(); }
    }

    public string Result
    {
        get => _result;
        set { _result = value; OnPropertyChanged(); }
    }

    public string WholePartText
    {
        get => _wholePartText;
        set { _wholePartText = value; OnPropertyChanged(); }
    }

    public void CreateFraction()
    {
        if (int.TryParse(NumeratorInput, out int num) &&
            int.TryParse(DenominatorInput, out int denom))
        {
            try
            {
                var fraction = new Fraction(num, denom);

                // שינוי כאן: תוצאה מוצגת עם פורמט מפורק
                Result = fraction.Formatted;

                WholePartText = fraction.WholePart != 0
                    ? $"המספר השלם: {fraction.WholePart}"
                    : string.Empty;
            }
            catch (Exception ex)
            {
                Result = $"שגיאה: {ex.Message}";
                WholePartText = string.Empty;
            }
        }
        else
        {
            Result = "קלט לא חוקי";
            WholePartText = string.Empty;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
