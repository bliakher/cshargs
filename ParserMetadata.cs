using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace CShargs
{
    internal class ParserMetadata
    {
        private Type userType_;
        private List<IRule> rules_;

        public ParserConfigAttribute Config { get; private set; }
        public readonly Dictionary<char, OptionMetadata> OptionsByShort = new();
        public readonly Dictionary<string, OptionMetadata> OptionsByLong = new();
        public readonly Dictionary<string, OptionMetadata> OptionsByProperty = new();

        public ParserMetadata(Type userType)
        {
            this.userType_ = userType;
        }

        public void LoadAtrributes()
        {
            if (userType_.IsDefined(typeof(ParserConfigAttribute))) {
                Config = userType_.GetCustomAttribute<ParserConfigAttribute>();
            } else {
                Config = new();
            }

            createOptionsMetadata();
            injectUseWithReferences();
            rules_ = extractRules();

            registerAliases();
        }

        public void CheckRules(HashSet<OptionMetadata> parsedOptions)
        {
            foreach (var rule in rules_) {
                rule.Check(parsedOptions);
            }
        }


        private void createOptionsMetadata()
        {
            var properties = userType_.GetProperties();
            foreach (var property in properties) {
                var attributes = Attribute.GetCustomAttributes(property, typeof(IOptionAttribute));
                if (attributes.Length > 1) {
                    throw new ConfigurationException("One property cannot be annotated with multiple attributes.");
                }
                if (attributes.Length == 1) {
                    var attrib = (IOptionAttribute)attributes[0];
                    var option = createOption(property, attrib);

                    registerOptionByLongName(option, attrib.LongName);
                    if (attrib.ShortName != '\0') {
                        registerOptionByShortName(option, attrib.ShortName);
                    }
                }
            }
        }

        private OptionMetadata createOption(MemberInfo member, IOptionAttribute optionAttribute)
        {
            var option = new OptionMetadata(this, member, optionAttribute);
            OptionsByProperty.Add(member.Name, option);

            return option;
        }

        private void registerOptionByLongName(OptionMetadata option, string longName)
        {
            if (OptionsByLong.ContainsKey(longName)) {
                throw new ConfigurationException($"Option name {longName} is duplicate.");
            }

            OptionsByLong.Add(longName, option);
        }

        private void registerOptionByShortName(OptionMetadata option, char shortName)
        {
            Debug.Assert(shortName != '\0');

            if (OptionsByShort.ContainsKey(shortName)) {
                throw new ConfigurationException($"Option name {shortName} is duplicate.");
            }

            OptionsByShort.Add(shortName, option);
        }

        private void injectUseWithReferences()
        {
            foreach (var option in OptionsByProperty.Values) {
                if (option.UseWithName != null) {
                    option.UseWith = OptionsByProperty[option.UseWithName];
                }
            }
        }

        private List<IRule> extractRules()
        {
            List<IRule> result = new();
            foreach (var option in OptionsByProperty.Values) {
                var rules = option.ExtractRules();
                result.AddRange(rules);
            }

            var groupAttributes = userType_.GetCustomAttributes<OptionGroupAttribute>();
            foreach (var groupAttribute in groupAttributes) {
                var group = new GroupRule(groupAttribute, OptionsByProperty);
                result.Add(group);
            }

            return result;
        }

        private void registerAliases()
        {
            var attributes = userType_.GetCustomAttributes<AliasOptionAttribute>();
            foreach (var attr in attributes) {
                foreach (string target in attr.Targets) {

                    if (OptionsByProperty.TryGetValue(target, out var option)) {

                        // if (attr.Targets.Count == 1 || option is FlagOption) {
                        //     TODO:
                        // } else {
                        //      throw new ConfigurationException(
                        //          $"Cant make alias '{attr.Alias}' with multiple targts for non-flag option '{target}'");
                        // }

                    } else {
                        throw new ConfigurationException($"Alias '{attr.Alias}' for an unknown option '{target}'.");
                    }
                }
            }
        }
    }
}