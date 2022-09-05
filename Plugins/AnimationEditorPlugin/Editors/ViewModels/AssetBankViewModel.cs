using System;
using System.Collections.Generic;
using System.ComponentModel;
using AnimationEditorPlugin.Managers;
using Frosty.Core;
using Frosty.Core.Controls;

namespace AnimationEditorPlugin.Editors.ViewModels
{
    public class AssetBankViewModel : INotifyPropertyChanged
    {
        public List<AssetBankFileEntry> Entries => m_entries;
        public object Data
        {
            get => m_data;
            set
            {
                if (m_data != value)
                {
                    m_data = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public RelayCommand OnDoubleClickedCommand => new RelayCommand(AssetExplorerDoubleClicked);
        
        private object m_data;
        private readonly List<AssetBankFileEntry> m_entries;
        private AssetBankFileEntry m_selectedAsset;
        
        private FrostyBaseEditor m_owner;
        
        public AssetBankViewModel(FrostyBaseEditor inOwner)
        {
            m_owner = inOwner;
            
            m_entries = new List<AssetBankFileEntry>();
            foreach (AssetBankFileEntry entry in App.AssetManager.EnumerateCustomAssets("assetbank"))
            {
                m_entries.Add(entry);
            }
        }

        private void AssetExplorerDoubleClicked(object args)
        {
        }
        
        #region -- INotifyPropertyChanged --
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}