using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VisualPlayer.Data.ColorModels;
using VisualPlayer.ViewModels;

namespace VisualPlayer.Data.Configuration
{
    public class WindowConfig : BaseViewModel
    {

        //  VARIABLES

        private Point _position;
        private Size _size;


        //  GETTERS & SETTERS

        public Point Position
        {
            get => _position;
            set
            {
                UpdateProperty(ref _position, value);
                NotifyPropertyChanged(nameof(PositionX));
                NotifyPropertyChanged(nameof(PositionY));
            }
        }

        public Size Size
        {
            get => _size;
            set
            {
                UpdateProperty(ref _size, value);
                NotifyPropertyChanged(nameof(SizeWidth));
                NotifyPropertyChanged(nameof(SizeHeight));
            }
        }

        [JsonIgnore]
        public double PositionX
        {
            get => Position.X;
        }

        [JsonIgnore]
        public double PositionY
        {
            get => Position.Y;
        }

        [JsonIgnore]
        public double SizeWidth
        {
            get => Size.Width;
        }

        [JsonIgnore]
        public double SizeHeight
        {
            get => Size.Height;
        }


        //  METHODS

        #region CLASS METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> WindowConfig class constructor. </summary>
        [JsonConstructor]
        public WindowConfig(
            Point? position = null,
            Size? size = null)
        {
            Position = position ?? new Point(200, 200);
            Size = size ?? new Size(700, 500);
        }

        #endregion CLASS METHODS

        #region UPDATE METHODS

        //  --------------------------------------------------------------------------------
        /// <summary> Update position value. </summary>
        /// <param name="x"> Position X. </param>
        /// <param name="y"> Position Y. </param>
        public void UpdatePosition(double x, double y)
        {
            Position = new Point(x, y);
        }

        //  --------------------------------------------------------------------------------
        /// <summary> Update size value. </summary>
        /// <param name="width"> Width. </param>
        /// <param name="height"> Height. </param>
        public void UpdateSize(double width, double height)
        {
            Size = new Size(width, height);
        }

        #endregion UPDATE METHODS

    }
}
