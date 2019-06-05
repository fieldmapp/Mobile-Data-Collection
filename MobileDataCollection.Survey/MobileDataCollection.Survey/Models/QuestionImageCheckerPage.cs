using System;
using System.Collections.Generic;
using System.Text;

namespace MobileDataCollection.Survey.Models
{
    class QuestionImageCheckerPage
    {
        /// <summary>
        /// Maximum count of selected answers allowed
        /// </summary>
        public int NumberOfPossibleAnswers { get; set; } = 4;

        /// <summary>
        /// Reflects whether image 1 should be selected in a correct answer.
        /// </summary>
        public bool Image1Correct { get; set; }

        /// <summary>
        /// Reflects whether image 2 should be selected in a correct answer.
        /// </summary>
        public bool Image2Correct { get; set; }

        /// <summary>
        /// Reflects whether image 3 should be selected in a correct answer.
        /// </summary>
        public bool Image3Correct { get; set; }

        /// <summary>
        /// Reflects whether image 4 should be selected in a correct answer.
        /// </summary>
        public bool Image4Correct { get; set; }

        /// <summary>
        /// Represents the URI of Image 2
        /// </summary>
        public string Image1Source { get; set; }

        /// <summary>
        /// Represents the URI of Image 2
        /// </summary>
        public string Image2Source { get; set; }

        /// <summary>
        /// Represents the URI of Image 3
        /// </summary>
        public string Image3Source { get; set; }

        /// <summary>
        /// Represents the URI of Image 4
        /// </summary>
        public string Image4Source { get; set; }

        public QuestionImageCheckerPage(bool im1Correct, bool im2Correct, bool im3Correct, bool im4Corect, 
            string im1Source, string im2Source, string im3Source, string im4Source)
        {
            Image1Correct = im1Correct;
            Image2Correct = im2Correct;
            Image3Correct = im3Correct;
            Image4Correct = im4Corect;
            Image1Source = im1Source;
            Image2Source = im2Source;
            Image3Source = im3Source;
            Image4Source = im4Source;
        }
    }
}
