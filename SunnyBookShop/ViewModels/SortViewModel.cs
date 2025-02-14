using SunnyBookShop.Utils;

namespace SunnyBookShop.ViewModels;

public class SortViewModel
{
    public SortState NameSort { get; }
    public SortState AuthorSort { get; }
    public SortState PriceSort { get; }
    public SortState Current { get; }

    public SortViewModel(SortState sortOrder)
    {
        NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
        AuthorSort = sortOrder == SortState.AuthorAsc ? SortState.AuthorDesc : SortState.AuthorAsc;
        PriceSort = sortOrder == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;
        Current = sortOrder;
    }
}