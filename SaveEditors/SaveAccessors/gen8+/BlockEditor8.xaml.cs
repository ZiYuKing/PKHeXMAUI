
using CommunityToolkit.Maui.Storage;
using PKHeX.Core;
using System.Security.Cryptography;
namespace PKHeXMAUI
{
    public partial class BlockEditor8 : ContentPage
    {
        private readonly ISCBlockArray SAV;
        private readonly SCBlockMetadata Metadata;
        public static ComboItem[] SortedBlockKeys = [];

        private SCBlock CurrentBlock = null!;

        public BlockEditor8(ISCBlockArray sav)
        {
            InitializeComponent();
            SAV = sav;
            Metadata = new SCBlockMetadata(sav.Accessor, [], []);
            SortedBlockKeys = Metadata.GetSortedBlockKeyList().ToArray();
            BlockKey_Picker.ItemSource = SortedBlockKeys;
            BlockKey_Picker.TextChanged += (s, e) => BlockDataFilter.GetMatchingIndexes(s, (TextChangedEventArgs)e);
        }

        private void Update_BlockCV(object sender, EventArgs e)
        {
            if (BlockKey_Picker.SelectedItem != null)
            {
                var key = (uint)((ComboItem)BlockKey_Picker.SelectedItem).Value;
                CurrentBlock = SAV.Accessor.GetBlock(key);
                UpdateBlockSummaryControls();
            }
            else
            {
                BlockStack.Clear();
                BlockSummary.Text = string.Empty;
            }
        }
        private void UpdateBlockSummaryControls()
        {
            BlockStack.Clear();
            BlockEditor_Hex.IsVisible = true;
            BlockSummary.IsVisible = true;
            BlockSummary.Text = $"Block Detail:\n{SCBlockUtil.GetBlockSummary(CurrentBlock)}";
            var block = CurrentBlock;
            var blockName = Metadata.GetBlockName(block, out var obj);
            BlockEditor_Hex.Text = string.Join(" ", block.Data.Select(z => $"{z:X2}"));
            if(CurrentBlock.Type.IsBoolean())
            {
                BlockEditor_Hex.IsVisible = false;
                var CB_TypeToggle = new comboBox
                {
                    ItemSource = new[]
                {
                    new ComboItem(nameof(SCTypeCode.Bool1), (int)SCTypeCode.Bool1),
                    new ComboItem(nameof(SCTypeCode.Bool2), (int)SCTypeCode.Bool2),
                },
                    SelectedIndex = (int)CurrentBlock.Type - 1
                };
                CB_TypeToggle.SelectedIndexChanged += CB_TypeToggle_SelectionChanged;
                BlockStack.Add(CB_TypeToggle);
            }
            if (obj != null)
            {
                var props = ReflectUtil.GetPropertiesCanWritePublicDeclared(obj.GetType());
                if (props.Count() > 1)
                {
                    int row = 0;
                    BlockEditor_Hex.IsVisible = false;
                    foreach (var prop in props)
                    {
                        var propLabel = new Label() { Text = prop };
                        BlockStack.Add(propLabel, 0, row);
                        var BlockEntry = new Entry
                        {
                            BindingContext = obj
                        };
                        try { BlockEntry.SetBinding(Entry.TextProperty, prop, BindingMode.TwoWay); }
                        catch (Exception) { BlockStack.Remove(propLabel); continue; }
                        BlockStack.Add(BlockEntry, 1, row);
                        row++;
                    }
                    return;
                }
            }
            var o = SCBlockMetadata.GetEditableBlockObject(block);
            if (o != null)
            {
                var props = ReflectUtil.GetPropertiesPublic(o.GetType());
                if (props.Count() > 1)
                {
                    int row = 0;
                    BlockEditor_Hex.IsVisible = false;
                    foreach (var prop in props)
                    {
                        BlockStack.Add(new Label() { Text = prop }, 0, row);
                        var BlockEntry = new Entry
                        {
                            BindingContext = o
                        };
                        BlockEntry.SetBinding(Entry.TextProperty, prop, BindingMode.TwoWay);
                        BlockStack.Add(BlockEntry, 1, row);
                        row++;
                    }
                    return;
                }
            }
        }

        private void CB_TypeToggle_SelectionChanged(object? sender, EventArgs? e)
        {
            var block = CurrentBlock;
            var cType = block.Type;
            var cValue = (SCTypeCode?)((ComboItem?)((comboBox?)sender)?.SelectedItem)?.Value??0;
            if (cType == cValue)
                return;
            block.ChangeBooleanType(cValue);
            UpdateBlockSummaryControls();
        }

        private void CloseBlockEditor8(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private async void ExportBlocksFolder(object sender, EventArgs e)
        {
            var FolderResults = await FolderPicker.PickAsync(CancellationToken.None);
            if (FolderResults.IsSuccessful)
            {
                var path = FolderResults.Folder.Path;
                var blocks = SAV.AllBlocks;
                ExportAllBlocks(blocks, path);
            }
        }
        private static void ExportAllBlocks(IEnumerable<SCBlock> blocks, string path)
        {
            foreach (var b in blocks.Where(z => z.Data.Length != 0))
            {
                var fn = $"{SCBlockUtil.GetBlockFileNameWithoutExtension(b)}.bin";
                var fileName = Path.Combine(path, fn);
                File.WriteAllBytes(fileName, b.Data);
            }
        }

        private async void ImportBlocksFolder(object sender, EventArgs e)
        {
            var FolderResults = await FolderPicker.PickAsync(CancellationToken.None);
            if (FolderResults.IsSuccessful)
            {
                var failed = SCBlockUtil.ImportBlocksFromFolder(FolderResults.Folder.Path,SAV);
                if(failed.Count != 0)
                {
                    var msg = string.Join(Environment.NewLine, failed);
                    await DisplayAlert("Failed", $"Failed to import: {msg}", "cancel");
                }
            }
        }

        private void ExportCurrentBlock_Clicked(object sender, EventArgs e) => ExportSelectBlock(CurrentBlock);

        private async void ExportSelectBlock(SCBlock block)
        {
            var name = SCBlockUtil.GetBlockFileNameWithoutExtension(block);
            await using var BlockStreams = new MemoryStream(block.Data);
            var result = await FileSaver.SaveAsync($"{name}.bin", BlockStreams, CancellationToken.None);
            if (result.IsSuccessful)
                await DisplayAlert("Success", $"Block File saved at {result.FilePath}", "cancel");
            else
                await DisplayAlert("Failure", $"Block File did not save due to {result.Exception.Message}", "cancel");
        }

        private void ImportCurrentBlock_Clicked(object sender, EventArgs e) => ImportSelectBlock(CurrentBlock);
        private async void ImportSelectBlock(SCBlock blockTarget)
        {
            var Pickedfile = await FilePicker.PickAsync();
            if (Pickedfile is null)
                return;
            var data = blockTarget.Data;
            var path = Pickedfile.FileName;
            var file = new FileInfo(path);
            if(file.Length != data.Length)
            {
                await DisplayAlert("Error", string.Format(MessageStrings.MsgFileSize, $"0x{file.Length:X8}"), "cancel");
                return;
            }
            var newdata = File.ReadAllBytes(path);
            blockTarget.ChangeData(newdata);
        }

        private async void ExportAllSingleFile(object sender, EventArgs e)
        {
            var folder = await FolderPicker.PickAsync(CancellationToken.None);
            if (!folder.IsSuccessful)
                return;
            var path = folder.Folder.Path;
            var blocks = SAV.Accessor.BlockInfo;
            var option = GetExportOption();
            SCBlockUtil.ExportAllBlocksAsSingleFile(blocks, path, option);
        }
        private SCBlockExportOption GetExportOption()
        {
            var option = SCBlockExportOption.None;
            if (CHK_DataOnly.IsChecked)
                option |= SCBlockExportOption.DataOnly;
            if (CHK_Key.IsChecked)
                option |= SCBlockExportOption.Key;
            if (CHK_Type.IsChecked)
                option |= SCBlockExportOption.TypeInfo;
            if (CHK_FakeHeader.IsChecked)
                option |= SCBlockExportOption.FakeHeader;
            return option;
        }
    }
    public static class BlockDataFilter
    {
        public static void GetMatchingIndexes(object? sender, TextChangedEventArgs? filterInfo)
        {
            comboBox source = (comboBox?)sender??new();
            List<int> filteredlist = [];
            List<ComboItem> SourceList = [.. ((ComboItem[])source.ItemSource)];
            var text = filterInfo?.NewTextValue??"";
            if (text.Length == 8)
            {
                var hex = (int)Util.GetHexValue(text);
                if (hex != 0)
                {
                    // Input is hexadecimal number, select the item
                    filteredlist.Add(BlockEditor8.SortedBlockKeys.ToList().IndexOf(BlockEditor8.SortedBlockKeys.ToList().Find(z => z.Value == hex)??new ComboItem("",0)));
                    source.ItemSource = filteredlist;
                    return;
                }
            }
            filteredlist.AddRange(from ComboItem item in BlockEditor8.SortedBlockKeys where item.Text.Contains(filterInfo?.NewTextValue??"", StringComparison.InvariantCultureIgnoreCase) select BlockEditor8.SortedBlockKeys.ToList().IndexOf(item));
            source.ItemSource= filteredlist;
            return;
        }
    }
}