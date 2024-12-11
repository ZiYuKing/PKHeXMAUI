#nullable disable

using PKHeX.Core;
using static PKHeXMAUI.MainPage;
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
        if (!sav.State.Exportable || sav is BulkStorage)
            return;
        Button_BlockData.IsVisible = true;
        if (sav is not SAV8BS or SAV8SWSH)
            TrainerInfoButton.IsVisible = true;
        if (sav is SAV1 or SAV2)
            Button_EventFlags1.IsVisible = true;
        if (sav is SAV1 or SAV2 or SAV3)
            Button_Pokedex1.IsVisible = true;
        if (sav is SAV2 sav2)
        {
            MailBoxButton.IsVisible = true;
            GSBallButton.IsVisible = sav.Version is GameVersion.C;
            GSBallButton.IsEnabled = !sav2.IsEnabledGSBallMobileEvent;
        }
        if(sav is SAV2 or SAV3)
            Button_RTCEditor.IsVisible = true;
        if(sav is SAV3)
            B_Misc.IsVisible = true;
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
            case SAV7: Navigation.PushModalAsync(new TrainerTab7()); break;
            case SAV8LA: Navigation.PushModalAsync(new TrainerTab8a()); break;
            case SAV9SV: Navigation.PushModalAsync(new TrainerTab9()); break;
        }
    }

    private void OpenEventFlagEditor(object sender, EventArgs e)
    {
        if (sav is SAV1)
            Navigation.PushModalAsync(new EventFlags1((SAV1)MainPage.sav));
        else
            Navigation.PushModalAsync(new EventFlags2Tab());
    }

    private void OpenSimplePokedex(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new Pokedex1(MainPage.sav));
    }

    private async void OpenRTCEditor(object sender, EventArgs e)
    {
        switch (sav.Generation)
        {
            case 2:
                var sav2 = ((SAV2)sav);
                var msg = MessageStrings.MsgSaveGen2RTCResetBitflag;
                if (!sav2.Japanese) // show Reset Key for non-Japanese saves
                    msg = string.Format(MessageStrings.MsgSaveGen2RTCResetPassword, sav2.ResetKey) + Environment.NewLine + Environment.NewLine + msg;
                var dr = await DisplayAlert("Reset RTC", msg, "Yes", "cancel");
                if (dr)
                    sav2.ResetRTC();
                break;
            case 3:
                Navigation.PushModalAsync(new RTC3Editor(sav));
                break;
        }
    }

    private void OpenMailBoxEditor(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new MailBox(sav));
    }

    private void EnableGSBallEvent(object sender, EventArgs e)
    {
        var sav2 = sav as SAV2;
        sav2.EnableGSBallMobileEvent();
        GSBallButton.IsEnabled = false;
    }

    private void OpenMisc3Editor(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new MiscTab());
    }
}