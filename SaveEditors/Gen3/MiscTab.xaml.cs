namespace PKHeXMAUI;

public partial class MiscTab : TabbedPage
{
	public static MiscMainEditor MME;
	public MiscTab()
	{
		InitializeComponent();
        misctab3.BarBackgroundColor = Color.FromArgb("303030");
        misctab3.BarTextColor = Colors.White;
        MME = new MiscMainEditor();
		misctab3.Children.Add(MME);
		misctab3.Children.Add(new cancelpage());
	}
}