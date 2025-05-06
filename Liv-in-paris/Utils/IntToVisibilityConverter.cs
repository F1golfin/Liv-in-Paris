using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Liv_in_paris
{
    /// <summary>
    /// Convertisseur qui transforme un entier en une valeur de visibilité WPF.
    /// </summary>
    /// <remarks>
    /// Si la valeur est un entier supérieur à 0, retourne <see cref="Visibility.Visible"/>, sinon <see cref="Visibility.Collapsed"/>.
    /// </remarks>
    public class IntToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convertit une valeur entière en <see cref="Visibility"/>.
        /// </summary>
        /// <param name="value">Valeur à convertir, attendue comme un <see cref="int"/>.</param>
        /// <param name="targetType">Type cible de la liaison (normalement <see cref="Visibility"/>).</param>
        /// <param name="parameter">Paramètre optionnel (non utilisé).</param>
        /// <param name="culture">Culture utilisée pour la conversion.</param>
        /// <returns><see cref="Visibility.Visible"/> si la valeur est un entier strictement positif, sinon <see cref="Visibility.Collapsed"/>.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int count)
                return count > 0 ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        /// <summary>
        /// Non implémenté. La conversion inverse n'est pas supportée.
        /// </summary>
        /// <param name="value">Valeur à convertir en retour.</param>
        /// <param name="targetType">Type cible.</param>
        /// <param name="parameter">Paramètre optionnel.</param>
        /// <param name="culture">Culture utilisée.</param>
        /// <returns>Exception levée dans tous les cas.</returns>
        /// <exception cref="NotImplementedException">Toujours levée.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
