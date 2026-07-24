using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MassageBookingApp.Mobile.Models;
using System.Collections.ObjectModel;

namespace MassageBookingApp.Mobile.ViewModels
{
    public partial class PagingGenericViewModel<T> : ObservableObject
    {
        private List<T> allPagingItems = new();

        public ObservableCollection<T> PagedItems { get; } = new();
        public ObservableCollection<int> PageNumbers { get; } = new();
        public ObservableCollection<PagingItem> PagingItems { get; } = new();

        private readonly Func<T, string>? itemLabelSelector;

        [ObservableProperty]
        private int totalPages;

        [ObservableProperty]
        private int currentPage = 1;

        [ObservableProperty]
        private bool canGoNext;

        [ObservableProperty]
        private bool canGoPrevious;

        [ObservableProperty]
        private string pageDisplayText = string.Empty;

        [ObservableProperty]
        private int totalItemsCount;

        [ObservableProperty]
        private int selectedPage = 1;

        [ObservableProperty]
        private int pageWindow = 10;

        public IRelayCommand GoToFirstCommand { get; }
        public IRelayCommand GoToLastCommand { get; }
        public IRelayCommand NextPageCommand { get; }
        public IRelayCommand PreviousPageCommand { get; }
        public IRelayCommand<int> JumpToPageCommand { get; }

        public PagingGenericViewModel(IList<T> allItems, Func<T, string>? labelSelector = null)
        {
            itemLabelSelector = labelSelector;
            allPagingItems = [.. allItems];
            GoToFirstCommand = new RelayCommand(GoToFirst);
            GoToLastCommand = new RelayCommand(GoToLast);
            NextPageCommand = new RelayCommand(NextPage);
            PreviousPageCommand = new RelayCommand(PreviousPage);
            JumpToPageCommand = new RelayCommand<int>(OnJumpToPage);
            SelectedPage = CurrentPage;
            UpdatePagingState();
        }

        public T? GetItemByPage(int page)
        {
            if (page < 1 || page > allPagingItems.Count)
            {
                return default;
            }

            return allPagingItems[page - 1];
        }


        public void UpdateAllItems(IEnumerable<T> newAllItems)
        {
            allPagingItems = newAllItems.ToList();
            TotalItemsCount = allPagingItems.Count;
            TotalPages = allPagingItems.Count == 0 ? 0 : allPagingItems.Count;

            if (TotalPages == 0)
            {
                CurrentPage = 1;
                SelectedPage = 1;
            }
            else if (CurrentPage > TotalPages)
            {
                CurrentPage = TotalPages;
                SelectedPage = CurrentPage;
            }

            UpdatePagingState();
        }

        private void UpdatePagingState()
        {
            UpdatePagedItems();
            UpdatePagingItems();
            UpdateNavigationState();
        }

        private void UpdatePagedItems()
        {
            PagedItems.Clear();

            if (allPagingItems.Count == 0 || CurrentPage < 1 || CurrentPage > allPagingItems.Count)
            {
                return;
            }

            var item = allPagingItems[CurrentPage - 1];
            PagedItems.Add(item);
        }
        private void UpdatePagingItems()
        {
            PagingItems.Clear();

            if (TotalPages == 0)
            {
                PageDisplayText = "No pages";
                return;
            }

            int start = Math.Max(1, CurrentPage - PageWindow / 2);
            int end = Math.Min(TotalPages, start + PageWindow - 1);

            if ((end - start + 1) < PageWindow)
            {
                start = Math.Max(1, end - PageWindow + 1);
            }

            for (int i = start; i <= end; i++)
            {
                var item = GetItemByPage(i);
                var displayText = item != null && itemLabelSelector != null
                    ? itemLabelSelector(item)
                    : i.ToString();

                PagingItems.Add(new PagingItem
                {
                    Value = i,
                    DisplayText = displayText
                });
            }

            SelectedPage = CurrentPage;
            PageDisplayText = $"Page {CurrentPage} of {TotalPages}";
        }

        private void UpdatePageNumbers()
        {
            PageNumbers.Clear();

            if (TotalPages == 0)
            {
                PageDisplayText = "No pages";
                return;
            }

            int start = Math.Max(1, CurrentPage - PageWindow / 2);
            int end = Math.Min(TotalPages, start + PageWindow - 1);

            if ((end - start + 1) < PageWindow)
            {
                start = Math.Max(1, end - PageWindow + 1);
            }

            for (int i = start; i <= end; i++)
            {
                PageNumbers.Add(i);
            }

            SelectedPage = CurrentPage;
            PageDisplayText = $"Page {CurrentPage} of {TotalPages}";
        }

        private void UpdateNavigationState()
        {
            CanGoPrevious = CurrentPage > 1;
            CanGoNext = CurrentPage < TotalPages;
        }

        private void GoToFirst()
        {
            if (TotalPages == 0)
            {
                return;
            }

            CurrentPage = 1;
            UpdatePagingState();
        }

        private void GoToLast()
        {
            if (TotalPages == 0)
            {
                return;
            }

            CurrentPage = TotalPages;
            UpdatePagingState();
        }

        private void NextPage()
        {
            if (!CanGoNext)
            {
                return;
            }

            CurrentPage++;
            UpdatePagingState();
        }

        private void PreviousPage()
        {
            if (!CanGoPrevious)
            {
                return;
            }

            CurrentPage--;
            UpdatePagingState();
        }

        private void OnJumpToPage(int page)
        {
            if (page < 1 || page > TotalPages)
            {
                return;
            }

            CurrentPage = page;
            UpdatePagingState();
        }

        partial void OnSelectedPageChanged(int value)
        {
            if (value != CurrentPage && value >= 1 && value <= TotalPages)
            {
                CurrentPage = value;
                UpdatePagingState();
            }
        }
    }
}