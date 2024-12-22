
using PKHeX.Core;

namespace PKHeXMAUI;

public partial class TrainerEditor6Maison : ContentPage
{
    public SAV6 SAV = (SAV6)MainPage.sav;
    private readonly Entry[] MaisonRecords;
	public TrainerEditor6Maison()
	{
		InitializeComponent();
		MaisonRecords = [MSiNEntry,MSiSEntry,MDNEntry,MDSEntry,MTNEntry,MTSEntry,MRNEntry,MRSEntry,MMNEntry,MMSEntry,
						 MBSiNEntry,MBSiSEntry,MBDNEntry,MBDSEntry,MBTNEntry,MBTSEntry,MBRNEntry,MBRSEntry,MBMNEntry,
						 MBMSEntry];
        if (SAV is ISaveBlock6Main xyao)
        {
            for (int i = 0; i < MaisonBlock.MaisonStatCount; i++)
                MaisonRecords[i].Text = xyao.Maison.GetMaisonStat(i).ToString();
        }
    }
    public void SaveMaison()
    {
        if (SAV is ISaveBlock6Main xyao)
        {
            for (int i = 0; i < MaisonBlock.MaisonStatCount; i++)
                xyao.Maison.SetMaisonStat(i, ushort.Parse(MaisonRecords[i].Text));
        }
    }
}