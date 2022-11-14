using chkam05.VisualPlayer.Controls.Data;
using chkam05.VisualPlayer.Controls.Static;
using System.Collections.Generic;

namespace chkam05.VisualPlayer.Pages
{
    public interface IPage
    {

        //  VARIABLES

        MenuItemType? SpecialMenu { get; }
        IPagesManager PagesManager { get; }

    }
}
