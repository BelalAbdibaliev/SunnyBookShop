using SunnyBookShop.Models;

namespace SunnyBookShop.ViewModels;

public class BookIndexViewModel
{
    public IEnumerable<Book> Books { get; }
    public PageViewModel PageViewModel { get; }
    public SortViewModel SortViewModel { get; }

    public BookIndexViewModel(IEnumerable<Book> books, PageViewModel pageViewModel, SortViewModel sortViewModel)
    {
        Books = books;
        PageViewModel = pageViewModel;
        SortViewModel = sortViewModel;
    }
}