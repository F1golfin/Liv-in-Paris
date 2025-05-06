using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Liv_in_paris
{
    /// <summary>
    /// Convertisseur permettant de transformer un tableau de bytes représentant une image en <see cref="BitmapImage"/>.
    /// </summary>
    public class ByteArrayToImageSourceConverter : IValueConverter
    {
        /// <summary>
        /// Convertit un tableau de bytes en <see cref="BitmapImage"/>.
        /// </summary>
        /// <param name="value">Objet à convertir, attendu comme un tableau de bytes représentant une image.</param>
        /// <param name="targetType">Type cible de la liaison (ici <see cref="BitmapImage"/>).</param>
        /// <param name="parameter">Paramètre facultatif (non utilisé).</param>
        /// <param name="culture">Culture utilisée pour la conversion.</param>
        /// <returns>
        /// Une instance de <see cref="BitmapImage"/> si la conversion réussit,
        /// sinon une image par défaut située à <c>/images/no-image.png</c>.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is byte[] bytes && bytes.Length > 0)
            {
                try
                {
                    using var stream = new MemoryStream(bytes);
                    var image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = stream;
                    image.EndInit();
                    return image;
                }
                catch
                {
                    // ignored
                }
            }
            return new BitmapImage(new Uri("pack://application:,,,/images/no-image.png"));
        }

        /// <summary>
        /// Non implémenté. La conversion inverse (Image vers tableau de bytes) n'est pas supportée.
        /// </summary>
        /// <param name="value">Valeur à convertir.</param>
        /// <param name="targetType">Type cible.</param>
        /// <param name="parameter">Paramètre optionnel.</param>
        /// <param name="culture">Culture utilisée.</param>
        /// <returns>Exception levée dans tous les cas.</returns>
        /// <exception cref="NotImplementedException">Toujours levée.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
