using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LadderApp.Model
{
    public class NormallyClosedContact : NormallyOpenContact
    {
        public NormallyClosedContact() : base(OperationCode.NormallyClosedContact)
        {
        }

        public override bool GetValue()
        {
            return !base.GetValue();
        }
    }
}
