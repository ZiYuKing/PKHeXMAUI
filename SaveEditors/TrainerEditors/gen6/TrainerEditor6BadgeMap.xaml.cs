
#nullable disable

using PKHeX.Core;

namespace PKHeXMAUI;

public partial class TrainerEditor6BadgeMap : ContentPage
{
    private readonly CheckBox[] cba;
    public SAV6 SAV = (SAV6)MainPage.sav;
    public TrainerEditor6BadgeMap()
	{
		InitializeComponent();
        cba = [Badge1, Badge2, Badge3, Badge4, Badge5, Badge6, Badge7, Badge8];
        int badgeval = SAV.Badges;
        for (int i = 0; i < 8; i++)
            cba[i].IsChecked = (badgeval & (1 << i)) != 0;
        var sit = SAV.Situation;
        CurrentMapEntry.Text = sit.M.ToString();
        RotationEntry.Text = sit.R.ToString();
        try
        {
            XCoordEntry.Text = (sit.X / 18.0).ToString();
            YCoordEntry.Text = (sit.Y / 18.0).ToString();
            ZCoordEntry.Text = (sit.Z / 18.0).ToString();
        }
        catch (Exception) { MapGrid.IsVisible = false; }
    }
    public void SaveBadgeMap()
    {
        var sit = SAV.Situation;
        var parsed = int.TryParse(CurrentMapEntry.Text, out var result);
        if (parsed) sit.M = result;
        parsed = float.TryParse(XCoordEntry.Text, out _);
        if (parsed) sit.X = result * 18;
        parsed = float.TryParse(YCoordEntry.Text, out _);
        if (parsed) sit.Y = result * 18;
        parsed = float.TryParse(ZCoordEntry.Text, out _);
        if (parsed) sit.Z = result * 18;
        parsed = int.TryParse(RotationEntry.Text, out result);
        if (parsed) sit.R = result;
        int badgeval = 0;
        for (int i = 0; i < 8; i++)
            badgeval |= (cba[i].IsChecked ? 1 : 0) << i;
        SAV.Badges = badgeval;
    }
}