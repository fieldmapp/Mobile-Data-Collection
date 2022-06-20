//Main contributors: Maximilian Enderling, Maya Koehnen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using static DLR_Data_App.Services.Helpers;

namespace DLR_Data_App.Controls
{
    /// <summary>
    /// View which works like a button but can have other views as children.
    /// </summary>
    public class ContentButton : Frame
    {
        private readonly TapGestureRecognizer ElementClicked;

        public ContentButton()
        {
            ElementClicked = new TapGestureRecognizer();
            GestureRecognizers.Add(ElementClicked);
        }

        protected override void OnChildAdded(Element child)
        {
            if (child is View childview)
                childview.GestureRecognizers.Add(ElementClicked);
        }

        public event EventHandler Clicked
        {
            add => ElementClicked.Tapped += value;
            remove => ElementClicked.Tapped -= value;
        }

        public void OnTap()
        {
            ElementClicked.RaiseEventViaReflection(nameof(ElementClicked.Tapped), EventArgs.Empty);
        }
    }
}