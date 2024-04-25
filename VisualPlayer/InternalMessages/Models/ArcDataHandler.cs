using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using VisualPlayer.ViewModels;

namespace VisualPlayer.InternalMessages.Models
{
    public class ArcDataHandler : BaseViewModel
    {

        //  VARIABLES

        private Point _startPoint;
        private Point _endPoint;
        private bool _isLargeArc;
        private Size _size;


        //  GETTERS & SETTERS

        public Point StartPoint
        {
            get => _startPoint;
            set => UpdateProperty(ref _startPoint, value);
        }

        public Point EndPoint
        {
            get => _endPoint;
            set => UpdateProperty(ref _endPoint, value);
        }

        public bool IsLargeArc
        {
            get => _isLargeArc;
            set => UpdateProperty(ref _isLargeArc, value);
        }

        public Size Size
        {
            get => _size;
            set => UpdateProperty(ref _size, value);
        }

    }
}
