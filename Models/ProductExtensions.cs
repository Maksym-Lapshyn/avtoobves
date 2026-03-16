using System.Globalization;

namespace Avtoobves.Models
{
    public static class ProductExtensions
    {
        private static bool IsUkrainian =>
            CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "uk";

        public static string LocalizedName(this Product product) =>
            IsUkrainian && !string.IsNullOrWhiteSpace(product.NameUk)
                ? product.NameUk
                : product.Name;

        public static string LocalizedDescription(this Product product) =>
            IsUkrainian && !string.IsNullOrWhiteSpace(product.DescriptionUk)
                ? product.DescriptionUk
                : product.Description;
    }
}
