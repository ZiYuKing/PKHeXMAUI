#nullable disable

using PKHeX.Core;

namespace PKHeXMAUI;

public partial class MiscRecords : ContentPage
{
	public bool setting = false;
	public static Record3 records;
	public static List<ComboItem> items;
	public MiscRecords(SAV3 sav)
	{
		InitializeComponent();
		setting = true;
		records = new Record3(sav);
		items = [.. Record3.GetItems(sav)];
        ComboRecord.ItemSource = items;
        ComboRecord.DisplayMemberPath = "Text";
        ComboRecord.SelectedIndexChanged += (_, _) =>
		{
			if (ComboRecord.SelectedItem is null) return;
			var index = ((ComboItem)ComboRecord.SelectedItem).Value;
			EntryRecordValue.Text = records.GetRecord(index).ToString();
			EntryFameH.IsVisible = EntryFameM.IsVisible = EntryFameS.IsVisible = index == 1;
        };
		ComboRecord.SelectedIndex = 1;
		ComboRecord.SelectedIndex = 0;
		EntryRecordValue.TextChanged += (_, _) =>
		{
			if (ComboRecord.SelectedItem is null || setting) return;
			var index = ((ComboItem)ComboRecord.SelectedItem).Value;
			var value = uint.Parse(EntryRecordValue.Text);
			records.SetRecord(index, value);
			if (index == 1)
			{
				setting = true;
				EntryFameH.Text = (value >> 16).ToString();
				EntryFameM.Text = (value >> 8).ToString();
				EntryFameS.Text = value.ToString();
				setting = false;
			}
		};
		EntryFameH.TextChanged += (_, _) => ChangeFame(records);
		EntryFameM.TextChanged += (_, _) => ChangeFame(records);
		EntryFameS.TextChanged += (_, _) => ChangeFame(records);
        void ChangeFame(Record3 r3) { if (setting) return; r3.SetRecord(1, uint.Parse(GetFameTime())); }
		setting = false;
    }
    public string GetFameTime()
    {
        var hrs = Math.Min(9999, uint.Parse(EntryFameH.Text));
        var min = Math.Min(59, uint.Parse(EntryFameM.Text));
        var sec = Math.Min(59, uint.Parse(EntryFameS.Text));

        return ((hrs << 16) | (min << 8) | sec).ToString();
    }
}