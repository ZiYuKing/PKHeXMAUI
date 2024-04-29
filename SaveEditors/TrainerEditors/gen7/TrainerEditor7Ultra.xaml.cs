using PKHeX.Core;

namespace PKHeXMAUI;

public partial class TrainerEditor7Ultra : ContentPage
{
	public static SAV7 SAV = (SAV7)MainPage.sav;
	public TrainerEditor7Ultra()
	{
		InitializeComponent();
		MeleSurfScoreEntry.Text = SAV.Misc.GetSurfScore(0).ToString();
		AkaSurfScoreEntry.Text = SAV.Misc.GetSurfScore(1).ToString();
		UlaSurfScoreEntry.Text = SAV.Misc.GetSurfScore(2).ToString();
		PoniSurfScoreEntry.Text = SAV.Misc.GetSurfScore(3).ToString();
		RotomOTEntry.Text = SAV.FieldMenu.RotomOT;
		AffectionEntry.Text = SAV.FieldMenu.RotomAffection.ToString();
		Loto1Check.IsChecked = SAV.FieldMenu.RotomLoto1;
		Loto2Check.IsChecked = SAV.FieldMenu.RotomLoto2;
	}
	public void SaveTE7U()
	{
		var parsed = int.TryParse(MeleSurfScoreEntry.Text, out var result);
		if (parsed) SAV.Misc.SetSurfScore(0, result);
		parsed = int.TryParse(AkaSurfScoreEntry.Text, out result);
		if (parsed) SAV.Misc.SetSurfScore(1, result);
		parsed = int.TryParse(UlaSurfScoreEntry.Text, out result);
		if (parsed) SAV.Misc.SetSurfScore(2, result);
		parsed = int.TryParse(PoniSurfScoreEntry.Text, out result);
		if (parsed) SAV.Misc.SetSurfScore(3, result);
		SAV.FieldMenu.RotomOT = RotomOTEntry.Text;
		parsed = int.TryParse(AffectionEntry.Text, out result);
		if (parsed) SAV.FieldMenu.RotomAffection = (ushort)result;
		SAV.FieldMenu.RotomLoto1 = Loto1Check.IsChecked;
		SAV.FieldMenu.RotomLoto2 = Loto2Check.IsChecked;
		
	}
}