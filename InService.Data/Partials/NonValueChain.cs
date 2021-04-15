using InService.Lib.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InService.Data
{
    public partial class NonValueChain
    {
        public NonValueChain MainCategory => NonValueChain2;

        public IEnumerable<NonValueChain> SubCategories => NonValueChain1;

        public INonValueChain INonValueChain => new INonValueChain
        {
            ParentID = ParentID,
            Name = Name,
            BranchID = BranchID,
            CreationDate = CreationDate,
            CreatorID = CreatorID,
            ID = ID,
        };
    }
}
