using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Application.DTO
{
    public class VariableExpenseResponseDTO
    {
        public Guid Id {get; init;}
        public Guid? CategoryId { get; init; }
        public Guid? PaymentMethodId {get; init;}
        public decimal Amount { get; init; }
        public DateTime Date { get; init; }
        public string Description { get; init; } = null!;
        public int Mes { get; init; }
        public int Ano { get; init; }
        public bool IsEssential { get; init; } = false;
 
    }
}