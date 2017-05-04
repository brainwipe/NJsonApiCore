using System.Collections.Generic;

namespace NJsonApiCore.Test.TestModel
{
    internal class Widget
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<WidgetPart> Parts { get; set; }
    }
}
