﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Department
{
	public virtual int DepartmentID
	{
		get;
		set;
	}

	public virtual string DepartmentName
	{
		get;
		set;
	}

	public virtual ManagerProduct ManagerProduct
	{
		get;
		set;
	}

	public virtual ManagerEmployee ManagerEmployee
	{
		get;
		set;
	}

}
