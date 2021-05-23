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
        public readonly Dictionary<string, OptionMetadata> OptionsByShort = new();
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

        public void CheckRules(ICollection<OptionMetadata> parsedOptions)
        {
            foreach (var rule in rules_) {
                rule.Check(parsedOptions);
            }
        }
        
        private void createOptionsMetadata()
        {
            var properties = userType_.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var property in properties) {
                var attributes = property.GetCustomAttributes().Where(attr => attr is IOptionAttribute).ToArray();
                if (attributes.Length > 1) {
                    throw new ConfigurationException("One property cannot be annotated with multiple attributes.");
                }
                if (attributes.Length == 1) {
                    if (property.SetMethod.IsPrivate) {
                        throw new ConfigurationException($"Option property '{property.Name}' cannot have private setter.");
                    }

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
            var option = optionAttribute.CreateMetadata(this, member);
            OptionsByProperty.Add(member.Name, option);
            return option;
        }

        private void registerOptionByLongName(OptionMetadata option, string longName)
        {
            Debug.Assert(longName != null);

            if (OptionsByLong.ContainsKey(longName)) {
                throw new ConfigurationException($"Option name {longName} is duplicate.");
            }

            if (Config.OptionFlags.HasFlag(OptionFlags.LongCaseInsensitive)) {
                longName = longName.ToUpper();
            }

            OptionsByLong.Add(longName, option);
        }

        private void registerOptionByShortName(OptionMetadata option, char shortName)
        {
            Debug.Assert(shortName != '\0');

            if (OptionsByShort.ContainsKey(shortName.ToString())) {
                throw new ConfigurationException($"Option name {shortName} is duplicate.");
            }

            if (Config.OptionFlags.HasFlag(OptionFlags.ShortCaseInsensitive)) {
                shortName = char.ToUpper(shortName);
            }

            OptionsByShort.Add(shortName.ToString(), option);
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
            foreach (var attribute in attributes) {
                var targetOptions = new List<OptionMetadata>();
                foreach (string target in attribute.Targets) {

                    if (OptionsByProperty.TryGetValue(target, out var option)) {

                        if (attribute.Targets.Count == 1 || option is FlagOption) {
                            targetOptions.Add(option);
                        } else {
                             throw new ConfigurationException(
                                 $"Cant make alias '{attribute.Alias}' with multiple targts for non-flag option '{target}'");
                        }
                    } else {
                        throw new ConfigurationException($"Alias '{attribute.Alias}' for an unknown option '{target}'.");
                    }
                }
                var alias = new AliasOption(this, attribute, targetOptions);
                if (attribute.LongName != null) {
                    registerOptionByLongName(alias, attribute.LongName);
                }
                else {
                    registerOptionByShortName(alias, attribute.ShortName);
                }
            }
        }
    }
}