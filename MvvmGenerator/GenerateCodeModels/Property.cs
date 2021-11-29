using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Generators.Extensions;
using Microsoft.CodeAnalysis;

namespace Generators.GenerateCodeModels
{
    class Property
    {
        public List<AttributeData>? Attributes;
        public List<string> RelatedProperties { get; } = new();
        public string TypeName { get; }
        public string FieldName { get; }
        public string Name { get; }
        public bool IsAutoGenerateTarget { get; }
        public bool GenerateOnChanged { get; }
        public bool IncludeAttributes { get; }
        public double DelayMs { get; }

        public Property(string typeName, string fieldName, string name, bool isAutoGenerateTarget, ImmutableArray<AttributeData> attributes)
        {
            TypeName = typeName;
            FieldName = fieldName;
            Name = name;
            IsAutoGenerateTarget = isAutoGenerateTarget;
            var autoNotifyAttr = attributes.FirstOrDefault(x => x.AttributeClass?.Name == "AutoNotifyAttribute");

            if (autoNotifyAttr?.ConstructorArguments.Length == 4)
            {
                GenerateOnChanged = autoNotifyAttr.ConstructorArguments[1].Value as bool? ?? false;
                IncludeAttributes = autoNotifyAttr.ConstructorArguments[2].Value as bool? ?? false;
                if (IncludeAttributes)
                {
                    Attributes = attributes.Where(x => x.AttributeClass?.Name != "AutoNotifyAttribute").ToList();
                }

                DelayMs = autoNotifyAttr.ConstructorArguments[3].Value as double? ?? 0D;
            }
        }

        public string ToCSharpCode()
        {
            var s = new StringBuilder();

            if (IsAutoGenerateTarget)
            {
                string relatedProperties = string.Join("\n", RelatedProperties.GroupBy(x=>x).Select(prop => $"{"",16}OnPropertyChanged(nameof({prop.Key}));"));

                if (Attributes != null)
                    s.AppendLine(string.Join("\n", Attributes.Select(attr => $"{"",10}[{attr}]")));

                s.AppendLine($@"{"",4}public {TypeName} {Name}
        {{
            get => this.{FieldName};
            set
            {{
                if (System.Collections.Generic.EqualityComparer<{TypeName}>.Default.Equals(this.{FieldName}, value)) return;
                
                this.{FieldName} = value;
                
                OnPropertyChanged(nameof({Name}));
{relatedProperties.GetIfTrue(RelatedProperties.Any())}
{$"                On{Name}Changed();".GetIfTrue(GenerateOnChanged)}
            }}
        }}
{$"       partial void On{Name}Changed();".GetIfTrue(GenerateOnChanged)}");
            }
            return s.ToString();
        }


        //        public string ToCSharpCodeDelayed()
        //        {
        //            var s = new StringBuilder();

        //            if (IsAutoGenerateTarget)
        //            {
        //                string relatedProperties = string.Join("\n", RelatedProperties.GroupBy(x => x).Select(prop => $"{"",22}OnPropertyChanged(nameof({prop.Key}));"));

        //                s.AppendLine($"{"",8}private System.Windows.Threading.DispatcherTimer {FieldName}DelayTimer;");
        //                s.AppendLine($"{"",8}private {TypeName} {FieldName}NextValue;");

        //                if (Attributes != null)
        //                    s.AppendLine(string.Join("\n", Attributes.Select(attr => $"{"",10}[{attr}]")));

        //                s.AppendLine($@"{"",8}public {TypeName} {Name}
        //        {{
        //            get => this.{FieldName};
        //            set
        //            {{
        //                if ({FieldName}DelayTimer is {{IsEnabled: true}})
        //                  {{
        //                    if (System.Collections.Generic.EqualityComparer<{TypeName}>.Default.Equals(this.{FieldName}NextValue, value)) return;
        //                  }}
        //                else if (System.Collections.Generic.EqualityComparer<{TypeName}>.Default.Equals(this.{FieldName}, value)) return;

        //                {FieldName}NextValue = value;

        //                {FieldName}DelayTimer?.Stop();
        //                {FieldName}DelayTimer ??= new System.Windows.Threading.DispatcherTimer (System.TimeSpan.FromMilliseconds({DelayMs}),
        //                    System.Windows.Threading.DispatcherPriority.Background,
        //                    (sender, _) =>
        //                    {{
        //                        (sender as System.Windows.Threading.DispatcherTimer)?.Stop();
        //                        this.{FieldName} = {FieldName}NextValue;
        //                        OnPropertyChanged(nameof({Name}));
        //                        {FieldName}NextValue = null;

        //{relatedProperties.GetIfTrue(RelatedProperties.Any())}
        //                        {$"\n{"",18}On{Name}Changed();".GetIfTrue(GenerateOnChanged)}
        //                    }},
        //                    System.Windows.Threading.Dispatcher.CurrentDispatcher);

        //                {FieldName}DelayTimer.Start();
        //            }}
        //        }}

        //        {$"partial void On{Name}Changed();".GetIfTrue(GenerateOnChanged)}");
        //            }

        //            var sss = s.ToString();
        //            return s.ToString();
        //        }

        public string ToCSharpCodeDelayed()
        {
            var s = new StringBuilder();

            if (IsAutoGenerateTarget)
            {
                string relatedProperties = string.Join("\n", RelatedProperties.GroupBy(x => x).Select(prop => $"{"",22}OnPropertyChanged(nameof({prop.Key}));"));

                s.AppendLine($"{"",8}private System.Windows.Threading.DispatcherTimer {FieldName}DelayTimer;");

                if (Attributes != null)
                    s.AppendLine(string.Join("\n", Attributes.Select(attr => $"{"",10}[{attr}]")));

                s.AppendLine($@"{"",8}public {TypeName} {Name}
        {{
            get => this.{FieldName};
            set
            {{
                if (System.Collections.Generic.EqualityComparer<{TypeName}>.Default.Equals(this.{FieldName}, value)) return;

                {FieldName} = value;

                {FieldName}DelayTimer?.Stop();
                {FieldName}DelayTimer ??= new System.Windows.Threading.DispatcherTimer (System.TimeSpan.FromMilliseconds({DelayMs}),
                    System.Windows.Threading.DispatcherPriority.Background,
                    (sender, _) =>
                    {{
                        (sender as System.Windows.Threading.DispatcherTimer)?.Stop();
                        OnPropertyChanged(nameof({Name}));
{relatedProperties.GetIfTrue(RelatedProperties.Any())}
                        {$"\n{"",20}On{Name}Changed();".GetIfTrue(GenerateOnChanged)}
                    }},
                    System.Windows.Threading.Dispatcher.CurrentDispatcher);
                
                {FieldName}DelayTimer.Start();
            }}
        }}

        {$"partial void On{Name}Changed();".GetIfTrue(GenerateOnChanged)}");
            }

            var sss = s.ToString();
            return s.ToString();
        }
    }
}
