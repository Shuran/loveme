using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;

namespace meloveService.DataObjects
{
    public class User : EntityData
    {
        public string mName { get; set; }
        public byte[] mSalt { get; set; }
        public byte[] mSaltedAndHashedPd { get; set; }

        public string mGender { get; set; }

        public string mProfileBlobAddress { get; set; }

        [InverseProperty("mDater")]
        public virtual ICollection<Dating> mProposedDatings { get; set; }

        [InverseProperty("mDatee")]
        public virtual ICollection<Dating> mAcceptedDatings { get; set; }
    }
}
