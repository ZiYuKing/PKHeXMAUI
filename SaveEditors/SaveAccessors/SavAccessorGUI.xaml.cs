using CommunityToolkit.Maui.Storage;
using PKHeX.Core;
using Syncfusion.Maui.Inputs;
using System.Reflection;
namespace PKHeXMAUI;

public partial class SavAccessorGUI : ContentPage 
{
    private readonly SaveBlockMetadata<BlockInfo> Metadata;
    private IDataIndirect CurrentBlock = null!;

    public SavAccessorGUI(SaveFile sav, ISaveBlockAccessor<BlockInfo> accessor)
	{
		InitializeComponent();
        Metadata = new SaveBlockMetadata<BlockInfo>(accessor);
        BlockKey_Picker.ItemsSource = Metadata.GetSortedBlockList().ToArray();

    }
    private void UpdateBlockSummaryControls(IDataIndirect obj)
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
    private void Update_BlockCV(object sender, Syncfusion.Maui.Inputs.SelectionChangedEventArgs e)
    {
        if (((SfComboBox)sender).SelectedItem is not null)
        {
            var name = ((SfComboBox)sender).SelectedItem as string;
            CurrentBlock = Metadata.GetBlock(name);
            UpdateBlockSummaryControls(CurrentBlock);
        }
        else
        {
            BlockStack.Clear();
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
    private void ChangeComboBoxFontColor(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        SfComboBox box = (SfComboBox)sender;
        box.TextColor = box.IsDropDownOpen ? Colors.Black : Colors.White;
    }
}