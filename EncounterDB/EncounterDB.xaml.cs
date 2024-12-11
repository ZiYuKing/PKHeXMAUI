
#nullable disable

using PKHeX.Core;
using System.Runtime.CompilerServices;
using static PKHeXMAUI.MainPage;

namespace PKHeXMAUI;

public partial class EncounterDB : ContentPage
{
    public static PKHeX.Core.Searching.SearchSettings encSettings;
    public EncounterDB()
    {
        InitializeComponent();
        EncounterCollection.ItemTemplate = new DataTemplate(() =>
        {
            Grid grid = new() { Padding = 10 };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            Image image = new() { WidthRequest = 50, HeightRequest = 50 };
            Image shinysp = new() { Source = "rare_icon.png", WidthRequest = 16, HeightRequest = 16, VerticalOptions = LayoutOptions.Start };
            shinysp.TranslateTo(shinysp.TranslationX + 15, shinysp.TranslationY);
            image.SetBinding(Image.SourceProperty, "url");
            shinysp.SetBinding(Image.IsVisibleProperty, "EncounterInfo.IsShiny");
            Image EggSprite = new() { Source = "a_egg.png", HeightRequest=40, WidthRequest=40, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.End };
            EggSprite.SetBinding(Image.IsVisibleProperty, "EncounterInfo.IsEgg");
            Image AlphaSprite = new() { Source = "ribbonmarkalpha.png", HeightRequest = 40, WidthRequest = 40, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Start };
            AlphaSprite.SetBinding(Image.IsVisibleProperty, "Alpha");
            Image BallSprite = new() { HeightRequest = 16, WidthRequest = 16, VerticalOptions = LayoutOptions.End, HorizontalOptions = LayoutOptions.Start };
            BallSprite.SetBinding(Image.SourceProperty, "BallUrl");
            BallSprite.TranslateTo(TranslationX - 10, TranslationY);
            Image MightySprite = new() { HeightRequest = 40, WidthRequest = 40, Source = "ribbonmarkmightiest.png", VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.Center };
            MightySprite.SetBinding(Image.IsVisibleProperty, "Mighty");
            Image GMaxSprite = new() { HeightRequest = 16, WidthRequest = 16, Source = "dyna.png", VerticalOptions = LayoutOptions.Start, HorizontalOptions = LayoutOptions.Center };
            GMaxSprite.SetBinding(Image.IsVisibleProperty, "GMax");
            grid.SetBinding(Grid.BackgroundColorProperty, "EncColor");
            grid.Add(MightySprite);
            grid.Add(AlphaSprite);
            grid.Add(image);
            grid.Add(shinysp);
            grid.Add(EggSprite);
            grid.Add(BallSprite);
            grid.Add(GMaxSprite);
            var tap = new TapGestureRecognizer();
            tap.SetBinding(TapGestureRecognizer.CommandParameterProperty, "itsself");
            tap.Tapped += ShowViewBox;
            grid.GestureRecognizers.Add(tap);
            return grid;
        });
        EncounterCollection.ItemsLayout = new GridItemsLayout(5, ItemsLayoutOrientation.Vertical);
    }
    private async void ShowViewBox(object sender, TappedEventArgs e)
    {
        EncounterCollection.SelectedItem = (EncounterSprite)e.Parameter;
        var view = await DisplayAlert("View Encounter", "View this encounter?", "view", "cancel");
        if (view)
            applyencpk(sender, e);
        EncounterCollection.SelectedItem = null;
    }
    private void SetSearchSettings(object sender, EventArgs e)
    {
        Navigation.PushModalAsync(new SearchSettings());
    }

    public void applyencpk(object sender, EventArgs e)
    {
        IEncounterInfo enc = ((EncounterSprite)EncounterCollection.SelectedItem).EncounterInfo;
        var pkm = enc.ConvertToPKM(sav, EncounterCriteria.Unrestricted);
        pk = EntityConverter.ConvertToType(pkm, sav.PKMType, out _);
        if (pk.Species == (ushort)Species.Manaphy && pk.IsEgg)
            pk.IsNicknamed = false;
    }
    private void SearchEncountersClick(object sender, EventArgs e)
    {
        List<EncounterSprite> sprites = [];
        var Encounters = GetEncounters();
       foreach(var enc in Encounters)
        {
            sprites.Add(new(enc));
        }
        EncounterCollection.ItemsSource = sprites;
    }
    public List<IEncounterInfo> GetEncounters()
    {
        if (encSettings is { Species: 0, Moves.Count: 0 } || encSettings is null)
            return [];
        var pk = sav.BlankPKM;

        var moves = encSettings.Moves.ToArray();
        var versions = encSettings.GetVersions(sav);
        var species = new[] { encSettings.Species };
        var results = GetAllSpeciesFormEncounters(species, sav.Personal, versions, moves, pk);
        if (encSettings.SearchEgg == true)
            results = results.Where(z => z.IsEgg == encSettings.SearchEgg);
        if (encSettings.SearchShiny == true)
            results = results.Where(z => z.IsShiny == encSettings.SearchShiny);
        var comparer = new ReferenceComparer<IEncounterInfo>();
        results = results.Distinct(comparer);
        if (EncounterSettings.FilterUnavailableSpecies)
        {
            static bool IsPresentInGameSV(ISpeciesForm pk) => PersonalTable.SV.IsPresentInGame(pk.Species, pk.Form);
            static bool IsPresentInGameSWSH(ISpeciesForm pk) => PersonalTable.SWSH.IsPresentInGame(pk.Species, pk.Form);
            static bool IsPresentInGameBDSP(ISpeciesForm pk) => PersonalTable.BDSP.IsPresentInGame(pk.Species, pk.Form);
            static bool IsPresentInGameLA(ISpeciesForm pk) => PersonalTable.LA.IsPresentInGame(pk.Species, pk.Form);
            results = sav switch
            {
                SAV9SV => results.Where(IsPresentInGameSV),
                SAV8SWSH => results.Where(IsPresentInGameSWSH),
                SAV8BS => results.Where(IsPresentInGameBDSP),
                SAV8LA => results.Where(IsPresentInGameLA),
                _ => results.Where(z => z.Generation <= 7),
            };
        }
        return results.ToList();
    }
    public IEnumerable<IEncounterInfo> GetAllSpeciesFormEncounters(IEnumerable<ushort> species, IPersonalTable pt, IReadOnlyList<GameVersion> versions, ushort[] moves, PKM pk)
    {
        var returnlist = new List<IEncounterInfo>();
        foreach (var s in species)
        {
            var pi = pt.GetFormEntry(s, 0);
            var fc = pi.FormCount;

            for (byte f = 0; f < fc; f++)
            {
                if (FormInfo.IsBattleOnlyForm(s, f, pk.Format))
                    continue;
                var encs = GetEncounters(s, f, moves, pk, versions);
                returnlist.AddRange(encs);
            }
        }
        return returnlist;
    }
    private IEnumerable<IEncounterInfo> GetEncounters(ushort species, byte form, ushort[] moves, PKM pk, IReadOnlyList<GameVersion> vers)
    {
        pk.Species = species;
        pk.Form = form;
        pk.SetGender(pk.GetSaneGender());
        EncounterMovesetGenerator.OptimizeCriteria(pk, sav);
        return EncounterMovesetGenerator.GenerateEncounters(pk, moves, vers);
    }
}
public class ReferenceComparer<T> : IEqualityComparer<T> where T : class
{
    public bool Equals(T x, T y)
    {
        if (x == null)
            return false;
        if (y == null)
            return false;
        return RuntimeHelpers.GetHashCode(x).Equals(RuntimeHelpers.GetHashCode(y));
    }

    public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
}
public class EncounterSprite
{
    public EncounterSprite itsself { get; set; }
    public IEncounterInfo EncounterInfo { get; set; }
    public string url { get; set; }
    public int[] NoFormSpriteSpecies = [664, 665, 744, 982, 855, 854, 869, 892, 1012, 1013];
    public bool Alpha { get; set; }
    public string BallUrl { get; set; }
    public bool Mighty { get; set; }
    public bool GMax { get; set; }
    public Color EncColor { get; set; }
    public EncounterSprite(IEncounterInfo info)
    {
        EncounterInfo = info;
        var index = (info.GetType().Name.GetHashCode() * 0x43FD43FD);
        EncColor = Color.FromArgb(Convert.ToString(index,16));
        if (EncColor == Colors.Black || EncColor == null)
            EncColor = Colors.Transparent;
        Mighty = info is EncounterMight9;
        Alpha = info is IAlphaReadOnly { IsAlpha: true };
        GMax = info is IGigantamaxReadOnly { CanGigantamax:true };
        if(info is IFixedBall { FixedBall: not Ball.None }b)
        {
            BallUrl = $"ball{(int)b.FixedBall}.png";
        }
        url = info.Species == 0
            ? ""
            : $"a_{info.Species}{((info.Form > 0 && !NoFormSpriteSpecies.Contains(info.Species)) ? $"_{info.Form}" : "")}.png";
        itsself = this;
    }
}