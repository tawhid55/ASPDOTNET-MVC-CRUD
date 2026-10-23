namespace EmployeeManagement.Helpers
{
    public static class DisplayHelpers
    {
        public static string Initials(string name)
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                return "?";
            }

            if (parts.Length == 1)
            {
                return parts[0].Substring(0, 1).ToUpperInvariant();
            }

            return $"{parts[0].Substring(0, 1)}{parts[parts.Length - 1].Substring(0, 1)}".ToUpperInvariant();
        }

        public static int Tone(string value)
        {
            var sum = 0;

            foreach (var character in value)
            {
                sum += character;
            }

            return (sum % 5) + 1;
        }
    }
}
