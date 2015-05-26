using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Mobile.Service;
using System.ComponentModel.DataAnnotations.Schema;

namespace meloveService.DataObjects
{
    public class Dating : EntityData
    {
        public string mDaterId { get; set; }
        public string mDateeId { get; set; }

        [ForeignKey("mDaterId")]
        public virtual User mDater { get; set; }

        [ForeignKey("mDateeId")]
        public virtual User mDatee { get; set; }
    }
}
