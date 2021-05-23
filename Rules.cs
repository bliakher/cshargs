using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CShargs
{

    interface IRule
    {
        public void Check(ICollection<OptionMetadata> parsedOptions);
    }

    class RequiredRule : IRule
    {
        private OptionMetadata target;

        public RequiredRule(OptionMetadata target)
        {
            this.target = target;
        }

        public void Check(ICollection<OptionMetadata> parsedOptions)
        {
            if (!parsedOptions.Contains(target)) {
                throw new MissingOptionException(target.LongName);
            }
        }
    }

    class DependencyRule : IRule
    {
        private OptionMetadata target;

        public DependencyRule(OptionMetadata target)
        {
            Debug.Assert(target.UseWith != null, $"In {nameof(DependencyRule)}, target dependency cannot be null.");
            this.target = target;
        }

        public void Check(ICollection<OptionMetadata> parsedOptions)
        {
            var dependency = target.UseWith;

            if (parsedOptions.Contains(target) && !parsedOptions.Contains(dependency)) {
                throw new OptionDependencyError(target.LongName, dependency.LongName);
            }
        }
    }
    
    internal class GroupRule : IRule
    {
        private HashSet<OptionMetadata> groupOptions_ = new();
        private OptionGroupAttribute attribute_;
        public bool Required => attribute_.Required;

        public GroupRule(OptionGroupAttribute groupAttribute, IDictionary<string, OptionMetadata> optionProperties)
        {
            attribute_ = groupAttribute;
            foreach (var name in attribute_.OptionGroup) {
                if (!optionProperties.ContainsKey(name)) {
                    throw new ConfigurationException($"Error in option groups, property name '{name}' not known.");
                }
                var option = optionProperties[name];

                if (option.Required) {
                    throw new ConfigurationException($"Can't have required option '{option.LongName}' in required group.");
                }

                groupOptions_.Add(option);
            }
        }
        public void Check(ICollection<OptionMetadata> parsedOptions)
        {
            int count = 0;
            foreach (var option in parsedOptions) {
                if (groupOptions_.Contains(option)) {
                    count++;
                }
            }
            if (Required && count == 0) {
                throw new MissingGroupException(getOptionNames());
            }
            if (count > 1) {
                throw new MultipleOptionsFromExclusiveGroup(getOptionNames());
            }
        }

        private IEnumerable<string> getOptionNames() => groupOptions_.Select(opt => opt.GetRawName());
    }
}