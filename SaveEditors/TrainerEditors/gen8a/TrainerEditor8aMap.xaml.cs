using PKHeX.Core;

namespace PKHeXMAUI;

public partial class TrainerEditor8aMap : ContentPage
{
	public SAV8LA SAV = (SAV8LA)MainPage.sav;
	public TrainerEditor8aMap()
	{
		InitializeComponent();
		TE8aCMapEntry.Text = SAV.Coordinates.M;
		TE8aXEntry.Text = SAV.Coordinates.X.ToString();
		TE8aZEntry.Text = SAV.Coordinates.Z.ToString();
		TE8aYEntry.Text = SAV.Coordinates.Y.ToString();
		TE8aREntry.Text = (Math.Atan2(SAV.Coordinates.RZ, SAV.Coordinates.RW) * 360.0 / Math.PI).ToString();
    }

	public void SaveTE8aMap()
	{
		SAV.Coordinates.M = TE8aCMapEntry.Text;
		var parsed = float.TryParse(TE8aXEntry.Text, out var result);
		SAV.Coordinates.X = parsed ? result : SAV.Coordinates.X;
		parsed = float.TryParse(TE8aZEntry.Text, out result);
		SAV.Coordinates.Z = parsed ? result : SAV.Coordinates.Z;
		parsed = float.TryParse(TE8aYEntry.Text, out result);
		SAV.Coordinates.Y = parsed ? result : SAV.Coordinates.Y;
		parsed = double.TryParse(TE8aREntry.Text, out var DResult);
		if (parsed)
		{
			var angle = DResult * Math.PI / 360.0;
			SAV.Coordinates.RX = 0;
			SAV.Coordinates.RZ = (float)Math.Sin(angle);
			SAV.Coordinates.RY = 0;
			SAV.Coordinates.RW = (float)Math.Cos(angle);
		}
	}
}