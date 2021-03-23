using AvtoPro.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AvtoPro.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly SearchProcessor searchProcessor;
        private int progress;
        private int saveProgress;
        private bool saveCompleted;
        private string searchStatus;
        private string saveStatus;
        private string searchReqestMessage;
        private List<SearchRequest> failedSearchReqests;

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel(SearchProcessor searchProcessor)
        {
            Files = new ObservableCollection<FileItemViewModel>();
            var codeBase = Assembly.GetExecutingAssembly().CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);
            var directory = Path.GetDirectoryName(path);

            OutputFile = $"{directory}\\SearchResult{DateTime.UtcNow:_yyyy_MM_dd_HH_mm_ss}.xlsx";
            SelectFiles = new RelayCommand(OnSelectFiles);
            RemoveFile = new RelayCommand(OnRemoveFile);
            Search = new RelayCommand(OnSearch);
            CanceleSearch = new RelayCommand(OnCanceleSearch);
            OpenFile = new RelayCommand(OnOpenFile);
            SkipCurrentReqest = new RelayCommand(OnSkipCurrentRequest);
            Retry = new RelayCommand(OnRetry);
            SearchForFailedResults = new RelayCommand(OnSearchForFailedResults);
            SearchEnabled = Files.Any();
            this.searchProcessor = searchProcessor;
            searchProcessor.ProgressChagned += SearchProcessor_ProgressChagned;
            searchProcessor.SearchCompleted += SearchProcessor_SearchCompleted;
            ExcelFile.SaveProgressChagned += ExcelFile_SaveProgressChagned;
            ExcelFile.SaveCompleted += ExcelFile_SaveCompleted;
            SearchStatus = "Not started";
            SaveStatus = "Not started";
            searchProcessor.SearchReqestChanged += SearchProcessor_ReqestChanged;
            SearchReqestMessage = "Not started";

        }

        private void OnSelectFiles(object @object)
        {
            var openDialog = new OpenFileDialog { Filter = "Excel File (*.xlsx)|*.xlsx" };
            var openResult = openDialog.ShowDialog();
            if (openResult ?? true)
            {
                foreach (var path in openDialog.FileNames)
                {
                    Files.Add(new FileItemViewModel { FileName = path, RemoveItem = RemoveFile });
                }
            }
            SearchEnabled = Files.Any();
        }
        private void OnRemoveFile(object @object)
        {
            var fileItem = @object as FileItemViewModel;
            if (fileItem != null)
            {
                Files.Remove(fileItem);
            }
            SearchEnabled = Files.Any();
        }
        private void OnSearch(object @object)
        {
            FailedSearchReqests = null;
            searchProcessor.Search(Files);
            SaveCompleted = false;
            SaveStatus = "Not started";
        }
        private void OnCanceleSearch(object @object)
        {
            searchProcessor.CanceleSearch();
        }
        private void OnOpenFile(object @object)
        {
            try
            {
                Process.Start(OutputFile);
            }
            catch
            {
            }
        }
        private void OnSkipCurrentRequest(object @object)
        {
            searchProcessor.SkipCurrentReqest();
        }
        private void OnRetry(object @object)
        {
            searchProcessor.Retry();
        }
        private void OnSearchForFailedResults(object @object)
        {
            searchProcessor.Search(FailedSearchReqests);
            FailedSearchReqests = null;
            SaveCompleted = false;
            SaveStatus = "Not started";
        }
        private void SearchProcessor_SearchCompleted(List<SearchResult> results, bool isCancelled, bool updateExistingResult)
        {
            FailedSearchReqests = results.Where(r => !r.IsSuccessful).Select(result => result as SearchRequest).ToList();
            
            if (updateExistingResult)
            {
                if (isCancelled)
                {
                    SearchStatus = "Cancelled";
                    SearchStatus = ExcelFile.UpdateFailedResults(results, OutputFile);
                    Progress = 0;
                }
                else
                {
                    SearchStatus = "Saving results...";
                    SearchStatus = ExcelFile.UpdateFailedResults(results, OutputFile);
                }
            }
            else
            {
                if (isCancelled)
                {
                    SearchStatus = "Cancelled";
                    SearchStatus = ExcelFile.SaveResults(results, OutputFile);
                    Progress = 0;
                }
                else
                {
                    SearchStatus = "Saving results...";
                    SearchStatus = ExcelFile.SaveResults(results, OutputFile);
                }
            }
            
        }
        private void SearchProcessor_ProgressChagned(int progress)
        {
            SearchStatus = $"Searching... {progress}%";
            Progress = progress;
        }
        private void SearchProcessor_ReqestChanged(string searchReqestMessage)
        {
            SearchReqestMessage = $"{SearchReqestMessage}";
            SearchReqestMessage = searchReqestMessage;
        }
        private void ExcelFile_SaveProgressChagned(int saveProgress)
        {
            SaveProgress = saveProgress;
            SaveStatus = $"Savinging... {saveProgress}%";
        }
        private void ExcelFile_SaveCompleted()
        {
            SaveCompleted = true;
            SaveStatus = $"Saved";
            SaveProgress = 0;
        }

        public ObservableCollection<FileItemViewModel> Files { get; set; }

        public string OutputFile { get; set; }

        public bool SearchEnabled { get; set; }

        public ICommand SelectFiles { get; set; }

        public ICommand OpenFile { get; set; }

        public ICommand RemoveFile { get; set; }

        public ICommand Search { get; set; }

        public ICommand CanceleSearch { get; set; }

        public ICommand SkipCurrentReqest { get; set; }

        public ICommand Retry { get; set; }

        public ICommand SearchForFailedResults { get; set; }

        public string SearchReqestMessage 
        { 
            get => searchReqestMessage;
            set 
            {
                if (searchReqestMessage != value)
                {
                    searchReqestMessage = value;
                    if (PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SearchReqestMessage)));
                    }
                }
            
            }
        }

        public List<SearchRequest> FailedSearchReqests 
        {
            get => failedSearchReqests;
            set 
            {
                if (failedSearchReqests != value)
                {
                    failedSearchReqests = value;
                    if (PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(FailedSearchReqests)));
                    }
                }
            }
        }

        public string SearchStatus
        {
            get => searchStatus;
            set
            {
                if (searchStatus != value)
                {
                    searchStatus = value;
                    if (PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SearchStatus)));
                    }
                }
            }
        }
        public int SaveProgress
        {
            get => saveProgress;
            set
            {
                if (saveProgress != value)
                {
                    saveProgress = value;
                    if (PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SaveProgress)));
                    }
                }
            }
        }
        public bool SaveCompleted
        {
            get => saveCompleted;
            set
            {
                if (saveCompleted != value)
                {
                    saveCompleted = value;
                    if (PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SaveCompleted)));
                    }
                }
            }
        }
        public string SaveStatus
        {
            get => saveStatus;
            set
            {
                if (saveStatus != value)
                {
                    saveStatus = value;
                    if (PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(SaveStatus)));
                    }
                }
            }
        }
        public int Progress
        {
            get => progress;
            set
            {
                if (progress != value)
                {
                    progress = value;
                    if (PropertyChanged != null)
                    {
                        this.PropertyChanged(this, new PropertyChangedEventArgs(nameof(Progress)));
                    }
                }
            }
        }
        public int Timeout 
        {
            get { return searchProcessor.timeout; }
            set { searchProcessor.timeout = value; }
        }
    }
}
