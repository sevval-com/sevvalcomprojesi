namespace Sevval.Application.Utilities;

public class NameFilter
{
    public static string FilterName(string name, string surname)
    {
        if (string.IsNullOrEmpty(name)) return "";

        string title = string.Empty;

        title = name;
        if (string.IsNullOrEmpty(surname))
            title = title + " " + surname;

        if (title.Length > 2 && title.Length < 6) return title[0] + "." + title[title.Length - 1];

        if (title.Length > 5) return title[0] + "..." + title[title.Length - 1];

        return "....";
    }
}
