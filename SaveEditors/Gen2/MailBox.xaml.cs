using PKHeX.Core;
using System.Collections.ObjectModel;
namespace PKHeXMAUI;

public partial class MailBox : ContentPage
{
    private readonly MailDetail[] m = null!;
    private readonly int[] MailItemID = null!;
    private readonly Label[] PKMLabels, PKMHeldItems;
    private readonly Entry[] PKMNUDs, Miscs;
    private readonly int PartyBoxCount;
    private ObservableCollection<string> PartyBoxList = [];
    private ObservableCollection<string> PCBoxList = [];
    private List<string> MailItemIDs = [];
    private readonly byte Generation;
    private readonly IList<PKM> p;
    private int entry;
    private readonly SaveFile SAV;
    private string loadedLBItemLabel = null!;
    public MailBox(SaveFile sav)
	{
		InitializeComponent();
        SAV = sav;
        Generation = sav.Generation;
        p = sav.PartyData;
        PKMLabels = [pkmlabel1, pkmlabel2, pkmlabel3, pkmlabel4, pkmlabel5, pkmlabel6];
        PKMHeldItems = [helditemlabel1,helditemlabel2,helditemlabel3,helditemlabel4,helditemlabel5,helditemlabel6];
        PKMNUDs = [pkmnud1,pkmnud2,pkmnud3,pkmnud4,pkmnud5,pkmnud6];
        MBP_CV.ItemTemplate = new DataTemplate(() =>
		{
			var grid = new Grid();
			Label lb = new();
			lb.SetBinding(Label.TextProperty, new Binding("."));
			grid.Add(lb);
			return grid;
		});
        MBPC_CV.ItemTemplate = new DataTemplate(() =>
        {
            var grid = new Grid();
            Label lb = new();
            lb.SetBinding(Label.TextProperty, new Binding("."));
            grid.Add(lb);
            return grid;
        });
        for (int i = p.Count; i < 6; i++)
            PKMNUDs[i].IsVisible = PKMLabels[i].IsVisible = PKMHeldItems[i].IsVisible = false;
        if (Generation != 3)
        {
            for (int i = 0; i < PKMNUDs.Length; i++)
            {
                PKMNUDs[i].Text = $"{i}";
                PKMNUDs[i].IsReadOnly = true;
            }
        }
        switch (sav)
		{
			case SAV2 sav2:
                m = new Mail2[6 + 10];
                for (int i = 0; i < m.Length; i++)
                    m[i] = new Mail2(sav2, i);
                MBServed.Text = sav.Data[Mail2.GetMailboxOffset(sav.Language)].ToString();
                MailItemID = [0x9E, 0xB5, 0xB6, 0xB7, 0xB8, 0xB9, 0xBA, 0xBB, 0xBC, 0xBD];
                PartyBoxCount = 6;
                break;
        }
        for (int i = 0; i < PartyBoxCount; i++)
            PartyBoxList.Add(GetLBLabel(i));
        if (Generation == 2)
        {
            for (int i = PartyBoxCount, j = 0, boxsize = int.Parse(MBServed.Text); i < m.Length; i++, j++)
            {
                if (j < boxsize)
                    PCBoxList.Add(GetLBLabel(i));
            }
        }
        else
        {
            for (int i = PartyBoxCount; i < m.Length; i++)
                PCBoxList.Add(GetLBLabel(i));
        }
        MBPC_CV.ItemsSource = PCBoxList;
        MBP_CV.ItemsSource = PartyBoxList;
        for (int i = 0; i < p.Count; i++)
        {
            PKMLabels[i].Text = GetSpeciesNameFromCB(p[i].Species);
            int j = Array.IndexOf(MailItemID, p[i].HeldItem);
            PKMHeldItems[i].Text = j >= 0 ? GameInfo.Strings.GetItemStrings(sav.Context, sav.Version)[j + 1]! : "(not Mail)";
            if (Generation != 3)
                continue;
            int k = ((PK3)p[i]).HeldMailID;
            PKMNUDs[i].Text = k is >= -1 and <= 5 ? k.ToString() : "-1";
        }
        AppearPKMPicker.ItemsSource = GameInfo.FilteredSources.Species.ToList();
        AppearPKMPicker.ItemDisplayBinding = new Binding("Text");
        var vers = GameInfo.VersionDataSource
                .Where(z => ((GameVersion)z.Value).GetGeneration() == Generation);
        AuthorLang.ItemsSource = GameInfo.LanguageDataSource(sav.Generation).ToList();
        AuthorLang.ItemDisplayBinding = new Binding("Text");
        var ItemList = GameInfo.Strings.GetItemStrings(sav.Context, sav.Version);
        MailItemIDs.Add(ItemList[0]);
        foreach(var item in MailItemID)
            MailItemIDs.Add(ItemList[item]);
        MailTypePicker.ItemsSource = MailItemIDs;
        entry = -1;
        if (PartyBoxList.Count > 0)
            MBP_CV.SelectedItem = null;
    }

    private void EntryControl(object sender, SelectionChangedEventArgs e)
    {
        int partyindex = Array.IndexOf(PartyBoxList.ToArray(), MBP_CV.SelectedItem);
        int pcboxindex = Array.IndexOf(PCBoxList.ToArray(), MBPC_CV.SelectedItem);
        if(entry >= 0)
        {
            TempSave();
            if (GetLBLabel(entry) != loadedLBItemLabel)
                LoadList();

        }
        if (sender == MBP_CV && partyindex >= 0)
        {
            entry = partyindex;
            MBPC_CV.SelectedItem = null;
        }
        else if (sender == MBPC_CV && pcboxindex >= 0)
        {
            entry = PartyBoxCount + pcboxindex;
            MBP_CV.SelectedItem = null;
        }
        else
            entry = -1;
        if(entry >= 0)
        {
            LoadMail();
            loadedLBItemLabel = GetLBLabel(entry);
        }
    }
    private void LoadMail()
    {
        MailDetail mail = m[entry];
        AuthorOT.Text = mail.AuthorName;
        AuthorTID.Text = mail.AuthorTID.ToString();
        AuthorLang.SelectedItem = GameInfo.LanguageDataSource(SAV.Generation).FirstOrDefault(z=>z.Value == (int)mail.AuthorLanguage);
        MailTypePicker.SelectedIndex = MailTypeToCBIndex(mail);
        var species = mail.AppearPKM;
        if (Generation == 2)
        {
            AppearPKMPicker.SelectedItem = GameInfo.FilteredSources.Species.FirstOrDefault(z=>z.Value == (int)species);
            Message1.Text = mail.GetMessage(false);
            Message2.Text = mail.GetMessage(true);
            AuthorLang.IsEnabled = AuthorLang.SelectedItem is not (int)LanguageID.Japanese and not (int)LanguageID.Korean;
            UserEnteredCB.IsChecked = mail.UserEntered;
            return;
        }
    }
    private string GetLBLabel(int index) => m[index].IsEmpty != true ? $"{index}: From {m[index].AuthorName}" : $"{index}:  (empty)";
    private string GetSpeciesNameFromCB(int index)
    {
        var result = GameInfo.FilteredSources.Species.FirstOrDefault(z => z.Value == index);
        return result != null ? result.Text : "PKM";
    }
    private int MailTypeToCBIndex(MailDetail mail) => Generation <= 3 ? 1 + Array.IndexOf(MailItemID, mail.MailType) : (mail.IsEmpty == false ? 1 + mail.MailType : 0);
    private int CBIndexToMailType(int cbindex) => Generation <= 3 ? (cbindex > 0 ? MailItemID[cbindex - 1] : 0) : (cbindex > 0 ? cbindex - 1 : 0xFF);
    private void TempSave()
    {
        MailDetail mail = m[entry];
        mail.AuthorName = AuthorOT.Text;
        mail.AuthorTID = ushort.Parse(AuthorTID.Text);
        // ReSharper disable once ConstantNullCoalescingCondition
        mail.AuthorLanguage = (byte)((int?)AuthorLang.SelectedIndex ?? (int)LanguageID.English);
        mail.MailType = CBIndexToMailType(MailTypePicker.SelectedIndex);
        // ReSharper disable once ConstantNullCoalescingCondition
        var species = (ushort)((ComboItem)AppearPKMPicker.SelectedItem).Value;
        if (Generation == 2)
        {
            mail.AppearPKM = species;
            mail.SetMessage(Message1.Text, Message2.Text, UserEnteredCB.IsChecked);
            return;
        }
        
    }
    private void LoadList()
    {
        if (entry < PartyBoxCount) MakePartyList();
        else MakePCList();
    }

    private void MBServedValueChanged(object sender, TextChangedEventArgs e) => MakePCList();

    private void MakePartyList()
    {
        PartyBoxList.Clear();
        for (int i = 0; i < PartyBoxCount; i++)
            PartyBoxList.Add(GetLBLabel(i));
    }

    private void close(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private void MakePCList()
    {
        PCBoxList.Clear();
        if (Generation == 2)
        {
            var parsed = int.TryParse(MBServed.Text,out var result);
            if (parsed)
            {
                for (int i = PartyBoxCount, j = 0, boxsize = result; i < m.Length; i++, j++)
                {
                    if (j < boxsize)
                        PCBoxList.Add(GetLBLabel(i));
                }
            }
        }
        else
        {
            for (int i = PartyBoxCount; i < m.Length; i++)
                PCBoxList.Add(GetLBLabel(i));
        }
    }
    private void Save(object sender, EventArgs e)
    {
        switch (Generation)
        {
            case 2:
                foreach (var n in m) n.CopyTo(SAV);
                if (SAV is SAV2)
                {
                    // duplicate
                    int ofs = 0x600;
                    int len = Mail2.GetMailSize(SAV.Language) * 6;
                    Array.Copy(SAV.Data, ofs, SAV.Data, ofs + len, len);
                    ofs += len << 1;
                    SAV.Data[ofs] = byte.Parse(MBServed.Text);
                    len = (Mail2.GetMailSize(SAV.Language) * 10) + 1;
                    Array.Copy(SAV.Data, ofs, SAV.Data, ofs + len, len);
                }
                else if (SAV is SAV2Stadium)
                {
                    int ofs = Mail2.GetMailboxOffsetStadium2(SAV.Language);
                    SAV.Data[ofs] = byte.Parse(MBServed.Text);
                }
                break;
        }
        Navigation.PopModalAsync();
    }
}