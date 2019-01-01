
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using static MCAutoVote.CLI.Command.Commands;

namespace MCAutoVote.CLI.Command
{
    public class Command
    {
        public ReadOnlyCollection<string> Aliases { get; }
        public string Description { get; }
        public bool IsHidden { get; }

        public CommandDelegate Delegate { get; }

        public Command(CommandDelegate @delegate, IList<string> aliases, string desc, bool hidden)
        {
            Delegate = @delegate;
            Aliases = new ReadOnlyCollection<string>(aliases);
            Description = desc;
            IsHidden = hidden;
        }

        public static Command ByStaticDelegatedField(FieldInfo field)
        {
            if (field.FieldType != typeof(CommandDelegate))
                throw new ArgumentException("Invalid field type!");

            CommandDelegate @delegate = (CommandDelegate)field.GetValue(null);
            List<string> aliases = new List<string>();
            string desc = null;
            bool hidden = false;

            foreach (AliasAttribute attr in field.GetCustomAttributes(typeof(AliasAttribute), false))
                aliases.Add(attr.Alias);

            DescriptionAttribute descriptionAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault();
            if (descriptionAttribute != null)
                desc = descriptionAttribute.Description;

            if (field.GetCustomAttributes(typeof(HiddenAttribute), false).Cast<HiddenAttribute>().Any())
                hidden = true;
            
            return new Command(@delegate, aliases, desc, hidden);
        }
    }
}
