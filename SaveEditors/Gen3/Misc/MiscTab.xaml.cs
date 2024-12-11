#nullable disable

using PKHeX.Core;
using static PKHeXMAUI.MainPage;
namespace PKHeXMAUI;

public partial class MiscTab : TabbedPage
{
	public static MiscMainEditor MME;
	public static MiscRecords MRE;
	public static MiscPokeblock MPB;
	public static MiscDecorations MDE;
	public static MiscPaintings MPE;
	public MiscTab()
	{
		InitializeComponent();
        misctab3.BarBackgroundColor = Color.FromArgb("303030");
        misctab3.BarTextColor = Colors.White;
        MME = new MiscMainEditor();
		MRE = new((SAV3)sav);
		MPB = new((IGen3Hoenn)sav);
		MDE = new((IGen3Hoenn)sav);
		MPE = new((SAV3)sav);
		misctab3.Children.Add(MME);
		misctab3.Children.Add(MRE);
		misctab3.Children.Add(MPB);
		misctab3.Children.Add(MDE);
		misctab3.Children.Add(MPE);
		misctab3.Children.Add(new cancelpage());
		misctab3.Children.Add(new Misc3Save());
	}
}
public class Misc3Save : ContentPage
{
	public Misc3Save()
	{
		this.Title = "Save";
		this.Content = new Label() { Text = "The MAUI Framework has bugs. This is the save page. Navigate to another page, and then select the page you were trying to reach!" };
	}
    protected override void OnNavigatedTo(NavigatedToEventArgs args)
	{
		MiscTab.MPB.Save();
		Navigation.PopModalAsync();
	}
}