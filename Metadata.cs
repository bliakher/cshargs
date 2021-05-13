using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CShargs
{
    class Metadata
    {
        private Type type;

        public Dictionary<char, Option> OptionsShort;
        public Dictionary<string, Option> OptionsLong;

        public Metadata(Type type)
        {
            this.type = type;
            OptionsShort = new();
            OptionsLong = new();
        }

        public void LoadData()
        {
            var properties = type.GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(typeof(IOptionAttribute));
                // ToDo: check count of attributes
            }
        }

        private void registerOption(PropertyInfo property, IOptionAttribute optionAttribute)
        {
            var option = new Option(optionAttribute, property);
            // ToDo: add rules to option
            if (OptionsLong[optionAttribute.Name] != null || OptionsShort[optionAttribute.ShortName] != null)
            {
                throw new ConfigurationException($"Option names must be unique.");
            }
            OptionsLong[optionAttribute.Name] = option;
            OptionsShort[optionAttribute.ShortName] = option;
        }

        public void CheckConflict()
        {
            
        }
    }

    class Option
    {
        private IOptionAttribute optionAttribute;
        private PropertyInfo propertyInfo;

        public bool IsRequired { get; }
        // use with - string, reference to option..??


        public Option(IOptionAttribute optionAttribute, PropertyInfo propertyInfo )
        {
            this.optionAttribute = optionAttribute;
            this.propertyInfo = propertyInfo;
            IsRequired = optionAttribute.Required;
        }
    }
}