using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace DlrDataApp.Modules.Base.Shared.Services
{
    /// <summary>
    /// Provides static Methods and Markers to support user-friendly typing of <see cref="FormattedString"/>.
    /// Behaves similar to Markdown.
    /// </summary>
    /// <remarks>Only supports bold text at the moment</remarks>
    public static class FormattedStringSerializerHelper
    {
        /// <summary>
        /// Represents the boundaries of a part of a FormattedString which is bold
        /// </summary>
        public const string BoldMarker = "*";

        /// <summary>
        /// Represents a linebreak/NewLine.
        /// </summary>
        public const string NewLineMarker = "\\n";


        /// <summary>
        /// Deserializes a FormattedString using the <see cref="BoldMarker"/> and <see cref="NewLineMarker"/>
        /// </summary>
        /// <remarks>Only supports bold text at the moment</remarks>
        public static FormattedString StringWithAnnotationsToFormattedString(string annotatedInput)
        {

            if (string.IsNullOrWhiteSpace(annotatedInput))
                return new FormattedString();

            annotatedInput = annotatedInput.Replace(NewLineMarker, Environment.NewLine);
            var markerIndices = annotatedInput.AllIndicesOf(BoldMarker);
            int prevPartEndIndex = 0;
            var result = new FormattedString();

            bool bold = false;
            foreach (var markerIndex in markerIndices.Concat(new int[] { annotatedInput.Length }))
            {
                var part = annotatedInput.Substring(prevPartEndIndex, markerIndex - prevPartEndIndex);
                if (!string.IsNullOrWhiteSpace(part))
                {
                    var span = new Span { Text = part };
                    if (bold)
                        span.FontAttributes = FontAttributes.Bold;

                    result.Spans.Add(span);
                }

                bold = !bold;
                prevPartEndIndex = markerIndex + 1;
            }

            return result;
        }

        /// <summary>
        /// Serializes a FormattedString using the <see cref="BoldMarker"/> and <see cref="NewLineMarker"/>
        /// </summary>
        /// <remarks>Only supports bold text at the moment</remarks>
        public static string FormattedStringToAnnotatedString(FormattedString formattedString)
        {
            if (formattedString == null)
                return string.Empty;

            StringBuilder builder = new StringBuilder();
            foreach (var span in formattedString.Spans)
            {
                if (span.FontAttributes.HasFlag(FontAttributes.Bold))
                    builder.Append(BoldMarker);
                builder.Append(span.Text);
                if (span.FontAttributes.HasFlag(FontAttributes.Bold))
                    builder.Append(BoldMarker);
            }
            return builder.ToString().Replace(Environment.NewLine, NewLineMarker);
        }
    }
}
