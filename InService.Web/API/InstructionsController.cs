using InService.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using InService.Lib.Data;

namespace InService.Web.API
{
    public class InstructionsController : SysController
    {
        [AllowAnonymous]
        public IEnumerable<IInstruction> Get()
        {
            var instructions = new List<IInstruction>();
            foreach (var item in DB.Instructions.OrderByDescending(c => c.CreationDate)) instructions.Add(item.IInstruction);
            return instructions;
        }
    }
}
