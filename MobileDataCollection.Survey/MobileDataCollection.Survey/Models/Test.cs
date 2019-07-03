using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace MobileDataCollection.Survey.Models
{
    public class Test : IQuestionContent
    {
        public static readonly BindableProperty InternIdProperty = BindableProperty.Create(nameof(ID), typeof(int), typeof(AnswerImageCheckerPage), 0, BindingMode.OneWay);

        public Test()
        {
           
        }
        
        public int ID { get; set; }

        public int Difficulty => throw new NotImplementedException();
    }
}