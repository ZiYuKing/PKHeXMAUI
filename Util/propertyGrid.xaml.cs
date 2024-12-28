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
                    comboBox cb = new()
                    {
                        ItemSource = Enum.GetValues(pi.PropertyType),
                        SelectedItem = pi?.GetValue(CurrentItem)??new()
                    };
                    cb.SelectedIndexChanged += (_, _) => { try { pi?.SetValue(CurrentItem, cb.SelectedItem); } catch (Exception) { } };
                    stack.Add(cb, 1, i);
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
                                comboBox cb = new()
                                {
                                    ItemSource = Enum.GetValues(pr.PropertyType),
                                    SelectedItem = pr?.GetValue(pi?.GetValue(CurrentItem)) ?? null
                                };
                                cb.SelectedIndexChanged += (_, _) => { try { pr?.SetValue(pi?.GetValue(CurrentItem), cb.SelectedItem); } catch (Exception) { } };
                                G_stack.Add(cb, 1, o);
                            }
                            else
                            {
                                Entry pentry = new();
                                try
                                {
                                    pentry.Text = pr.GetValue(pi?.GetValue(CurrentItem))?.ToString();
                                }
                                catch (Exception) { pentry.Text = "Method is not Supported"; }
                                G_stack.Add(pentry, 1, o);
                                pentry.TextChanged += (_, _) =>
                                {
                                    try
                                    {
                                        if (string.IsNullOrEmpty(pentry.Text)) return;
                                        var value = Convert.ChangeType(pentry.Text, pr.PropertyType);
                                        pr.SetValue(pi?.GetValue(CurrentItem), value);
                                    }
                                    catch (Exception) { }
                                };
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
                    Entry pentry = new();
                    try
                    {
                        pentry.Text = pi.GetValue(CurrentItem)?.ToString();
                    }
                    catch (Exception) { pentry.Text = "Method is not Supported"; }
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
                    Entry pentry = new();
                    try
                    {
                        pentry.Text = pi.GetValue(CurrentItem)?.ToString();
                    }
                    catch (Exception) { pentry.Text = "Method is not Supported"; }
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