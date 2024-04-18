using PKHeX.Core;
using System.Xml.Serialization;

namespace PKHeXMAUI;

public partial class TrainerEditor1 : ContentPage
{
    private readonly CheckBox[] cba;
    public SaveFile SAV = MainPage.sav;
    public TrainerEditor1()
	{
		InitializeComponent();
        cba = [Badge1, Badge2, Badge3, Badge4, Badge5, Badge6, Badge7, Badge8];
        OTGenderPicker.ItemsSource = GameInfo.GenderSymbolUnicode.Take(2).ToArray();
        MaxCoinsButton.IsVisible = MainPage.sav.Generation < 3;
        OTGenderPicker.IsVisible = SAV.Generation > 1;
        CoinLabel.IsVisible = CoinsEntry.IsVisible = SAV.Generation < 3;
        SIDLabel.IsVisible = SIDEntry.IsVisible = SAV.Generation > 2;
        RegionLabel.IsVisible = RegionPicker.IsVisible = CountryLabel.IsVisible = CountryPicker.IsVisible = SAV.Generation > 3;
        PFEntry.IsVisible = PFLabel.IsVisible = PBLabel.IsVisible = PBEntry.IsVisible = SAV.Generation == 1;
        OTNameEntry.Text = SAV.OT;
        OTGenderPicker.SelectedIndex = SAV.Gender;
        TIDEntry.Text = SAV.TID16.ToString("00000");
        SIDEntry.Text = SAV.SID16.ToString("00000");
        OTMoneyEntry.Text = SAV.Money.ToString();
        HrsEntry.Text = SAV.PlayedHours.ToString();
        MinsEntry.Text = SAV.PlayedMinutes.ToString();
        SecEntry.Text = SAV.PlayedSeconds.ToString();
        int badgeval = 0;
        if(SAV is SAV1 sav1)
        {
            CoinsEntry.Text = sav1.Coin.ToString();
            badgeval = sav1.Badges;
            GSLabel.IsVisible = GSDatePicker.IsVisible = GSTimerPicker.IsVisible = HOFLabel.IsVisible = HOFDatePicker.IsVisible = HOFTimePicker.IsVisible = false;
            MapGrid.IsVisible = false;
            BattleStylePicker.ItemsSource = new string[] { "Switch", "Set" };
            SoundTypePicker.ItemsSource = new string[] { "Mono", "Stereo", "Left", "Right" };
            TextSpeedPicker.ItemsSource = new string[] { "0 (Instant)", "1 (Fast)", "2", "3 (Normal)", "4", "5 (Slow)", "6", "7" };
            UseBattleEffectsCB.IsChecked = sav1.BattleEffects;
            BattleStylePicker.SelectedIndex = sav1.BattleStyleSwitch ? 0 : 1;
            SoundTypePicker.SelectedIndex = sav1.Sound;
            TextSpeedPicker.SelectedIndex = sav1.TextSpeed;
            PFEntry.Text = sav1.PikaFriendship.ToString();
            PBEntry.Text = sav1.PikaBeachScore.ToString();
            if (!sav1.Version.Contains(GameVersion.YW))
            {
               PFLabel.IsVisible = PFEntry.IsVisible = PBEntry.IsVisible = PBLabel.IsVisible = PBEntry.IsVisible = SoundTypePicker.IsVisible = SoundTypeLabel.IsVisible = false;
            }
        }
        if(SAV is SAV2 sav2)
        {
            CoinsEntry.Text = sav2.Coin.ToString();
            GSLabel.IsVisible = GSDatePicker.IsVisible = GSTimerPicker.IsVisible = HOFLabel.IsVisible = HOFDatePicker.IsVisible = HOFTimePicker.IsVisible = false;
            MapGrid.IsVisible = false;
            BattleStylePicker.ItemsSource = new string[] { "Switch", "Set" };
            SoundTypePicker.ItemsSource = new string[] { "Mono", "Stereo", "Left", "Right" };
            TextSpeedPicker.ItemsSource = new string[] { "0 (Instant)", "1 (Fast)", "2", "3 (Normal)", "4", "5 (Slow)", "6", "7" };
            UseBattleEffectsCB.IsChecked = sav2.BattleEffects;
            BattleStylePicker.SelectedIndex = sav2.BattleStyleSwitch ? 0 : 1;
            SoundTypePicker.SelectedIndex = sav2.Sound;
            TextSpeedPicker.SelectedIndex = sav2.TextSpeed;
            badgeval = sav2.Badges;
            cba = cba = [Badge1, Badge2, Badge3, Badge4, Badge5, Badge6, Badge7, Badge8, Badge9, Badge10, Badge11, Badge12, Badge13, Badge14, Badge15, Badge16];

        }
        if(SAV is SAV3 sav3)
        {
            GSLabel.IsVisible = GSDatePicker.IsVisible = GSTimerPicker.IsVisible = HOFLabel.IsVisible = HOFDatePicker.IsVisible = HOFTimePicker.IsVisible = false;
            MapGrid.IsVisible = false;
            OptionsGrid.IsVisible = false;
            badgeval = sav3.Badges;

        }
        if (SAV is SAV3Colosseum or SAV3XD)
        {
            GSLabel.IsVisible = GSDatePicker.IsVisible = GSTimerPicker.IsVisible = HOFLabel.IsVisible = HOFDatePicker.IsVisible = HOFTimePicker.IsVisible = false;
            MapGrid.IsVisible = false;
            OptionsGrid.IsVisible = false;
            HrsEntry.IsVisible = MinsEntry.IsVisible = SecEntry.IsVisible = minLabel.IsVisible = hrsLabel.IsVisible = secLabel.IsVisible = false;
            return;
        }
        if(SAV is SAV4 sav4)
        {
            OptionsGrid.IsVisible = false;
            CurrentMapEntry.Text = sav4.M.ToString();
            XCoordEntry.Text = sav4.X.ToString();
            YCoordinate.Text = sav4.Y.ToString();
            ZCoordEntry.Text = sav4.Z.ToString();
            badgeval = sav4.Badges;
            if (sav4 is SAV4HGSS hgss)
            {
                badgeval |= hgss.Badges16 << 8;
                cba = [.. cba, Badge9, Badge10, Badge11, Badge12, Badge13, Badge14, Badge15, Badge16];
            }
            CountryPicker.ItemsSource = Util.GetCountryRegionList("gen4_countries", GameInfo.CurrentLanguage);
            CountryPicker.ItemDisplayBinding = new Binding("Text");
            CountryPicker.SelectedItem = Util.GetCountryRegionList("gen4_countries", GameInfo.CurrentLanguage).Select(z=>z.Value == sav4.Country);
        }
        if(SAV is SAV5 sav5)
        {
            BPEntry.IsVisible = BPLabel.IsVisible = MaxBPButton.IsVisible = true;
            BPEntry.Text = sav5.BattleSubway.BP.ToString();
            var pd = sav5.PlayerPosition;
            OptionsGrid.IsVisible = false;
            MapGrid.IsVisible = true;
            CurrentMapEntry.Text = pd.M.ToString();
            XCoordEntry.Text = pd.X.ToString();
            YCoordinate.Text = pd.Y.ToString();
            ZCoordEntry.Text = pd.Z.ToString();
            badgeval = sav5.Misc.Badges;
            CountryPicker.ItemsSource = Util.GetCountryRegionList("gen5_countries", GameInfo.CurrentLanguage);
            CountryPicker.SelectedItem = Util.GetCountryRegionList("gen5_countries", GameInfo.CurrentLanguage).Select(z => z.Value == sav5.Country);

        }
        for (int i = 0; i < cba.Length; i++)
        {
            cba[i].IsVisible = true;
            cba[i].IsChecked = (badgeval & (1 << i)) != 0;
        }
        DateUtil.GetDateTime2000(SAV.SecondsToStart, out var date, out var time);
        GSDatePicker.Date = date;
        GSTimerPicker.Time = time.TimeOfDay;
        DateUtil.GetDateTime2000(SAV.SecondsToFame, out date, out time);
        HOFDatePicker.Date = date;
        HOFTimePicker.Time = time.TimeOfDay;
    }

    private void MaxMoney(object sender, EventArgs e)
    {
		OTMoneyEntry.Text =  MainPage.sav.MaxMoney.ToString();
    }

    private void MaxCoins(object sender, EventArgs e)
    {
        CoinsEntry.Text = MainPage.sav.MaxCoins.ToString();
    }

    private void MaxBP(object sender, EventArgs e)
    {
        BPEntry.Text = MainPage.sav.MaxCoins.ToString();
    }

    private void CloseTE1(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private void SaveTE1(object sender, EventArgs e)
    {

    }

    private void UpdateRegions(object sender, EventArgs e)
    {
        if(SAV is SAV4 sav4)
        {
            var index = ((Picker)sender).SelectedIndex;
            var RegionItems = Util.GetCountryRegionList($"gen4_sr_{index:000}", GameInfo.CurrentLanguage);
            if(RegionItems.Count == 0)
                RegionItems = Util.GetCountryRegionList("gen4_sr_default", GameInfo.CurrentLanguage);
            RegionPicker.ItemsSource = RegionItems;
            RegionPicker.ItemDisplayBinding = new Binding("Text");
        }
        if(SAV is SAV5)
        {
            var index = ((Picker)sender).SelectedIndex;
            var RegionItems = Util.GetCountryRegionList($"gen5_sr_{index:000}", GameInfo.CurrentLanguage);
            if (RegionItems.Count == 0)
                RegionItems = Util.GetCountryRegionList("gen5_sr_default", GameInfo.CurrentLanguage);
            RegionPicker.ItemsSource = RegionItems;
        }
    }
}