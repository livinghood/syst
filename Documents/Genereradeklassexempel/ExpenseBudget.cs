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

public class ExpenseBudget
{
	public virtual int ExpenseBudgetID
	{
		get;
		set;
	}

	public virtual bool AdministrativeLock
	{
		get;
		set;
	}

	public virtual bool SellLock
	{
		get;
		set;
	}

	public virtual bool DevelopmentLock
	{
		get;
		set;
	}

	public virtual bool OperationLock
	{
		get;
		set;
	}

	public virtual int CauseOfCapital
	{
		get;
		set;
	}

}
