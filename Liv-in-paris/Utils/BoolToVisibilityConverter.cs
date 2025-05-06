using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Liv_in_paris
{
    /// <summary>
    /// Convertisseur permettant de transformer un booléen en une valeur de visibilité WPF.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convertit une valeur booléenne en <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">Valeur à convertir (doit être un booléen).</param>
        /// <param name="targetType">Type cible (normalement <see cref="Visibility"/>).</param>
        /// <param name="parameter">Paramètre optionnel de la conversion.</param>
        /// <param name="culture">Culture utilisée pour la conversion.</param>
        /// <returns><see cref="Visibility.Visible"/> si le booléen est vrai, sinon <see cref="Visibility.Collapsed"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Conversion inverse non prise en charge.
        /// </summary>
        /// <param name="value">Valeur à convertir.</param>
        /// <param name="targetType">Type cible.</param>
        /// <param name="parameter">Paramètre de conversion.</param>
        /// <param name="culture">Culture utilisée pour la conversion.</param>
        /// <returns>Exception levée dans tous les cas.</returns>
        /// <exception cref="NotImplementedException">Toujours levée.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}