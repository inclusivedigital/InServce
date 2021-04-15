using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace InService.Lib
{
    public static class LibExtensions
    {
        private static Random rng = new Random();
        public static bool IsNumeric(this DataTypes dataType) =>
            dataType == DataTypes.INTEGER ||
            dataType == DataTypes.REAL_NUMBER;
        public static bool IsString(this DataTypes dataType) =>
            dataType == DataTypes.SINGLE_LINE_TEXT ||
            dataType == DataTypes.PHONE_NUMBER ||
            dataType == DataTypes.NAME ||
            dataType == DataTypes.EMAIL ||
            dataType == DataTypes.MULTI_LINE_TEXT ||
            dataType == DataTypes.URL ||
            dataType == DataTypes.VAT_NUMBER ||
            dataType == DataTypes.CITY ||
            dataType == DataTypes.COUNTRY ||
            dataType == DataTypes.ENTITY_NAME ||
            dataType == DataTypes.PLACE_NAME ||
            dataType == DataTypes.STREET_ADDRESS;
        public static bool IsText(this DataTypes dataType) =>
            dataType == DataTypes.SINGLE_LINE_TEXT ||
            dataType == DataTypes.NAME ||
            dataType == DataTypes.MULTI_LINE_TEXT ||
            dataType == DataTypes.VAT_NUMBER ||
            dataType == DataTypes.CITY ||
            dataType == DataTypes.COUNTRY ||
            dataType == DataTypes.ENTITY_NAME ||
            dataType == DataTypes.PLACE_NAME ||
            dataType == DataTypes.STREET_ADDRESS;
        public static bool IsAddress(this DataTypes dataType) =>
            dataType == DataTypes.CITY ||
            dataType == DataTypes.COUNTRY ||
            dataType == DataTypes.PLACE_NAME ||
            dataType == DataTypes.STREET_ADDRESS;
        public static bool IsDate(this DataTypes dataType) => dataType == DataTypes.DATE;
        public static bool IsURL(this DataTypes dataType) => dataType == DataTypes.URL;
        public static bool IsEmail(this DataTypes dataType) => dataType == DataTypes.EMAIL;
        public static bool IsPhoneNumber(this DataTypes dataType) => dataType == DataTypes.PHONE_NUMBER;
        public static bool IsBoolean(this DataTypes dataType) => dataType == DataTypes.BOOLEAN;


        public static bool IsOptions(this DataTypes dataType) =>
            dataType == DataTypes.OPTIONS ||
            dataType == DataTypes.MULTI_SELECT_OPTIONS;
        public static bool IsFile(this DataTypes dataType) =>
            dataType == DataTypes.THUMBNAIL;// ||
                                            //dataType == DataTypes.BANNER;


        public static string ToEnumString(this Enum @enum)
           => @enum.ToString().Replace("_", " ").Trim();

        public static string ToEnumString(this string value)
       => value.Replace("_", " ").Trim();
        public static string AsEnum(this string value) => value?.ToUpper().Replace("  ", "__").Replace(" ", "_");

        public const string urlRegEx = @"((http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)";

        public static string ReplaceUrlsWithLinks(this string input)
        {
            if (input == null) return "";
            Regex rx = new Regex(urlRegEx, RegexOptions.IgnoreCase);
            if (input != null) input = input.Replace("\n", "<br />");
            string result = rx.Replace(input, delegate (Match match)
            {
                string url = match.ToString();
                return string.Format("<a target='_blank' href=\"{0}\">{0}</a>", url);
            });
            return result;
        }

        public static string BasicFormat(this DateTime dateTime) => dateTime.ToString("dd MMM yyy");
        public static string BasicFormatWithTime(this DateTime dateTime) => dateTime.ToString("dd MMM yyy HH:mm");

        public static string FirstCharToUpper(this string input)
        {
            switch (input)
            {
                case null: throw new ArgumentNullException(nameof(input));
                case "": throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input));
                default: return input.First().ToString().ToUpper() + input.Substring(1).ToLower();
            }
        }

        public static string ProperlyGreet(this DateTime Value)
        {
            if (Value.Hour < 12) return "Good morning";
            else if (Value.Hour > 12 && Value.Hour < 17) return "Good afternoon";
            else return "Good evening";
        }

        public static string RemoveWhitespace(this string input)
        {
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }

        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }

        public static IList<T> Shuffled<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }
    }
}
