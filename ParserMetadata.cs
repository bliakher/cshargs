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
        public List<GroupRule> Groups;
        private List<IRule> rules_;


        public ParserMetadata(Type type)
        {
            this.type = type;
            OptionsShort = new();
            OptionsLong = new();
            rules_ = new List<IRule>();
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
            extractRules();
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

        private void extractRules()
        {
            foreach (var option in OptionProperty.Values) {
                var rules = option.ExtractRules();
                rules_.AddRange(rules);
            }
            
            var groupAttributes = Attribute.GetCustomAttributes(type, typeof(OptionGroupAttribute));
            foreach (var groupAttribute in groupAttributes) {
                var group = new GroupRule((OptionGroupAttribute)groupAttribute, OptionProperty);
                rules_.Add(group);
            }
        }

        public void CheckRules(HashSet<OptionMetadata> parsedOptions)
        {
            foreach (var rule in rules_) {
                rule.Check(parsedOptions);
            }
        }
    }
    
}