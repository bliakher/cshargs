using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
            if (target.UseWith == null) {
                if (!parsedOptions.Contains(target)) {
                    throw new MissingOptionException(target.LongName);
                }
            } else {
                if (parsedOptions.Contains(target.UseWith) && !parsedOptions.Contains(target)) {
                    throw new MissingOptionException(target.LongName);
                }
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
                throw new MissingDependencyException(target.LongName, dependency.LongName);
            }
        }
    }

    internal class GroupRule : IRule
    {
        private HashSet<OptionMetadata> groupOptions_ = new();
        private OptionGroupAttribute attribute_;
        public bool Required => attribute_.Required;
        public OptionMetadata UseWith { get; }
        public IEnumerable<object> Options => groupOptions_;

        public GroupRule(OptionGroupAttribute groupAttribute, IDictionary<string, OptionMetadata> optionProperties)
        {
            attribute_ = groupAttribute;

            if (groupAttribute.useWith != null) {
                if (!optionProperties.ContainsKey(groupAttribute.useWith)) {
                    throw new ConfigurationException($"Property name '{groupAttribute.useWith}' not known in group {this}.");
                }
                UseWith = optionProperties[groupAttribute.useWith];
            }

            foreach (var name in attribute_.OptionGroup) {
                if (!optionProperties.ContainsKey(name)) {
                    throw new ConfigurationException($"Property name '{name}' not known in group {this}.");
                }
                var option = optionProperties[name];

                if (option.Required) {
                    throw new ConfigurationException($"Can't have required option '{option.GetRawName()}' in required group {this}.");
                }
                if (option.UseWith != null) {
                    throw new ConfigurationException($"Can't have required option '{option.GetRawName()}' in required group {this}.");
                }

                groupOptions_.Add(option);
            }
        }
        public void Check(ICollection<OptionMetadata> parsedOptions)
        {
            string usedName = null;
            int count = 0;
            bool checkRequired = Required;

            checkRequired &= UseWith == null || parsedOptions.Contains(UseWith);

            foreach (var option in parsedOptions) {
                if (groupOptions_.Contains(option)) {
                    count++;
                    usedName = option.GetRawName();
                }
            }
            if (checkRequired && count == 0) {
                throw new MissingGroupException(getOptionNames());
            }
            if (count > 1) {
                throw new TooManyOptionsException(getOptionNames());
            }
            if (count == 1 && UseWith != null && !parsedOptions.Contains(UseWith)) {
                throw new MissingDependencyException(usedName, UseWith.GetRawName());
            }
        }

        private IEnumerable<string> getOptionNames() => groupOptions_.Select(opt => opt.GetRawName());

        public override string ToString()
        {
            return string.Join(" | ", getOptionNames());
        }
    }
}