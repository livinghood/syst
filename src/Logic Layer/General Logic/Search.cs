using System;
using System.Collections.Generic;

namespace Logic_Layer.General_Logic
{
    public class Search
    {
        /// <summary>
        /// Lazy Instance of UserManagement Singelton
        /// </summary>
        private static readonly Lazy<Search> instance = new Lazy<Search>(() => new Search());

        /// <summary>
        /// The instance property
        /// </summary>
        /// <remarks></remarks>
        public static Search Instance
        {
            get { return instance.Value; }
        }

        public int UserRegisterSearch(string searchCriteria, IReadOnlyList<UserAccount> userAccountList)
        {
            int index = -1;

            for (int i = 0; i < userAccountList.Count; i++)
            {
                if (userAccountList[i].UserName.ToLower().Contains(searchCriteria))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }
    }
}
