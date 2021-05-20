using System.Collections.Generic;

namespace CShargs
{

    interface IRule
    {
        public void Check(HashSet<OptionMetadata> parsedOptions);
    }

    class RequiredRule : IRule
    {
        private OptionMetadata target;

        public RequiredRule(OptionMetadata target)
        {
            this.target = target;
        }

        public void Check(HashSet<OptionMetadata> parsedOptions)
        {
            if (!parsedOptions.Contains(target)) {
                throw new MissingOptionException(target.Name);
            }
        }
    }

    class DependencyRule : IRule
    {
        private OptionMetadata target;

        public DependencyRule(OptionMetadata target)
        {
            this.target = target;
        }

        public void Check(HashSet<OptionMetadata> parsedOptions)
        {
            var dependency = target.UseWith;
            if (dependency != null && parsedOptions.Contains(target) && !parsedOptions.Contains(dependency)) {
                throw new OptionDependencyError(target.Name, dependency.Name);
            }
        }
    }
    
    internal class GroupRule : IRule
    {
        private HashSet<OptionMetadata> groupOptions_;
        private OptionGroupAttribute attribute_;
        public bool Required => attribute_.Required;

        public GroupRule(OptionGroupAttribute groupAttribute, Dictionary<string, OptionMetadata> optionProperties)
        {
            attribute_ = groupAttribute;
            foreach (var name in attribute_.OptionGroup) {
                if (!optionProperties.ContainsKey(name)) {
                    throw new ConfigurationException($"Error in option groups, property name {name} not known.");
                }
                groupOptions_.Add(optionProperties[name]);
            }
        }
        public void Check(HashSet<OptionMetadata> parsedOptions)
        {
            int count = 0;
            foreach (var option in parsedOptions) {
                if (groupOptions_.Contains(option)) {
                    count++;
                }
            }
            if (Required && count == 0) {
                throw new MissingGroupOptionException(getOptionNames());
            }
            if (count > 1) {
                throw new MultipleOptionsFromExclusiveGroup(getOptionNames());
            }
        }

        private IEnumerable<string> getOptionNames()
        {
            //ToDo
            return null;
        }
    }
}