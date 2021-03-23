using AvtoPro.Models;
using AvtoPro.Rest.Controllers;
using AvtoPro.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace AvtoPro
{
    public class SearchProcessor
    {
        private bool RetryOnCanceled;
        public int timeout = 30000;
        public string currentSearchRqestMessage;
        private readonly SearchController searchController;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationTokenSource currentReqestCancelationTokenSource;
        public delegate void OnSearchCompleted(List<SearchResult> results, bool isCancelled, bool updateExistingResults);
        public event OnSearchCompleted SearchCompleted;
        public delegate void OnProgressChanged(int progress);
        public event OnProgressChanged ProgressChagned;
        public delegate void OnSearchReqestChanged(string searchReqestMessage);
        public event OnSearchReqestChanged SearchReqestChanged;

        public SearchProcessor(SearchController searchController)
        {
            this.searchController = searchController;
        }

        public void Search(IEnumerable<FileItemViewModel> files)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            
            var searchRequests = ExcelFile.GetSearchRequests(files.Select(file => file.FileName).ToList())
                .Where(readResult => readResult.Success)
                .SelectMany(readResult => readResult.Requests)
                .ToList();

            Search(searchRequests, cancellationTokenSource.Token, false);
        }

        public void Search(List<SearchRequest> failedSearchReqests)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = new CancellationTokenSource();
            Search(failedSearchReqests, cancellationTokenSource.Token, true);
        }

        private void Search(List<SearchRequest> searchRequests, CancellationToken cancellationToken, bool updateExistingResults)
        {
            Task.Run(() =>
            {
                ProgressChagned(0);
                var listParser = new ListParser();
                var resutls = new List<SearchResult>();
                var counter = 1;
                int reqestIndex;

                try
                {
                    for (reqestIndex = 0; reqestIndex < searchRequests.Count; reqestIndex++)
                    {
                        currentReqestCancelationTokenSource?.Cancel();
                        currentReqestCancelationTokenSource?.Dispose();
                        currentReqestCancelationTokenSource = new CancellationTokenSource();

                        var currentRequestToken = currentReqestCancelationTokenSource.Token;

                        var result = new SearchResult(searchRequests[reqestIndex])
                        {
                            Brand = searchRequests[reqestIndex].Brand,
                            Id = searchRequests[reqestIndex].Id,
                            IsSuccessful = false,
                            IsSkiped = false,
                            IsCanceled = false
                        };
                        SearchReqestChanged(currentSearchRqestMessage = $" Part# - {result.Id} Brand - {result.Brand}");
                        if (cancellationToken.IsCancellationRequested)
                        {
                            result.IsSkiped = true;
                            currentReqestCancelationTokenSource.Cancel();
                            resutls.Add(result);

                            ProgressChagned(counter * 100 / searchRequests.Count);
                            counter++;
                            continue;
                        }
                        resutls.Add(result);
                        try
                        {
                            if (currentRequestToken.IsCancellationRequested)
                            {
                                result.IsSkiped = true;
                                result.IsCanceled = true;
                            }
                            var searchResult = searchController.Search(searchRequests[reqestIndex].Id, timeout, currentRequestToken);
                            var matchinSuggestion = searchResult.Suggestions.FirstOrDefault(suggestion =>
                            suggestion.FoundPart.Part.Brand.Name.Equals(searchRequests[reqestIndex].Brand, StringComparison.InvariantCultureIgnoreCase)
                            && (suggestion.FoundPart.Part.FullNr.Equals(searchRequests[reqestIndex].Id, StringComparison.InvariantCultureIgnoreCase)
                            || suggestion.FoundPart.Part.ShortNr.Equals(searchRequests[reqestIndex].Id, StringComparison.InvariantCultureIgnoreCase)));
                            if (matchinSuggestion != null)
                            {
                                var tableRows = listParser.ParseList(matchinSuggestion.Uri);
                                if (tableRows != null)
                                {
                                    result.IsSuccessful = true;

                                    var orderedRows = tableRows.Where(row => row.Brand.Equals(searchRequests[reqestIndex].Brand)
                                                                            && row.Id.Equals(searchRequests[reqestIndex].Id)
                                                                            && Regex.Match(row.DeliveryText, "(В наличии)|(Сегодня)").Success
                                                                            && double.TryParse(row.Price, out _))
                                                               .OrderBy(row => double.Parse(row.Price));
                                    if (orderedRows.Count() > 1)
                                    {
                                        result.SecondDeliveryText = orderedRows.ElementAt(1).DeliveryText;
                                        result.SecondPrice = orderedRows.ElementAt(1).Price;
                                    }
                                    if (orderedRows.Count() > 0)
                                    {
                                        result.FirstDeliveryText = orderedRows.ElementAt(0).DeliveryText;
                                        result.FirstPrice = orderedRows.ElementAt(0).Price;
                                    }
                                }
                            }
                        }
                        catch
                        {
                            result.IsSuccessful = false;
                        }

                        ProgressChagned(counter * 100 / searchRequests.Count);

                        counter++;

                        if (currentRequestToken.IsCancellationRequested && RetryOnCanceled)
                        {
                            reqestIndex--;
                            RetryOnCanceled = false;
                        }
                    }

                }
                finally
                {
                    currentReqestCancelationTokenSource?.Dispose();
                    currentReqestCancelationTokenSource = null;
                }
                SearchCompleted(resutls, cancellationToken.IsCancellationRequested, updateExistingResults);
            });
        }
        
        public void SkipCurrentReqest()
        {
            currentReqestCancelationTokenSource?.Cancel();
        }

        public void CanceleSearch()
        {
            cancellationTokenSource?.Cancel();
            currentReqestCancelationTokenSource?.Cancel();
        }
        public void Retry()
        {
            RetryOnCanceled = true;
            currentReqestCancelationTokenSource?.Cancel();
        }
    }
}
