namespace CalculatorTask.Models;

public class FractionOperation
{
    private readonly string _name;
    private readonly Func<Fraction, Fraction, Fraction> _operation;
    private Fraction? _fraction1; // Marked as nullable to resolve CS8618
    private Fraction? _fraction2; // Marked as nullable to resolve CS8618

    public FractionOperation(string name, Func<Fraction, Fraction, Fraction> operation)
    {
        _name = name;
        _operation = operation;
    }

    public void SetFractions(Fraction f1, Fraction f2)
    {
        _fraction1 = f1;
        _fraction2 = f2;
    }

    public Fraction Execute()
    {
        if (_fraction1 == null || _fraction2 == null)
            throw new InvalidOperationException("שני שברים חייבים להיות מוזנים לפני ביצוע הפעולה.");

        return _operation(_fraction1, _fraction2);
    }

    public override string ToString() => _name;
}
