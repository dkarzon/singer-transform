using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingerTransform.Models
{
    public enum TransformType
    {
        /// <summary>
        /// Renames a given stream (useful for renaming database tables between tap and target)
        /// </summary>
        RenameStream,
        /// <summary>
        /// Adds a new field as a hash of an existing field
        /// </summary>
        AddHashId,
        /// <summary>
        /// Added a new field with a calculated value using Octostache syntax
        /// </summary>
        CalculatedField,
        /// <summary>
        /// Renames a field in a stream
        /// </summary>
        RenameField,
        /// <summary>
        /// Formats a string date as a regular date (useful for Google Analytics date formats)
        /// </summary>
        FormatDate
    }
}
