using System.Windows.Input;
using PKHeX.Core;
using static PKHeXMAUI.MainPage;
namespace PKHeXMAUI;

public partial class OTTab : ContentPage
{
    public bool SkipEvent = false;
    public bool FirstLoad = true;
	public OTTab()
	{
		InitializeComponent();
        htlanguagepicker.ItemsSource = Enum.GetValues(typeof(LanguageID));
        var OpenTrash = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
        OpenTrash.Tapped += OpenTrashEditor;
        OTLabel.GestureRecognizers.Add(OpenTrash);
        var OpenHiddenTrash = new TapGestureRecognizer() { NumberOfTapsRequired = 2 };
        OpenHiddenTrash.Tapped += OpenHiddenTrashEditor;
        HTLabel.GestureRecognizers.Add(OpenHiddenTrash);
        CountryPicker.ItemsSource = Util.GetCountryRegionList("countries", GameInfo.CurrentLanguage);
        CountryPicker.ItemDisplayBinding= new Binding("Text");
        DSregionPicker.ItemsSource = datasourcefiltered.ConsoleRegions.ToList();
        DSregionPicker.ItemDisplayBinding = new Binding("Text");
        ICommand refreshCommand = new Command(async () =>
        {
            await applyotinfo(pk);
            OTRefresh.IsRefreshing = false;
        });
        OTRefresh.Command = refreshCommand;
        applyotinfo(pk);
        FirstLoad = false;
    }

	public async Task applyotinfo(PKM pkm)
	{
        SkipEvent = true;
        eggsprite.IsVisible = pkm.IsEgg;
        countrylabel.IsVisible = false;
        if (pkm.HeldItem > 0)
        {
            itemsprite.Source = itemspriteurl;
            itemsprite.IsVisible = true;
        }
        else
        {
            itemsprite.IsVisible = false;
        }

        shinysparklessprite.IsVisible = pkm.IsShiny;
        spriteurl = pkm.Species == 0
            ? "a_egg.png"
            : $"a_{pkm.Species}{((pkm.Form > 0 && !MainPage.NoFormSpriteSpecies.Contains(pkm.Species)) ? $"_{pkm.Form}" : "")}.png";
        OTpic.Source = spriteurl;
        SIDdisplay.Text = pkm.DisplaySID.ToString();
        TIDdisplay.Text = pkm.DisplayTID.ToString();
        otdisplay.Text = pkm.OriginalTrainerName;
        ecdisplay.Text = $"{pkm.EncryptionConstant:X}";
        if (sav.Generation > 5)
        {
            htname.IsVisible = true;
            HTLabel.IsVisible = true;
            HTcurrentcheck.IsVisible = true;
            HTNameLabel.IsVisible = true;

            htname.Text = pkm.HandlingTrainerName;
            if (pkm is IHandlerLanguage htl)
            {
                htlanguagelabel.IsVisible = true;
                htlanguagepicker.IsVisible = true;
                htlanguagepicker.SelectedIndex = htl.HandlingTrainerLanguage;
            }
            switch (pkm.CurrentHandler)
            {
                case 0: OTcurrentcheck.IsChecked = true; HTcurrentcheck.IsChecked = false; break;
                case 1: HTcurrentcheck.IsChecked = true; OTcurrentcheck.IsChecked = false; break;
            }
        }
        if (pkm is IHomeTrack home)
        {
            HomeLabel.IsVisible = true;
            trackereditor.IsVisible = true;
            trackereditor.Text = home.Tracker.ToString("X16");
        }
        extrabytespicker.Items.Clear();
        for (var i=0;i<pkm.ExtraBytes.Length;i++)
            extrabytespicker.Items.Add($"0x{pkm.ExtraBytes[i]:X2}");
        extrabytespicker.SelectedIndex = 0;
        var offset = Convert.ToInt32((string)extrabytespicker.SelectedItem, 16);
        var value = pkm.Data[offset];

        extrabytesvalue.Text = value.ToString();
        otgenderpicker.Source = $"gender_{pkm.OriginalTrainerGender}.png";
        if(pk is IRegionOrigin regionOrigin)
        {
            countrylabel.IsVisible = true;
            CountryPicker.IsVisible = true;
            var selectedCountry = Util.GetCountryRegionList("countries", GameInfo.CurrentLanguage).Find(z => z.Value == regionOrigin.Country);
            CountryPicker.SelectedItem = selectedCountry;
            subregionlabel.IsVisible = true;
            subregionPicker.IsVisible = true;
            if (regionOrigin.Country != 0)
                subregionPicker.SelectedItem = Util.GetCountryRegionList($"sr_{selectedCountry.Value:000}", GameInfo.CurrentLanguage).Find(z => z.Value == regionOrigin.Region);
            DSregion.IsVisible = true;
            DSregionPicker.IsVisible = true;
            DSregionPicker.SelectedItem = datasourcefiltered.ConsoleRegions.ToList().Find(z=>z.Value == regionOrigin.ConsoleRegion);
        }
        SkipEvent = false;
    }

    private void applySID(object sender, TextChangedEventArgs e)
    {
        if(SIDdisplay.Text.Length > 0 && !SkipEvent)
        {
             var parsed = uint.TryParse(SIDdisplay.Text, out var SID);
            if(parsed)
                pk.DisplaySID = SID;
        }
    }

    private void applyTID(object sender, TextChangedEventArgs e)
    {
        if(TIDdisplay.Text.Length > 0 && !SkipEvent)
        {
            var parsed = uint.TryParse(TIDdisplay.Text, out var TID);
            if(parsed)
                pk.DisplayTID = TID;
        }
    }

    private void applyot(object sender, TextChangedEventArgs e)
    {
        if(!SkipEvent)
            pk.OriginalTrainerName = otdisplay.Text;
    }

    private void applyec(object sender, TextChangedEventArgs e)
    {
        if (!SkipEvent)
        {
            try
            {
                pk.EncryptionConstant = Convert.ToUInt32(ecdisplay.Text, 16);
            }
            catch { }
        }
    }

    private void applyHT(object sender, TextChangedEventArgs e)
    {
        if (!SkipEvent)
            pk.HandlingTrainerName = htname.Text;
    }

    private void applyhtlanguage(object sender, EventArgs e)
    {
        if(pk is IHandlerLanguage htl && !SkipEvent)
            htl.HandlingTrainerLanguage = (byte)htlanguagepicker.SelectedIndex;
    }

    private void MakeOTCurrent(object sender, CheckedChangedEventArgs e)
    {
        if (!SkipEvent && e.Value)
        {
            pk.CurrentHandler = 0;
            SkipEvent = true;
            HTcurrentcheck.IsChecked = false;
            SkipEvent = false;
        }
        else if (!SkipEvent && !e.Value)
        {
            pk.CurrentHandler = 1;
            SkipEvent = true;
            HTcurrentcheck.IsChecked = true;
            SkipEvent = false;
        }
    }

    private void MakeHTCurrent(object sender, CheckedChangedEventArgs e)
    {
        if (!SkipEvent && e.Value)
        {
            pk.CurrentHandler = 1;
            SkipEvent = true;
            OTcurrentcheck.IsChecked = false;
            SkipEvent = false;
        }
        else if(!SkipEvent && !e.Value)
        {
            pk.CurrentHandler = 0;
            SkipEvent = true;
            OTcurrentcheck.IsChecked = true;
            SkipEvent = false;
        }
    }

    private void applyotgender(object sender, EventArgs e)
    {
        if (pk.OriginalTrainerGender == 0)
        {
            pk.OriginalTrainerGender = 1;
            otgenderpicker.Source = "gender_1.png";
        }
        else
        {
            pk.OriginalTrainerGender = 0;
            otgenderpicker.Source = "gender_0.png";
        }
    }

    private void refreshOT(object sender, EventArgs e)
    {
        if(pk.Species !=0)
            applyotinfo(pk);
    }

    private void applyhometracker(object sender, TextChangedEventArgs e)
    {
        if (ulong.TryParse(trackereditor.Text, out var result) && !SkipEvent)
        {
            if (pk is IHomeTrack home)
            {
                home.Tracker = result;
             }
        }
    }

    private void extrabytestuff(object sender, EventArgs e)
    {
        if (!SkipEvent)
        {
            var offset = Convert.ToInt32((string)extrabytespicker.SelectedItem, 16);
            var value = pk.Data[offset];
            extrabytesvalue.Text = value.ToString();
        }
    }

    private void applyextrabytesvalue(object sender, TextChangedEventArgs e)
    {
        if (!SkipEvent)
        {
            var offset = Convert.ToInt32((string)extrabytespicker.SelectedItem, 16);
            pk.Data[offset] = Convert.ToByte(extrabytesvalue.Text);
        }
    }

    private void applyCountry(object sender, EventArgs e)
    {
        if(pk is IRegionOrigin regionOrigin && !SkipEvent)
        {
            var country = (ComboItem)CountryPicker.SelectedItem;
            regionOrigin.Country = (byte)country.Value;
            if (CountryPicker.SelectedIndex != 0)
            {
                subregionPicker.ItemsSource = Util.GetCountryRegionList($"sr_{country.Value:000}", GameInfo.CurrentLanguage);
                subregionPicker.ItemDisplayBinding = new Binding("Text");
            }
            else
            {
                subregionPicker.Items.Clear();
            }
        }
    }

    private void applySubregion(object sender, EventArgs e)
    {
        if (pk is IRegionOrigin regionOrigin && !SkipEvent)
        {
            var subregion = (ComboItem)CountryPicker.SelectedItem;
            regionOrigin.Region = (byte)subregion.Value;
        }
    }

    private void apply3DSregion(object sender, EventArgs e)
    {
        if (pk is IRegionOrigin regionOrigin && !SkipEvent)
        {
            var subregion = (ComboItem)DSregionPicker.SelectedItem;
            regionOrigin.ConsoleRegion = (byte)subregion.Value;
        }
    }
    public async void OpenTrashEditor(object sender, TappedEventArgs e)
    {
        TrashWindow = new TextEditor(otdisplay.Text, pk.OriginalTrainerTrash, MainPage.sav, MainPage.sav.Generation);
        await Navigation.PushModalAsync(TrashWindow);
        Task.Run(() => WaitForTrashToClose());
    }
    private void WaitForTrashToClose()
    {
        var trash = pk.OriginalTrainerTrash;
        EditingTrash = true;
        while (EditingTrash) { };
        TrashWindow.FinalBytes.CopyTo(pk.OriginalTrainerTrash);
        pk.OriginalTrainerName = TrashWindow.FinalString;
        applyotinfo(pk);
        
    }
    public async void OpenHiddenTrashEditor(object sender, TappedEventArgs e)
    {
        TrashWindow = new TextEditor(htname.Text, pk.HandlingTrainerTrash, MainPage.sav, MainPage.sav.Generation);
        await Navigation.PushModalAsync(TrashWindow);
        Task.Run(() => WaitForHiddenTrashToClose());
    }
    private void WaitForHiddenTrashToClose()
    {
        var trash = pk.HandlingTrainerTrash;
        EditingTrash = true;
        while (EditingTrash) { };
        TrashWindow.FinalBytes.CopyTo(pk.HandlingTrainerTrash);
        pk.HandlingTrainerName = TrashWindow.FinalString;
        applyotinfo(pk);

    }
}