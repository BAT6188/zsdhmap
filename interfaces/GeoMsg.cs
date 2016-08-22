using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace zsdpmap.interfaces
{
	interface IGeoMsg
	{
		double x
		{
			get;
			set;
		}

		double y
		{
			get;
			set;
		}

		string id
		{
			get;
			set;
		}

		string title
		{
			get;
			set;
		}

	}
}
