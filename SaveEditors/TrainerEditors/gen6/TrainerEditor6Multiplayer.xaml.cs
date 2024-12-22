
using PKHeX.Core;
namespace PKHeXMAUI;

public partial class TrainerEditor6Multiplayer : ContentPage
{
	public SAV6 SAV = (SAV6)MainPage.sav;
	public TrainerEditor6Multiplayer()
	{
        InitializeComponent();
		var status = SAV.Status;
		MegaEvolutionCheck.IsChecked = status.IsMegaEvolutionUnlocked;
        var names = Enum.GetNames<TrainerSprite6>();
        var values = Enum.GetValues<TrainerSprite6>();
        var max = SAV is not SAV6AO ? (int)TrainerSprite6.Trevor : names.Length;
        var data = new ComboItem[max];
        for (int i = 0; i < max; i++)
            data[i] = new ComboItem(names[i], (int)values[i]);
        MSPicker.ItemsSource = data;
        MSPicker.ItemDisplayBinding = new Binding("Text");
        if (SAV is IMultiplayerSprite ms)
        {
            MSPicker.SelectedItem = data.First(z => z.Value == ms.MultiplayerSpriteID);
            string file = $"tr_{ms.MultiplayerSpriteID:00}.png";
            MSSprite.Source = file;
        }
        SS1.Text = status.Saying1;
        SS2.Text = status.Saying2;
        SS3.Text = status.Saying3;
        SS4.Text = status.Saying4;
        SS5.Text = status.Saying5;
        RayquazeMECheck.IsVisible = SAV is SAV6AO;
        MERayquazaLabel.IsVisible = SAV is SAV6AO;
        RayquazeMECheck.IsChecked = status.IsMegaRayquazaUnlocked;
    }

    private void UpdateSprite(object sender, EventArgs e)
    {
        var MultiSprite = ((ComboItem)((Picker)sender).SelectedItem).Value;
        string file = $"tr_{MultiSprite:00}.png";
        MSSprite.Source = file;
    }

    public void SaveMultiplayer()
    {
        var status = SAV.Status;
        status.Saying1 = SS1.Text;
        status.Saying2 = SS2.Text;
        status.Saying3 = SS3.Text;
        status.Saying4 = SS4.Text;
        status.Saying5 = SS5.Text;
        status.IsMegaEvolutionUnlocked = MegaEvolutionCheck.IsChecked;
        if (SAV is IMultiplayerSprite ms)
            ms.MultiplayerSpriteID = ((ComboItem)MSPicker.SelectedItem).Value;
        status.IsMegaRayquazaUnlocked = RayquazeMECheck.IsChecked;
    }
}