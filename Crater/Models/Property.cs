using System;
using System.Collections.Generic;
using System.Linq;

namespace Crater.Models
{
    public abstract class Property
    {
        public Property(string name)
        {
            Name = name;
            Values = new List<string>();
        }

        /// <summary>
        /// Effectively the name of all instances of a particular property class.
        /// It is what's serialized in markdown to identify a property as being a given type.
        /// </summary>
        /// <remarks>
        /// Would like to make this a static property but that is not yet supported in .NET.
        /// </remarks>
        public abstract string Identifier { get; }
        public string Name { get; set; }
        public List<string> Values { get; protected set; }

        /// <summary>
        /// Performs the necessary value-checking logic to determine whether the given value 
        /// is of the correct type and format that it's supposed to be.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract bool IsValidValue(string value);

        /// <summary>
        /// Creates a deep clone of a property and its values.
        /// </summary>
        /// <returns></returns>
        public abstract Property Clone();

        public void SetValue(string value, int index)
        {
            if (!IsValidValue(value))
            {
                throw new Exception($"Can't convert value \"{value}\" to a {Identifier}.");
            }

            if (Values.ElementAtOrDefault(index) is null)
            {
                throw new Exception($"Can't add value \"{value}\" to the list at index {index}.");
            }

            Values.Add(value);
        }

        public void AddValue(string value)
        {
            if (!IsValidValue(value))
            {
                throw new Exception($"Can't convert value \"{value}\" to a {Identifier}.");
            }

            Values.Add(value);
        }
    }
}
