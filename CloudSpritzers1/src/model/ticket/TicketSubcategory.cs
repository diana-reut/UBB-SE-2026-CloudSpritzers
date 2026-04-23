using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudSpritzers1.Src.Model.Ticket
{
    public class TicketSubcategory
    {
        public int SubcategoryId { get; }
        public string SubcategoryName { get; }
        public int SubcategoryExternalReferenceId { get; }
        public TicketCategory ParentCategory { get; }

        public TicketSubcategory(int subcategoryId, string subcategoryName, int externalId, TicketCategory parentCategory)
        {
            SubcategoryId = subcategoryId;
            SubcategoryName = subcategoryName;
            SubcategoryExternalReferenceId = externalId;
            ParentCategory = parentCategory;
        }
    }
}