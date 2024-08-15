namespace CustomSorting;

public class CustomSorter : IComparer<Person>
{
    public int Compare(Person x, Person y)
    {
        int result = GetSurnameFromDisplayName(x.DisplayName).CompareTo(GetSurnameFromDisplayName(y.DisplayName));
        //Debug.WriteLine($"{x.DisplayName} - {y.DisplayName} = {result}");
        return result;
    }

    private static string GetSurnameFromDisplayName(string displayName)
    {
        if(displayName.Contains(','))
        {
            //surname first
            var parts = displayName.Split(',');
            if (parts.Length > 0)
                return parts[0].Trim();
        }
        else
        {
            //surname last
            var parts = displayName.Split(' ');
            //return last name
            return parts[parts.Length - 1].Trim();
        }

        return string.Empty;
    }
}
