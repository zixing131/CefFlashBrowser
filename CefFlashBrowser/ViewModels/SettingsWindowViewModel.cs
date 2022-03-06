﻿using CefFlashBrowser.FlashBrowser;
using CefFlashBrowser.Models;
using CefFlashBrowser.Models.Data;
using CefFlashBrowser.Utils;
using CefFlashBrowser.Views;
using CefFlashBrowser.Views.Dialogs.JsDialogs;
using SimpleMvvm;
using SimpleMvvm.Command;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;

namespace CefFlashBrowser.ViewModels
{
    public class SettingsWindowViewModel : ViewModelBase
    {
        public DelegateCommand SetNavigationTypeCommand { get; set; }
        public DelegateCommand SetSearchEngineCommand { get; set; }
        public DelegateCommand DeleteCacheCommand { get; set; }
        public DelegateCommand PopupAboutCefCommand { get; set; }
        public DelegateCommand SetNewPageBehaviorCommand { get; set; }

        public ObservableCollection<EnumDescription<SearchEngine.Engine>> SearchEngines { get; set; }
        public ObservableCollection<EnumDescription<NavigationType>> NavigationTypes { get; set; }
        public ObservableCollection<EnumDescription<NewPageBehavior>> NewPageBehaviors { get; set; }

        public int CurrentNavigationTypeIndex
        {
            get
            {
                var li = (from i in EnumDescriptions.GetNavigationTypes() select i.Value).ToList();
                return li.IndexOf(GlobalData.Settings.NavigationType);
            }
        }

        public int CurrentSearchEngineIndex
        {
            get
            {
                var li = (from i in EnumDescriptions.GetSearchEngines() select i.Value).ToList();
                return li.IndexOf(GlobalData.Settings.SearchEngine);
            }
        }

        public int CurrentNewPageBehaviorIndex
        {
            get
            {
                var li = (from i in EnumDescriptions.GetNewPageBehaviors() select i.Value).ToList();
                return li.IndexOf(GlobalData.Settings.NewPageBehavior);
            }
        }

        public bool DisableOnBeforeUnloadDialog
        {
            get => GlobalData.Settings.DisableOnBeforeUnloadDialog;
            set
            {
                GlobalData.Settings.DisableOnBeforeUnloadDialog = value;
                RaisePropertyChanged();
            }
        }

        private void SetNavigationType(NavigationType type)
        {
            GlobalData.Settings.NavigationType = type;
        }

        private void SetSearchEngine(SearchEngine.Engine engine)
        {
            GlobalData.Settings.SearchEngine = engine;
        }

        private void DeleteDirectory(string path)
        {
            new PathInfo(PathInfo.PathType.Directory, path).Delete();
        }

        private void DeleteCache()
        {
            JsConfirmDialog.Show(LanguageManager.GetString("message_deleteCache"), "", result =>
            {
                if (result == true)
                {
                    while (true)
                    {
                        try
                        {
                            CefSharp.Cef.Shutdown();
                            DeleteDirectory(FlashBrowserBase.CachePath);
                            break;
                        }
                        catch (Exception e)
                        {
                            var tmp = System.Windows.MessageBox.Show(
                                string.Format("{0}\n\n{1}:\n{2}", LanguageManager.GetString("error_deleteCachesRetry"), LanguageManager.GetString("error_message"), e.Message),
                                LanguageManager.GetString("title_error"), System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Error);

                            if (tmp == System.Windows.MessageBoxResult.No)
                            {
                                break;
                            }
                        }
                    }

                    Process.Start(Process.GetCurrentProcess().MainModule.FileName);
                    System.Windows.Application.Current.Shutdown();
                }
            });
        }

        private void PopupAboutCef()
        {
            BrowserWindow.Show("chrome://version/");
        }

        private void SetNewPageBehavior(NewPageBehavior newPageBehavior)
        {
            GlobalData.Settings.NewPageBehavior = newPageBehavior;
        }

        public SettingsWindowViewModel()
        {
            NavigationTypes = new ObservableCollection<EnumDescription<NavigationType>>(EnumDescriptions.GetNavigationTypes());
            SearchEngines = new ObservableCollection<EnumDescription<SearchEngine.Engine>>(EnumDescriptions.GetSearchEngines());
            NewPageBehaviors = new ObservableCollection<EnumDescription<NewPageBehavior>>(EnumDescriptions.GetNewPageBehaviors());

            SetNavigationTypeCommand = new DelegateCommand<NavigationType>(SetNavigationType);
            SetSearchEngineCommand = new DelegateCommand<SearchEngine.Engine>(SetSearchEngine);
            DeleteCacheCommand = new DelegateCommand(DeleteCache);
            PopupAboutCefCommand = new DelegateCommand(PopupAboutCef);
            SetNewPageBehaviorCommand = new DelegateCommand<NewPageBehavior>(SetNewPageBehavior);
        }
    }
}
