using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Liv_in_paris
{
    /// <summary>
    /// Convertisseur qui transforme un entier (count) en visibilité.
    /// </summary>
    /// <remarks>
    /// Si la valeur est supérieure à 0, retourne <see cref="Visibility.Visible"/>, sinon <see cref="Visibility.Collapsed"/>.
    /// </remarks>
    public class CountToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un entier en visibilité.
        /// </summary>
        /// <param name="value">Valeur à convertir, attendue comme un <see cref="int"/> représentant un nombre d'éléments.</param>
        /// <param name="targetType">Type cible de la liaison (ici <see cref="Visibility"/>).</param>
        /// <param name="parameter">Paramètre optionnel de la conversion (non utilisé).</param>
        /// <param name="culture">Culture utilisée pour la conversion.</param>
        /// <returns>
        /// <see cref="Visibility.Visible"/> si le nombre est strictement supérieur à 0, sinon <see cref="Visibility.Collapsed"/>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int count = (int)value;
            return count > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Non implémenté. La conversion inverse n'est pas prise en charge.
        /// </summary>
        /// <param name="value">Valeur à convertir.</param>
        /// <param name="targetType">Type cible.</param>
        /// <param name="parameter">Paramètre optionnel.</param>
        /// <param name="culture">Culture utilisée.</param>
        /// <returns>Exception levée dans tous les cas.</returns>
        /// <exception cref="NotImplementedException">Toujours levée.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
