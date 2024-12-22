using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using System.Reflection;

namespace PKHeXMAUI;

public partial class propertyGrid : ContentView
{
    public BindableProperty CurrentItemProperty = BindableProperty.Create(nameof(CurrentItem), typeof(object), typeof(propertyGrid),propertyChanged: OnPChanged);
    public static Dictionary<string, List<PropertyInfo>>? Categories;
    public object CurrentItem { get=>GetValue(CurrentItemProperty); set=>SetValue(CurrentItemProperty,value); }
    public propertyGrid(object item)
    {
        InitializeComponent();
        Categories = [];
        CurrentItem = item;
        PGrid.Clear();
        var props = item.GetType().GetProperties();
        foreach (var prop in props)
        {
            CategoryAttribute att = prop.GetCustomAttribute<CategoryAttribute>() ?? new CategoryAttribute("Misc");
            if (Categories.TryGetValue(att.Category, out List<PropertyInfo>? value))
                value.Add(prop);
            else
                Categories.Add(att.Category,[prop]);
        }
        int p = 0;
        foreach (var cat in Categories)
        {
            Label label = new()
            {
                Text = cat.Key+ "▽"
            };
            Expander expander = new() { Header = label, Margin=20 };
            var stack = new Grid();
            for (int i = 0; i < cat.Value.Count; i++)
            {
                var pi = cat.Value[i];
                Label plabel = new() { Text = pi.Name, VerticalOptions = LayoutOptions.Center };
                stack.Add(plabel, 0, i);
                if (pi.PropertyType.IsEnum)
                {
                    comboBox cb = new()
                    {
                        ItemSource = Enum.GetValues(pi.PropertyType),
                        SelectedItem = pi?.GetValue(CurrentItem)??new()
                    };
                    cb.SelectedIndexChanged += (_, _) => pi?.SetValue(CurrentItem, cb.SelectedItem);
                    stack.Add(cb, 1, i);
                }
                else
                {
                    Entry pentry = new()
                    {
                        Text = pi.GetValue(CurrentItem)?.ToString()
                    };
                    stack.Add(pentry, 1, i);
                    pentry.TextChanged += (_, _) =>
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(pentry.Text)) return;
                            var value = Convert.ChangeType(pentry.Text, pi.PropertyType);
                            pi.SetValue(CurrentItem, value);
                        }
                        catch (Exception) { }
                    };
                }
            }
            expander.Content = stack;
            PGrid.RowDefinitions.Add(new() { Height = GridLength.Star });
            PGrid.Add(expander,row:p);
            p++;
        }
    }
    static void OnPChanged(BindableObject bindable,object oldValue, object newValue)
    {
        ((propertyGrid)bindable).PChanged();
    }
    public void PChanged()
    {
        Categories = [];
        PGrid.Clear();
        var props = CurrentItem.GetType().GetProperties();
        foreach (var prop in props)
        {
            CategoryAttribute att = prop.GetCustomAttribute<CategoryAttribute>() ?? new CategoryAttribute("Misc");
            if (Categories.TryGetValue(att.Category, out List<PropertyInfo>? value))
                value.Add(prop);
            else
                Categories.Add(att.Category, [prop]);
        }
        int p = 0;
        foreach (var cat in Categories)
        {
            Label label = new()
            {
                Text = cat.Key + "▽"
            };
            Expander expander = new() { Header = label, Margin = 20  };
            var stack = new Grid();
            for (int i = 0; i < cat.Value.Count; i++)
            {
                var pi = cat.Value[i];
                Label plabel = new() { Text = pi.Name, VerticalOptions = LayoutOptions.Center };
                stack.Add(plabel, 0, i);
                if (pi.PropertyType.IsEnum)
                {
                    comboBox cb = new()
                    {
                        ItemSource = Enum.GetValues(pi.PropertyType),
                        SelectedItem = pi?.GetValue(CurrentItem)??new()
                    };
                    cb.SelectedIndexChanged += (_, _) => pi?.SetValue(CurrentItem, cb.SelectedItem);
                    stack.Add(cb, 1, i);
                }
                else
                {
                    Entry pentry = new()
                    {
                        Text = pi.GetValue(CurrentItem)?.ToString()
                    };
                    stack.Add(pentry, 1, i);
                    pentry.TextChanged += (_, _) =>
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(pentry.Text)) return;
                            var value = Convert.ChangeType(pentry.Text, pi.PropertyType);
                            pi.SetValue(CurrentItem, value);
                        }
                        catch (Exception) { }
                    };
                }
            }
            expander.Content = stack;
            PGrid.RowDefinitions.Add(new() { Height = GridLength.Star });
            PGrid.Add(expander, row: p);
            p++;
        }
    }
}