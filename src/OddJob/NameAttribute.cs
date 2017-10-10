using System;

namespace OddJob
{
    /// <summary>
    /// Represents an <see cref="Attribute"/> for a class that has a specific name at runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class NameAttribute : Attribute
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="NameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public NameAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the Name defined.
        /// </summary>
        /// <returns>Returns the name.</returns>
        public string GetName()
        {
            return this.name;
        }
    }
}
