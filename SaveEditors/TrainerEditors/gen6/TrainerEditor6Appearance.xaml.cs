using PKHeX.Core;

namespace PKHeXMAUI;

public partial class TrainerEditor6Appearance : ContentPage
{
    public SAV6 SAV = (SAV6)MainPage.sav;
    public TrainerEditor6Appearance()
    {
        InitializeComponent();
        if (SAV is SAV6XY xy)
        {
            var xystat = (MyStatus6XY)xy.Status;
            UpdateBlockSummaryControls(xystat.Fashion);
            TNickEntry.Text = xystat.Nickname;
        }

    }
    private void UpdateBlockSummaryControls(TrainerFashion6 obj)
    {
        BlockStack.Clear();

        if (obj != null)
        {
            var props = ReflectUtil.GetPropertiesCanWritePublicDeclared(obj.GetType());
            if (props.Count() > 1)
            {
                int row = 0;
                foreach (var prop in props)
                {
                    var pi = obj.GetType().GetProperty(prop);
                    try
                    {
                        if (IsNumericType(pi.GetValue(obj)))
                        {
                            var propLabel = new Label() { Text = prop };
                            BlockStack.Add(propLabel, 0, row);
                            var BlockEntry = new Entry();
                            BlockEntry.BindingContext = obj;
                            try { BlockEntry.SetBinding(Entry.TextProperty, prop, BindingMode.TwoWay); }
                            catch (Exception) { BlockStack.Remove(propLabel); continue; }
                            BlockStack.Add(BlockEntry, 1, row);
                            row++;
                        }
                        else
                        {
                            var propLabel = new Label() { Text = prop };
                            BlockStack.Add(propLabel, 0, row);
                            var BlockEntry = new Picker();
                            BlockEntry.BindingContext = obj;
                            try { BlockEntry.SetBinding(Picker.ItemsSourceProperty, prop, BindingMode.TwoWay); }
                            catch (Exception) { BlockStack.Remove(propLabel); continue; }
                            BlockStack.Add(BlockEntry, 1, row);
                            row++;
                        }
                    }
                    catch (Exception)
                    {
                        var propLabel = new Label() { Text = prop };
                        BlockStack.Add(propLabel, 0, row);
                        var BlockEntry = new Picker();
                        BlockEntry.BindingContext = obj;
                        try { BlockEntry.SetBinding(Picker.ItemsSourceProperty, prop, BindingMode.TwoWay); }
                        catch (Exception) { BlockStack.Remove(propLabel); continue; }
                        BlockStack.Add(BlockEntry, 1, row);
                        row++;
                    }
                }
                return;
            }
        }
    }
    public void SaveAppearance()
    {
        if(SAV is SAV6XY xy)
        {
            var xystat = (MyStatus6XY)xy.Status;
            xystat.Nickname = TNickEntry.Text;
        }
            
    }
    public static bool IsNumericType(object o)
    {
        switch (Type.GetTypeCode(o.GetType()))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            default:
                return false;
        }
    }

    private void GiveAllAccessories(object sender, EventArgs e)
    {
        if (SAV is SAV6XY xy)
            xy.Blocks.Fashion.UnlockAllAccessories();
    }
}