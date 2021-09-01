﻿//Main contributors: Max Moebius, Henning Woydt
using DlrDataApp.Modules.Profiling.Shared.Localization;
using DlrDataApp.Modules.Base.Shared;
using DlrDataApp.Modules.Profiling.Shared.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DlrDataApp.Modules.Base.Shared.Localization;

namespace DlrDataApp.Modules.Profiling.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntrospectionPage : ContentPage, IProfilingPage
    {
        /// <summary>
        /// Bindings of QuestionItem and AnswerItem
        /// </summary>
        public static readonly BindableProperty QuestionItemProperty = BindableProperty.Create(nameof(QuestionItem), typeof(QuestionIntrospectionPage), typeof(IntrospectionPage), new QuestionIntrospectionPage(1, string.Empty), BindingMode.OneWay);
        public static readonly BindableProperty AnswerItemProperty = BindableProperty.Create(nameof(AnswerItem), typeof(AnswerIntrospectionPage), typeof(IntrospectionPage), null, BindingMode.OneWay);

        /// <summary>
        /// Item of the given Question
        /// </summary>
        public QuestionIntrospectionPage QuestionItem
        {
            get { return (QuestionIntrospectionPage)GetValue(QuestionItemProperty); }
            set { SetValue(QuestionItemProperty, value); }
        }

        /// <summary>
        /// Item of the corresponding Answer of the Question
        /// </summary>
        public AnswerIntrospectionPage AnswerItem
        {
            get { return (AnswerIntrospectionPage)GetValue(AnswerItemProperty); }
            set { SetValue(AnswerItemProperty, value); }
        }

        IQuestionContent IProfilingPage.QuestionItem => QuestionItem;

        IUserAnswer IProfilingPage.AnswerItem => AnswerItem;

        public IntrospectionPage(QuestionIntrospectionPage question)
        {
            InitializeComponent();
            QuestionLabel.BindingContext = this;
            QuestionItem = question;
            RadioButtonIndex = new Dictionary<Base.Shared.Controls.RadioButton, int>()
            {
                {Button1, 1},
                {Button2, 2},
                {Button3, 3},
                {Button4, 4},
                {Button5, 5}
            };
        }

        private void Button_Tapped(object sender, EventArgs e)
        {
            Button1.IsChecked = false;
            Button2.IsChecked = false;
            Button3.IsChecked = false;
            Button4.IsChecked = false;
            Button5.IsChecked = false;

            (sender as Base.Shared.Controls.RadioButton).IsChecked = true;
        }

        Dictionary<Base.Shared.Controls.RadioButton, int> RadioButtonIndex;

        public event EventHandler<PageResult> PageFinished;

        void OnWeiterButtonClicked(object sender, EventArgs e)
        {
            var selectedRadioButton = RadioButtonIndex.Keys.FirstOrDefault(r => r.IsChecked);
            if (selectedRadioButton == null)
            {
                DisplayAlert(ProfilingResources.hint, SharedResources.pleaseMakeSelection, SharedResources.ok);
                return;
            }

            AnswerItem = new AnswerIntrospectionPage(QuestionItem.InternId, RadioButtonIndex[selectedRadioButton]);
            PageFinished?.Invoke(this, PageResult.Continue);
        }

        void OnAuswertungButtonClicked(object sender, EventArgs e)
        {
            PageFinished?.Invoke(this, PageResult.Evaluation);
        }

        protected override bool OnBackButtonPressed()
        {
            PageFinished?.Invoke(this, PageResult.Abort);
            return true;
        }
    }
}