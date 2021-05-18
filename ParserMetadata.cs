using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CShargs
{
    internal class ParserMetadata
    {
        private Type type;

        public Dictionary<char, OptionMetadata> OptionsShort;
        public Dictionary<string, OptionMetadata> OptionsLong;
        public Dictionary<string, OptionMetadata> OptionProperty;
        public List<GroupMetadata> Groups;


        public ParserMetadata(Type type)
        {
            this.type = type;
            OptionsShort = new();
            OptionsLong = new();
        }

        public void LoadData()
        {
            var properties = type.GetProperties();
            foreach (var property in properties) {
                var attributes = Attribute.GetCustomAttributes(property, typeof(IOptionAttribute));
                if (attributes.Length > 1) {
                    throw new ConfigurationException("One property cannot be annotated with multiple attributes.");
                }
                if (attributes.Length == 1) {
                    registerOption(property, (IOptionAttribute)attributes[0]);
                }
            }
            insertUseWithReferences();
            var groupAttributes = Attribute.GetCustomAttributes(type, typeof(OptionGroupAttribute));
            foreach (var groupAttribute in groupAttributes) {
                var group = new GroupMetadata((OptionGroupAttribute)groupAttribute, OptionProperty);
                Groups.Add(group);
            }
            //ToDo: aliases
        }

        private void registerOption(MemberInfo member, IOptionAttribute optionAttribute)
        {
            var option = new OptionMetadata(member, optionAttribute);
            // ToDo: add rules to option
            if (OptionsLong.ContainsKey(optionAttribute.Name) || OptionsShort.ContainsKey(optionAttribute.ShortName)) {
                throw new ConfigurationException($"Option names must be unique.");
            }

            OptionsLong[optionAttribute.Name] = option;
            if (optionAttribute.ShortName != '\0') {
                OptionsShort[optionAttribute.ShortName] = option;
            }
            OptionProperty[member.Name] = option;
        }

        private void insertUseWithReferences()
        {
            foreach (var option in OptionProperty.Values) {
                if (option.UseWithName != null) {
                    option.UseWith = OptionProperty[option.UseWithName];
                }
            }
        }

        public void CheckRules(HashSet<OptionMetadata> parsedOptions)
        {
            checkRequired(parsedOptions);
            checkUseWith(parsedOptions);
            checkGroups(parsedOptions);
        }

        private void checkRequired(HashSet<OptionMetadata> parsedOptions)
        {
            foreach (var option in OptionProperty.Values) {
                if (option.Required && !parsedOptions.Contains(option)) {
                    throw new MissingOptionException(option.Name);
                }
            }
        }

        private void checkUseWith(HashSet<OptionMetadata> parsedOptions)
        {
            foreach (var option in parsedOptions) {
                if (option.UseWith != null && !parsedOptions.Contains(option.UseWith)) {
                    throw new OptionDependencyError(option.Name, option.UseWithName);
                }
            }
        }
        private void checkGroups(HashSet<OptionMetadata> parsedOptions)
        {
            foreach (var group in Groups) {
                group.Check(parsedOptions);
            }
        }
    }

    internal class GroupMetadata
    {
        private HashSet<OptionMetadata> groupOptions;
        public bool Required {get; private set;}

        public GroupMetadata(OptionGroupAttribute groupAttribute, Dictionary<string, OptionMetadata> optionProperties)
        {
            foreach (var name in groupAttribute.OptionGroup) {
                if (!optionProperties.ContainsKey(name)) {
                    throw new ConfigurationException($"Error in option groups, property name {name} not known.");
                }
                groupOptions.Add(optionProperties[name]);
            }
        }

        public void Check(HashSet<OptionMetadata> parsedOptions)
        {
            // ToDo: implement
        }
    }
}