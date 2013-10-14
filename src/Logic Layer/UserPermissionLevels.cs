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
        Administrationschef,
        Allmän,
        Ekonomichef,
        [Description("Kund- och marknadschef")]
        Kundchef,
        Personalchef,
        Produktchef,
        Systemadministratör,
        Säljare,                  
        [Description("Utvecklings- och förvaltningschef")]
        Utvecklingschef       
    }
}
