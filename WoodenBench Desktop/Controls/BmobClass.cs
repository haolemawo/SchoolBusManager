﻿//Game表对应的模型类
using cn.bmob.io;
using System;

class BmobObject : BmobTable
{
	private string PriTable;
	//以下对应云端字段名称
	public string UserName { get; set; }
	public string Password { get; set; }
	public string UserActAs { get; set; }

	//构造函数
	public BmobObject() { }

	//构造函数
	public BmobObject(String TableName)
	{
		PriTable = TableName;
	}

	public override string table
	{
		get
		{
			if (PriTable != null) { return PriTable; }
			return base.table;
		}
	}

	//读字段信息
	public override void readFields(BmobInput input)
	{
		base.readFields(input);
		UserName = input.getString("Username");
		Password = input.getString("Password");
		UserActAs = input.getString("UserActAs");

	}

	//写字段信息
	public override void write(BmobOutput output, bool all)
	{
		base.write(output, all);
		output.Put("Username", this.UserName);
		output.Put("Password", this.Password);
		output.Put("UserActAs", this.UserActAs);
	}
}