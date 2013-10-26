using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Logic_Layer
{
    public enum ActivityDepartments
    {
        AO,
        FO
    }

    public class ActivityManagement
    {
        /// <summary>
        /// Lazy Instance of ActivityManagement Singelton
        /// </summary>
        private static readonly Lazy<ActivityManagement> instance = new Lazy<ActivityManagement>(() => new ActivityManagement());

        /// <summary>
        /// The instance property
        /// </summary>
        /// <remarks></remarks>
        public static ActivityManagement Instance
        {
            get { return instance.Value; }
        }

        /// <summary>
        /// Database context
        /// </summary>
        private readonly DatabaseConnection db = new DatabaseConnection();

        public ObservableCollection<Activity> Activities { get; set; }

        /// <summary>
        /// Standard constructor with initialization of activties list
        /// </summary>
        public ActivityManagement()
        {
            Activities = new ObservableCollection<Activity>(GetActivities());
        }

        /// <summary>
        /// Get department names for use in ActivityGUI
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetDepartmentNames()
        {
            return from d in db.Department
                   where d.DepartmentID == "FO" || d.DepartmentID == "AO"
                   orderby d.DepartmentID
                   select d.DepartmentID;
        }

        /// <summary>
        /// Get a list of all activitys
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Activity> GetActivities()
        {
            return db.Activity.OrderBy(a => a.ActivityName);
        }

        /// <summary>
        /// Add new activity
        /// </summary>
        /// <param name="activity"></param>
        public void AddActicity(Activity activity)
        {
            string id = activity.ActivityID;

            if (activity.DepartmentID.Equals("AO"))
                id += "AO";

            else
                id += "FO";

            activity.ActivityID = id;

            db.Activity.Add(activity);
            db.SaveChanges();
            Activities.Add(activity);
        }

        /// <summary>
        /// Delete an activity
        /// </summary>
        /// <param name="activity"></param>
        public void DeleteActivity(Activity activity)
        {
            Activities.Remove(activity);
            db.Activity.Remove(activity);
            db.SaveChanges();
        }

        /// <summary>
        /// Update an activity
        /// </summary>
        public void Update()
        {
            db.SaveChanges();
        }
    }
}
