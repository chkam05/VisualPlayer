using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace chkam05.VisualPlayer.Data.Fonts
{
    public class FontsManager : INotifyPropertyChanged
    {

        //  CONST

        private static readonly Uri FONT_URI = new Uri("pack://application:,,,/Resources/Fonts/");


        //  EVENTS

        public event PropertyChangedEventHandler PropertyChanged;


        //  VARIABLES

        private static FontsManager _instance;

        private ObservableCollection<FontContainer> _fonts;


        //  GETTERS & SETTERS

        public static FontsManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FontsManager();

                return _instance;
            }
        }

        public FontContainer DefaultFont
        {
            get => Fonts.First(f => f.Name == "Segoe UI");
        }

        public ObservableCollection<FontContainer> Fonts
        {
            get => _fonts;
            set
            {
                _fonts = value;
                OnPropertyChanged(nameof(Fonts));
            }
        }

        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> FontsManager private class constructor. </summary>
        private FontsManager()
        {
            Fonts = new ObservableCollection<FontContainer>();

            LoadApplicationFonts();
            LoadSystemFonts();
        }

        #endregion CLASS METHODS

        #region GET METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Get font container by name. </summary>
        /// <param name="fontName"> Font name. </param>
        /// <returns> Font container. </returns>
        public FontContainer GetFontByName(string fontName)
        {
            var fontContainer = Fonts.FirstOrDefault(f => f.Name == fontName);
            return fontContainer != null ? fontContainer : DefaultFont;
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Get font container by name and sub name. </summary>
        /// <param name="fontName"> Font name. </param>
        /// <param name="subName"> Font sub name. </param>
        /// <returns> Font container. </returns>
        public FontContainer GetFontByNameAndSubName(string fontName, string subName)
        {
            var fontContainer = Fonts.FirstOrDefault(f => f.Name == fontName && f.SubName == subName);
            return fontContainer != null ? fontContainer : DefaultFont;
        }

        #endregion GET METHODS

        #region NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Method for invoking PropertyChangedEventHandler external method. </summary>
        /// <param name="propertyName"> Changed property name. </param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion NOTIFY PROPERTIES CHANGED INTERFACE METHODS

        #region SETUP METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Load fonts from application resources. </summary>
        private void LoadApplicationFonts()
        {
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#Digital dream Regular"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#Digital dream Narrow"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#Digital dream Fat Regular"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#Digital dream Fat Narrow"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#Digital dream Fat Skew Regular"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#Digital dream Fat Skew Narrow"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#Digital dream Skew Regular"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#Digital dream Skew Narrow"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Regular"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Condensed"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Expanded"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Condensed Italic"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Condensed Italic"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Expanded Italic"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Leftalic"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Straight Regular"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Straight Condensed"), true));
            Fonts.Add(new FontContainer(new FontFamily(FONT_URI, "./#LED Sled Straight Expanded"), true));
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Load fonts installed in system. </summary>
        private void LoadSystemFonts()
        {
            System.Windows.Media.Fonts.SystemFontFamilies
                .OrderBy(o => o.FamilyNames.First().Value)
                .ToList()
                .ForEach(f => Fonts.Add(new FontContainer(f)));
        }

        #endregion SETUP METHODS

    }
}
