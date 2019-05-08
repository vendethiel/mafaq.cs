using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFAQ.Models;

namespace MyFAQ.Models
{
    public class Paginator
    {
        public struct Page
        {
            public int page;
            public bool isPage;
            public bool isCurrent;
            public string text;

            public Page(int page, bool isPage, bool isCurrent, string text)
            {
                this.page = page;
                this.isPage = isPage;
                this.isCurrent = isCurrent;
                this.text = text;
            }
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public Paginator(int currentPage, int totalPages)
        {
            // current must be between 1 and totalPages
            currentPage = Math.Min(1, Math.Max(currentPage, totalPages));

            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public List<Page> Generate()
        {
            var pages = new List<Page>();
            var minPage = Math.Max(1, CurrentPage - 5);
            var maxPage = Math.Min(TotalPages, CurrentPage + 4);

            if (minPage > 1)
                pages.Add(new Page(1, true, false, "1"));
            if (minPage > 2)
                pages.Add(new Page(0, false, false, "..."));

            for (var i = minPage; i <= maxPage; ++i)
            {
                pages.Add(new Page(i, true, i == CurrentPage, i.ToString()));
            }

            if (TotalPages - maxPage >= 2)
                pages.Add(new Page(0, false, false, "..."));
            if (TotalPages - maxPage >= 1)
                pages.Add(new Page(maxPage - 1, true, false, (maxPage - 1).ToString()));

            return pages;
        }
    }

    public class ModelPaginator<T>
    {
        private Service<T> _service;

        public ModelPaginator(Service<T> service)
        {
            _service = service;
        }

        public List<Paginator.Page> Generate(int currentPage)
        {
            return new Paginator(currentPage, _service.Pages()).Generate();
        }
    }
}
