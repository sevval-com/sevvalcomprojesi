using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Domain.Enums
{
   
    public class TransferType(int value, string name, string? displayName)
    {
        public int Value { get; private set; } = value;
        public string Name { get; private set; } = name;
        public string DisplayName { get; private set; } = displayName ?? name;

        public static TransferType International { get; } = new TransferType(1, "Uluslararasi", "Uluslar arası");
        public static TransferType Intercity { get; } = new TransferType(2, "Sehirlerarasi", "Şehirler arası");
        public static TransferType InnerCity { get; } = new TransferType(3, "Sehirici", "Şehir içi");
      

        public static IEnumerable<TransferType> List()
        {
            return [International, Intercity, InnerCity];
        }

        public static TransferType FromString(string roleString)
        {
            return List().Single(r => string.Equals(r.Name, roleString, StringComparison.OrdinalIgnoreCase));
        }

        public static TransferType FromValue(int value)
        {
            return List().Single(r => r.Value == value);
        }
    }

}
