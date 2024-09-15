using fs;
using NTDLS.Katzebase.Client.Types;
using static NTDLS.Katzebase.Client.KbConstants;

namespace NTDLS.Katzebase.Engine.Query.Sorting
{
    public class ResultValueComparer : IComparer<KbInsensitiveDictionary<fstring?>>
    {
        private readonly List<(string fieldName, KbSortDirection direction)> _sortingColumns;

        public ResultValueComparer(List<(string fieldName, KbSortDirection sortDirection)> sortingColumns)
        {
            _sortingColumns = sortingColumns;
        }

        public int Compare(KbInsensitiveDictionary<fstring?>? x, KbInsensitiveDictionary<fstring?>? y)
        {
            foreach (var (fieldName, sortDirection) in _sortingColumns)
            {
                //if (fieldName >= x?.Count || fieldName >= y?.Count)
                //    return 0;

                //int result = string.Compare(x?[fieldName], y?[fieldName], StringComparison.OrdinalIgnoreCase);
                int result = fstring.Compare(x?[fieldName], y?[fieldName]);

                if (result != 0)
                {
                    return sortDirection == KbSortDirection.Descending ? -result : result;
                }
            }

            return 0;
        }
    }
}
