using System.Collections.Generic;
using System.Linq;
using System.Text;
using Generators.Extensions;

namespace Generators.GenerateCodeModels
{
    class Clazz
    {
        public string Namespace { get; }
        public string Name { get; }
        public bool IsAlreadyImplementsInpc { get; set; }
        public IEnumerable<Property> Properties { get; set; }
        public IDictionary<string, Property> PropertyMap { get; }

        public Clazz(string ns, string name, IEnumerable<Property> properties)
        {
            Namespace = ns;
            Name = name;
            Properties = properties.ToArray();
            PropertyMap = Properties.ToDictionary(x => x.Name);
        }

        public string ToCSharpCode()
        {
            var s = new StringBuilder();
            s.AppendLine($@"namespace {Namespace}
{{
    public partial class {Name}{" : System.ComponentModel.INotifyPropertyChanged".GetIfTrue(!IsAlreadyImplementsInpc)}
    {{
        {"\n        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;".GetIfTrue(!IsAlreadyImplementsInpc)}");

            foreach (var prop in Properties)
            {
                s.AppendLine(prop.DelayMs>0D? prop.ToCSharpCodeDelayed(): prop.ToCSharpCode());
            }

            if (!IsAlreadyImplementsInpc)
                s.AppendLine(
                    $"     protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)\n             => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));");

            s.Append($@"    }}
}}
");
            return s.ToString();
        }
    }

}
