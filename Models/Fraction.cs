namespace CalculatorTask.Models;

public class Fraction
{
    private int _numerator;
    private int _denominator;
    private int _wholePart;

    public int Numerator => _numerator;
    public int Denominator => _denominator;
    public int WholePart => _wholePart;

    public Fraction(int numerator, int denominator)
    {
        // בדיקה אם המכנה הוא אפס, כי חלוקה באפס אינה מוגדרת
        if (denominator == 0)
            throw new ArgumentException("מכנה לא יכול להיות אפס.");

        // שמירת הערכים של המונה והמכנה
        _numerator = numerator;
        _denominator = denominator;

        // פישוט השבר לגורם המשותף הגדול ביותר
        Simplify();

        // עדכון החלק השלם של השבר (אם קיים)
        UpdateWholePart();
    }

    public Fraction(int wholeNumber) : this(wholeNumber, 1) { }

    private void Simplify()
    {
        int gcd = GCD(Math.Abs(_numerator), Math.Abs(_denominator));
        _numerator /= gcd;
        _denominator /= gcd;

        if (_denominator < 0)
        {
            _numerator *= -1;
            _denominator *= -1;
        }
    }

    private void UpdateWholePart()
    {
        _wholePart = _numerator / _denominator;
    }

    public string Formatted
    {
        get
        {
            int whole = _numerator / _denominator;
            int remainder = Math.Abs(_numerator % _denominator);

            if (remainder == 0)
                return $"{whole}";
            if (whole != 0)
                return $"{whole} {remainder}/{_denominator}";
            return $"{_numerator}/{_denominator}";
        }
    }

    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    public override string ToString()
    {
        return $"{_numerator}/{_denominator}";
    }
}
