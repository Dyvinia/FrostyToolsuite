using Frosty.Core;
using FrostySdk.IO;
using System.Windows;
using System.Windows.Media;

namespace GuidPlugin
{
    public class CopyGuidContextMenuItem : DataExplorerContextMenuExtension
    {

        public override string ContextItemName => "Copy GUID";

        public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyCore;component/Images/Copy.png") as ImageSource;

        public override RelayCommand ContextItemClicked => new RelayCommand((o) =>
        {
            EbxAsset asset = App.AssetManager.GetEbx(App.SelectedAsset);
            Clipboard.SetText(asset.RootInstanceGuid.ToString());
        });
    }

    public class LogGuidContextMenuItem : DataExplorerContextMenuExtension
    {

        public override string ContextItemName => "Log GUIDs";

        public override ImageSource Icon => new ImageSourceConverter().ConvertFromString("pack://application:,,,/FrostyEditor;component/Images/Properties.png") as ImageSource;

        public override RelayCommand ContextItemClicked => new RelayCommand((o) =>
        {
            EbxAsset asset = App.AssetManager.GetEbx(App.SelectedAsset);

            App.Logger.Log($"File GUID: {asset.FileGuid}");
            App.Logger.Log($"Root GUID: {asset.RootInstanceGuid}");
        });
    }
}
