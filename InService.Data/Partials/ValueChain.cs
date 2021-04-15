using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class ValueChain
    {
        public ValueChain MainCategory => ValueChain2;

        public IEnumerable<ValueChain> SubCategories => ValueChain1;

        public IValueChain IValueChain => new IValueChain
        {
            ID = ID,
            BranchID = BranchID,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            Name = Name,
            ParentID = ParentID
        };
    }
}
