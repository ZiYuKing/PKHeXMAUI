using System.Globalization;

namespace PKHeXMAUI;

public partial class NumericUpDown : ContentView
{
    public static BindableProperty MaxValueProperty = BindableProperty.Create(nameof(MaxValue), typeof(ulong), typeof(NumericUpDown), ulong.MaxValue);
	public static BindableProperty NumberProperty = BindableProperty.Create(nameof(Number), typeof(object), typeof(NumericUpDown), (ulong)0, propertyChanged: SetEntryNumber);
	public ulong Number { get => (ulong)GetValue(NumberProperty); set=>SetValue(NumberProperty,value); }
    public ulong MaxValue { get=>(ulong)GetValue(MaxValueProperty); set=>SetValue(MaxValueProperty,value); }
    public event EventHandler? ValueChanged;
	public NumericUpDown()
	{
		InitializeComponent();
        E_Number.SetBinding(Entry.TextProperty, new Binding("Number",BindingMode.TwoWay,new intConverter(), source: ThisView));
	}
	public static void SetEntryNumber(object bindable, object oldValue,object newValue)
	{
        if((ulong)newValue+1>((NumericUpDown)bindable).MaxValue)
            newValue = ((NumericUpDown)bindable).MaxValue;
        if ((ulong)newValue < 1)
            newValue = 0;
        ((NumericUpDown)bindable).E_Number.Text = newValue.ToString();
        ((NumericUpDown)bindable).ValueChanged?.Invoke(null, EventArgs.Empty);
	}

    private void Increase(object sender, EventArgs e)
    {
        if ((ulong)Number+1 > MaxValue)
            Number = MaxValue;
        else
            Number++;
    }

    private void Decrease(object sender, EventArgs e)
    {
        if (Number < 1)
            Number = 0;
        else
            Number--;
    }

    private void EnforceLimitations(object sender, TextChangedEventArgs e)
    {
        if ((ulong)Number + 1 > MaxValue)
            Number = MaxValue;
        if (Number < 1)
            Number = 0;
    }
}
public class intConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (ulong.TryParse(value?.ToString(), out ulong result))
            return result;
        return value;
    }
}