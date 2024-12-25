
using CommunityToolkit.Maui.Storage;
using PKHeX.Core;
using System.Reflection;
namespace PKHeXMAUI;

public partial class SavAccessorGUI : ContentPage
{
    private readonly SaveBlockMetadata<BlockInfo>? Metadata;
    private IDataIndirect? CurrentBlock = null;
#nullable enable
    public SavAccessorGUI(SaveFile sav, ISaveBlockAccessor<BlockInfo>? accessor)
	{
		InitializeComponent();
        if (accessor is not null)
        {
            Metadata = new SaveBlockMetadata<BlockInfo>(accessor);
            BlockKey_Picker.ItemSource = Metadata.GetSortedBlockList().ToArray();
        }
        else
        {
            BlockKey_Picker.IsVisible = false;
            UpdateSimpleBlockSummaryControls(sav);
        }
        BlockKey_Picker.TextChanged +=(s,e)=> GetMatchingIndexes(s,(TextChangedEventArgs)e);
    }
    private void UpdateBlockSummaryControls(IDataIndirect? obj)
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
                   var pi= obj.GetType().GetProperty(prop);
                    try
                    {
                        var value = pi?.GetValue(obj);
                        if (value is not null && IsNumericType(value))
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
    private void UpdateSimpleBlockSummaryControls(SaveFile obj)
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
                        var value = pi?.GetValue(obj);
                        if (value is not null && IsNumericType(value))
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
    private void Update_BlockCV(object sender, EventArgs e)
    {
        if (((comboBox)sender).SelectedItem is not null)
        {
            if (((comboBox)sender).SelectedItem is not string name) { BlockStack.Clear(); return; }
            CurrentBlock = Metadata?.GetBlock(name);
                UpdateBlockSummaryControls(CurrentBlock);
        }
        else
        {
            BlockStack.Clear();
        }
    }
    public static bool IsNumericType(object o)
    {
        return Type.GetTypeCode(o.GetType()) switch
        {
            TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
            _ => false,
        };
    }
    public void GetMatchingIndexes(object? sender, TextChangedEventArgs? filterInfo)
    {
        List<int> filteredlist = [];
        var text = filterInfo?.NewTextValue ?? "";
        if (text.Length == 8)
        {
            var hex = (int)Util.GetHexValue(text);
            if (hex != 0)
            {
                // Input is hexadecimal number, select the item
                filteredlist.Add(BlockEditor8.SortedBlockKeys.ToList().IndexOf(BlockEditor8.SortedBlockKeys.ToList().Find(z => z.Value == hex) ?? new ComboItem("", 0)));
                BlockKey_Picker.ItemSource = filteredlist;
                return;
            }
        }
    }
}