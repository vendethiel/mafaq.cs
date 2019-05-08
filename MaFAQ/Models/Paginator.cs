using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaFAQ.Models
{
    public static class Paginator
    {
        private static int PER_PAGE = 20;
        static public IEnumerable<T> Paginate<T>(this IEnumerable<T> el, int page)
        {
            page = Math.Max(1, page); // page must be at least one
            page--; // page 1 is actually page 0
            return el.Skip(page * PER_PAGE).Take(PER_PAGE);
        }

        static public int Pages(int count)
        {
            if (count < 1)
                return 1;
            return 1 + (int)Math.Floor((double)count / PER_PAGE);
        }
    }
}
