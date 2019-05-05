namespace Vhc.DataTransformer.Core.Utils
{
    public static class Extensions
    {
        public static string EncloseIn(this string source, string encloser)
            => $"{encloser}{source}{encloser}";

        public static string ToFormattedString(this System.TimeSpan timeSpan)
            => timeSpan.ToString(@"hh\:mm\:ss\.fff");

        public static IronPython.Runtime.PythonDictionary ToPythonDictionary(this System.Collections.Generic.IDictionary<string, object> dictionary)
        {
            var pyDictionary = new IronPython.Runtime.PythonDictionary();
            foreach (var pair in dictionary)
            {
                pyDictionary.Add(System.Collections.Generic.KeyValuePair.Create<object, object>(pair.Key.ToString(), pair.Value.ToString()));
            }
            return pyDictionary;
        }
    }
}