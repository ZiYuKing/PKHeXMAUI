using System.Globalization;

namespace PKHeXMAUI;

public partial class NumericUpDown : ContentView
{
	public static BindableProperty NumberProperty = BindableProperty.Create(nameof(Number), typeof(object), typeof(NumericUpDown), 0, propertyChanged: SetEntryNumber);
	public int Number { get => (int)GetValue(NumberProperty); set=>SetValue(NumberProperty,value); }
    public event EventHandler ValueChanged;
	public NumericUpDown()
	{
		InitializeComponent();
        E_Number.SetBinding(Entry.TextProperty, new Binding("Number",BindingMode.TwoWay, source: ThisView));
	}
	public static void SetEntryNumber(object bindable, object oldValue,object newValue)
	{
		((NumericUpDown)bindable).E_Number.Text = newValue.ToString();
        ((NumericUpDown)bindable).ValueChanged?.Invoke(null, EventArgs.Empty);
	}

    private void Increase(object sender, EventArgs e)
    {
        if (Number+1 > int.MaxValue)
            Number = int.MaxValue;
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
}
public class intConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value?.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (int.TryParse(value?.ToString(), out int result))
            return result;
        return value;
    }
}