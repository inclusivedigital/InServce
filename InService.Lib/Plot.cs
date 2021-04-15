using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace InService.Lib
{
    [DataContract]
    public class Plot
    {
        public Plot(decimal y, string label)
        {
            this.Y = y;
            this.Label = label;
        }
        //Explicitly setting the name to be used while serializing to JSON.
        [DataMember(Name = "y")]
        public Nullable<decimal> Y = null;

        [DataMember(Name = "label")]
        public string Label = null;
    }
}
