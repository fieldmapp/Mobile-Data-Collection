using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace DlrDataApp.Modules.Base.Shared
{
    public interface IActiveElementInfo<T> where T:class
    {

        /// <summary>
        /// The Id should use the following Attributes:
        /// [PrimaryKey, AutoIncrement]
        /// </summary>
        int? Id { get; set; }

        /// <summary>
        /// The ActiveElementId MUST use the following Attribute:
        /// [ForeignKey(typeof(T))]
        /// </summary>
        int ActiveElementId { get; set; }

        /// <summary>
        /// The ActiveElement MUST use the following Attribute:
        /// [ManyToOne(CascadeOperations = CascadeOperation.CascadeRead)]
        /// </summary>
        T ActiveElement { get; set; }
    }
}
