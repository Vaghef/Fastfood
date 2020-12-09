using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fastfood.Models.ViewModels
{
    public class OrderListViewModel
    {
        public List<OrderDetailsViewModel> Orders { get; set; }
    }
}
