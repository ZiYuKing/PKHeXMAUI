
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
                        if (IsNumericType(pi?.GetValue(obj)))
                        {
                            var propLabel = new Label() { Text = prop };
                            BlockStack.Add(propLabel, 0, row);
                            var BlockEntry = new Entry
                            {
                                BindingContext = obj
                            };
                            try { BlockEntry.SetBinding(Entry.TextProperty, prop, BindingMode.TwoWay); }
                            catch (Exception) { BlockStack.Remove(propLabel); continue; }
                            BlockStack.Add(BlockEntry, 1, row);
                            row++;
                        }
                        else
                        {
                            var propLabel = new Label() { Text = prop };
                            BlockStack.Add(propLabel, 0, row);
                            var BlockEntry = new Picker
                            {
                                BindingContext = obj
                            };
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
                        var BlockEntry = new Picker
                        {
                            BindingContext = obj
                        };
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
    public static bool IsNumericType(object? o)
    {
        return Type.GetTypeCode(o?.GetType()) switch
        {
            TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
            _ => false,
        };
    }

    private void GiveAllAccessories(object sender, EventArgs e)
    {
        if (SAV is SAV6XY xy)
            xy.Blocks.Fashion.UnlockAllAccessories();
    }
}