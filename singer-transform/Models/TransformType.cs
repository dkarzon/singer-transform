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
        /// Adds a new static field to the output
        /// </summary>
        AddStaticField,
        /// <summary>
        /// Renames a given stream (useful for renaming database tables between tap and target)
        /// </summary>
        RenameStream
    }
}
