using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer
{
    /// <summary>
    /// Enum for the permission levels a user can have
    /// </summary>
    public enum UserPermissionLevels
    {  
        Administrationschef = 0,
        Allmän = 1,
        Ekonomichef = 2,
        [Description("Kund- och marknadschef")]
        Kundchef = 3,
        Personalchef = 4,
        Produktchef = 5,
        Systemadministratör = 6,
        Säljare = 7,                  
        [Description("Utvecklings- och förvaltningschef")]
        Utvecklingschef = 8      
    }
}
