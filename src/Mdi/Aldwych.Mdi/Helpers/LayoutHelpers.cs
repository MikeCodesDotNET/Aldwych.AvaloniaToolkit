using System.Text.RegularExpressions;

namespace Aldwych.Mdi.Helpers
{
    internal static class LayoutHelpers
    {
        public static double GridColumnWidthCoarse { get; set; }

        public static double GridColumnWidthFine { get; set; }

        public static double GridRowHeightCoarse { get; set; }

        public static double GridRowHeightFine { get; set; }


        private static string[] removableWords = new string[] { "ViewModel", "View", "Model", "Controller", "Service", "Manager" };

        public static string SanitizeTypeName(string input)
        {

            var camelCaseSplit = Regex.Split(input, @"(?<!^)(?=[A-Z])");
            var outputStr = string.Empty;

            foreach (var s in camelCaseSplit)
            {
                outputStr = $"{outputStr}{s} ";
            }

            string cleaned = Regex.Replace(outputStr, "\\b" + string.Join("\\b|\\b", removableWords) + "\\b", "");
            return cleaned.Trim();
        }
    }
}
