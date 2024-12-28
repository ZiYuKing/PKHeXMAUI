using CommunityToolkit.Maui.Views;
using System.ComponentModel;
using System.Reflection;

namespace PKHeXMAUI;

public partial class propertyGrid : ContentView
{
    public BindableProperty CurrentItemProperty = BindableProperty.Create(nameof(CurrentItem), typeof(object), typeof(propertyGrid),propertyChanged: OnPChanged);
    public static Dictionary<string, List<PropertyInfo>>? Categories;
    public object CurrentItem { get=>GetValue(CurrentItemProperty); set=>SetValue(CurrentItemProperty,value); }
    private bool init = true;
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
                    stack.Add(GetEnumCombo(pi), 1, i);
                }
                else if (pi.PropertyType.IsClass)
                {
                    Label L_intern = new() { Text = pi.PropertyType.ToString()+ "▽" };
                    Expander E_intern = new() { Header = L_intern, Margin = 20 };
                    Grid G_stack = [];
                    int o = 0;
                    
                    foreach (var pr in pi.PropertyType.GetProperties())
                    {
                        try
                        {
                            if (pi.PropertyType.GetCustomAttribute<TypeConverterAttribute>() == null) { L_intern.Text = pi.PropertyType.ToString(); break; };
                            Label L_prop = new() { Text = pr.Name };
                            G_stack.Add(L_prop, 0, o);
                            if (pr.PropertyType.IsEnum)
                            {
                               
                                G_stack.Add(GetClassEnumCombo(pi,pr), 1, o);
                            }
                            else
                            {
                               
                                G_stack.Add(GetClassPropertyEntry(pi,pr), 1, o);
                               
                            }
                            o++;
                        }
                        catch (Exception) { continue; }
                    }
                    E_intern.Content = G_stack;
                    stack.Add(E_intern, 1, i);
                }
                else
                {
                   
                    stack.Add(GetPropertyEntry(pi), 1, i);
                    
                }
            }
            expander.Content = stack;
            PGrid.RowDefinitions.Add(new() { Height = GridLength.Star });
            PGrid.Add(expander,row:p);
            p++;
        }
        init = false;
    }
    static void OnPChanged(BindableObject bindable,object oldValue, object newValue)
    {
        ((propertyGrid)bindable).PChanged();
    }
    public void PChanged()
    {
        if (init) return;
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
            Expander expander = new() { Header = label, Margin = 20 };
            var stack = new Grid();
            for (int i = 0; i < cat.Value.Count; i++)
            {
                var pi = cat.Value[i];
                Label plabel = new() { Text = pi.Name, VerticalOptions = LayoutOptions.Center };
                stack.Add(plabel, 0, i);
                if (pi.PropertyType.IsEnum)
                {
                    stack.Add(GetEnumCombo(pi), 1, i);
                }
                else if (pi.PropertyType.IsClass)
                {
                    Label L_intern = new() { Text = pi.PropertyType.ToString() + "▽" };
                    Expander E_intern = new() { Header = L_intern, Margin = 20 };
                    Grid G_stack = [];
                    int o = 0;

                    foreach (var pr in pi.PropertyType.GetProperties())
                    {
                        try
                        {
                            if (pi.PropertyType.GetCustomAttribute<TypeConverterAttribute>() == null) { L_intern.Text = pi.PropertyType.ToString(); break; };
                            Label L_prop = new() { Text = pr.Name };
                            G_stack.Add(L_prop, 0, o);
                            if (pr.PropertyType.IsEnum)
                            {

                                G_stack.Add(GetClassEnumCombo(pi, pr), 1, o);
                            }
                            else
                            {

                                G_stack.Add(GetClassPropertyEntry(pi, pr), 1, o);

                            }
                            o++;
                        }
                        catch (Exception) { continue; }
                    }
                    E_intern.Content = G_stack;
                    stack.Add(E_intern, 1, i);
                }
                else
                {

                    stack.Add(GetPropertyEntry(pi), 1, i);

                }
            }
            expander.Content = stack;
            PGrid.RowDefinitions.Add(new() { Height = GridLength.Star });
            PGrid.Add(expander, row: p);
            p++;
        }
    }
    public comboBox GetEnumCombo(PropertyInfo CurrentProperty)
    {
        comboBox cb = new()
        {
            ItemSource = Enum.GetValues(CurrentProperty.PropertyType),
            SelectedItem = CurrentProperty?.GetValue(CurrentItem) ?? new()
        };
        cb.SelectedIndexChanged += (_, _) => { try { CurrentProperty?.SetValue(CurrentItem, cb.SelectedItem); } catch (Exception) { } };
        return cb;
    }
    public comboBox GetClassEnumCombo(PropertyInfo upperProperty, PropertyInfo LowerProperty)
    {
        comboBox cb = new()
        {
            ItemSource = Enum.GetValues(LowerProperty.PropertyType),
            SelectedItem = LowerProperty?.GetValue(upperProperty?.GetValue(CurrentItem)) ?? null
        };
        cb.SelectedIndexChanged += (_, _) => { LowerProperty?.SetValue(upperProperty?.GetValue(CurrentItem), cb.SelectedItem); };
        return cb;
    }
    public Entry GetPropertyEntry(PropertyInfo CurrentProperty)
    {
        Entry pentry = new();
        try
        {
            pentry.Text = CurrentProperty.GetValue(CurrentItem)?.ToString();
        }
        catch (Exception) { pentry.Text = "Method is not Supported"; }
        pentry.TextChanged += (_, _) =>
        {
            try
            {
                if (string.IsNullOrEmpty(pentry.Text)) return;
                var value = Convert.ChangeType(pentry.Text, CurrentProperty.PropertyType);
                CurrentProperty.SetValue(CurrentItem, value);
            }
            catch (Exception) { }
        };
        return pentry;
    }
    public Entry GetClassPropertyEntry(PropertyInfo UpperProperty, PropertyInfo LowerProperty)
    {
        Entry pentry = new();
        try
        {
            pentry.Text = LowerProperty.GetValue(UpperProperty?.GetValue(CurrentItem))?.ToString();
        }
        catch (Exception) { pentry.Text = "Method is not Supported"; }
        pentry.TextChanged += (_, _) =>
        {
            try
            {
                if (string.IsNullOrEmpty(pentry.Text)) return;
                var value = Convert.ChangeType(pentry.Text, LowerProperty.PropertyType);
                LowerProperty.SetValue(UpperProperty?.GetValue(CurrentItem), value);
            }
            catch (Exception) { }
        };
        return pentry;
    }
}