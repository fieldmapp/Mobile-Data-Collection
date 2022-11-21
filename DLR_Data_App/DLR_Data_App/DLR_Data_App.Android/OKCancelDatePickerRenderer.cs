using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using com.DLR.DLR_Data_App.Droid;
using DlrDataApp.Modules.Base.Shared.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(OKCancelDatePicker), typeof(OKCancelDatePickerRenderer))]
namespace com.DLR.DLR_Data_App.Droid
{
    public class OKCancelDatePickerRenderer : DatePickerRenderer
    {
        // taken from https://stackoverflow.com/a/70342138 (altered)
        public OKCancelDatePickerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            //Disposing
            if (e.OldElement != null)
            {
                _element = null;
            }

            //Creating
            if (e.NewElement != null)
            {
                _element = e.NewElement as OKCancelDatePicker;
            }
        }

        protected OKCancelDatePicker _element;

        protected override DatePickerDialog CreateDatePickerDialog(int year, int month, int day)
        {
            // This mimics what the original renderer did.
            var dialog = new DatePickerDialog(Context, (o, e) =>
            {
                _element.Date = e.Date;
                ((IElementController)_element).SetValueFromRenderer(VisualElement.IsFocusedPropertyKey, false);
            }, year, month, day);

            // These use our custom actions when buttons pressed.
            dialog.SetButton((int)DialogButtonType.Positive, Context.Resources.GetString(global::Android.Resource.String.Ok), OnOk);
            dialog.SetButton((int)DialogButtonType.Negative, Context.Resources.GetString(global::Android.Resource.String.Cancel), OnCancel);

            return dialog;
        }

        private void OnCancel(object sender, DialogClickEventArgs e)
        {
            // This is what the original renderer did when Cancel pressed.
            _element.Unfocus();

            // This is our custom logic.
            _element.CallClosed(OKCancelDatePicker.CloseType.Cancel);
        }
        private void OnOk(object sender, DialogClickEventArgs e)
        {
            // This is what the original renderer did when OK pressed.
            _element.Date = ((DatePickerDialog)sender).DatePicker.DateTime;
            _element.Unfocus();

            // This is our custom logic.
            _element.CallClosed(OKCancelDatePicker.CloseType.Ok);
        }
    }
}