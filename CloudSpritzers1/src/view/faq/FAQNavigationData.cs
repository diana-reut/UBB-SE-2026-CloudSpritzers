using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CloudSpritzers1.Src.Dto;

namespace CloudSpritzers1.Src.View.Faq
{
    public class FAQNavigationData
    {
        public int CurrentPersonId { get; set; }
        public bool IsEmployee { get; set; }
        public FAQEntryDTO? FAQEntry { get; set; }
    }
}
