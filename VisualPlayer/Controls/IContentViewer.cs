using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualPlayer.Pages.Base;

namespace VisualPlayer.Controls
{
    public interface IContentViewer
    {

        //  GETTERS & SETTERS

        bool CanGoBack { get; }
        BasePage CurrentPage { get; }
        int CurrentPageIndex { get; }
        int PagesCount { get; }


        //  METHODS

        void GoBack(int stepsBack = 1);

        bool HasPage(BasePage page);

        bool HasPageWithType(Type pageType);

        int IndexOfPage(BasePage page);

        void LoadPage(BasePage page);

    }
}
