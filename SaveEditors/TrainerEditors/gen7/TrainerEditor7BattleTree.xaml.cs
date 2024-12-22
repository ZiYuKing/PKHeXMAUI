
using PKHeX.Core;

namespace PKHeXMAUI;

public partial class TrainerEditor7BattleTree : ContentPage
{
	public SAV7 SAV = (SAV7)MainPage.sav;
	public TrainerEditor7BattleTree()
	{
		InitializeComponent();
		var bt = SAV.BattleTree;
		CSSNEntry.Text = bt.GetTreeStreak(0, false, false).ToString();
		CSSSEntry.Text = bt.GetTreeStreak(0, true, false).ToString();
		MSSNEntry.Text = bt.GetTreeStreak(0, false, true).ToString();
		MSSSEntry.Text = bt.GetTreeStreak(0, true, true).ToString();
		CSDNEntry.Text = bt.GetTreeStreak(1,false, false).ToString();
		CSDSEntry.Text = bt.GetTreeStreak(1,true, false).ToString();
		MSDNEntry.Text = bt.GetTreeStreak(1, false, true).ToString();
		MSDSEntry.Text = bt.GetTreeStreak(1, true, true).ToString();
		CSMNEntry.Text = bt.GetTreeStreak(2, false, false).ToString();
		CSMSEntry.Text = bt.GetTreeStreak(2, true, false).ToString();
		MSMNEntry.Text = bt.GetTreeStreak(2, false, true).ToString();
		MSMSEntry.Text = bt.GetTreeStreak(2, true, true).ToString();
		SuperSingleCheck.IsChecked = SAV.EventWork.GetEventFlag(333);
		SuperDoubleCheck.IsChecked = SAV.EventWork.GetEventFlag(334);
		SuperMultiCheck.IsChecked = SAV.EventWork.GetEventFlag(335);
	}

	public void SaveTE7B()
	{
        var bt = SAV.BattleTree;
		var parsed = int.TryParse(CSSNEntry.Text, out var result);
		if (parsed) bt.SetTreeStreak(result, 0, false, false);
		parsed = int.TryParse(CSSSEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 0, true, false);
		parsed = int.TryParse(MSSNEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 0, false, true);
		parsed = int.TryParse(MSSSEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 0, true, true);
		parsed = int.TryParse(CSDNEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 1, false, false);
		parsed = int.TryParse(CSDSEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 1, true, false);
		parsed = int.TryParse(MSDNEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 1, false, true);
		parsed = int.TryParse(MSDSEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 1, true, true);
		parsed = int.TryParse(CSMNEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 2, false, false);
		parsed = int.TryParse(CSMSEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 2, true, false);
		parsed = int.TryParse(MSMNEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 2, false, true);
		parsed = int.TryParse(MSMSEntry.Text, out result);
		if (parsed) bt.SetTreeStreak(result, 2, true, true);
		SAV.EventWork.SetEventFlag(333, SuperSingleCheck.IsChecked);
		SAV.EventWork.SetEventFlag(334, SuperDoubleCheck.IsChecked);
		SAV.EventWork.SetEventFlag(335, SuperMultiCheck.IsChecked);
    }
}