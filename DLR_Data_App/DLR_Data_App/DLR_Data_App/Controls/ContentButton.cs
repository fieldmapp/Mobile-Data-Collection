//Main contributors: Maximilian Enderling, Maya Koehnen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace DLR_Data_App.Controls
{
    /// <summary>
    /// View which works like a button but can have other views as children.
    /// </summary>
    public class ContentButton : ContentView
    {
        private readonly TapGestureRecognizer elementClicked;

        public ContentButton()
        {
            elementClicked = new TapGestureRecognizer();
            GestureRecognizers.Add(elementClicked);
        }

        protected override void OnChildAdded(Element child)
        {
            if (child is View childview)
                childview.GestureRecognizers.Add(elementClicked);
        }
        private static void CommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is ICommand command && bindable is ContentButton contentButton)
            {
                contentButton.elementClicked.Command = command;
            }
        }

        public event EventHandler Tapped
        {
            add => elementClicked.Tapped += value;
            remove => elementClicked.Tapped -= value;
        }
    }
}