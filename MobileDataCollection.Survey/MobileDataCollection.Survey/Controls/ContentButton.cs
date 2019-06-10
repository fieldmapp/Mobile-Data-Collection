using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace MobileDataCollection.Survey.Controls
{
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
            base.OnChildAdded(child);
            if (child is View childview)
            {
                childview.GestureRecognizers.Add(elementClicked);
                childview.GestureRecognizers.Add(new TapGestureRecognizer() {
                    Command = new Command(() => {childview.BackgroundColor = Color.LightGray; })
                });
            }
        }

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand),
            typeof(ContentButton), null, BindingMode.Default, null, CommandPropertyChanged);

        private static void CommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue is ICommand command && bindable is ContentButton contentButton)
            {
                contentButton.elementClicked.Command = command;
            }
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }
    }
}