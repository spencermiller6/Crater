using Markdig.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace To_Do_List_App
{
    /// <summary>
    /// This is a bit too ambitious to currently implement, but someday I'll get around to it. For now properties will
    /// be limited to strings, with the built-in date property being explicitely defined and the lone deviation from
    /// this rule. As a result, the classes in this file will go unused.
    /// </summary>
    public abstract class ItemProperty
    {
        public string Name { get; set; }
        public bool SupportsMultiple { get; set; }
        public object? Value { get; set; }

        public ItemProperty(string name, bool supportsMultiple)
        {
            Name = name;
            SupportsMultiple = supportsMultiple;
        }
    }

    public interface IMarkdownSerializable
    {
        /// <summary>
        /// Uniquely identifies this value type in markdown.
        /// </summary>
        static readonly string? Token;

        /// <summary>
        /// Parses <param name="input"/> and adds its value to <see cref="Value"/>.
        /// </summary>
        /// <param name="input">Raw markdown of a property's value.</param>
        public void AddValueFromMarkdown(string input);

        /// <summary>
        /// Recursively serializes an object and its children into a markdown string.
        /// </summary>
        /// <param name="ordinalPosition">The amount of indentation the current element has.</param>
        /// <returns></returns>
        string ToMarkdown(ref int ordinalPosition);
    }

    public class StringItemProperty : ItemProperty, IMarkdownSerializable
    {
        static readonly string Token = "string";

        public StringItemProperty(string name, bool supportsMultiple) : base(name, supportsMultiple)
        {
        }

        public void AddValueFromMarkdown(string input)
        {
            if (Value is null)
            {
                Value = input;
            }
            else
            {
                if (!SupportsMultiple)
                {
                    throw new Exception("Tried to add multiple values to a property that support that.");
                }
            }
        }

        public string ToMarkdown(ref int ordinalPosition)
        {
            throw new NotImplementedException();
        }
    }
}
