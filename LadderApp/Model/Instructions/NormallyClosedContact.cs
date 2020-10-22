using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model.Instructions
{
    public class NormallyClosedContact : NormallyOpenContact
    {
        public NormallyClosedContact() : base(OperationCode.NormallyClosedContact)
        {
            Operands = new object[1];
        }

        public override bool GetValue()
        {
            return !base.GetValue();
        }
    }
}
