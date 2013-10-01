using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer
{
    /// <summary>
    /// Singelton Controller that is used for delegation from the GUI layer to the logic layer.
    /// The GUI layers entrance point.
    /// </summary>
    /// <remarks></remarks>
    public class LogicController
    {
        /// <summary>
        /// Lazy Instance of LogicController Singelton
        /// </summary>
        private static readonly Lazy<LogicController> instance = new Lazy<LogicController>(() => new LogicController());

        public ObservableCollection<CustomerNamespace.Customer> CustomerList { get; set; }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        /// <remarks></remarks>
        private LogicController()
        {

        }

        /// <summary>
        /// The instance property
        /// </summary>
        /// <remarks></remarks>
        public static LogicController Instance
        {
            get { return instance.Value; }
        }
    }
}
