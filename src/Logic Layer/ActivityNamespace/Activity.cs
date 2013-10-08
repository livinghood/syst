using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer.ActivityNamespace
{
    /// <summary>
    /// An activity belongs to an deparment
    /// </summary>
    public enum ActivityDepartments
	{
        [Description("Försäljnings- och Marknadsavdelning")] FO,
        [Description("Administrativa avdelningen")] AO,
	}

    /// <summary>
    /// Defines an activity
    /// </summary>
    public class Activity
    {
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public ActivityDepartments ActivityDepartment { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="activityId"></param>
        /// <param name="activityName"></param>
        /// <param name="activityDepartment"></param>
        public Activity(int activityId, string activityName, ActivityDepartments activityDepartment)
        {
            ActivityId = activityId;
            ActivityName = activityName;
            ActivityDepartment = activityDepartment;
        }
    }
}
