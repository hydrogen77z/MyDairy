using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Markup;

namespace MyDairy.Common;

[ContentProperty(Name = nameof(Objects))]
public class ObjectCollection
{
    public IList<object> Objects { get; } = [];
}
