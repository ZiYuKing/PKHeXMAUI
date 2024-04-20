using PKHeX.Core;

namespace PKHeXMAUI;

public partial class SaveEditors : ContentPage
{
	public SaveEditors()
	{
		InitializeComponent();
        ToggleControls();
	}

    private void OpenItems(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new Items());
    }

    private void OpenBlockEditor(object sender, EventArgs e)
    {
        switch (MainPage.sav)
        {
            case SAV1 s: Navigation.PushModalAsync(new SavAccessorGUI(s, null)); break;
            case SAV2 s: Navigation.PushModalAsync(new SavAccessorGUI(s, null)); break;
            case SAV3 s: Navigation.PushModalAsync(new SavAccessorGUI(s, null)); break;
            case SAV4 s: Navigation.PushModalAsync(new SavAccessorGUI(s, null)); break;
            case SAV5BW s: Navigation.PushModalAsync(new SavAccessorGUI(s, s.Blocks)); break;
            case SAV5B2W2 s: Navigation.PushModalAsync(new SavAccessorGUI(s, s.Blocks)); break;
            case SAV6XY s: Navigation.PushModalAsync(new SavAccessorGUI(s, s.Blocks)); break;
            case SAV6AO s: Navigation.PushModalAsync(new SavAccessorGUI(s, s.Blocks)); break;
            case SAV6AODemo s: Navigation.PushModalAsync(new SavAccessorGUI(s, s.Blocks)); break;
            case SAV7SM s: Navigation.PushModalAsync(new SavAccessorGUI(s, s.Blocks)); break;
            case SAV7USUM s: Navigation.PushModalAsync(new SavAccessorGUI(s, s.Blocks)); break;
            case SAV7b s: Navigation.PushModalAsync(new SavAccessorGUI(s, s.Blocks)); break;
            case ISCBlockArray: Navigation.PushModalAsync(new BlockDataTab()); break;

        }
        
    }
    private void ToggleControls()
    {
        if (!MainPage.sav.State.Exportable || MainPage.sav is BulkStorage)
            return;
        Button_BlockData.IsVisible = true;
        if (MainPage.sav is not SAV7 or SAV8BS or SAV8SWSH)
            TrainerInfoButton.IsVisible = true;
    }

    private void OpenTrainerEditor(object sender, EventArgs e)
    {
        switch (MainPage.sav)
        {
            case SAV1:
            case SAV2:
            case SAV3:
            case SAV3XD:
            case SAV3Colosseum:
            case SAV4:
            case SAV5: Navigation.PushModalAsync(new TrainerEditor1()); break;
            case SAV6: Navigation.PushModalAsync(new TrainerTab6()); break;
            case SAV8LA: Navigation.PushModalAsync(new TrainerTab8a()); break;
            case SAV9SV: Navigation.PushModalAsync(new TrainerTab9()); break;
        }
    }
}